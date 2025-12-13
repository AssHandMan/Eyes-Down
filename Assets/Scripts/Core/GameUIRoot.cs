using UnityEngine;

public class GameUIRoot : MonoBehaviour
{
    [SerializeField] private VotingUI _votingUI;

    public void Bind(VotingManager votingManager, GameConfig config, GameManager gameManager)
    {
        gameManager.OnPlayerInitialized += (player) =>
        {
            votingManager.OnShowVoteData += (_, _, _, _) =>
            {
                player.CameraController.UnlockCursor();
                player.CameraController.LockCamera();
            };

            votingManager.OnVotingEnd += () =>
            {
                player.CameraController.LockCursor();
                player.CameraController.UnlockCamera();
            };
        };
        
        votingManager.OnShowVoteData += _votingUI.ShowVotingScreen;
        votingManager.OnUpdateTimer += _votingUI.UpdateTimer;
        votingManager.OnVotesDataChanged += _votingUI.UpdateVoteCounts;
        votingManager.OnModifierWin += _votingUI.HighlightWinner;

        _votingUI.Initialize(config);
    }
}
