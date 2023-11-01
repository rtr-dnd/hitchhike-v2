using System;
using System.Linq;
using UnityEngine;
using Unity.Netcode;
using Oculus.Interaction.Input;

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