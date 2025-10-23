using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TaskImageManager : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer imageForClient0;
    [SerializeField] private SpriteRenderer imageForClient1;
    [SerializeField] private List<Sprite> taskImages;
    [SerializeField] private bool isTask2 = false;

    // 書き込み権限をOwnerからServerに変更
    private NetworkVariable<int> imageIndex = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server // サーバーのみが書き込めるように変更
    );

    public override void OnNetworkSpawn()
    {
        // OnValueChangedイベントにコールバックを登録
        imageIndex.OnValueChanged += OnImageIndexChanged;

        // サーバーのみが初期値を設定する
        if (IsServer)
        {
            imageIndex.Value = 0;
        }

        // 登録後、現在の値でUIを一度更新する（全クライアントで実行）
        // これにより、途中参加したクライアントも正しいスプライトが表示される
        OnImageIndexChanged(0, imageIndex.Value);
    }
    
    public override void OnNetworkDespawn()
    {
        // オブジェクトが破棄される際にイベントの購読を解除
        imageIndex.OnValueChanged -= OnImageIndexChanged;
    }

    // OnValueChangedで呼び出されるメソッド
    private void OnImageIndexChanged(int previousValue, int newValue)
    {
        // インデックスがリストの範囲外にならないようにチェック
        if (taskImages.Count > newValue * 2 + 1)
        {
            imageForClient0.sprite = taskImages[newValue * 2];
            imageForClient1.sprite = taskImages[newValue * 2 + 1];
            Debug.Log($"Task image changed. New index: {newValue}");
        }
        else
        {
            Debug.LogWarning("Task image index is out of range!");
        }
    }

    void Update()
    {
        // サーバー（ホスト）でなければ入力処理を受け付けない
        if (!IsServer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            int nextIndex = imageIndex.Value + 1;
            // 6になる場合は0に戻す (仮にタスクが6ペアある場合)
            if (nextIndex >= 6)
            {
                nextIndex = 0;
            }
            imageIndex.Value = nextIndex;
        }
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            isTask2 = !isTask2;
        }

        // 数字キーでの直接指定 (こちらは簡略化のためそのまま)
        if (Input.GetKeyDown(KeyCode.Alpha0)) imageIndex.Value = 0 + (isTask2 ? 6 : 0);
        if (Input.GetKeyDown(KeyCode.Alpha1)) imageIndex.Value = 1 + (isTask2 ? 6 : 0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) imageIndex.Value = 2 + (isTask2 ? 6 : 0);
        if (Input.GetKeyDown(KeyCode.Alpha3)) imageIndex.Value = 3 + (isTask2 ? 6 : 0);
        if (Input.GetKeyDown(KeyCode.Alpha4)) imageIndex.Value = 4 + (isTask2 ? 6 : 0);
        if (Input.GetKeyDown(KeyCode.Alpha5)) imageIndex.Value = 5 + (isTask2 ? 6 : 0);
    }
}