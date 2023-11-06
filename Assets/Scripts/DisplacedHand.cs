using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction.Input
{

    public class DisplacedHand : Hand
    {
        [HideInInspector] public Vector3 disposition = Vector3.zero;
        [HideInInspector] public float scale = 1f;
        [HideInInspector] public bool frozen = false;
        private readonly HandDataAsset _lastState = new HandDataAsset();

        private int _trackingState = 0;
        // 0: waiting for IsHighConfidence
        // 1: found first confident hand pose; is valid

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
            root.position += disposition;
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