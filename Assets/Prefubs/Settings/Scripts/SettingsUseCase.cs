using System.Linq;
using EyesDown.Core;
using R3;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace EyesDown.Settings
{
    public class SettingsUseCase
    {
        private SaveLoaderManager _saveLoaderManager;
        private AudioMixer _audioMixer;
        private UniversalRenderPipelineAsset _urpAsset;
        
        public SettingsUseCase(SaveLoaderManager saveLoaderManager, AudioMixer audioMixer)
        {
            _urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
            _saveLoaderManager = saveLoaderManager;
            _audioMixer = audioMixer;
        }
        
        public void Bind()
        {
            _saveLoaderManager
                .SoundsVolume
                .Subscribe(SetSoundsVolume);
            
            _saveLoaderManager
                .MusicVolume
                .Subscribe(SetMusicVolume);
            
            _saveLoaderManager
                .WindowMode
                .Subscribe(SetFullscreenMode);
            
            _saveLoaderManager
                .Resolution
                .Subscribe(SetResolution);
            
            _saveLoaderManager
                .FPS
                .Subscribe(SetFPS);
            
            _saveLoaderManager
                .RenderingScale
                .Subscribe(SetRenderingScale);
            
            _saveLoaderManager
                .VSync
                .Subscribe(SetVSync);
        }

        public void SetVolumeSettings()
        {
            SetSoundsVolume(_saveLoaderManager.SoundsVolume.CurrentValue);
            SetMusicVolume(_saveLoaderManager.MusicVolume.CurrentValue);
        }
        
        private void SetSoundsVolume(float value)
        {
            _audioMixer.SetFloat("SoundsVolume", -80 * (1 - value / 100f));
        }
        
        private void SetMusicVolume(float value)
        {
            _audioMixer.SetFloat("MusicVolume", -80 * (1 - value / 100f));
        }
        
        private void SetFullscreenMode(int value)
        {
            switch (value)
            {
                case 0:
                    Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                    break;
                case 1:
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                    break;
                case 2:
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                    break;
            }
        }

        private void SetResolution(string value)
        {
            var res = value.Split("x").Select(int.Parse).ToArray();
            Screen.SetResolution(res[0], res[1], Screen.fullScreenMode);
        }

        private void SetFPS(string value)
        {
            if (value == "Без ограничений")
                Application.targetFrameRate = 999;
            else
                Application.targetFrameRate = int.Parse(value);
        }

        private void SetRenderingScale(string value)
        {
            var val = value.Substring(0, value.Length - 1);
            _urpAsset.renderScale = int.Parse(val) / 100f;
        }

        private void SetVSync(bool value)
        {
            QualitySettings.vSyncCount = value ? 1 : 0;
        }
    }
}
