using UnityEngine;
using kcp2k;

public class TransportLogger : MonoBehaviour
{
    void Start()
    {
        KcpTransport transport = FindObjectOfType<KcpTransport>();
        if (transport != null)
        {
            transport.OnClientConnected = () => Debug.Log("TRANSPORT: Client connected");
            transport.OnClientDataReceived = (data, channel) => Debug.Log($"TRANSPORT: Data received {data.Count} bytes");
            transport.OnClientError = (error, reason) => Debug.LogError($"TRANSPORT ERROR: {error} - {reason}");
            transport.OnClientDisconnected = () => Debug.Log("TRANSPORT: Client disconnected");
        }
    }
}