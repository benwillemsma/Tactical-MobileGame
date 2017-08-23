using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class MenuManager : MonoBehaviour
{
    public static MenuManager SM_Instance;
    public static NetworkManager manager;
    public Canvas[] subCanvases;

    public Text AddressText;
    public Text nameText;
    public int matchIndex;

    private void Start()
    {
        if (!SM_Instance)
            SM_Instance = this;
        else Destroy(gameObject);

        if (!manager)
        {
            manager = FindObjectOfType<NetworkManager>();
            SceneManager.sceneLoaded += OnSceneWasLoaded;
        }

        if (SceneManager.GetActiveScene().name == "MultiplayerMenu")
        {
            AddressText = GameObject.Find("ipText").GetComponent<Text>();
            nameText = GameObject.Find("nameText").GetComponent<Text>();
            SetNetworkAddress();
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
            if (subCanvases[i] == canvas)
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
            AddressText = GameObject.Find("ipText").GetComponent<Text>();
            nameText = GameObject.Find("nameText").GetComponent<Text>();
            SetNetworkAddress();

            if (!AddressText || !nameText)
                Debug.Log("shit broke");
        }
    }

#if ENABLE_UNET
    // LAN Connections
    public void StartServer()
    {
        if (!NetworkClient.active && !NetworkServer.active)
            manager.StartServer();
    }
    public void StartHost()
    {
        if (!NetworkClient.active && !NetworkServer.active)
        {
            manager.networkAddress = AddressText.text;
            manager.StartHost();
        }
    }
    public void StartClient()
    {
        if (!NetworkClient.active && !NetworkServer.active)
            manager.StartClient();
    }

    public void SetNetworkAddress()
    {
        if (AddressText.text != "")
            manager.networkAddress = AddressText.text;
        else
            manager.networkAddress = "localhost";
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
                manager.matchMaker.CreateMatch(manager.matchName, manager.matchSize, true, "", "", "", 0, 1, manager.OnMatchCreate);
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
                manager.matchMaker.JoinMatch(manager.matches[matchIndex].networkId, "", "", "", 0, 1, manager.OnMatchJoined);
            }
        }
    }
#endif //ENABLE_UNET
}
