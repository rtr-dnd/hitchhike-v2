using UnityEngine;

public static class HitchhikeUtilities
{
  public static Pose ApplyOffset(Pose rawPose, Transform originalSpace, Transform thisSpace, Vector3 cameraRigDisplace, bool mirrored = false)
  {
    rawPose.position += cameraRigDisplace;

    var originalSpaceOrigin = originalSpace;
    var thisSpaceOrigin = thisSpace;
    Debug.Log("original: " + originalSpaceOrigin.position + ", this: " + thisSpaceOrigin.position);

    var originalToActiveRot = Quaternion.Inverse(thisSpaceOrigin.rotation) * originalSpaceOrigin.rotation;
    var originalToActiveScale = new Vector3(
        thisSpaceOrigin.lossyScale.x / originalSpaceOrigin.lossyScale.x,
        thisSpaceOrigin.lossyScale.y / originalSpaceOrigin.lossyScale.y,
        thisSpaceOrigin.lossyScale.z / originalSpaceOrigin.lossyScale.z
    );

    // calculate mirrored anchor
    var localAnchorPos = originalSpaceOrigin.InverseTransformPoint(rawPose.position);
    var mirroredAnchorPos = originalSpaceOrigin.TransformPoint(new Vector3(-localAnchorPos.x, localAnchorPos.y, localAnchorPos.z));
    var relativeRot = rawPose.rotation * Quaternion.Inverse(originalSpaceOrigin.rotation);
    var mirroredAnchorRot = Quaternion.Euler(0, 180, 0)
        * Quaternion.Euler(-relativeRot.eulerAngles.x, -relativeRot.eulerAngles.y, relativeRot.eulerAngles.z)
        * originalSpaceOrigin.rotation;

    var oMt = Matrix4x4.TRS(
        mirrored ? mirroredAnchorPos : rawPose.position,
        mirrored ? mirroredAnchorRot : rawPose.rotation,
        new Vector3(1, 1, 1)
    );

    var resMat =
    Matrix4x4.Translate(thisSpaceOrigin.position) // translation back to copied space
    * Matrix4x4.Rotate(Quaternion.Inverse(originalToActiveRot)) // rotation around origin
    * Matrix4x4.Scale(originalToActiveScale) // scale around origin
    * Matrix4x4.Translate(-originalSpaceOrigin.position) // offset translation to origin for next step
    * oMt; // hand anchor

    return new Pose(resMat.GetPosition() - cameraRigDisplace, resMat.rotation);
    // filteredPosition = filteredPosition * (1 - filterRatio) + resMat.GetPosition() * filterRatio;
    // filteredRotation = Quaternion.Lerp(filteredRotation, resMat.rotation, filterRatio);

    // return new Pose(
    //     filteredPosition,
    //     filteredRotation
    // );
  }
}