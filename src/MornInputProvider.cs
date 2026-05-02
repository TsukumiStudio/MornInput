using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MornLib
{
    public class MornInputProvider : MonoBehaviour
    {
        [SerializeField] private PlayerInputManager _playerInputManager;
        [SerializeField] private InputActionAsset _globalUIActions;
        [Tooltip("グローバル UI 入力を最若 playerIndex のプレイヤーが保持するデバイスにのみ限定する。" +
                 "未 join 時は誰も UI 操作できない。 OFF にすると従来通り全デバイスから受け付ける。")]
        [SerializeField] private bool _restrictGlobalUIToFirstPlayer = true;
        private readonly Dictionary<PlayerInput, IMornInput> _inputs = new();
        /// <summary>PlayerInput の join/leave で発火。購読側は登録中の最小 playerIndex 等を再計算する</summary>
        public static event Action OnPlayerInputsChanged;

        private void Awake()
        {
            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;

            // グローバルUIアクションを有効化（PlayerInputManager管理外＝デバイス制限はここで決める）
            if (_globalUIActions != null)
            {
                foreach (var actionMap in _globalUIActions.actionMaps)
                {
                    actionMap.Enable();
                }
                // 起動直後はまだ 1P が居ないので、 まず制限なしで開始 (UI が一切操作不能になる事故防止)。
                // 1P join 後に RefreshGlobalUIDevices で 1P デバイスへ縛る。
                _globalUIActions.devices = null;
            }
        }

        private void Start()
        {
            // プレイヤーが未Joinの場合、1人目を自動Joinさせる
            if (_inputs.Count == 0)
            {
                _playerInputManager.JoinPlayer();
            }
        }

        private void OnPlayerJoined(PlayerInput playerInput)
        {
            playerInput.transform.SetParent(transform);
            _inputs.Add(playerInput, new MornInputHandler(playerInput));
            Debug.Log($"Input added: index {playerInput.playerIndex}");
            RefreshGlobalUIDevices();
            OnPlayerInputsChanged?.Invoke();
        }

        private void OnPlayerLeft(PlayerInput playerInput)
        {
            if (_inputs.Remove(playerInput))
            {
                Debug.Log($"Input removed: index {playerInput.playerIndex}");
                RefreshGlobalUIDevices();
                OnPlayerInputsChanged?.Invoke();
            }
        }

        private void RefreshGlobalUIDevices()
        {
            if (_globalUIActions == null) return;
            if (!_restrictGlobalUIToFirstPlayer)
            {
                _globalUIActions.devices = null;
                return;
            }

            PlayerInput first = null;
            foreach (var pi in _inputs.Keys)
            {
                if (first == null || pi.playerIndex < first.playerIndex)
                {
                    first = pi;
                }
            }

            if (first == null)
            {
                _globalUIActions.devices = null;
                return;
            }

            var devices = first.devices;
            if (devices.Count == 0)
            {
                // ペア未確立は安全側で制限なし (UI が一切操作できない事故防止)
                _globalUIActions.devices = null;
                return;
            }

            _globalUIActions.devices = devices.ToArray();
        }

        public IReadOnlyDictionary<PlayerInput, IMornInput> Inputs => _inputs;

        public PlayerInputManager PlayerInputManager => _playerInputManager;

        /// <summary>デバイス制限なしのグローバルUIアクションを取得</summary>
        public InputAction GetGlobalUIAction(string actionName)
        {
            return _globalUIActions?.FindAction(actionName);
        }

        public IMornInput GetInput(int playerIndex)
        {
            foreach (var input in _inputs)
            {
                if (input.Key.playerIndex == playerIndex)
                {
                    return input.Value;
                }
            }
            return null;
        }
    }
}
