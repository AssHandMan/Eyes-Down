using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class EdgegapManager : MonoBehaviour
{
    private string apiToken = "token 8a2a8aee-9727-4d86-b3cd-c6000cf481d6";
    private string appName = "eyes-down";
    private string currentDeploymentId;

    public void Play()
    {
        Debug.Log("Play() called");
        StartCoroutine(PlayCoroutine());
    }

    private IEnumerator PlayCoroutine()
    {
        Debug.Log("Starting connection process");

        if (!string.IsNullOrEmpty(currentDeploymentId))
        {
            Debug.Log("Using existing deployment: " + currentDeploymentId);
            yield return StartCoroutine(GetServerAddress(currentDeploymentId, ConnectToServer));
            yield break;
        }

        Debug.Log("Looking for active deployments");
        yield return StartCoroutine(FindActiveDeployment());

        if (!string.IsNullOrEmpty(currentDeploymentId))
        {
            Debug.Log("Found active deployment: " + currentDeploymentId);
            yield return StartCoroutine(GetServerAddress(currentDeploymentId, ConnectToServer));
        }
        else
        {
            Debug.Log("No active deployments found, creating new one");
            yield return StartCoroutine(CreateAndConnect());
        }
    }

    private IEnumerator FindActiveDeployment()
    {
        string url = "https://api.edgegap.com/v1/deployments";
        Debug.Log("Requesting active deployments: " + url);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", apiToken);
            yield return request.SendWebRequest();

            Debug.Log("Request status: " + request.result);

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Response received: " + request.downloadHandler.text);
                var response = JsonUtility.FromJson<DeploymentListResponse>(request.downloadHandler.text);

                if (response.data != null && response.data.Length > 0)
                {
                    foreach (var dep in response.data)
                    {
                        Debug.Log("Checking deployment: " + dep.request_id + " status: " + dep.status);
                        // Ищем ЛЮБОЙ активный деплоймент
                        if (dep.status == "Ready" || dep.status == "Deploying" || dep.status == "Running" || dep.status == "Status.READY")
                        {
                            currentDeploymentId = dep.request_id; 
                            Debug.Log("Found active deployment: " + currentDeploymentId + " with status: " + dep.status);
                            break;
                        }
                    }
                }
                else
                {
                    Debug.Log("No active deployments found");
                }
            }
            else
            {
                Debug.LogError("Request error: " + request.error);
            }
        }
    }

    private IEnumerator CreateAndConnect()
    {
        Debug.Log("Creating new deployment");

        string url = "https://api.edgegap.com/v1/deploy";

        // Исправленный JSON с указанием локации
        string json = @"{
            ""app_name"": """ + appName + @""",
            ""version_name"": ""25.11.11-16.35.00-UTC"",
            ""ip_list"": [""auto""]
        }";

        Debug.Log("JSON data: " + json);

        byte[] body = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", apiToken);

            Debug.Log("Sending deployment creation request");
            yield return request.SendWebRequest();

            Debug.Log("Creation status: " + request.result);
            Debug.Log("HTTP response code: " + request.responseCode);
            Debug.Log("Error: " + request.error);
            Debug.Log("Response: " + request.downloadHandler?.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Deployment created successfully: " + request.downloadHandler.text);
                var response = JsonUtility.FromJson<DeploymentResponse>(request.downloadHandler.text);
                currentDeploymentId = response.request_id;

                Debug.Log("Waiting for server to be ready...");
                yield return new WaitForSeconds(10f);
                yield return StartCoroutine(GetServerAddress(currentDeploymentId, ConnectToServer));
            }
            else
            {
                Debug.LogError("Deployment creation failed: " + request.error);
                if (request.downloadHandler != null && !string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError("Error details: " + request.downloadHandler.text);
                }
            }
        }
    }

    private IEnumerator GetServerAddress(string deploymentId, System.Action<string, int> onComplete)
    {
        string url = "https://api.edgegap.com/v1/status/" + deploymentId;
        Debug.Log("Проверяю статус сервера: " + deploymentId);

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("Authorization", apiToken);
            yield return request.SendWebRequest();

            Debug.Log("HTTP код: " + request.responseCode);
            Debug.Log("Ответ: " + request.downloadHandler.text);

            if (request.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<StatusResponseNew>(request.downloadHandler.text);
                Debug.Log("Статус: " + response.current_status);

                // Ждем не только "Status.READY", но и другие рабочие статусы
                if (response.current_status == "Status.READY" || response.current_status == "Status.RUNNING")
                {
                    Debug.Log("Сервер готов! Подключаюсь...");
                    int port = response.ports.gameport.external;
                    onComplete?.Invoke(response.public_ip, port);
                }
                else if (response.current_status == "Status.DEPLOYING")
                {
                    Debug.Log("Сервер еще разворачивается, жду 10 секунд");
                    yield return new WaitForSeconds(10f);
                    yield return StartCoroutine(GetServerAddress(deploymentId, onComplete));
                }
                else
                {
                    Debug.Log("Сервер в статусе: " + response.current_status + ", жду 5 секунд");
                    yield return new WaitForSeconds(5f);
                    yield return StartCoroutine(GetServerAddress(deploymentId, onComplete));
                }
            }
            else
            {
                Debug.LogError("Ошибка запроса статуса: " + request.error);
            }
        }
    }

    private void ConnectToServer(string host, int port)
    {
        Debug.Log("Connecting to server: " + host + ":" + port);

        NetworkManager networkManager = FindObjectOfType<NetworkManager>();
        if (networkManager != null)
        {
            networkManager.networkAddress = host;
            networkManager.GetComponent<kcp2k.KcpTransport>().Port = (ushort)port;
            networkManager.StartClient();
            Debug.Log("Client started connecting");
        }
        else
        {
            Debug.LogError("NetworkManager not found!");
        }
    }

    [System.Serializable]
    private class DeploymentResponse
    {
        public string request_id;
    }

    [System.Serializable]
    private class StatusResponse
    {
        public string public_ip;
        public Port[] ports;
        public string status;
    }

    [System.Serializable]
    private class Port
    {
        public int external;
    }

    [System.Serializable]
    private class DeploymentListResponse
    {
        public DeploymentData[] data;
    }

    [System.Serializable]
    private class DeploymentData
    {
        public string request_id;
        public string status;
    }

    [System.Serializable]
    private class StatusResponseNew
    {
        public string request_id;
        public string public_ip;
        public PortsContainer ports;
        public string current_status;
    }

    [System.Serializable]
    private class PortsContainer
    {
        public GamePort gameport;
    }

    [System.Serializable]
    private class GamePort
    {
        public int external;
        public int internal_port;
        public string protocol;
    }
}