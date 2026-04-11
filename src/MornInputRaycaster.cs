using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MornLib
{
    [RequireComponent(typeof(Canvas))]
    public sealed class MornInputRaycaster : GraphicRaycaster
    {
        public override void Raycast(PointerEventData eventData, System.Collections.Generic.List<RaycastResult> resultAppendList)
        {
#if USE_INPUTSYSTEM
            // カーソルが非表示の時はマウスによるレイキャストを行わない
            if (!MornInputCursorShowHide.IsMouseRaycastEnabled)
            {
                return;
            }
#endif
            base.Raycast(eventData, resultAppendList);
        }
    }
}