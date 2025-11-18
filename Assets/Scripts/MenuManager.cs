using Mirror;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipText;
    [SerializeField] private NetworkManager networkManager;
    public void ConnectWithLocalIP()
    {
        networkManager.networkAddress = ipText.text;
        networkManager.StartClient();
    }
    public void Return()
    {
        SceneManager.LoadScene(0);
    }
}
