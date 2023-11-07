using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Input
{

    public class DisplacedHand : Hand
    {
        [HideInInspector] public Transform originalSpace;
        [HideInInspector] public Transform thisSpace;
        [HideInInspector] public float scale = 1f;
        [HideInInspector] public bool frozen = false;
        private readonly HandDataAsset _lastState = new HandDataAsset();

        private int _trackingState = 0;
        // 0: waiting for IsHighConfidence
        // 1: found first confident hand pose; is valid

        private void Awake()
        {
        }

        protected override void Start()
        {
            base.Start();
            this.BeginStart(ref _started, () => base.Start());
            this.EndStart(ref _started);
        }

        protected override void Apply(HandDataAsset data)
        {
            if (_trackingState == 0 && data.IsHighConfidence)
            {
                _trackingState = 1;
                _lastState.CopyFrom(data);
            }

            if (frozen) data.CopyPosesFrom(_lastState);

            UpdateRootPose(ref data.Root);
            ScaleHand(ref data.HandScale);
            data.RootPoseOrigin = PoseOrigin.FilteredTrackedPose;

            if (!frozen) _lastState.CopyFrom(data);
        }

        private void UpdateRootPose(ref Pose root)
        {
            var cameraRigDisplace = HitchhikeManager.Instance.cameraRig.transform.position - HitchhikeManager.Instance.initialCameraRigPosition;
            if (originalSpace == null || thisSpace == null) return;
            var newPose = HitchhikeUtilities.ApplyOffset(
                new Pose(root.position, root.rotation),
                originalSpace,
                thisSpace,
                cameraRigDisplace
            );
            root.position = newPose.position;
            root.rotation = newPose.rotation;
            // todo: filter pos/rot
        }
        private void ScaleHand(ref float handScale)
        {
            handScale = scale;
        }

        #region Inject

        public void InjectAllSyntheticHandModifier(UpdateModeFlags updateMode, IDataSource updateAfter,
            DataModifier<HandDataAsset> modifyDataFromSource, bool applyModifier)
        {
            base.InjectAllHand(updateMode, updateAfter, modifyDataFromSource, applyModifier);
        }
        #endregion
    }

}