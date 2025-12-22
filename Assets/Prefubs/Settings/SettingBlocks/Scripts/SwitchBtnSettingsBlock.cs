using System;
using EyesDown.Settings.Elements;
using UnityEngine;

namespace EyesDown.Settings.SettingBlocks
{
    public class SwitchBtnSettingsBlock : MonoBehaviour
    {
        [SerializeField] private SwitchBtn[] _btns;
        private Action<int> _onValueChanged;    
        
        private void Awake()
        {
            foreach (var btn in _btns)
                btn.AddListener(() => OnBtnClicked(btn));
        }

        private void OnBtnClicked(SwitchBtn button)
        {
            for (int i = 0; i < _btns.Length; i++)
            {
                var btn = _btns[i];
                btn.SetState(btn == button);
                if (btn == button)
                    _onValueChanged?.Invoke(i);
            }
        }

        public void SetValue(int index)
        {
            OnBtnClicked(_btns[index]);
        }

        public void AddListener(Action<int> callback)
        {
            _onValueChanged += callback;
        }
    }
}
