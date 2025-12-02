using UnityEngine;
using Mirror;

public class PlayerConnectionMeneger : NetworkBehaviour
{
    [SerializeField] private LaptopController laptop;
    [SerializeField] private bool isReady;
    private GameManager manager;

    [SyncVar(hook = nameof(OnStageChanged))]
    public string stage;

    public override void OnStartServer()
    {
        base.OnStartServer();
        manager = FindAnyObjectByType<GameManager>();
        if (manager != null)
        {
            manager.AddPlayer(gameObject);
            isReady = true;
            manager.SetReadyPlayer();
        }
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        if (manager != null)
            manager.RemovePlayer(gameObject);
    }

    public bool GetReady()
    {
        return isReady;
    }

    public LaptopController Getlaptop()
    {
        return laptop;
    }

    [Server]
    public void SetStage(string newStage)
    {
        stage = newStage;
    }

    void OnStageChanged(string oldVal, string newVal)
    {
        laptop.SetGameStage(newVal);
    }
}
