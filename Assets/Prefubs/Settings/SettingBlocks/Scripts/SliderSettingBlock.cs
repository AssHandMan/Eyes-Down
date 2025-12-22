using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Settings.SettingBlocks
{
    public class SliderSettingBlock : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TMP_Text _valueTxt;

        private void Awake()
        {
            _slider.onValueChanged.AddListener((value) => _valueTxt.text = ((int)value).ToString());
        }

        public void AddListener(System.Action<float> callback)
        {
            _slider.onValueChanged.AddListener((value) => callback.Invoke(value));
        }

        public void SetValue(float value)
        {
            _slider.value = value;
            _valueTxt.text = ((int)value).ToString();
        }
    }
}
