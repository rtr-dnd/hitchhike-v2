using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Oculus.Interaction;
using Oculus.Interaction.Input;
using System.Linq;
using System;

// reads the joint angle from the hand and forces DrivenHandVisual to have the angles
public class NetworkHandVisualDriver : NetworkBehaviour
{
    [SerializeField, Interface(typeof(IHand))]
    private UnityEngine.Object _hand;
    public IHand hand;
    public DrivenHandVisual visual;
    private NetworkVariable<NetworkHandJointPoses> joints = new NetworkVariable<NetworkHandJointPoses>();

    private void Awake()
    {
        hand = _hand as IHand;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        joints.Value = new ReadOnlyHandJointPoses(new Pose[] { });
    }

    void Update()
    {
        if (IsHost)
        {
            hand.GetJointPosesLocal(out ReadOnlyHandJointPoses localJoints);
            if (localJoints == null) return;
            joints.Value = localJoints;
        }
        if (joints.Value.poses != null && joints.Value.poses.Length != 0) visual.Drive(Pose.identity, joints.Value);
    }

    struct NetworkHandJointPoses : INetworkSerializable, IEquatable<NetworkHandJointPoses>
    {
        public NetworkPose[] poses;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            int length = 0;
            if (!serializer.IsReader) length = poses.Length;
            serializer.SerializeValue(ref length);
            if (serializer.IsReader) poses = new NetworkPose[length];
            for (int i = 0; i < length; ++i)
            {
                serializer.SerializeValue(ref poses[i]);
            }
        }
        public bool Equals(NetworkHandJointPoses other)
        {
            if (poses == null) return other.poses == null;
            if (other.poses == null) return false;
            if (poses.Length != other.poses.Length) return false;
            return Enumerable.SequenceEqual(poses, other.poses);
        }

        public static implicit operator NetworkHandJointPoses(ReadOnlyHandJointPoses r) => new NetworkHandJointPoses()
        {
            poses = r.Select(p => new NetworkPose(p)).ToArray()
        };
        public static implicit operator ReadOnlyHandJointPoses(NetworkHandJointPoses n) => new ReadOnlyHandJointPoses(
            n.poses.Select(p => p.ToPose()).ToArray()
        );
    }

    struct NetworkPose : INetworkSerializable, IEquatable<NetworkPose>
    {
        Vector3 position;
        Quaternion rotation;
        public NetworkPose(Pose p) { position = p.position; rotation = p.rotation; }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotation);
        }
        public override string ToString()
        {
            return position.ToString() + ", " + rotation.ToString();
        }
        public Pose ToPose() => new Pose(position, rotation);

        public bool Equals(NetworkPose other)
        {
            return position == other.position && rotation == other.rotation;
        }

        public static implicit operator Pose(NetworkPose n) => n.ToPose();
        public static implicit operator NetworkPose(Pose p) => new NetworkPose() { position = p.position, rotation = p.rotation };
    }
}
