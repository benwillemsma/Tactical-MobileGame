using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

[System.Serializable]
public class UIGroup
{
    public List<Canvas> Canvases;
}

public class MenuManager : MonoBehaviour
{
    public static NetworkLobbyManager netManager;
    public static bool training = false;

    public Text AddressText;
    public Text nameText;
    private int matchIndex;

    [SerializeField]
    private List<UIGroup> UIGroups = new List<UIGroup>();

    public bool Training
    {
        get { return training; }
        set { training = value; }
    }

    private void Start()    
    {

        if (!netManager)
        {
            netManager = FindObjectOfType<NetworkLobbyManager>();
            SceneManager.sceneLoaded += OnSceneWasLoaded;
        }

        if (SceneManager.GetActiveScene().name == "Multiplayer")
        {
            AddressText = GameObject.Find("ipText").GetComponent<Text>();
            nameText = GameObject.Find("nameText").GetComponent<Text>();
            SetNetworkAddress();
        }
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Home))
    //        PlayerPrefs.DeleteAll();
    //}

    #region Menu Navigation
    public void Quit()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }

    public void LoadScene(string name)
    {
        if (NetworkServer.active)
            netManager.ServerChangeScene(name);
        else
            SceneManager.LoadScene(name);
    }

    public void OnSceneWasLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Multiplayer" && (NetworkServer.active || NetworkClient.active))
            ToggleCanvas("Lobby");
    }

    public void ToggleCanvas(Canvas canvas)
    {
        int groupIndex = -1;
        int index = -1;

        for (int g = 0; g < UIGroups.Count; g++)
        {
            for (int i = 0; i < UIGroups[g].Canvases.Count; i++)
            {
                if (UIGroups[g].Canvases[i] == canvas)
                {
                    groupIndex = g;
                    index = i;
                    break;
                }
            }
            if (groupIndex != -1)
            break;
        }
        for (int i = 0; i < UIGroups[groupIndex].Canvases.Count; i++)
            UIGroups[groupIndex].Canvases[i].gameObject.SetActive(false);
        UIGroups[groupIndex].Canvases[index].gameObject.SetActive(true);
    }
    public void ToggleCanvas(string tag)
    {
        int groupIndex = -1;
        int index = -1;

        for (int g = 0; g < UIGroups.Count; g++)
        {
            for (int i = 0; i < UIGroups[g].Canvases.Count; i++)
            { 
                if (UIGroups[g].Canvases[i].tag == tag)
                {
                    groupIndex = g;
                    index = i;
                    break;
                }
            }
            if (groupIndex != -1)
                break;
        }
        for (int i = 0; i < UIGroups[groupIndex].Canvases.Count; i++)
            UIGroups[groupIndex].Canvases[i].gameObject.SetActive(false);
        UIGroups[groupIndex].Canvases[index].gameObject.SetActive(true);
    }
    
    #endregion

#if ENABLE_UNET
    #region LAN Connections
    public void StartServer()
    {
        if (!NetworkClient.active && !NetworkServer.active)
            netManager.StartServer();
    }
    public void StartHost()
    {
        if (!NetworkClient.active && !NetworkServer.active)
            netManager.StartHost();
    }

    public void StartClient()
    {
        if (!NetworkClient.active && !NetworkServer.active)
            netManager.StartClient();
    }

    public void SetNetworkAddress()
    {
        if (AddressText)
        {
            if (AddressText.text != "")
                netManager.networkAddress = AddressText.text;
            else
                netManager.networkAddress = "localhost";
        }
    }

    public void StopServer()
    {
        netManager.StopServer();
    }
    public void StopHost()
    {
        netManager.StopHost();
    }
    public void StopClient()
    {
        netManager.StopClient();
    }
    #endregion

    #region MatchMaking
    public void StartMatchMaking()
    {
        if (netManager.matchMaker == null)
            netManager.StartMatchMaker();
    }
    public void StopMatchMaking()
    {
        if (netManager.matchMaker != null)
            netManager.StopMatchMaker();
    }

    public void CreateMatch()
    {
        if (!NetworkServer.active && !NetworkClient.active)
        {
            if (netManager.matchMaker != null && netManager.matchInfo == null && netManager.matches == null)
            {
                netManager.matchMaker.CreateMatch(netManager.matchName, netManager.matchSize, true, "", "", "", 0, 1, netManager.OnMatchCreate);
                netManager.matchName = nameText.text;
            }
        }
    }
    public void ListMatches()
    {
        if (!NetworkServer.active && !NetworkClient.active)
        {
            if (netManager.matchMaker != null && netManager.matchInfo == null && netManager.matches == null)
            {
                netManager.matchMaker.ListMatches(0, 20, "", true, 1, 1, netManager.OnMatchList);
            }
        }
    }
    public void JoinMatch()
    {
        if (!NetworkServer.active && !NetworkClient.active)
        {
            if (netManager.matchMaker != null && netManager.matchInfo == null && netManager.matches != null && netManager.matches.Count > 0)
            {
                netManager.matchName = netManager.matches[matchIndex].name;
                netManager.matchSize = (uint)netManager.matches[matchIndex].currentSize;
                netManager.matchMaker.JoinMatch(netManager.matches[matchIndex].networkId, "", "", "", 0, 1, netManager.OnMatchJoined);
            }
        }
    }
    #endregion
#endif
}
