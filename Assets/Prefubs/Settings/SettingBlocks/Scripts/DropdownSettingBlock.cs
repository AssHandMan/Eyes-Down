using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace EyesDown.Settings.SettingBlocks
{
    public class DropdownSettingBlock : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        [SerializeField] private bool _useValues;
        [ShowIf("_useValues")]
        [SerializeField] private string[] _values;
        private Action<string> _onValueChanged;
        
        private void Awake()
        {
            if (!_useValues)
                _values = GetValues();
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var v in _values)
                options.Add(new TMP_Dropdown.OptionData(v));
            _dropdown.options = options;
            
            _dropdown.onValueChanged.AddListener((index) => _onValueChanged?.Invoke(_values[index]));
        }

        public void AddListener(Action<string> callback)
        {
            _onValueChanged += callback;
        }

        public void SetValue(string value)
        {
            for (int i = 0;i < _values.Length;i++)
            {
                if (value == _values[i])
                {
                    _dropdown.value = i;
                    break;
                }
            }
        }

        protected virtual string[] GetValues()
        {
            return _values;
        }

        public void SetEnabled(bool value)
        {
            _dropdown.interactable = value;
        }
    }
}
