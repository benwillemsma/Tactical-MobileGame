using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class MenuManager : MonoBehaviour
{
    public static MenuManager SM_Instance;
    public static NetworkManager manager;
    public Canvas[] subCanvases;

    public Text ipText;
    public Text nameText;
    public int matchIndex;

    private void Start ()
    {
        if (!SM_Instance)
            SM_Instance = this;
        else Destroy(gameObject);

        if (!manager)
        {
            manager = FindObjectOfType<NetworkManager>();
            SceneManager.sceneLoaded += OnSceneWasLoaded;
        }
    }

    public void LoadScene(string name)
    {
        if (NetworkServer.active)
           manager.ServerChangeScene(name);
        else
            SceneManager.LoadScene(name);
    }

    public void ToggleCanvas(Canvas canvas)
    {
        for (int i = 0; i < subCanvases.Length; i++)
        {
            if(subCanvases[i] == canvas)
                subCanvases[i].gameObject.SetActive(true);
            else
                subCanvases[i].gameObject.SetActive(false);
        }
    }

    public void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + ": " + NetworkServer.connections.Count);
        if (scene.name == "MultiplayerMenu")
        {
            ipText = GameObject.Find("ipText").GetComponent<Text>();
            nameText = GameObject.Find("nameText").GetComponent<Text>();

            if (!ipText || !nameText)
                Debug.Log("shit broke");
        }
    }

    // LAN Connections
    public void StartServer()
    {
        if (!NetworkClient.active && !NetworkServer.active)
            manager.StartServer();
    }
    public void StartHost()
    {
        if (!NetworkClient.active && !NetworkServer.active)
            manager.StartHost();
    }
    public void StartClient()
    {
        if (!NetworkClient.active && !NetworkServer.active)
            manager.StartClient();
    }

    public void SetClientReady()
    {
        if (NetworkClient.active && !ClientScene.ready)
        {
            ClientScene.Ready(manager.client.connection);

            if (ClientScene.localPlayers.Count == 0)
                ClientScene.AddPlayer(0);
        }
    }

    public void StopServer()
    {
        manager.StopServer();
    }
    public void StopHost()
    {
        manager.StopHost();
    }
    public void StopClient()
    {
        manager.StopClient();
    }

    // MatchMaking
    public void StartMatchMaking()
    {
        if (manager.matchMaker == null)
            manager.StartMatchMaker();
    }
    public void StopMatchMaking()
    {
        if (manager.matchMaker != null)
            manager.StopMatchMaker();
    }

    public void CreateMatch()
    {
        if (!NetworkServer.active && !NetworkClient.active)
        {
            if (manager.matchMaker != null && manager.matchInfo == null && manager.matches == null)
            {
                Debug.Log(ipText);
                manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", ipText.text, 0, 1, manager.OnMatchCreate);
                manager.matchName = nameText.text;
            }
        }
    }
    public void ListMatches()
    {
        if (!NetworkServer.active && !NetworkClient.active)
        {
            if (manager.matchMaker != null && manager.matchInfo == null && manager.matches == null)
            {
                manager.matchMaker.ListMatches(0, 20, "", true, 1, 1, manager.OnMatchList);
            }
        }
    }
    public void JoinMatch()
    {
        if (!NetworkServer.active && !NetworkClient.active)
        {
            if (manager.matchMaker != null && manager.matchInfo == null && manager.matches != null && manager.matches.Count > 0)
            {
                manager.matchName = manager.matches[matchIndex].name;
                manager.matchSize = (uint)manager.matches[matchIndex].currentSize;
                manager.matchMaker.JoinMatch(manager.matches[matchIndex].networkId, "", "", ipText.text, 0, 1, manager.OnMatchJoined);
            }
        }
    }
}
