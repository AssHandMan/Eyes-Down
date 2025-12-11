using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private GameUIRoot _gameUIRoot;
    [SerializeField] private VotingManager _votingManager;
    [SerializeField] private GameConfig _config;
    [SerializeField] private GameManager _gameManager;
    
    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        _votingManager.Initialize(_config);
        _gameManager.Initialize(_votingManager);
        _gameUIRoot.Bind(_votingManager, _config);
    }
}
