using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Game/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Common")] 
    public string QuestionaireLink;
    
    [Header("Modifiers & Voting")]
    public ModifierData[] Modifiers;
    public float VotingTime = 30f;
    public float WinnerShowDuration = 6f;
    
}
