using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
public class ShowAnswer : NetworkBehaviour
{
    // 同期したい「有効/無効」の状態を保持する変数
    // デフォルトは true (有効)
    // サーバー（ホスト）だけが書き込み可能、全員が読み取り可能
    private NetworkVariable<bool> isEnabled = new NetworkVariable<bool>(
        false, // デフォルト値
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);
        
    private List<Renderer> renderers = new List<Renderer>();

    public override void OnNetworkSpawn()
    {
        // ネットワーク変数の値が変更されたときに、ローカルのenabled状態を更新する
        isEnabled.OnValueChanged += OnEnabledChanged;
        renderers = this.GetComponentsInChildren<Renderer>().ToList();

        // ★重要: スポーン時（または途中参加時）に、現在の最新の値で初期状態を設定する
        OnEnabledChanged(isEnabled.Value, isEnabled.Value);
    }

    public override void OnNetworkDespawn()
    {
        // オブジェクトが破棄されるときは、イベント購読を解除する
        isEnabled.OnValueChanged -= OnEnabledChanged;
    }

    /// <summary>
    /// NetworkVariable の値が変更されたときに呼び出される
    /// </summary>
    private void OnEnabledChanged(bool previousValue, bool newValue)
    {
        // ネットワーク変数の値 (newValue) を、このコンポーネントの
        // 実際の 'enabled' プロパティに反映させる
        foreach (var renderer in renderers)
        {
            renderer.enabled = newValue;
        }

        // (応用) もしGameObject自体を非表示にしたい場合は
        // gameObject.SetActive(newValue);
        // ※ただし、下記「注意点」を参照
    }

    /// <summary>
    /// サーバーが状態を変更するための公開メソッド (例)
    /// </summary>
    public void SetEnabledState(bool newState)
    {
        // サーバー（ホスト）でなければ何もしない
        if (!IsServer) return;

        // ネットワーク変数の値を変更する
        // これにより、全クライアントで OnValueChanged が呼び出される
        isEnabled.Value = newState;
    }

}