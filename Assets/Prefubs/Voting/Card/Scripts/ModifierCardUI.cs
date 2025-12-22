using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ModifierCardUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _modifierNameText;
    [SerializeField] private TMP_Text _voteCountText;
    [SerializeField] private Button _voteButton;
    [SerializeField] private Image _cardBackground;

    [Header("Colors")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _selectedColor = Color.green;
    [SerializeField] private Color _winnerColor = Color.yellow;

    private ModifierData _modifierData;
    private System.Action<ModifierData> _onVoteCallback;
    private int _voteCount;
    private bool _isInteractable = true;
    private const string _votesFormat = "Голосов: {0}";

    public void Initialize(ModifierData data, System.Action<ModifierData> onVote)
    {
        _modifierData = data;
        _onVoteCallback = onVote;
        _modifierNameText.text = data.ModifierName;

        _voteButton.onClick.RemoveListener(OnVoteButtonClicked);
        _voteButton.onClick.AddListener(OnVoteButtonClicked);

        _voteCount = 0;
        _isInteractable = true;
        SetCardState(CardState.Normal);
        UpdateVoteCount(0);
    }

    private void OnVoteButtonClicked()
    {
        if (!_isInteractable) return;
        _onVoteCallback?.Invoke(_modifierData);
    }

    public void SetSelected(bool isSelected)
    {
        SetCardState(isSelected ? CardState.Selected : CardState.Normal);
    }

    public void SetInteractable(bool interactable)
    {
        _isInteractable = interactable;
    }

    public void UpdateVoteCount(int count)
    {
        _voteCount = count;
        _voteCountText.text = string.Format(_votesFormat, _voteCount);
    }

    public void SetCardState(CardState state)
    {
        Color targetColor = state switch
        {
            CardState.Selected => _selectedColor,
            CardState.Winner => _winnerColor,
            _ => _normalColor
        };

        _cardBackground.DOColor(targetColor, 0.3f);
    }

    public void HighlightAsWinner()
    {
        SetCardState(CardState.Winner);
        transform.DOPunchScale(Vector3.one * 0.2f, 0.5f, 5, 0.5f);
    }

    public string GetModifierName()
    {
        return _modifierData?.ModifierName ?? "";
    }

    private void OnDestroy()
    {
        _voteButton.onClick.RemoveListener(OnVoteButtonClicked);
        DOTween.Kill(transform);
        DOTween.Kill(_cardBackground);
    }
}

public enum CardState
{
    Normal,
    Selected,
    Winner
}
