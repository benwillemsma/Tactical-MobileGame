using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer
{
    #region Initilization
    private GameObject PlayerInfoPanel;
    private RectTransform playerUI;
    [SerializeField]
    private Dropdown colorOptionsPrefab;
    [SerializeField]
    private InputField nameInput;

    [Space(20)]
    public static Color[] colors = new Color[] { Color.red, Color.blue, Color.green, Color.yellow, Color.black, Color.cyan, Color.magenta };
    public static List<LobbyPlayer> players = new List<LobbyPlayer>();

    [SyncVar]
    public int index;
    [SyncVar(hook = "ChangeNameHook")]
    public string teamName;
    [SyncVar(hook = "ChangeColorHook")]
    public Color teamColor; 
    
    bool isReady = false;

    private Dropdown colorOptions;

    private void Awake()
    {
        if (!players.Contains(this))
            players.Add(this);
        playerUI = (RectTransform)transform.GetChild(0);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (isServer)
            index = NetworkServer.connections.Count;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        teamName = PlayerPrefs.GetString("UserName", "Team " + index);
        teamColor = colors[PlayerPrefs.GetInt("UserColor", index - 1)];
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        PlayerInfoPanel = GameObject.FindGameObjectWithTag("PlayerInfoPanel");

        playerUI.SetParent(PlayerInfoPanel.transform);
        playerUI.localPosition = new Vector3(0, 0, 0);
        playerUI.localRotation = Quaternion.identity;
        playerUI.anchoredPosition = new Vector2(10, (((index - 1) * -32) - 10));
        playerUI.localScale = new Vector3(1, 1, 1);
        playerUI.sizeDelta = new Vector3(-260, 30);
        
        colorOptions = CreateColorOptions(colorOptionsPrefab.gameObject, playerUI);
        Button readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
        if (isLocalPlayer)
        {
            nameInput.onEndEdit.AddListener(delegate { ChangeName(); });
            colorOptions.onValueChanged.AddListener(delegate { ChangeColor(); });
            readyButton.onClick.AddListener(delegate { ToggleReady(); });
        }
        else
        {
            colorOptions.interactable = false;
            nameInput.interactable = false;
        }
        
        nameInput.text = teamName;
        ChangeName();
        for (int i = 0; i < colors.Length; i++)
            if (teamColor == colors[i])
                colorOptions.value = i;
    }

    public static Dropdown CreateColorOptions(GameObject dropDownPrefab, Transform parent)
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
    #endregion

    #region Functions and Commands
    void ToggleReady()
    {
        isReady = !isReady;
        readyToBegin = isReady;

        if (isReady)
            SendReadyToBeginMessage();
        else
            SendNotReadyToBeginMessage();
    }
    
    private void ChangeName()
    {
        PlayerPrefs.SetString("UserName", nameInput.text);
        CmdChangeName(nameInput.text);
    }
    [Command]
    public void CmdChangeName(string newName)
    {
        if (nameInput)
            teamName = newName;
    }
    public void ChangeNameHook(string newName)
    {
        teamName = newName;
        if (nameInput)
            nameInput.text = newName;
    }
    
    private void ChangeColor()
    {
        PlayerPrefs.SetInt("UserColor", colorOptions.value);
        CmdChangeColor(colors[colorOptions.value]);
    }
    [Command]
    public void CmdChangeColor(Color newColor)
    {
        if (colorOptions)
            teamColor = newColor;
    }
    public void ChangeColorHook(Color newColor)
    {
        teamColor = newColor;
        if (colorOptions)
        {
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i] == newColor)
                    colorOptions.value = i;
            }
        }
    }
    #endregion
}
