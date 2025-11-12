
using UnityEngine;
using Mirror;
using TMPro;
using System.Collections.Generic;

public class GameManager : NetworkBehaviour
{
    [SerializeField] private Transform center;
    [SerializeField] private float radius = 5f;

    private readonly List<GameObject> spawnedPlayers = new List<GameObject>();

    [Server]
    public void AddPlayer(GameObject player)
    {
        if (!spawnedPlayers.Contains(player))
            spawnedPlayers.Add(player);

        ArrangePlayersInCircle();
    }

    [Server]
    public void RemovePlayer(GameObject player)
    {
        if (spawnedPlayers.Contains(player))
            spawnedPlayers.Remove(player);

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

    public void ExitTheGame()
    {
        Application.Quit();
    }

}