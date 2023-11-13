using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using Oculus.Interaction;
using System.Linq;

public class HandAreaCoordinate : NetworkBehaviour
{
    private NetworkVariable<NetworkPose> leftPose = new NetworkVariable<NetworkPose>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    private NetworkVariable<NetworkPose> rightPose = new NetworkVariable<NetworkPose>(
        default,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public HandsWrap handsWrap; // for IsOwner coordinate: actual hand
    DrivenHandVisual leftVisual; // for !IsOwner coordinate: hand visual
    DrivenHandVisual rightVisual;
    PlayerHitchhikeManager player;
    PlayerMovementPool playerMovementPool;

    // each coordinate is owned by each player
    private NetworkVariable<bool> n_isOriginal = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public bool isOriginal
    {
        get { return n_isOriginal.Value; }
        set
        {
            if (!IsOwner) return;
            n_isOriginal.Value = value;
        }
    }
    private NetworkVariable<bool> n_isEnabled = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );
    public bool isEnabled
    {
        get { return n_isEnabled.Value; }
        set
        {
            if (!IsOwner) return;
            n_isEnabled.Value = value;
        }
    }
    void n_isEnabled_OnValueChanged(bool previousValue, bool newValue)
    {
        if (handsWrap != null) handsWrap.frozen = !newValue;
        if (leftVisual != null) leftVisual.skinnedMeshRenderer.material = newValue ? LocalHitchhikeManager.Instance.remoteEnabledMaterial : LocalHitchhikeManager.Instance.remoteDisabledMaterial;
        if (rightVisual != null) rightVisual.skinnedMeshRenderer.material = newValue ? LocalHitchhikeManager.Instance.remoteEnabledMaterial : LocalHitchhikeManager.Instance.remoteDisabledMaterial;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        player = FindObjectsOfType<PlayerHitchhikeManager>().First(player => player.OwnerClientId == OwnerClientId);
        playerMovementPool = FindObjectsOfType<PlayerMovementPool>().First(pool => pool.OwnerClientId == OwnerClientId);
        if (IsOwner)
        {
            OwnerOnSpawn();
        }
        else
        {
            NonOwnerOnSpawn();
        }

        n_isEnabled.OnValueChanged += n_isEnabled_OnValueChanged;
    }
    void OwnerOnSpawn()
    {
        handsWrap = Instantiate(LocalHitchhikeManager.Instance.handsWrapPrefab, LocalHitchhikeManager.Instance.handsWrapPrefab.transform.parent);
        handsWrap.gameObject.SetActive(true);
        handsWrap.coordinate = this;
        if (player != null && player.PlayerOriginalCoordinate() != null) handsWrap.originalCoordinate = player.PlayerOriginalCoordinate();
        handsWrap.frozen = true;
    }

    void NonOwnerOnSpawn()
    {
        leftVisual = Instantiate(LocalHitchhikeManager.Instance.drivenHandPrefabLeft);
        rightVisual = Instantiate(LocalHitchhikeManager.Instance.drivenHandPrefabRight);
        if (leftVisual != null) leftVisual.skinnedMeshRenderer.material = isEnabled ? LocalHitchhikeManager.Instance.remoteEnabledMaterial : LocalHitchhikeManager.Instance.remoteDisabledMaterial;
        if (rightVisual != null) rightVisual.skinnedMeshRenderer.material = isEnabled ? LocalHitchhikeManager.Instance.remoteEnabledMaterial : LocalHitchhikeManager.Instance.remoteDisabledMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            OwnerUpdate();
        }
        else
        {
            NonOwnerUpdate();
        }
    }

    void OwnerUpdate()
    {
        if (LocalHitchhikeManager.Instance.billboardToHead) Billboard();
        if (handsWrap == null) return;
        handsWrap.leftFinalHand.GetRootPose(out var tempLeftPose);
        handsWrap.rightFinalHand.GetRootPose(out var tempRightPose);
        leftPose.Value = tempLeftPose;
        rightPose.Value = tempRightPose;

        if (!isEnabled || playerMovementPool == null) return;
        handsWrap.leftFinalHand.GetJointPosesLocal(out var tempLeftJoints);
        handsWrap.rightFinalHand.GetJointPosesLocal(out var tempRightJoints);
        playerMovementPool.leftJointsPool.Value = tempLeftJoints;
        playerMovementPool.rightJointsPool.Value = tempRightJoints;
    }

    // state; 0: not initialized yet, 1: initialized
    int leftVisualState = 0;
    int rightVisualState = 0;
    HandAreaCoordinate originalCoordinate;
    void NonOwnerUpdate()
    {
        if (player == null) return;
        if (leftVisual != null && leftPose != null) leftVisual.transform.SetPose(leftPose.Value);
        if (rightVisual != null && rightPose != null) rightVisual.transform.SetPose(rightPose.Value);

        originalCoordinate = player.PlayerOriginalCoordinate();
        float scale = LocalHitchhikeManager.Instance.scaleHandModel ? HitchhikeUtilities.ApplyScaling(originalCoordinate.transform, transform) : 1;
        leftVisual.SetScale(scale);
        rightVisual.SetScale(scale);

        if (leftVisualState == 0)
        {
            if (leftVisual != null && playerMovementPool.leftJointsPool != null) leftVisual.Drive(Pose.identity, playerMovementPool.leftJointsPool.Value);
            leftVisualState = 1;
        }
        if (rightVisualState == 0)
        {
            if (rightVisual != null && playerMovementPool.rightJointsPool != null) rightVisual.Drive(Pose.identity, playerMovementPool.rightJointsPool.Value);
            rightVisualState = 1;
        }
        if (isEnabled && leftVisual != null && playerMovementPool.leftJointsPool != null) leftVisual.Drive(Pose.identity, playerMovementPool.leftJointsPool.Value);
        if (isEnabled && rightVisual != null && playerMovementPool.rightJointsPool != null) rightVisual.Drive(Pose.identity, playerMovementPool.rightJointsPool.Value);

    }

    void Billboard()
    {
        if (LocalHitchhikeManager.Instance.headAnchor == null) return;
        var vec = LocalHitchhikeManager.Instance.headAnchor.transform.position - transform.position;
        transform.forward = new Vector3(
          -vec.x,
          0,
          -vec.z
        );
    }

    public void SetOriginalCoordinate(HandAreaCoordinate coordinate)
    {
        if (!IsOwner) return;
        if (coordinate == this) isOriginal = true;
        if (handsWrap != null) handsWrap.originalCoordinate = coordinate;
    }

    public override void OnNetworkDespawn()
    {
        if (leftVisual != null) Destroy(leftVisual.gameObject);
        if (rightVisual != null) Destroy(rightVisual.gameObject);
        if (!IsOwner && isOriginal) GetComponentInParent<HandArea>().RequestDespawn();
        base.OnNetworkDespawn();
    }
}
