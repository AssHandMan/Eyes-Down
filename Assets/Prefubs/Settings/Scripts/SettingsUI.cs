using DG.Tweening;
using EyesDown.Core;
using EyesDown.Settings.SettingBlocks;
using Settings.SettingBlocks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace EyesDown.Settings
{
    public class SettingsUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Button _backBtn;
        [Header("Fields")]
        [SerializeField] private SliderSettingBlock _sensivity;
        [SerializeField] private SliderSettingBlock _musicVolume;
        [SerializeField] private SliderSettingBlock _soundsVolume;
        [SerializeField] private SwitchBtnSettingsBlock _windowMode;
        [SerializeField] private ResolutionsDropdownSettingBlock _resolution;
        [SerializeField] private DropdownSettingBlock _fps;
        [SerializeField] private ToggleSettingBlock _vSync;
        [SerializeField] private DropdownSettingBlock _renderingScale;
        [Inject] private SaveLoaderManager _saveLoaderManager;
        private const float _duration = 0.15f;
        
        #region INITIALIZATION
        public void Initialize()
        {
            LoadDefaultValues();
            Bind();
        }

        private void LoadDefaultValues()
        {
            _sensivity.SetValue(_saveLoaderManager.Sensivity.CurrentValue);
            _musicVolume.SetValue(_saveLoaderManager.MusicVolume.CurrentValue);
            _soundsVolume.SetValue(_saveLoaderManager.SoundsVolume.CurrentValue);
            _windowMode.SetValue(_saveLoaderManager.WindowMode.CurrentValue);
            _resolution.SetValue(_saveLoaderManager.Resolution.CurrentValue);
            _fps.SetValue(_saveLoaderManager.FPS.CurrentValue);
            _vSync.SetValue(_saveLoaderManager.VSync.CurrentValue);
            _renderingScale.SetValue(_saveLoaderManager.RenderingScale.CurrentValue);
        }

        private void Bind()
        {
            _sensivity.AddListener((value) => _saveLoaderManager.Sensivity.Value = value);
            _musicVolume.AddListener((value) => _saveLoaderManager.MusicVolume.Value = value);
            _soundsVolume.AddListener((value) => _saveLoaderManager.SoundsVolume.Value = value);
            _windowMode.AddListener((value) => _saveLoaderManager.WindowMode.Value = value);
            _resolution.AddListener((value) => _saveLoaderManager.Resolution.Value = value);
            _fps.AddListener((value) => _saveLoaderManager.FPS.Value = value);
            _vSync.AddListener((value) => _saveLoaderManager.VSync.Value = value);
            _renderingScale.AddListener((value) => _saveLoaderManager.RenderingScale.Value = value);
            _backBtn.onClick.AddListener(() => SetState(false));
        }
        #endregion

        public void SetState(bool value)
        {
            _group.blocksRaycasts = value;
            _group.DOFade(value ? 1 : 0, _duration);
        }
    }
}