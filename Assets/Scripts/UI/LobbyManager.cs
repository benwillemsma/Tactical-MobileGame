using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    public static LobbyManager Instance;

    private List<LobbyPlayer> lobbyPlayers = new List<LobbyPlayer>();

    public GameObject lobbyPlayerPrefab;

    public static Color[] colors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow, Color.black, Color.cyan, Color.magenta };

    private int connections = 0;

    private void Awake()
    {
        NetworkServer.Spawn(gameObject);
    }

    private void Start()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(5, 20, 100, 20), "" + NetworkServer.connections.Count);
    }

    private void Update()
    {
        if(connections < NetworkServer.connections.Count)
        {
            CmdAddLobbyPlayer();
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
                CmdAddLobbyPlayer();
        }
    }

    [Command]
    public void CmdAddLobbyPlayer()
    {
        LobbyPlayer newPlayer = Instantiate(lobbyPlayerPrefab).GetComponent<LobbyPlayer>();

        newPlayer.index = connections;
        newPlayer.teamColor = colors[lobbyPlayers.Count];
        lobbyPlayers.Add(newPlayer);
        newPlayer.teamName = "Team " + lobbyPlayers.Count;

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
