using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oculus.Interaction.Input
{

    public class DisplacedHand : Hand
    {
        [HideInInspector] public Transform originalSpace;
        [HideInInspector] public Transform thisSpace;
        private bool m_frozen = false;
        [HideInInspector]
        public bool frozen
        {
            get { return m_frozen; }
            set
            {
                m_frozen = value;
                skinnedMeshRenderer.material = value ? LocalHitchhikeManager.Instance.localDisabledMaterial : LocalHitchhikeManager.Instance.localEnabledMaterial;
            }
        }
        public SkinnedMeshRenderer skinnedMeshRenderer;
        private readonly HandDataAsset _lastState = new HandDataAsset();

        // Tracking state
        // 0: waiting for IsHighConfidence
        // 1: found first confident hand pose; is valid
        private int _trackingState = 0;

        protected override void Start()
        {
            base.Start();
            this.BeginStart(ref _started, () => base.Start());
            this.EndStart(ref _started);
        }

        protected override void Apply(HandDataAsset data)
        {
            if (_trackingState == 0)
            {
                if (!data.IsHighConfidence) return;
                _trackingState = 1;
                _lastState.CopyFrom(data);
            }

            if (frozen) data.CopyPosesFrom(_lastState);
            if (!frozen) _lastState.CopyFrom(data);

            UpdateRootPose(ref data.Root);
            ScaleHand(ref data.HandScale);
            data.IsDataValid = true;
            data.IsTracked = true;
            data.IsHighConfidence = true;
            data.RootPoseOrigin = PoseOrigin.FilteredTrackedPose;
        }

        private void UpdateRootPose(ref Pose root)
        {
            var cameraRigDisplace = LocalHitchhikeManager.Instance.GetCameraRigDisplace();
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
            handScale = LocalHitchhikeManager.Instance.scaleHandModel ? HitchhikeUtilities.ApplyScaling(originalSpace, thisSpace) : 1;
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