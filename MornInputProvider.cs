#if USE_INPUTSYSTEM
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace MornLib
{
    public class MornInputProvider : MonoBehaviour
    {
        [SerializeField] private PlayerInputManager _playerInputManager;
        [SerializeField] private InputSystemUIInputModule _uiInputModule;
        private readonly Dictionary<PlayerInput, IMornInput> _inputs = new();

        private void Awake()
        {
            _playerInputManager.onPlayerJoined += OnPlayerJoined;
            _playerInputManager.onPlayerLeft += OnPlayerLeft;
            SetUpUIInputModule();
        }

        private void Start()
        {
            // プレイヤーが未Joinの場合、1人目を自動Joinさせる
            if (_inputs.Count == 0)
            {
                _playerInputManager.JoinPlayer();
            }
        }

        /// <summary>UIInputModuleのActionAssetを独立インスタンスに差し替え、全デバイスのUI操作を受け付ける</summary>
        private void SetUpUIInputModule()
        {
            if (_uiInputModule == null)
            {
                return;
            }

            // PlayerInputManagerが管理するActionAssetとは別のインスタンスを作り、デバイス制限を受けないようにする
            var originalAsset = _uiInputModule.actionsAsset;
            if (originalAsset == null)
            {
                return;
            }

            var clonedAsset = Instantiate(originalAsset);
            _uiInputModule.actionsAsset = clonedAsset;

            // クローンしたActionAssetの全アクションを有効化
            foreach (var actionMap in clonedAsset.actionMaps)
            {
                actionMap.Enable();
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