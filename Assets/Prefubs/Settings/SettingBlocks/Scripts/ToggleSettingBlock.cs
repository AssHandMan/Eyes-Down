using UnityEngine;
using UnityEngine.UI;

namespace Settings.SettingBlocks
{
    public class ToggleSettingBlock : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;
        
        public void SetValue(bool state) => _toggle.isOn = state;

        public void AddListener(System.Action<bool> callback)
        {
            _toggle.onValueChanged.AddListener((value) => callback?.Invoke(value));
        }
    }
}