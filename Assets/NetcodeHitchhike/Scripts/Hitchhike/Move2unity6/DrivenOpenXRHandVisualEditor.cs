using Oculus.Interaction.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace Oculus.Interaction.Hands.Editor
{
    [CustomEditor(typeof(DrivenOpenXRHandVisual))]
    public class DrivenOpenXRHandVisualEditor : SimplifiedEditor
    {
        private SerializedProperty _rootProperty;
        private SerializedProperty _jointsProperty;

        protected override void OnEnable()
        {
            base.OnEnable();

#if ISDK_OPENXR_HAND
            _jointsProperty = serializedObject.FindProperty("_openXRJointTransforms");
            _rootProperty = serializedObject.FindProperty("_openXRRoot");
#else
            _jointsProperty = serializedObject.FindProperty("_jointTransforms");
            _rootProperty = serializedObject.FindProperty("_root");
#endif
        }

        public override void OnInspectorGUI()
        {
            DrawPropertiesExcluding(serializedObject);
            serializedObject.ApplyModifiedProperties();

            DrivenOpenXRHandVisual visual = (DrivenOpenXRHandVisual)target;
            HandJointsAutoPopulatorHelper.InitializeCollection(_jointsProperty);

            if (GUILayout.Button("Auto Map Joints"))
            {
                AutoMapJoints(visual);
                EditorUtility.SetDirty(visual);
                EditorSceneManager.MarkSceneDirty(visual.gameObject.scene);
            }

            HandJointsAutoPopulatorHelper.DisplayJoints(_jointsProperty);
            serializedObject.ApplyModifiedProperties();
        }

        private void AutoMapJoints(DrivenOpenXRHandVisual visual)
        {
            Transform rootTransform = visual.transform;
            if (_rootProperty.objectReferenceValue is Transform customRoot)
            {
                rootTransform = customRoot;
            }
            HandJointsAutoPopulatorHelper.AutoMapJoints(_jointsProperty, rootTransform);
        }
    }
}
