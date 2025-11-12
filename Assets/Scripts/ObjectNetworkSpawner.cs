using Mirror;
using UnityEngine;

public class ObjectNetworkSpawner : NetworkBehaviour
{
    [SerializeField] private Transform[] pos;
    [SerializeField] private GameObject Object;

    private void Update()
    {
        if(!isLocalPlayer) return;
        if(Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            SpawnObj();
        }
    }

    [Command]
    private void SpawnObj()
    {
        Debug.Log($"CMD SpawnObj received from {connectionToClient.connectionId}");

        for (int i = 0; i < pos.Length; i++)
        {
            GameObject obj = Instantiate(Object, pos[i].position, Quaternion.identity);
            NetworkServer.Spawn(obj, connectionToClient);
            Debug.Log($"Spawned object for client {connectionToClient.connectionId}");
        }
    }
}
