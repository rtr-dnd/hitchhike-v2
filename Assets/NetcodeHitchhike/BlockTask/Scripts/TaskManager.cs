using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace NetcodeHitchhike.BlockTask
{
    public class TaskManager : NetworkBehaviour
    {
        [SerializeField] private NetworkVariable<int> stateIndex = new NetworkVariable<int>(
            0,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server // サーバーのみが書き込めるように変更
        );

        [SerializeField] private List<BlockPlaceManager> blockPlaceManagers;
        [SerializeField] private TaskImageManager taskImageManager;
        [SerializeField] private ShowAnswer showAnswer1;
        [SerializeField] private ShowAnswer showAnswer2;
        [SerializeField] private ShowTutorialCard showTutorialCard1;
        [SerializeField] private ShowTutorialCard showTutorialCard2;
        private int internalStateIndex = 0;
        [SerializeField] private bool isTask2 = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public override void OnNetworkSpawn()
        {
            // OnValueChangedイベントにコールバックを登録
            stateIndex.OnValueChanged += OnStateIndexChanged;

            // サーバーのみが初期値を設定する
            if (IsServer)
            {
                stateIndex.Value = 0;
            }

            // 登録後、現在の値でUIを一度更新する（全クライアントで実行）
            // これにより、途中参加したクライアントも正しいスプライトが表示される
            OnStateIndexChanged(0, stateIndex.Value);
        }
        
        public override void OnNetworkDespawn()
        {
            // オブジェクトが破棄される際にイベントの購読を解除
            stateIndex.OnValueChanged -= OnStateIndexChanged;
        }

        // OnValueChangedで呼び出されるメソッド
        private void OnStateIndexChanged(int previousValue, int newValue)
        {
            bool stateIndex2isTask2 = newValue >= 12;
            int stateIndex2taskImageIndex = CalculateTaskImageIndex(newValue - (stateIndex2isTask2 ? 12 : 0));
            if (stateIndex2taskImageIndex == 0)
            {
                ResetAllBlockPlacers();
            }
            taskImageManager.SetImageIndex(stateIndex2taskImageIndex + (stateIndex2isTask2 ? 6 : 0));
            if (newValue == 2)
            {
                showAnswer1.SetEnabledState(true);
            }
            else if (newValue == 14)
            {
                showAnswer2.SetEnabledState(true);
            }
            else
            {
                showAnswer1.SetEnabledState(false);
                showAnswer2.SetEnabledState(false);
            }
            if (stateIndex2taskImageIndex == 1)
            {
                showTutorialCard1.SetEnabledState(!stateIndex2isTask2);
                showTutorialCard2.SetEnabledState(stateIndex2isTask2);
            }
            else
            {
                showTutorialCard1.SetEnabledState(false);
                showTutorialCard2.SetEnabledState(false);
            }


        }

        // Update is called once per frame
        void Update()
        {
            //Debug.Log("yyyy");
            if (!IsServer) return;
            Debug.Log($"Current state index: {stateIndex.Value}, internal index: {internalStateIndex}, isTask2: {isTask2}");
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                int nextIndex = internalStateIndex + 1;
                if (internalStateIndex >= 12)
                {
                    nextIndex = 11;
                }
                internalStateIndex = nextIndex;
                Debug.Log($"State index increased to {stateIndex.Value}");
                stateIndex.Value = internalStateIndex + (isTask2 ? 12 : 0);
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                int nextIndex = internalStateIndex - 1;
                if (nextIndex <= -1)
                {
                    nextIndex = 0;
                }
                internalStateIndex = nextIndex;
                //Debug.Log($"State index decreased to {stateIndex.Value}");
                stateIndex.Value = internalStateIndex + (isTask2 ? 12 : 0);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetAllBlockPlacers();
                Debug.Log("All block placers have been reset.");
            }

            if (Input.GetKeyDown(KeyCode.T))
            {
                isTask2 = !isTask2;
                stateIndex.Value = internalStateIndex + (isTask2 ? 12 : 0);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                internalStateIndex = 0;
                stateIndex.Value = internalStateIndex + (isTask2 ? 12 : 0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                internalStateIndex = 3;
                stateIndex.Value = internalStateIndex + (isTask2 ? 12 : 0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                internalStateIndex = 5;
                stateIndex.Value = internalStateIndex + (isTask2 ? 12 : 0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                internalStateIndex = 7;
                stateIndex.Value = internalStateIndex + (isTask2 ? 12 : 0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                internalStateIndex = 9;
                stateIndex.Value = internalStateIndex + (isTask2 ? 12 : 0);
            }
        }

        private int CalculateTaskImageIndex(int newValue)
        {
            int stateIndex2taskImageIndex = 0;
            if (newValue == 0 || newValue == 1)
            {
                stateIndex2taskImageIndex = newValue;
            }
            else if (newValue % 2 == 0)
            {
                stateIndex2taskImageIndex = newValue / 2;
            }
            else
            {
                stateIndex2taskImageIndex = 0;
            }
            return stateIndex2taskImageIndex;
        }

        private void ResetAllBlockPlacers()
        {
            foreach (var manager in blockPlaceManagers)
            {
                manager.Reset();
            }
        }
    }
}
