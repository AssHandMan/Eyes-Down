using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class VotingManager : NetworkBehaviour
{
    public System.Action<List<ModifierData>, System.Action<ModifierData>> OnShowVoteData;
    public System.Action<float> OnUpdateTimer;
    public System.Action<Dictionary<string, int>> OnVotesDataChanged;
    public System.Action<string> OnModifierWin;
    private Dictionary<string, int> _votingResults = new ();
    private List<ModifierData> _currentVotingModifiers = new ();
    private Dictionary<uint, string> _playerVotes = new ();
    private GameConfig _config;
    private bool _allPlayersVoted;
    private int _totalPlayers;

    public void Initialize(GameConfig config)
    {
        _config = config;
    }
    
    [Server]
    public IEnumerator StartVotingPhase(List<GameObject> players)
    {
        _votingResults.Clear();
        _playerVotes.Clear();
        _allPlayersVoted = false;
        _totalPlayers = players.Count;
        _currentVotingModifiers = SelectRandomModifiers(_config);

        foreach (var modifier in _currentVotingModifiers)
        {
            _votingResults[modifier.ModifierName] = 0;
        }

        string[] modifierNames = GetModifierNames(_currentVotingModifiers);
        RpcShowVotingScreen(modifierNames);

        float timeRemaining = _config.VotingTime;
        while (timeRemaining > 0 && !_allPlayersVoted)
        {
            RpcUpdateVotingTimer(timeRemaining);
            yield return new WaitForSeconds(1f);
            timeRemaining -= 1f;
        }

        if (!_allPlayersVoted)
        {
            HandleVotingTimeout(players);
        }

        CalculateVotingResults();

        yield return new WaitForSeconds(_config.WinnerShowDuration);
    }

    [Server]
    private List<ModifierData> SelectRandomModifiers(GameConfig config)
    {
        List<ModifierData> selectedModifiers = new List<ModifierData>();
        List<ModifierData> tempList = new List<ModifierData>(config.Modifiers);

        int count = Mathf.Min(4, config.Modifiers.Length);
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
    private void HandleVotingTimeout(List<GameObject> players)
    {
        foreach (var player in players)
        {
            var netId = player.GetComponent<NetworkIdentity>();
            if (!_playerVotes.ContainsKey(netId.netId))
            {
                int randomModifierIndex = Random.Range(0, _currentVotingModifiers.Count);
                string randomModifierName = _currentVotingModifiers[randomModifierIndex].ModifierName;
                _votingResults[randomModifierName]++;
                _playerVotes[netId.netId] = randomModifierName;
            }
        }
    }

    [Server]
    private void CalculateVotingResults()
    {
        VoteCount[] voteCounts = GetVoteCountsArray();
        RpcUpdateVoteCounts(voteCounts);

        // Находим максимальное количество голосов
        int maxVotes = _votingResults.Max(x => x.Value);

        // Находим всех модификаторов с максимальным количеством голосов
        var winners = _votingResults.Where(x => x.Value == maxVotes).ToList();

        // Выбираем случайного победителя из тех, кто набрал максимальное количество голосов
        var winner = winners[Random.Range(0, winners.Count)];

        RpcHighlightWinner(winner.Key);
    }

    [Command(requiresAuthority = false)]
    public void CmdVoteForModifier(string modifierName, NetworkConnectionToClient sender = null)
    {
        if (sender == null) return;

        uint playerId = sender.identity.netId;

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
            if (_playerVotes.Count >= _totalPlayers)
                _allPlayersVoted = true;
        }

        // Обновляем счетчики на всех клиентах
        VoteCount[] voteCounts = GetVoteCountsArray();
        RpcUpdateVoteCounts(voteCounts);
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

    private void ShowVotingScreenLocal(string[] modifierNames)
    {
        List<ModifierData> tempModifiers = new ();
        foreach (var name in modifierNames)
        {
            ModifierData modifier = _config.Modifiers.FirstOrDefault((x) => x.ModifierName == name);
            if (modifier)
                tempModifiers.Add(modifier);
        }
        OnShowVoteData?.Invoke(tempModifiers, OnPlayerVoted);
    }

    [ClientRpc]
    private void RpcShowVotingScreen(string[] modifierNames)
    {
        ShowVotingScreenLocal(modifierNames);
    }

    [ClientRpc]
    private void RpcUpdateVotingTimer(float timeRemaining)
    {
        OnUpdateTimer?.Invoke(timeRemaining);
    }

    private void UpdateVoteCountsLocal(VoteCount[] voteCounts)
    {
        Dictionary<string, int> countsDict = new Dictionary<string, int>();
        foreach (var voteCount in voteCounts)
            countsDict[voteCount.modifierName] = voteCount.count;
        OnVotesDataChanged?.Invoke(countsDict);
    }

    [ClientRpc]
    private void RpcUpdateVoteCounts(VoteCount[] voteCounts)
    {
        UpdateVoteCountsLocal(voteCounts);
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
}

[System.Serializable]
public struct VoteCount
{
    public string modifierName;
    public int count;
}
