using Unity.Netcode;
using UnityEngine;

public class ImageVisibility : NetworkBehaviour
{
    [Tooltip("このオブジェクトを表示させたいクライアントのID")]
    public ulong targetClientId = 1;

    public override void OnNetworkSpawn()
    {
        // 全てのクライアント上でこの処理が実行される
        var renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            Debug.LogError("Rendererコンポーネントが見つかりません！", this.gameObject);
            return;
        }

        // 自分のローカルClientIdとターゲットのIDを比較
        if (NetworkManager.Singleton.LocalClientId == targetClientId)
        {
            // IDが一致したので、表示する (Rendererをオン)
            renderer.enabled = true;
        }
        else
        {
            // IDが一致しなかったので、非表示にする (Rendererをオフ)
            renderer.enabled = false;
        }
    }
}