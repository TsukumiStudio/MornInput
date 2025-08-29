using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MornInput
{
    [RequireComponent(typeof(Canvas))]
    public sealed class MornInputRaycaster : GraphicRaycaster
    {
        public override void Raycast(PointerEventData eventData, System.Collections.Generic.List<RaycastResult> resultAppendList)
        {
            // カーソルが非表示の時はマウスによるレイキャストを行わない
            if (!MornInputCursorShowHide.IsMouseRaycastEnabled)
            {
                return;
            }
            
            base.Raycast(eventData, resultAppendList);
        }
    }
}