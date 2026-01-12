using System.Linq;
using UnityEngine;

namespace MornLib
{
    [ExecuteAlways]
    public sealed class MornPressableInputIconSetter : MonoBehaviour
    {
        public bool isGrayscale = false;
        public bool isPressed = false;
        public bool showGauge = false;
        public float gaugeValue;
        [Header("top")]
        [SerializeField] private MornInputIconSettings _topSettings;
        [SerializeField] private MornInputIconSetter _topBase;
        [Header("top")]
        [SerializeField] private MornInputIconSettings _topMaskSettings;
        [SerializeField] private MornInputIconSetter _topMaskBase;
        [Header("bottom")]
        [SerializeField] private MornInputIconSettings _bottomSettings;
        [SerializeField] private MornInputIconSetter _bottomBase;
        [Header("gauge")]
        [SerializeField] private MornInputIconSettings _gaugeSettings;
        [SerializeField] private MornInputIconSetter _gaugeFillBase;
        [SerializeField] private MornInputIconSetter _gaugeEmptyBase;
        [Header("icon")]
        [SerializeField] private MornInputIconSettings _iconSettings;
        [SerializeField] private MornInputIconSetter _icon;
        [SerializeField] private MornPressableInputIconSettings _pressableSettings;
        private bool _beforeIsGrayscale = false;
        private bool _beforeIsPressed = false;
        private bool _beforeShowGauge = false;
        private float _beforeGaugeValue = 0;

        private void OnEnable()
        {
            if (Application.isPlaying)
            {
                Adjust();
            }
        }

        private void Reset()
        {
            _topBase = GetComponentsInChildren<MornInputIconSetter>().FirstOrDefault(x => x.name == "Top");
            _bottomBase = GetComponentsInChildren<MornInputIconSetter>().FirstOrDefault(x => x.name == "Bottom");
            _gaugeEmptyBase = GetComponentsInChildren<MornInputIconSetter>()
               .FirstOrDefault(x => x.name == "GaugeEmpty");
            _gaugeFillBase = GetComponentsInChildren<MornInputIconSetter>().FirstOrDefault(x => x.name == "GaugeFill");
            _icon = GetComponentsInChildren<MornInputIconSetter>().FirstOrDefault(x => x.name == "Icon");
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                Adjust();
            }

            if (_beforeIsPressed != isPressed || _beforeIsGrayscale != isGrayscale)
            {
                _icon.IconColor = isGrayscale ? _pressableSettings.IconColor :
                    isPressed ? _pressableSettings.IconPressedColor : _pressableSettings.IconColor;
                _topBase.IconColor = isGrayscale ? _pressableSettings.GrayTopColor :
                    isPressed ? _pressableSettings.TopPressedColor : _pressableSettings.TopColor;
                _bottomBase.IconColor =
                    isGrayscale ? _pressableSettings.GrayBottomColor : _pressableSettings.BottomColor;
                var localPos = isPressed ? _pressableSettings.PressedOffset : _pressableSettings.NormalOffset;
                _topBase.transform.localPosition = localPos;
                _gaugeEmptyBase.transform.localPosition = localPos;
                _gaugeFillBase.transform.localPosition = localPos;
                _bottomBase.gameObject.SetActive(!isPressed);
                _beforeIsPressed = isPressed;
                _beforeIsGrayscale = isGrayscale;
            }

            if (_beforeShowGauge != showGauge)
            {
                _gaugeEmptyBase.gameObject.SetActive(showGauge);
                _gaugeFillBase.gameObject.SetActive(showGauge);
                _beforeShowGauge = showGauge;
            }

            if (!Mathf.Approximately(_beforeGaugeValue, gaugeValue))
            {
                if (showGauge)
                {
                    _gaugeFillBase.FillAmount = gaugeValue;
                }

                _beforeGaugeValue = gaugeValue;
            }
        }

