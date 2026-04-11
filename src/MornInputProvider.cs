#if USE_INPUTSYSTEM
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MornLib
{
    public class MornInputProvider : MonoBehaviour
    {
        [SerializeField] private PlayerInputManager _playerInputManager;
        [SerializeField] private InputActionAsset _globalUIActions;
        private readonly Dictionary<PlayerInput, IMornInput> _inputs = new();

        private void Awake()
        {
            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;

            // グローバルUIアクションを有効化（PlayerInputManager管理外＝デバイス制限なし）
            if (_globalUIActions != null)
            {
                foreach (var actionMap in _globalUIActions.actionMaps)
                {
                    actionMap.Enable();
                }
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
        }
        
        private void OnPlayerLeft(PlayerInput playerInput)
        {
            if (_inputs.Remove(playerInput))
            {
                Debug.Log($"Input removed: index {playerInput.playerIndex}");
            }
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
#endif