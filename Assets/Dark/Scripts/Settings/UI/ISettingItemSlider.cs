using UnityEngine.UI;

namespace Dark.Scripts.Settings.UI
{
    public interface ISettingItemSlider : ISettingItemLogic<Slider>
    {
        new void Initialize(Slider slider);
    }

}