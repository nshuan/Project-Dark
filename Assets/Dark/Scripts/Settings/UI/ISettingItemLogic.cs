using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public interface ISettingItemLogic<in T> where T : Selectable
    {
        void Initialize(T target) { }
        void Save();
        void UpdateValue(bool onEnable);
    }
}