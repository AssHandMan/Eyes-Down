using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameUIRoot _gameUIRoot;
    [SerializeField] private VotingManager _votingManager;
    [SerializeField] private GameConfig _config;
    [SerializeField] private GameManager _gameManager;
    private PlayerController _player;
    
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _votingManager.Initialize(_config);
        _votingManager.OnVotingStarted += () =>
        {
            if (_player) _player.FocusedOnUI.Value = true;
        };
        _votingManager.OnVotingEnd += () =>
        {
            if (_player) _player.FocusedOnUI.Value = false;
        };
        
        _gameManager.Initialize(_votingManager);
        _gameManager.OnPlayerInitialized += (player) => _player = player;
        
        _gameUIRoot.Bind(_votingManager, _config, _gameManager);
    }
}
