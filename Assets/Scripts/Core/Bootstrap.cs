using System;
using EyesDown.Core;
using EyesDown.Settings;
using Mirror;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameUIRoot _gameUIRoot;
    [SerializeField] private VotingManager _votingManager;
    [SerializeField] private GameConfig _config;
    [SerializeField] private GameManager _gameManager;
    [SerializeField] private AudioMixer _audioMixer;
    [Inject] private SaveLoaderManager _saveLoaderManager;
    private PlayerController _player;
    private SettingsUseCase _settings;

    [Server]
    private void ActivateGameManager()
    {
        _gameManager.gameObject.SetActive(true);
    }

    private void Start()
    {
        _settings = new SettingsUseCase(_saveLoaderManager, _audioMixer);
        _settings.Bind();
        _settings.SetVolumeSettings();
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        ActivateGameManager();
        _votingManager.Initialize(_config);
        _votingManager.OnVotingStarted += () =>
        {
            if (_player) _player.FocusedOnUI.Value = true;
        };
        _votingManager.OnVotingEnd += () =>
        {
            if (_player) _player.FocusedOnUI.Value = false;
        };
        
        _gameManager.Initialize(_votingManager, _config);
        _gameManager.OnPlayerInitialized += (player) =>
        {
            _player = player;
            _player.Initialize(_saveLoaderManager);
        };
        
        _gameUIRoot.Bind(_votingManager, _config, _gameManager);
    }
}
