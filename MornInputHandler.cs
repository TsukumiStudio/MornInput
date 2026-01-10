using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine.InputSystem;

namespace MornLib
{
    public class MornInputHandler : IMornInput
    {
        private readonly PlayerInput _playerInput;

        public MornInputHandler(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }

        public string CurrentScheme => _playerInput.currentControlScheme;
        private readonly Subject<(string prev, string next)> _schemeSubject = new();
        public IObservable<(string prev, string next)> OnSchemeChanged => _schemeSubject;
        private string _cachedControlScheme;

        private readonly Dictionary<string, InputAction> _cachedActionDictionary = new();

        bool IMornInput.IsPressedAny(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AnyPressed() ?? false;
        }

        bool IMornInput.IsPressedAll(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AllPressed() ?? false;
        }

        bool IMornInput.IsPerformed(string actionName)
        {
            var action = GetAction(actionName);
            return action?.WasPerformedThisFrame() ?? false;
        }

        bool IMornInput.IsPressingAny(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AnyPressing() ?? false;
        }

        bool IMornInput.IsPressingAll(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AllPressing() ?? false;
        }

        bool IMornInput.IsReleaseAny(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AnyReleased() ?? false;
        }

        bool IMornInput.IsReleaseAll(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AllReleased() ?? false;
        }

        T IMornInput.ReadValue<T>(string actionName)
        {
            var action = GetAction(actionName);
            return action != null ? action.ReadValue<T>() : default;
        }

        private InputAction GetAction(string actionName)
        {
            try
            {
                if (string.IsNullOrEmpty(actionName))
                {
                    MornInputGlobal.LogError("Action name is null or empty");
                    return null;
                }

                if (_cachedActionDictionary.TryGetValue(actionName, out var action))
                {
                    return action;
                }

                if (_playerInput?.actions == null)
                {
                    MornInputGlobal.LogError("PlayerInput or actions is null");
                    return null;
                }

                action = _playerInput.actions[actionName];
                if (action == null)
                {
                    MornInputGlobal.LogWarning($"Action '{actionName}' not found in PlayerInput");
                    return null;
                }

                _cachedActionDictionary[actionName] = action;
                return action;
            }
            catch (Exception ex)
            {
                MornInputGlobal.LogError($"Error getting action '{actionName}': {ex.Message}");
                return null;
            }
        }
    }
}