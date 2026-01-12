using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MornLib
{
    [RequireComponent(typeof(PlayerInput))]
    public sealed class MornInputSolver : MonoBehaviour, IMornInput
    {
        [SerializeField] private PlayerInput _playerInput;
        private readonly Dictionary<string, InputAction> _cachedActionDictionary = new();
        private readonly Subject<(string prev, string next)> _schemeSubject = new();
        private string _cachedControlScheme;
        string IMornInput.CurrentScheme => _playerInput.currentControlScheme;
        IObservable<(string prev, string next)> IMornInput.OnSchemeChanged => _schemeSubject;

        private void Awake()
        {
            UpdateScheme();
        }

        private void Reset()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            UpdateScheme();
        }

        private void UpdateScheme()
        {
            try
            {
                if (_playerInput == null)
                {
                    MornInputGlobal.Logger.LogError("PlayerInput is null in MornInputSolver.Update");
                    return;
                }

                var currentControlScheme = _playerInput.currentControlScheme;
                if (_cachedControlScheme == currentControlScheme)
                {
                    return;
                }

                _schemeSubject.OnNext((_cachedControlScheme, currentControlScheme));
                MornInputGlobal.Logger.Log(
                    $"ControlScheme changed: {_cachedControlScheme ?? "None"} -> {currentControlScheme}");
                _cachedControlScheme = currentControlScheme;
            }
            catch (Exception ex)
            {
                MornInputGlobal.Logger.LogError($"Error in MornInputSolver.Update: {ex.Message}");
            }
        }

        public bool IsPressedAny(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AnyPressed() ?? false;
        }

        public bool IsPressedAll(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AllPressed() ?? false;
        }

        public bool IsPerformed(string actionName)
        {
            var action = GetAction(actionName);
            return action?.WasPerformedThisFrame() ?? false;
        }

        public bool IsPressingAny(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AnyPressing() ?? false;
        }

        public bool IsPressingAll(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AllPressing() ?? false;
        }

        public bool IsReleaseAny(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AnyReleased() ?? false;
        }

        public bool IsReleaseAll(string actionName)
        {
            var action = GetAction(actionName);
            return action?.AllReleased() ?? false;
        }

        public T ReadValue<T>(string actionName) where T : struct
        {
            var action = GetAction(actionName);
            return action != null ? action.ReadValue<T>() : default(T);
        }

        private InputAction GetAction(string actionName)
        {
            try
            {
                if (string.IsNullOrEmpty(actionName))
                {
                    MornInputGlobal.Logger.LogError("Action name is null or empty");
                    return null;
                }

                if (_cachedActionDictionary.TryGetValue(actionName, out var action))
                {
                    return action;
                }

                if (_playerInput?.actions == null)
                {
                    MornInputGlobal.Logger.LogError("PlayerInput or actions is null");
                    return null;
                }

                action = _playerInput.actions[actionName];
                if (action == null)
                {
                    MornInputGlobal.Logger.LogWarning($"Action '{actionName}' not found in PlayerInput");
                    return null;
                }

                _cachedActionDictionary[actionName] = action;
                return action;
            }
            catch (Exception ex)
            {
                MornInputGlobal.Logger.LogError($"Error getting action '{actionName}': {ex.Message}");
                return null;
            }
        }
    }
}