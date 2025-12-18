using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace EyesDown.Settings.Elements
{
    public class SwitchBtn : MonoBehaviour
    {
        [SerializeField] private Button _btn;
        [SerializeField] private Image _back;
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _defaultColor;

        private const float _duration = 0.3f;
        
        public void SetState(bool state)
        {
            _back.DOColor(state ? _activeColor : _defaultColor, _duration);
        }

        public void AddListener(System.Action callback)
        {
            _btn.onClick.AddListener(() => callback?.Invoke());
        }
    }

}
