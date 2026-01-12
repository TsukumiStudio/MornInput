using UnityEngine;

namespace MornLib
{
    [CreateAssetMenu(fileName = nameof(MornInputGlobal), menuName = "Morn/" + nameof(MornInputGlobal))]
    internal sealed class MornInputGlobal : MornGlobalBase<MornInputGlobal>
    {
        protected override string ModuleName => "MornInput";
        [SerializeField] private string _defaultSchemeKey;
        public string DefaultSchemeKey => _defaultSchemeKey;

        internal static void SetDirty(Object obj)
        {
            I.SetDirtyInternal(obj);
        }
    }
}