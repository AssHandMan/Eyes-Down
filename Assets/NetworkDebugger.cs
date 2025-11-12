using Mirror;
using UnityEngine;

public class NetworkDebugger : MonoBehaviour
{
    void Start()
    {

        // Подписаться на события NetworkClient
        NetworkClient.OnConnectedEvent += OnClientConnected;
        NetworkClient.OnDisconnectedEvent += OnClientDisconnected;

        // Подписаться на события NetworkServer
        NetworkServer.OnConnectedEvent += OnServerClientConnected;
        NetworkServer.OnDisconnectedEvent += OnServerClientDisconnected;
    }

    // Клиентские события
    private void OnClientConnected()
    {
        Debug.Log("Клиент подключен к серверу");
    }

    private void OnClientDisconnected()
    {
        Debug.Log("Клиент отключен от сервера");
    }

    // Серверные события
    private void OnServerClientConnected(NetworkConnectionToClient conn)
    {
        Debug.Log($"Клиент подключился к серверу: ID {conn.connectionId}, IP: {conn.address}");
    }

    private void OnServerClientDisconnected(NetworkConnectionToClient conn)
    {
        Debug.Log($"Клиент отключился от сервера: ID {conn.connectionId}");
    }

    void OnDestroy()
    {
        // Отписаться от событий
        NetworkClient.OnConnectedEvent -= OnClientConnected;
        NetworkClient.OnDisconnectedEvent -= OnClientDisconnected;
        NetworkServer.OnConnectedEvent -= OnServerClientConnected;
        NetworkServer.OnDisconnectedEvent -= OnServerClientDisconnected;
    }
}