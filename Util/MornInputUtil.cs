using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace MornLib
{
    public static class MornInputUtil
    {
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