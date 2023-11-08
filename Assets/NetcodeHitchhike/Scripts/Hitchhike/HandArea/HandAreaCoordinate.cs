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
    [SerializeField] private Material enabledMaterial;
    [SerializeField] private Material disabledMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    HandsWrap handsWrap; // for IsOwner coordinate: actual hand
    DrivenHandVisual leftVisual; // for !IsOwner coordinate: hand visual
    DrivenHandVisual rightVisual;
    NetworkPlayer player;

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

    public void SetOriginalCoordinate(HandAreaCoordinate coordinate)
    {
        if (!IsOwner) return;
        if (handsWrap != null) handsWrap.originalCoordinate = coordinate;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        player = FindObjectsOfType<NetworkPlayer>().First(player => player.OwnerClientId == OwnerClientId);
        if (IsOwner)
        {
            handsWrap = Instantiate(HitchhikeManager.Instance.handsWrapPrefab, HitchhikeManager.Instance.handsWrapPrefab.transform.parent);
            handsWrap.gameObject.SetActive(true);
            handsWrap.coordinate = this;
            if (player != null && player.PlayerOriginalCoordinate() != null) handsWrap.originalCoordinate = player.PlayerOriginalCoordinate();
            handsWrap.frozen = true;
        }
        else
        {
            leftVisual = Instantiate(HitchhikeManager.Instance.drivenHandPrefabLeft);
            rightVisual = Instantiate(HitchhikeManager.Instance.drivenHandPrefabRight);
        }

        n_isEnabled.OnValueChanged += (previousValue, newValue) =>
        {
            meshRenderer.material = newValue ? enabledMaterial : disabledMaterial;
            if (handsWrap != null) handsWrap.frozen = !newValue;
        };
    }

    // state; 0: not initialized yet, 1: initialized
    int leftVisualState = 0;
    int rightVisualState = 0;
    // Update is called once per frame
    void Update()
    {
        if (!IsSpawned) return;
        if (IsOwner)
        {
            if (handsWrap == null) return;
            handsWrap.leftFinalHand.GetRootPose(out var tempLeftPose);
            handsWrap.rightFinalHand.GetRootPose(out var tempRightPose);
            leftPose.Value = tempLeftPose;
            rightPose.Value = tempRightPose;

            if (!isEnabled || player == null) return;
            handsWrap.leftFinalHand.GetJointPosesLocal(out var tempLeftJoints);
            handsWrap.rightFinalHand.GetJointPosesLocal(out var tempRightJoints);
            player.leftJointsPool.Value = tempLeftJoints;
            player.rightJointsPool.Value = tempRightJoints;
        }
        else
        {
            if (player == null) return;
            if (leftVisual != null && leftPose != null) leftVisual.transform.SetPose(leftPose.Value);
            if (rightVisual != null && rightPose != null) rightVisual.transform.SetPose(rightPose.Value);

            float scale = HitchhikeManager.Instance.scaleHandModel ? new float[] {
                transform.lossyScale.x / player.PlayerOriginalCoordinate().transform.lossyScale.x,
                transform.lossyScale.y / player.PlayerOriginalCoordinate().transform.lossyScale.y,
                transform.lossyScale.z / player.PlayerOriginalCoordinate().transform.lossyScale.z
            }.Average() : 1;
            rightVisual.SetScale(scale);

            if (leftVisualState == 0)
            {
                if (leftVisual != null && player.leftJointsPool != null) leftVisual.Drive(Pose.identity, player.leftJointsPool.Value);
                leftVisualState = 1;
            }
            if (rightVisualState == 0)
            {
                if (rightVisual != null && player.rightJointsPool != null) rightVisual.Drive(Pose.identity, player.rightJointsPool.Value);
                rightVisualState = 1;
            }
            if (isEnabled && leftVisual != null && player.leftJointsPool != null) leftVisual.Drive(Pose.identity, player.leftJointsPool.Value);
            if (isEnabled && rightVisual != null && player.rightJointsPool != null) rightVisual.Drive(Pose.identity, player.rightJointsPool.Value);
        }
    }
}
