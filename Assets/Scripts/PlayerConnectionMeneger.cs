
using UnityEngine;
using Mirror;

public class PlayerConnectionMeneger : NetworkBehaviour
{
    private GameManager manager;

    public override void OnStartServer()
    {
        base.OnStartServer();
        manager = FindAnyObjectByType<GameManager>();
        if (manager != null)
            manager.AddPlayer(gameObject);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        if (manager != null)
            manager.RemovePlayer(gameObject);
    }
}
