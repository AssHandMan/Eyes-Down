using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class VotingUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private ModifierCardUI[] _modifierCards;
    [SerializeField] private TMP_Text _timerText;

    private ModifierData _selectedModifier;
    private System.Action<ModifierData> _currentOnVote;
    private GameConfig _config;

    private const float _fadeDuration = 0.3f;
    private const string _timerFormat = "Осталось: {0}";

    public void Initialize(GameConfig config)
    {
        _config = config;
    }
    
    public void ShowVotingScreen(List<ModifierData> modifiers, System.Action<ModifierData> onVote)
    {
        SetState(true);
        
        _selectedModifier = null;
        _currentOnVote = onVote;

        for (int i = 0; i < _modifierCards.Length && i < modifiers.Count; i++)
        {
            _modifierCards[i].gameObject.SetActive(true);
            _modifierCards[i].Initialize(modifiers[i], (modifier) => OnModifierVoted(modifier));
            _modifierCards[i].SetSelected(false);
        }
        _timerText.gameObject.SetActive(true);
    }

    private void OnModifierVoted(ModifierData modifier)
    {
        // Если нажали на тот же модификатор - отменяем голос
        if (_selectedModifier && _selectedModifier.ModifierName == modifier.ModifierName)
        {
            UpdateCardSelection(modifier.ModifierName, false);
            _selectedModifier = null;
            _currentOnVote?.Invoke(null);
        }
        else
        {
            // Если был выбран другой модификатор - снимаем с него выделение
            if (_selectedModifier)
                UpdateCardSelection(_selectedModifier.ModifierName, false);

            // Выбираем новый модификатор
            _selectedModifier = modifier;
            UpdateCardSelection(modifier.ModifierName, true);
            _currentOnVote?.Invoke(modifier);
        }
    }

    private void UpdateCardSelection(string modifierName, bool isSelected)
    {
        foreach (var card in _modifierCards)
        {
            if (card.gameObject.activeSelf && card.GetModifierName() == modifierName)
            {
                card.SetSelected(isSelected);
                break;
            }
        }
    }
    
    private void SetState(bool state)
    {
        _group.DOFade(state ? 1 : 0, _fadeDuration);
        _group.blocksRaycasts = state;
        
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }
    
    public void UpdateTimer(float timeRemaining)
    {
        _timerText.text = string.Format(_timerFormat, Mathf.CeilToInt(timeRemaining));
    }

    public void UpdateVoteCounts(Dictionary<string, int> voteCounts)
    {
        foreach (var card in _modifierCards)
        {
            string modifierName = card.GetModifierName();
            if (voteCounts.TryGetValue(modifierName, out var count))
                card.UpdateVoteCount(count);
        }
    }

    public void HighlightWinner(string winnerName)
    {
        _timerText.gameObject.SetActive(false);

        // Отключаем возможность голосовать и сбрасываем цвета
        foreach (var card in _modifierCards)
        {
            card.SetInteractable(false);
            if (card.GetModifierName() == winnerName)
                card.HighlightAsWinner();
            else
                card.SetCardState(CardState.Normal);
        }

        DOVirtual.DelayedCall(_config.WinnerShowDuration, () => SetState(false));
    }
}
