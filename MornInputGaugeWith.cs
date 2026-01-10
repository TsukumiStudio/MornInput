using UnityEngine;

namespace MornLib
{
    public sealed class MornInputGaugeWith : MonoBehaviour
    {
        [SerializeField] private MornPressableInputIconSetter _parent;
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationClip _pressing;
        [SerializeField] private AnimationClip _released;
        private bool _cachedLongPressing;

        private void Update()
        {
            var longPressing = _parent.showGauge && _parent.isPressed;
            if (_cachedLongPressing != longPressing)
            {
                _cachedLongPressing = longPressing;
                var clip = longPressing ? _pressing : _released;
                _animator.CrossFadeInFixedTime(clip.name, clip.length, 0, 0);
            }
        }
    }
}