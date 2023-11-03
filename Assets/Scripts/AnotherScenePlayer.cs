using UnityEngine;

#if UNITY_EDITOR
 using UnityEditor;
 using UnityEditor.SceneManagement;
#endif

/// <summary>
/// 別のシーンから再生するようにする
/// </summary>
[ExecuteInEditMode]//ExecuteInEditModeを付ける事でOnValidateやOnDestroyが再生していなくても実行されるようになる
public class AnotherScenePlayer : MonoBehaviour
{
#if UNITY_EDITOR
 
   //再生する別のシーン(nullのままにすると、現状のシーンがそのまま再生される)
   [SerializeField]
   private SceneAsset _anotherScene = null;
 
   //=================================================================================
   //初期化、破棄
   //=================================================================================
   
   //有効になった時(シーンを開いた時)や、Inspectorで変数(_anotherScene)を変更した時に実行
   private void OnValidate() {
     //最初に再生するシーンを登録
     EditorSceneManager.playModeStartScene = _anotherScene;
   }

   //削除された時(シーンを閉じた時)に実行
   private void OnDestroy() {
     //最初に再生するシーンを削除(他のシーンを再生した時に再生シーンが変わらないように)
     EditorSceneManager.playModeStartScene = null;
   }
 
#endif
}