        private void Adjust()
        {
            if (_topSettings != null && _topBase.Settings != _topSettings)
            {
                _topBase.Settings = _topSettings;
                MornInputGlobal.Logger.Log("Settings Changed");
                MornInputGlobal.SetDirty(_topBase);
            }

            if (_topMaskSettings != null && _topMaskBase.Settings != _topMaskSettings)
            {
                _topMaskBase.Settings = _topMaskSettings;
                MornInputGlobal.Logger.Log("Settings Changed");
                MornInputGlobal.SetDirty(_topMaskBase);
            }

            if (_bottomSettings != null && _bottomBase.Settings != _bottomSettings)
            {
                _bottomBase.Settings = _bottomSettings;
                MornInputGlobal.Logger.Log("Settings Changed");
                MornInputGlobal.SetDirty(_bottomBase);
            }

            if (_gaugeSettings != null)
            {
                if (_gaugeEmptyBase.Settings != _gaugeSettings)
                {
                    _gaugeEmptyBase.Settings = _gaugeSettings;
                    MornInputGlobal.Logger.Log("Settings Changed");
                    MornInputGlobal.SetDirty(_gaugeEmptyBase);
                }

                if (_gaugeFillBase.Settings != _gaugeSettings)
                {
                    _gaugeFillBase.Settings = _gaugeSettings;
                    MornInputGlobal.Logger.Log("Settings Changed");
                    MornInputGlobal.SetDirty(_gaugeFillBase);
                }
            }

            if (_iconSettings != null && _icon.Settings != _iconSettings)
            {
                _icon.Settings = _iconSettings;
                MornInputGlobal.Logger.Log("Settings Changed");
                MornInputGlobal.SetDirty(_icon);
            }

            if (isPressed == _bottomBase.gameObject.activeSelf)
            {
                _bottomBase.gameObject.SetActive(!isPressed);
                MornInputGlobal.Logger.Log("BottomBase Changed");
                MornInputGlobal.SetDirty(_bottomBase);
            }

            var topColor = isPressed ? _pressableSettings.TopPressedColor : _pressableSettings.TopColor;
            if (_topBase.IconColor != topColor)
            {
                _topBase.IconColor = topColor;
                MornInputGlobal.Logger.Log("TopColor Changed");
                MornInputGlobal.SetDirty(_topBase);
            }

            if (_bottomBase.IconColor != _pressableSettings.BottomColor)
            {
                _bottomBase.IconColor = _pressableSettings.BottomColor;
                MornInputGlobal.Logger.Log("BottomColor Changed");
                MornInputGlobal.SetDirty(_bottomBase);
            }

            var iconColor = isPressed ? _pressableSettings.IconPressedColor : _pressableSettings.IconColor;
            if (_icon.IconColor != iconColor)
            {
                _icon.IconColor = iconColor;
                MornInputGlobal.Logger.Log("BottomColor Changed");
                MornInputGlobal.SetDirty(_bottomBase);
            }

            if (_gaugeEmptyBase.IconColor != _pressableSettings.GaugeEmptyColor)
            {
                _gaugeEmptyBase.IconColor = _pressableSettings.GaugeEmptyColor;
                MornInputGlobal.Logger.Log("GaugeEmptyColor Changed");
                MornInputGlobal.SetDirty(_gaugeEmptyBase);
            }

            if (_gaugeFillBase.IconColor != _pressableSettings.GaugeFillColor)
            {
                _gaugeFillBase.IconColor = _pressableSettings.GaugeFillColor;
                MornInputGlobal.Logger.Log("GaugeFillColor Changed");
                MornInputGlobal.SetDirty(_gaugeFillBase);
            }

            if (_gaugeEmptyBase.gameObject.activeSelf != showGauge)
            {
                _gaugeEmptyBase.gameObject.SetActive(showGauge);
                MornInputGlobal.Logger.Log("GaugeEmptyBase Changed");
                MornInputGlobal.SetDirty(_gaugeEmptyBase);
            }

            if (_gaugeFillBase.gameObject.activeSelf != showGauge)
            {
                _gaugeFillBase.gameObject.SetActive(showGauge);
                MornInputGlobal.Logger.Log("GaugeFillBase Changed");
                MornInputGlobal.SetDirty(_gaugeFillBase);
            }

            var localPos = isPressed ? _pressableSettings.PressedOffset : _pressableSettings.NormalOffset;
            if (_topBase.transform.localPosition != localPos)
            {
                _topBase.transform.localPosition = localPos;
                MornInputGlobal.Logger.Log("TopBase Changed");
                MornInputGlobal.SetDirty(_topBase);
            }

            if (_gaugeEmptyBase.transform.localPosition != localPos)
            {
                _gaugeEmptyBase.transform.localPosition = localPos;
                MornInputGlobal.Logger.Log("GaugeEmptyBase Changed");
                MornInputGlobal.SetDirty(_gaugeEmptyBase);
            }

            if (_gaugeFillBase.transform.localPosition != localPos)
            {
                _gaugeFillBase.transform.localPosition = localPos;
                MornInputGlobal.Logger.Log("GaugeFillBase Changed");
                MornInputGlobal.SetDirty(_gaugeFillBase);
            }
        }
    }
}