using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Collections;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private float radius = 5f;
    [SerializeField] private List<LaptopController> laptops;
    [SerializeField] private List<PlayerConnectionMeneger> playerConnections;
    private List<GameObject> spawnedPlayers = new List<GameObject>();
    private float timePrepare, timeBidding;

    [Server]
    public void AddPlayer(GameObject player)
    {
        if (!spawnedPlayers.Contains(player))
            spawnedPlayers.Add(player);

        ArrangePlayersInCircle();
        laptops.Add(player.GetComponent<PlayerConnectionMeneger>().Getlaptop());
        playerConnections.Add(player.GetComponent<PlayerConnectionMeneger>());
    }

    [Server]
    public void RemovePlayer(GameObject player)
    {
        if (spawnedPlayers.Contains(player))
        {
            laptops.Remove(player.GetComponent<PlayerConnectionMeneger>().Getlaptop());
            playerConnections.Remove(player.GetComponent<PlayerConnectionMeneger>());
            spawnedPlayers.Remove(player);
        }

        ArrangePlayersInCircle();
    }

    [Server]
    private void ArrangePlayersInCircle()
    {
        int count = spawnedPlayers.Count;
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            float angle = (i / (float)count) * 360f;
            Quaternion rotation = Quaternion.Euler(0, angle, 0);
            Vector3 pos = center.position + rotation * Vector3.forward * radius;

            spawnedPlayers[i].transform.position = pos;
            spawnedPlayers[i].transform.LookAt(new Vector3(center.position.x, spawnedPlayers[i].transform.position.y, center.position.z));
        }
    }

    [Server]
    public void SetReadyPlayer()
    {
        int count = 0;
        for (int i = 0; i < playerConnections.Count; i++)
        {
            if (playerConnections[i].GetReady()) count++;
        }
        if (count > 0)
        {
            StartCoroutine(GameStageProcces());
        }
    }

    [Server]
    public void SetGameStage(string type)
    {
        for (int i = 0; i < playerConnections.Count; i++)
        {
            playerConnections[i].SetStage(type);
        }
    }

    private IEnumerator GameStageProcces()
    {
        yield return new WaitForSeconds(3);
        SetGameStage("Preparing");
        yield return new WaitForSeconds(3);
        SetGameStage("Game");
        yield return new WaitForSeconds(3);
        SetGameStage("Preparing2");
        yield return new WaitForSeconds(3);
        SetGameStage("Game2");
    }

    public void ExitTheGame()
    {
        Application.Quit();
    }
}
