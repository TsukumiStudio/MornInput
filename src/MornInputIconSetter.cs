using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using VContainer;

namespace MornLib
{
    [ExecuteAlways]
    internal sealed class MornInputIconSetter : MonoBehaviour
    {
        private static readonly int _fillAmount = Shader.PropertyToID("_FillAmount");
        private static readonly int _mainTex = Shader.PropertyToID("_MainTex");
        [FormerlySerializedAs("_settings")] public MornInputIconSettings Settings;
        [SerializeField] private Image _image;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private SpriteMask _spriteMask;
#if USE_INPUTSYSTEM
        [Inject] private MornInputProvider _mornInputProvider;
#endif
        public Color IconColor
        {
            get => _image != null ? _image.color :
                _spriteRenderer != null ? _spriteRenderer.color : Color.clear;
            set
            {
                if (_image != null)
                {
                    _image.color = value;
                }
                else if (_spriteRenderer != null)
                {
                    _spriteRenderer.color = value;
                }
            }
        }
        private Material _cachedMaterial;
        private MaterialPropertyBlock _propertyBlock;

        private void OnDestroy()
        {
            if (_cachedMaterial != null)
            {
                Destroy(_cachedMaterial);
            }
        }

        public float FillAmount
        {
            set
            {
                if (_image != null)
                {
                    _image.fillAmount = value;
                }
                else if (_spriteRenderer != null)
                {
                    _propertyBlock.SetFloat(_fillAmount, value);
                    _spriteRenderer.SetPropertyBlock(_propertyBlock);
                }
            }
        }

        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                if (_spriteRenderer != null)
                {
                    _cachedMaterial = new Material(_spriteRenderer.material);
                    _spriteRenderer.material = _cachedMaterial;
                    _propertyBlock = new MaterialPropertyBlock();
                    _spriteRenderer.SetPropertyBlock(_propertyBlock);
                }

#if USE_INPUTSYSTEM
                var input = _mornInputProvider.GetInput(0);
                if (input != null)
                {
                    Adjust(input.CurrentScheme, true);
                    input.OnSchemeChanged.Subscribe(x => Adjust(x.next, false)).AddTo(this);
                }
                else
                {
                    Adjust(MornInputGlobal.I.DefaultSchemeKey, true);
                }
#endif
            }
        }

        private void Reset()
        {
            _image = GetComponent<Image>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteMask = GetComponent<SpriteMask>();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                Adjust(MornInputGlobal.I.DefaultSchemeKey, false);
            }
        }

        private void Adjust(string schemeKey, bool force)
        {
            if (Settings == null)
            {
                return;
            }

            var sprite = Settings.GetSpriteSettings(schemeKey);
            if (sprite == null)
            {
                return;
            }

            if (_image != null && (force || _image.sprite != sprite))
            {
                _image.sprite = sprite;
                MornGlobalUtil.SetDirty(_image);
            }

            if (_spriteRenderer != null && (force || _spriteRenderer.sprite != sprite))
            {
                _spriteRenderer.sprite = sprite;
                _propertyBlock.SetTexture(_mainTex, sprite.texture);
                MornGlobalUtil.SetDirty(_spriteRenderer);
            }

            if (_spriteMask != null && (force || _spriteMask.sprite != sprite))
            {
                _spriteMask.sprite = sprite;
                MornGlobalUtil.SetDirty(_spriteMask);
            }
        }
    }
}