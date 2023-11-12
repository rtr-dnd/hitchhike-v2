using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    NetworkVariable<ulong> originalHandAreaId = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public NetworkVariable<ulong> activeHandAreaId = new NetworkVariable<ulong>(
        ulong.MaxValue,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    int activeHandAreaIndex = 0;

    List<Handedness> handednesses = new List<Handedness>() { Handedness.Left, Handedness.Right };
    List<HandGrabInteractable> interactables;
    List<HandGrabTarget> handGrabTargets;
    IEnumerator seekActiveAreaLoop;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        originalHandAreaId.OnValueChanged += (previousValue, newValue) =>
        {
            Debug.Log("original hand area is: " + newValue);
            HitchhikeManager.Instance.handAreaManager.handAreas.ForEach(area =>
                area.GetCoordinateForClient(this.OwnerClientId).SetOriginalCoordinate(PlayerOriginalCoordinate()));
            if (!IsOwner) return;
            activeHandAreaId.Value = newValue;
        };
        activeHandAreaId.OnValueChanged += (previousValue, newValue) =>
        {
            if (!IsOwner) return;
            if (newValue == ulong.MaxValue) return;
            if (HitchhikeManager.Instance.handAreaManager.handAreas == null) return;

            if (seekActiveAreaLoop != null) StopCoroutine(seekActiveAreaLoop);
            seekActiveAreaLoop = SeekActiveAreaLoop(newValue);
            StartCoroutine(seekActiveAreaLoop);
        };
        if (IsOwner) Invoke(nameof(DisplacePlayer), 0.5f);
    }
    IEnumerator SeekActiveAreaLoop(ulong activeAreaId)
    {
        if (activeAreaId == ulong.MaxValue) yield break;
        var newArea = HitchhikeManager.Instance.handAreaManager.handAreas.Find(area => area.GetComponent<NetworkObject>().NetworkObjectId == activeAreaId);
        var i = HitchhikeManager.Instance.handAreaManager.handAreas.FindIndex(area => area == newArea);
        if (i != -1)
        {
            foreach (var item in HitchhikeManager.Instance.handAreaManager.handAreas.Select((area, index) => new { area, index }))
            {
                item.area.GetCoordinateForClient(this.OwnerClientId).isEnabled = item.index == i;
            }
            activeHandAreaIndex = i;

            // dnd grab
            if (!HitchhikeManager.Instance.DragAndDrop
                || interactables == null
                || interactables.Count == 0
                || interactables.All(i => i == null)) yield break;
            List<HandGrabInteractable> alreadyDroppedInteractables = new List<HandGrabInteractable>();
            foreach (var (rawInteractable, handIndex) in interactables.Select((value, index) => (value, index)))
            {
                if (rawInteractable == null) continue;
                var interactable = rawInteractable;
                var pdd = interactable.GetComponent<PreventDragAndDrop>();
                if (pdd != null)
                {
                    var mtoi = pdd.moveThisObjectInstead;
                    if (mtoi == null || mtoi.GetComponent<HandGrabInteractable>() == null) continue;
                    interactable = mtoi.GetComponent<HandGrabInteractable>();
                }
                if (alreadyDroppedInteractables.FindIndex(i => i == interactable) != -1) continue;
                var grabbable = interactable.GetComponentInParent<Grabbable>();
                var newCoord = newArea.GetCoordinateForClient(OwnerClientId);
                var newPose = HitchhikeUtilities.ApplyOffset(grabbable.transform.GetPose(Space.World), HitchhikeManager.Instance.beforeCoordTransform, newCoord.transform, Vector3.zero);
                grabbable.transform.SetPose(newPose, Space.World);
                var newScale = HitchhikeManager.Instance.scaleHandModel ? HitchhikeUtilities.ApplyScaling(HitchhikeManager.Instance.beforeCoordTransform, newCoord.transform) : 1;
                grabbable.transform.localScale *= newScale;

                var afterHandsWrap = newCoord.handsWrap;
                afterHandsWrap.Select(handednesses[handIndex], interactable, handGrabTargets[handIndex]);
                alreadyDroppedInteractables.Add(interactable);
            }
            yield break;
        }
        yield return new WaitForSeconds(0.5f);
        seekActiveAreaLoop = SeekActiveAreaLoop(activeAreaId);
        StartCoroutine(seekActiveAreaLoop);
    }
    void DisplacePlayer()
    {
        var cameraRig = FindObjectOfType<OVRCameraRig>();
        cameraRig.transform.position = transform.position;
        HitchhikeManager.Instance.handAreaManager.CreateHandArea(new Vector3(
            transform.position.x,
            0.7f,
            transform.position.z + 0.3f
        ), Quaternion.identity, NetworkManager.LocalClientId);
    }
    public void SetOriginalHandArea(ulong id)
    {
        if (!IsOwner) return;
        originalHandAreaId.Value = id;
    }
    public HandAreaCoordinate PlayerOriginalCoordinate()
    {
        if (originalHandAreaId == null || originalHandAreaId.Value == ulong.MaxValue) return null;
        return NetworkManager.SpawnManager.SpawnedObjects[originalHandAreaId.Value].GetComponent<HandArea>().GetCoordinateForClient(this.OwnerClientId);
    }

    void Update()
    {
        if (!IsOwner) return;
        if (!IsSpawned) return;

        var handAreaManager = HitchhikeManager.Instance.handAreaManager;

        // check integrity of hand area id / index
        var currentActiveIndex = handAreaManager.handAreas.FindIndex(area => area.GetComponent<NetworkObject>().NetworkObjectId == activeHandAreaId.Value);
        if (currentActiveIndex == -1) activeHandAreaId.Value = handAreaManager.handAreas[0].GetComponent<HandArea>().NetworkObjectId;
        if (currentActiveIndex != activeHandAreaIndex) activeHandAreaIndex = currentActiveIndex;

        // switching
        var newActiveHandAreaIndex = HitchhikeManager.Instance.switchTechnique.GetFocusedHandAreaIndex(activeHandAreaIndex);
        if (activeHandAreaIndex != newActiveHandAreaIndex)
        {
            var newActiveId = handAreaManager.handAreas[newActiveHandAreaIndex].GetComponent<NetworkObject>().NetworkObjectId;

            // dnd ungrab
            var beforeCoordinate = handAreaManager.handAreas[activeHandAreaIndex].GetCoordinateForClient(NetworkManager.LocalClientId);
            HitchhikeManager.Instance.beforeCoordTransform.position = beforeCoordinate.transform.position;
            HitchhikeManager.Instance.beforeCoordTransform.rotation = beforeCoordinate.transform.rotation;
            HitchhikeManager.Instance.beforeCoordTransform.localScale = beforeCoordinate.transform.lossyScale;
            var beforeHandsWrap = beforeCoordinate.handsWrap;
            interactables = handednesses.Select(h =>
                beforeHandsWrap.GetCurrentInteractable(h)
            ).ToList();
            handGrabTargets = handednesses.Select(h => beforeHandsWrap.Unselect(h)).ToList();

            // actual switch
            activeHandAreaId.Value = newActiveId;
        }
    }
}
