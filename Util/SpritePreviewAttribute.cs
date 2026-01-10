using UnityEngine;

namespace MornLib
{
    internal sealed class SpritePreviewAttribute : PropertyAttribute
    {
        public readonly float Size;

        public SpritePreviewAttribute(float size = 30)
        {
            Size = size;
        }
    }
}