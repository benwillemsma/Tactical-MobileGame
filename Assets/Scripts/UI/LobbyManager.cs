using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance; 

    private NetworkManager netManager;
    private List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer>();

    public Transform PlayerInfoPanel;
    public GameObject lobbyPlayerPrefab;

    public static Color[] colors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow, Color.black, Color.cyan, Color.magenta };

    private int connections = 0;

    private void Start()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        netManager = FindObjectOfType<NetworkManager>();
        SceneManager.sceneLoaded += OnSceneLoaded;

        connections = NetworkServer.connections.Count;

        for (int i = 0; i < NetworkServer.connections.Count; i++)
            AddLobbyPlayer();
    }

    private void Update()
    {
        if(connections < NetworkServer.connections.Count)
        {
            AddLobbyPlayer();
            connections++;
        }
    }

    private void OnSceneLoaded(Scene scene,LoadSceneMode mode)
    {
        if (scene.buildIndex < 2)
            Destroy(gameObject);
        if(scene.name == "Lobby")
        {
            for (int i = 0; i < NetworkServer.connections.Count; i++)
                AddLobbyPlayer();
        }
        if (scene.name == "Game")
        {
            for (int i = 0; i < NetworkServer.connections.Count; i++)
                ClientScene.AddPlayer(netManager.client.connection, 0);
        }
    }

    public void AddLobbyPlayer()
    {
        LobbyPlayer newPlayer = Instantiate(lobbyPlayerPrefab, PlayerInfoPanel).GetComponent<LobbyPlayer>();
        newPlayer.GetComponent<RectTransform>().anchoredPosition = new Vector2(10, (-32 * (NetworkServer.connections.Count - 1)) - 10);
        lobbyPlayers.Add(newPlayer);
        NetworkServer.Spawn(newPlayer.gameObject);
    }
    public void RemoveLobbyPlayer(int index)
    {
        LobbyPlayer temp = lobbyPlayers[index];
        RemoveLobbyPlayer(temp);
    }
    public void RemoveLobbyPlayer(LobbyPlayer player)
    {
        lobbyPlayers.Remove(player);
        Destroy(player.gameObject);
    }
    public LobbyPlayer GetLobbyPlayer(int index)
    {
        return lobbyPlayers[index];
    }

    public static Dropdown CreateColorOptions(GameObject dropDownPrefab,Transform parent)
    {
        Dropdown dropdown = Instantiate(dropDownPrefab, parent).GetComponent<Dropdown>();
        if (dropdown)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                Texture2D texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, colors[i]);
                texture.Apply();
                var item = new Dropdown.OptionData(Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0)));
                dropdown.options.Add(item);
                if (i == 0)
                {
                    Image temp = dropdown.transform.GetChild(0).GetComponent<Image>();
                    temp.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                    temp.enabled = true;
                }
            }
        }
        dropdown.name = "ColorOptions";
        return dropdown;
    }
}
