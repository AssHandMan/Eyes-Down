using UnityEngine;
using Mirror;

public class ServerPortSetter : MonoBehaviour
{
    void Start()
    {
        if (SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null)
        {
            NetworkManager.singleton.StartServer();
            Debug.Log("Server started in headless mode");
        }
    }
}