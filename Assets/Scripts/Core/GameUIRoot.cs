using System.Collections;
using EyesDown.Settings;
using UnityEngine;
using UnityEngine.UI;

public class GameUIRoot : MonoBehaviour
{
    [SerializeField] private VotingUI _votingUI;
    [SerializeField] private SettingsUI _settingsUI;
    [SerializeField] private Button _settingsBtn;
    [SerializeField] private Button _questionaireBtn;
    private PlayerController _player;
    
    
    public void Bind(VotingManager votingManager, GameConfig config, GameManager gameManager)
    {
        gameManager.OnPlayerInitialized += (player) => _player = player;
        
        votingManager.OnShowVoteData += _votingUI.ShowVotingScreen;
        votingManager.OnUpdateTimer += _votingUI.UpdateTimer;
        votingManager.OnVotesDataChanged += _votingUI.UpdateVoteCounts;
        votingManager.OnModifierWin += _votingUI.HighlightWinner;
        votingManager.OnVotingStarted += () =>
        {
            StartCoroutine(WaitForPlayerInitialized(() =>
            {
                _player.CameraController.UnlockCursor();
                _player.CameraController.LockCamera();
            }));
        };

        votingManager.OnVotingEnd += () =>
        {
            StartCoroutine(WaitForPlayerInitialized(() =>
            {
                _player.CameraController.LockCursor();
                _player.CameraController.UnlockCamera();
            }));
        };
        _votingUI.Initialize(config);
        
        _settingsUI.Initialize();
        _settingsBtn.onClick.AddListener(() => _settingsUI.SetState(true));
        
        _questionaireBtn.onClick.AddListener(() => Application.OpenURL(config.QuestionaireLink));
    }

    private IEnumerator WaitForPlayerInitialized(System.Action callback)
    {
        while (!_player)
            yield return new WaitForSeconds(0.1f);
        callback?.Invoke();
    }
}
