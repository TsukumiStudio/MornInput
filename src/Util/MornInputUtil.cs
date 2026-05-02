using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace MornLib
{
    public static class MornInputUtil
    {
        /// <summary>
        /// InputActionReference が指す Action を、 PlayerInput が clone した actions から取得する。
        /// 同一 InputActionAsset 参照なら id で一致するが、 別 asset 同士で名前だけ揃うケース
        /// (MornUGUIGlobal の Cancel と ゲーム側 PlayerInput の Cancel など) も拾うため、
        /// id ヒット失敗時は action.name で fallback 検索する。
        /// </summary>
        public static InputAction FindAction(this PlayerInput playerInput, InputActionReference reference)
        {
            if (playerInput == null || reference == null || reference.action == null)
            {
                return null;
            }

            var byId = playerInput.actions.FindAction(reference.action.id);
            if (byId != null)
            {
                return byId;
            }

            return playerInput.actions.FindAction(reference.action.name);
        }

        public static bool AnyPressed(this InputAction action)
        {
            return Any(action, control => control.wasPressedThisFrame);
        }

        public static bool AnyReleased(this InputAction action)
        {
            return Any(action, control => control.wasReleasedThisFrame);
        }

        public static bool AnyPressing(this InputAction action)
        {
            return Any(action, control => control.isPressed);
        }

        public static bool AllPressed(this InputAction action)
        {
            return All(action, control => control.wasPressedThisFrame);
        }

        public static bool AllReleased(this InputAction action)
        {
            return All(action, control => control.wasReleasedThisFrame);
        }

        public static bool AllPressing(this InputAction action)
        {
            return All(action, control => control.isPressed);
        }

        private static bool Any(this InputAction action, Func<ButtonControl, bool> func)
        {
            foreach (var control in action.controls)
            {
                if (control is ButtonControl buttonControl && func(buttonControl))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool All(this InputAction action, Func<ButtonControl, bool> func)
        {
            foreach (var control in action.controls)
            {
                if (control is ButtonControl buttonControl && !func(buttonControl))
                {
                    return false;
                }
            }

            return true;
        }
    }
}