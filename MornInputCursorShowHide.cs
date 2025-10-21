using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace MornInput
{
    /// <summary>マウス/キー入力に応じてカーソル表示を制御し、非表示時はマウスレイキャストをブロックする</summary>
    public sealed class MornInputCursorShowHide : MonoBehaviour
    {
        private Texture2D _blankTexture;
        private Vector2 _lastMousePosition;
        private readonly float _mouseMoveThreshold = 0.1f;
        private static bool _shouldShowCursor;
        /// <summary>マウスレイキャストが有効かどうか（MouseControlledRaycasterで参照）</summary>
        public static bool IsMouseRaycastEnabled => _shouldShowCursor;

        private void Start()
        {
            // マウスデバイスが存在する場合のみ初期化
            if (Mouse.current != null)
            {
                _lastMousePosition = Mouse.current.position.ReadValue();
            }

            UpdateCursorVisibility(true);
        }

        private void Update()
        {
            // マウスデバイスが存在しない場合はキー入力のみ処理
            if (Mouse.current == null)
            {
                if (CheckAnyNonMouseInput())
                {
                    UpdateCursorVisibility(false);
                }

                return;
            }

            var currentMousePosition = Mouse.current.position.ReadValue();
            var mouseDelta = Vector2.Distance(currentMousePosition, _lastMousePosition);

            // マウスが動いたら表示
            if (mouseDelta > _mouseMoveThreshold)
            {
                UpdateCursorVisibility(true);
                _lastMousePosition = currentMousePosition;
            }
            // キーボードまたはコントローラーの入力があったら非表示
            else if (CheckAnyNonMouseInput())
            {
                UpdateCursorVisibility(false);
            }
        }

        private bool CheckAnyNonMouseInput()
        {
            // キーボードの任意のキー入力をチェック
            if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            {
                return true;
            }

            // ゲームパッドの入力をチェック
            if (Gamepad.current != null)
            {
                // ボタン入力
                if (Gamepad.current.buttonSouth.wasPressedThisFrame
                    || Gamepad.current.buttonNorth.wasPressedThisFrame
                    || Gamepad.current.buttonEast.wasPressedThisFrame
                    || Gamepad.current.buttonWest.wasPressedThisFrame
                    || Gamepad.current.leftShoulder.wasPressedThisFrame
                    || Gamepad.current.rightShoulder.wasPressedThisFrame
                    || Gamepad.current.leftTrigger.wasPressedThisFrame
                    || Gamepad.current.rightTrigger.wasPressedThisFrame
                    || Gamepad.current.startButton.wasPressedThisFrame
                    || Gamepad.current.selectButton.wasPressedThisFrame
                    || Gamepad.current.leftStickButton.wasPressedThisFrame
                    || Gamepad.current.rightStickButton.wasPressedThisFrame
                    || Gamepad.current.dpad.up.wasPressedThisFrame
                    || Gamepad.current.dpad.down.wasPressedThisFrame
                    || Gamepad.current.dpad.left.wasPressedThisFrame
                    || Gamepad.current.dpad.right.wasPressedThisFrame)
                {
                    return true;
                }

                // スティック入力（デッドゾーンを考慮）
                var leftStick = Gamepad.current.leftStick.ReadValue();
                var rightStick = Gamepad.current.rightStick.ReadValue();
                const float deadZone = 0.2f;
                if (leftStick.magnitude > deadZone || rightStick.magnitude > deadZone)
                {
                    return true;
                }
            }

            return false;
        }

        private void UpdateCursorVisibility(bool visible)
        {
            _shouldShowCursor = visible;
        }

        private void LateUpdate()
        {
            if (_blankTexture == null)
            {
                _blankTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                _blankTexture.SetPixel(0, 0, new Color(0, 0, 0, 0));
                _blankTexture.Apply();
            }

            Cursor.SetCursor(_shouldShowCursor ? null : _blankTexture, Vector2.zero, CursorMode.Auto);
            Cursor.visible = _shouldShowCursor;
        }
    }
}