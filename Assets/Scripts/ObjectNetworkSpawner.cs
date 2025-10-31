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
            SpawnObj();
        }
    }

    [Command]
    private void SpawnObj()
    {
        for (int i = 0; i < pos.Length; i++)
        {
            GameObject obj = Instantiate(Object, pos[i].position, Quaternion.identity);
            NetworkServer.Spawn(obj, connectionToClient);
        }
    }
}
