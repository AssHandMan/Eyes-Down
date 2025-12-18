using R3;
using UnityEngine;

namespace EyesDown.Core
{
    public class SaveLoaderManager
    {
        public readonly ReactiveProperty<float> Sensivity = new (GetSensivity());
        public readonly ReactiveProperty<float> MusicVolume = new (GetMusicVolume());
        public readonly ReactiveProperty<float> SoundsVolume = new (GetSoundsVolume());
        public readonly ReactiveProperty<int> WindowMode = new (GetWindowMode());
        public readonly ReactiveProperty<string> Resolution  = new (GetResolution());
        public readonly ReactiveProperty<string> FPS  = new (GetFPS());
        public readonly ReactiveProperty<bool> VSync  = new (GetVSync());
        public readonly ReactiveProperty<string> RenderingScale  = new (GetRenderingScale());

        private const float _defaultSense = 100;
        private const float _defaultMusicVolume = 100;
        private const float _defaultSoundsVolume = 100;
        private const int _defaultWindowMode = 0;
        private const string _defaultFPS = "60";
        private const string _defaultRenderingScale = "100%";

        public SaveLoaderManager()
        {
            Sensivity.Subscribe(SetSensivity);
            MusicVolume.Subscribe(SetMusicVolume);
            SoundsVolume.Subscribe(SetSoundsVolume);
            WindowMode.Subscribe(SetWindowMode);
            Resolution.Subscribe(SetResolution);
            FPS.Subscribe(SetFPS);
            VSync.Subscribe(SetVSync);
            RenderingScale.Subscribe(SetRenderingScale);
        }
        
        private static float GetSensivity() => 
            PlayerPrefs.GetFloat("Sensivity", _defaultSense);

        private static void SetSensivity(float value) =>
            PlayerPrefs.SetFloat("Sensivity", value);

        private static void SetMusicVolume(float value) =>
            PlayerPrefs.SetFloat("MusicVolume", value);

        private static float GetMusicVolume() => 
            PlayerPrefs.GetFloat("MusicVolume", _defaultMusicVolume);
        
        private static void SetSoundsVolume(float value) =>
            PlayerPrefs.SetFloat("SoundsVolume", value);

        private static float GetSoundsVolume() => 
            PlayerPrefs.GetFloat("SoundsVolume", _defaultSoundsVolume);

        private static void SetWindowMode(int value) =>
            PlayerPrefs.SetInt("WindowMode", value);

        private static int GetWindowMode() => 
            PlayerPrefs.GetInt("WindowMode", _defaultWindowMode);
        
        private static void SetResolution(string value) =>
            PlayerPrefs.SetString("Resolution", value);

        private static string GetResolution()
        {
            var res = $"{Screen.width}x{Screen.height}";
            return PlayerPrefs.GetString("Resolution", res);
        }
        
        private static void SetFPS(string value) =>
            PlayerPrefs.SetString("FPS", value);

        private static string GetFPS() => 
            PlayerPrefs.GetString("FPS", _defaultFPS);
        
        private static void SetVSync(bool value) =>
            PlayerPrefs.SetInt("VSync", value ? 1 : 0);

        private static bool GetVSync() => 
            PlayerPrefs.GetInt("VSync", 1) == 1;
        
        private static void SetRenderingScale(string value) =>
            PlayerPrefs.SetString("RenderingScale", value);

        private static string GetRenderingScale() => 
            PlayerPrefs.GetString("RenderingScale", _defaultRenderingScale);
    }
}