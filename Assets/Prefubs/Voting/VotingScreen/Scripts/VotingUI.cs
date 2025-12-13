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
    [SerializeField] private ModifierData _rerollModifierData;

    private ModifierData _selectedModifier;
    private System.Action<ModifierData> _currentOnVote;
    private System.Action _currentOnRerollVote;
    private GameConfig _config;
    private ModifierCardUI _rerollCard;

    private const float _fadeDuration = 0.3f;
    private const string _timerFormat = "Осталось: {0}";
    private const string REROLL_NAME = "Реролл";

    public void Initialize(GameConfig config)
    {
        _config = config;

        // Убеждаемся, что у Reroll ModifierData правильное имя
        if (_rerollModifierData != null)
            _rerollModifierData.ModifierName = REROLL_NAME;
    }
    
    public void ShowVotingScreen(List<ModifierData> modifiers, System.Action<ModifierData> onVote, System.Action onRerollVote, bool rerollAvailable)
    {
        SetState(true);

        _selectedModifier = null;
        _currentOnVote = onVote;
        _currentOnRerollVote = onRerollVote;
        _rerollCard = null;

        // Сначала скрываем все карточки и сбрасываем их состояние
        foreach (var card in _modifierCards)
        {
            card.gameObject.SetActive(false);
            card.SetSelected(false);
        }

        // Инициализируем карточки модификаторов
        int cardIndex = 0;
        for (int i = 0; i < modifiers.Count && cardIndex < _modifierCards.Length; i++, cardIndex++)
        {
            _modifierCards[cardIndex].gameObject.SetActive(true);
            _modifierCards[cardIndex].Initialize(modifiers[i], (modifier) => OnModifierVoted(modifier));
        }

        // Если Reroll доступен, используем следующую карточку для него
        if (rerollAvailable && cardIndex < _modifierCards.Length)
        {
            _rerollCard = _modifierCards[cardIndex];
            _rerollCard.gameObject.SetActive(true);
            _rerollCard.Initialize(_rerollModifierData, (modifier) => OnModifierVoted(modifier));
            cardIndex++;
        }

        _timerText.gameObject.SetActive(true);
    }

    private void OnModifierVoted(ModifierData modifier)
    {
        // Проверяем, является ли это Reroll карточкой (по ссылке или по имени)
        bool isReroll = modifier == _rerollModifierData ||
                        (modifier != null && modifier.ModifierName == REROLL_NAME);

        // Если нажали на тот же модификатор/Reroll - отменяем голос
        if (_selectedModifier && _selectedModifier == modifier)
        {
            UpdateCardSelection(modifier.ModifierName, false);
            _selectedModifier = null;

            if (isReroll)
                _currentOnRerollVote?.Invoke();
            else
                _currentOnVote?.Invoke(null);
        }
        else
        {
            // Если был выбран другой модификатор - снимаем с него выделение
            if (_selectedModifier)
                UpdateCardSelection(_selectedModifier.ModifierName, false);

            // Выбираем новый модификатор/Reroll
            _selectedModifier = modifier;
            UpdateCardSelection(modifier.ModifierName, true);

            if (isReroll)
                _currentOnRerollVote?.Invoke();
            else
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
    }

    public void UpdateTimer(float timeRemaining)
    {
        _timerText.text = string.Format(_timerFormat, Mathf.CeilToInt(timeRemaining));
    }

    public void UpdateVoteCounts(Dictionary<string, int> voteCounts, int rerollCount)
    {
        foreach (var card in _modifierCards)
        {
            if (!card.gameObject.activeSelf) continue;

            string modifierName = card.GetModifierName();

            // Обновляем счетчик для Reroll карточки
            if (_rerollCard != null && card == _rerollCard)
            {
                card.UpdateVoteCount(rerollCount);
            }
            // Обновляем счетчик для обычных модификаторов
            else if (voteCounts.TryGetValue(modifierName, out var count))
            {
                card.UpdateVoteCount(count);
            }
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
