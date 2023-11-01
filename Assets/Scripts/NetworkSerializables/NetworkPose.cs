using System;
using UnityEngine;
using Unity.Netcode;

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