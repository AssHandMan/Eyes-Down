using System.Collections;
using UnityEngine;

public class GameUIRoot : MonoBehaviour
{
    [SerializeField] private VotingUI _votingUI;
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
    }

    private IEnumerator WaitForPlayerInitialized(System.Action callback)
    {
        while (!_player)
            yield return new WaitForSeconds(0.1f);
        callback?.Invoke();
    }
}
