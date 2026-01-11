using UnityEngine;

namespace MornLib
{
    [CreateAssetMenu(fileName = nameof(MornInputGlobal), menuName = "Morn/" + nameof(MornInputGlobal))]
    internal sealed class MornInputGlobal : MornGlobalBase<MornInputGlobal>
    {
        protected override string ModuleName => "MornInput";
        [SerializeField] private string _defaultSchemeKey;
        public string DefaultSchemeKey => _defaultSchemeKey;

        internal static void Log(string message)
        {
            I.LogInternal(message);
        }

        internal static void LogError(string message)
        {
            I.LogErrorInternal(message);
        }

        internal static void LogWarning(string message)
        {
            I.LogWarningInternal(message);
        }

        internal static void SetDirty(Object obj)
        {
            I.SetDirtyInternal(obj);
        }
    }
}