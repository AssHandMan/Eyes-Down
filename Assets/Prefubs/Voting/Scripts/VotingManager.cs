using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class VotingManager : NetworkBehaviour
{
    public System.Action<List<ModifierData>, System.Action<ModifierData>, System.Action, bool> OnShowVoteData;
    public System.Action<float> OnUpdateTimer;
    public System.Action<Dictionary<string, int>, int> OnVotesDataChanged;
    public System.Action<string> OnModifierWin;
    public System.Action OnVotingEnd;
    private Dictionary<string, int> _votingResults = new ();
    private List<ModifierData> _currentVotingModifiers = new ();
    private Dictionary<uint, string> _playerVotes = new ();
    private Dictionary<uint, bool> _rerollVotes = new ();
    private int _rerollVoteCount;
    private bool _rerollUsed;
    private GameConfig _config;
    private bool _allPlayersVoted;
    private int _totalPlayers;
    private const string REROLL_KEY = "REROLL";

    public void Initialize(GameConfig config)
    {
        _config = config;
    }
    
    [Server]
    public IEnumerator StartVotingPhase(List<GameObject> players)
    {
        _votingResults.Clear();
        _playerVotes.Clear();
        _rerollVotes.Clear();
        _rerollVoteCount = 0;
        _allPlayersVoted = false;
        _totalPlayers = players.Count;

        // Сбрасываем флаг Reroll для новой фазы голосования
        _rerollUsed = false;

        // Если Reroll доступен, выбираем 3 модификатора, иначе 4
        _currentVotingModifiers = SelectRandomModifiers(_config, !_rerollUsed);

        foreach (var modifier in _currentVotingModifiers)
        {
            _votingResults[modifier.ModifierName] = 0;
        }

        string[] modifierNames = GetModifierNames(_currentVotingModifiers);
        RpcShowVotingScreen(modifierNames, !_rerollUsed);

        bool rerollWon;
        do
        {
            rerollWon = false;
            float timeRemaining = _config.VotingTime;
            while (timeRemaining > 0 && !_allPlayersVoted)
            {
                RpcUpdateVotingTimer(timeRemaining);
                yield return new WaitForSeconds(1f);
                timeRemaining -= 1f;
            }

            // Автоматическое голосование убрано - игроки должны сами голосовать

            rerollWon = CalculateVotingResults();

            if (rerollWon)
            {
                _rerollUsed = true;

                // Перегенерируем модификаторы
                _votingResults.Clear();
                _playerVotes.Clear();
                _rerollVotes.Clear();
                _rerollVoteCount = 0;
                _allPlayersVoted = false;

                // После Reroll выбираем 4 модификатора (без Reroll карточки)
                _currentVotingModifiers = SelectRandomModifiers(_config, false);

                foreach (var modifier in _currentVotingModifiers)
                    _votingResults[modifier.ModifierName] = 0;

                modifierNames = GetModifierNames(_currentVotingModifiers);
                RpcShowVotingScreen(modifierNames, false);
            }
        } while (rerollWon);

        yield return new WaitForSeconds(_config.WinnerShowDuration);
        OnVotingEnd?.Invoke();
    }

    [Server]
    private List<ModifierData> SelectRandomModifiers(GameConfig config, bool rerollAvailable)
    {
        List<ModifierData> selectedModifiers = new List<ModifierData>();
        List<ModifierData> tempList = new List<ModifierData>(config.Modifiers);

        // Всегда выбираем 3 модификатора
        // Если Reroll доступен - будет показана 4-я карточка для реролла
        int count = Mathf.Min(3, config.Modifiers.Length);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            selectedModifiers.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        return selectedModifiers;
    }

    private string[] GetModifierNames(List<ModifierData> modifiers)
    {
        string[] names = new string[modifiers.Count];
        for (int i = 0; i < modifiers.Count; i++)
        {
            names[i] = modifiers[i].ModifierName;
        }
        return names;
    }


    [Server]
    private bool CalculateVotingResults()
    {
        VoteCount[] voteCounts = GetVoteCountsArray();
        RpcUpdateVoteCounts(voteCounts, _rerollVoteCount);

        // Находим максимальное количество голосов среди модификаторов
        int maxVotes = _votingResults.Count > 0 ? _votingResults.Max(x => x.Value) : 0;

        // Если Reroll имеет больше голосов, чем любой модификатор - он побеждает
        if (!_rerollUsed && _rerollVoteCount > maxVotes)
        {
            return true;
        }

        // Если Reroll имеет столько же голосов, сколько максимум - добавляем его в список победителей
        List<string> candidates = new List<string>();

        if (!_rerollUsed && _rerollVoteCount == maxVotes && _rerollVoteCount > 0)
        {
            candidates.Add(REROLL_KEY);
        }

        // Добавляем модификаторов с максимальным количеством голосов
        foreach (var kvp in _votingResults)
        {
            if (kvp.Value == maxVotes)
                candidates.Add(kvp.Key);
        }

        // Выбираем случайного победителя
        if (candidates.Count > 0)
        {
            string winner = candidates[Random.Range(0, candidates.Count)];

            if (winner == REROLL_KEY)
            {
                return true;
            }
            else
            {
                RpcHighlightWinner(winner);
                return false;
            }
        }

        // Если никто не проголосовал - выбираем случайный модификатор
        if (_votingResults.Count > 0)
        {
            var randomWinner = _votingResults.ElementAt(Random.Range(0, _votingResults.Count));
            RpcHighlightWinner(randomWinner.Key);
        }

        return false;
    }

    [Command(requiresAuthority = false)]
    public void CmdVoteForModifier(string modifierName, NetworkConnectionToClient sender = null)
    {
        if (sender == null) return;

        uint playerId = sender.identity.netId;

        // Убираем голос за Reroll, если был
        if (_rerollVotes.ContainsKey(playerId))
        {
            _rerollVotes.Remove(playerId);
            _rerollVoteCount--;
        }

        // Если modifierName пустой или null - отменяем голос
        if (string.IsNullOrEmpty(modifierName))
        {
            if (_playerVotes.ContainsKey(playerId))
            {
                string previousVote = _playerVotes[playerId];
                _votingResults[previousVote]--;
                _playerVotes.Remove(playerId);
            }
        }
        else if (_votingResults.ContainsKey(modifierName))
        {
            // Если игрок уже голосовал - отменяем предыдущий голос
            if (_playerVotes.ContainsKey(playerId))
            {
                string previousVote = _playerVotes[playerId];

                // Если голосует за тот же модификатор - это не должно происходить (обрабатывается на клиенте)
                if (previousVote == modifierName) return;

                _votingResults[previousVote]--;
            }

            // Добавляем новый голос
            _playerVotes[playerId] = modifierName;
            _votingResults[modifierName]++;

            // Проверяем, все ли игроки проголосовали
            CheckAllPlayersVoted();
        }

        // Обновляем счетчики на всех клиентах
        VoteCount[] voteCounts = GetVoteCountsArray();
        RpcUpdateVoteCounts(voteCounts, _rerollVoteCount);
    }

    [Command(requiresAuthority = false)]
    public void CmdVoteForReroll(NetworkConnectionToClient sender = null)
    {
        if (sender == null) return;
        if (_rerollUsed) return;

        uint playerId = sender.identity.netId;

        // Если игрок уже голосовал за Reroll - отменяем голос
        if (_rerollVotes.ContainsKey(playerId))
        {
            _rerollVotes.Remove(playerId);
            _rerollVoteCount--;
        }
        else
        {
            // Убираем голос за модификатор, если был
            if (_playerVotes.ContainsKey(playerId))
            {
                string previousVote = _playerVotes[playerId];
                _votingResults[previousVote]--;
                _playerVotes.Remove(playerId);
            }

            // Добавляем голос за Reroll
            _rerollVotes[playerId] = true;
            _rerollVoteCount++;

            // Проверяем, все ли игроки проголосовали
            CheckAllPlayersVoted();
        }

        // Обновляем счетчики на всех клиентах
        VoteCount[] voteCounts = GetVoteCountsArray();
        RpcUpdateVoteCounts(voteCounts, _rerollVoteCount);
    }

    private void CheckAllPlayersVoted()
    {
        int totalVotes = _playerVotes.Count + _rerollVotes.Count;
        if (totalVotes >= _totalPlayers)
            _allPlayersVoted = true;
    }

    private VoteCount[] GetVoteCountsArray()
    {
        VoteCount[] counts = new VoteCount[_votingResults.Count];
        int index = 0;
        foreach (var kvp in _votingResults)
        {
            counts[index] = new VoteCount { modifierName = kvp.Key, count = kvp.Value };
            index++;
        }
        return counts;
    }

    private void ShowVotingScreenLocal(string[] modifierNames, bool rerollAvailable)
    {
        List<ModifierData> tempModifiers = new ();
        foreach (var name in modifierNames)
        {
            ModifierData modifier = _config.Modifiers.FirstOrDefault((x) => x.ModifierName == name);
            if (modifier)
                tempModifiers.Add(modifier);
        }
        OnShowVoteData?.Invoke(tempModifiers, OnPlayerVoted, OnPlayerVotedReroll, rerollAvailable);
    }

    [ClientRpc]
    private void RpcShowVotingScreen(string[] modifierNames, bool rerollAvailable)
    {
        ShowVotingScreenLocal(modifierNames, rerollAvailable);
    }

    [ClientRpc]
    private void RpcUpdateVotingTimer(float timeRemaining)
    {
        OnUpdateTimer?.Invoke(timeRemaining);
    }

    private void UpdateVoteCountsLocal(VoteCount[] voteCounts, int rerollCount)
    {
        Dictionary<string, int> countsDict = new Dictionary<string, int>();
        foreach (var voteCount in voteCounts)
            countsDict[voteCount.modifierName] = voteCount.count;
        OnVotesDataChanged?.Invoke(countsDict, rerollCount);
    }

    [ClientRpc]
    private void RpcUpdateVoteCounts(VoteCount[] voteCounts, int rerollCount)
    {
        UpdateVoteCountsLocal(voteCounts, rerollCount);
    }

    [ClientRpc]
    private void RpcHighlightWinner(string winnerName)
    {
        OnModifierWin?.Invoke(winnerName);
    }

    private void OnPlayerVoted(ModifierData modifier)
    {
        CmdVoteForModifier(!modifier ? "" : modifier.ModifierName);
    }

    private void OnPlayerVotedReroll()
    {
        CmdVoteForReroll();
    }
}

[System.Serializable]
public struct VoteCount
{
    public string modifierName;
    public int count;
}
