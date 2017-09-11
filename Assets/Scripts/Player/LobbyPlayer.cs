using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkBehaviour
{
    private GameObject PlayerInfoPanel;
    [SerializeField]
    private Dropdown colorOptionsPrefab;
    [SerializeField]
    private InputField nameInput;

    [SyncVar, Space(20)]
    public int index;
    [SyncVar(hook = "ChangeName")]
    public string teamName;
    [SyncVar(hook = "ChangeColor")]
    public Color teamColor;

    private Dropdown colorOptions;

    private IEnumerator Start()
    {
        while (PlayerInfoPanel == null)
        {
            PlayerInfoPanel = GameObject.FindGameObjectWithTag("PlayerInfoPanel");
            yield return null;
        }
        transform.SetParent(PlayerInfoPanel.transform);

        RectTransform rect = GetComponent<RectTransform>();
        rect.localPosition = new Vector3(0, 0, 0);
        rect.localRotation = Quaternion.identity;
        rect.anchoredPosition = new Vector2(10, ((index * -32) - 10));
        rect.localScale = new Vector3(1, 1, 1);
        rect.sizeDelta = new Vector3(-260, 30);

        colorOptions = LobbyManager.CreateColorOptions(colorOptionsPrefab.gameObject, transform);
        colorOptions.onValueChanged.AddListener(delegate { CmdChangeColor(); });

        Button readyButton = GameObject.Find("ReadyButton").GetComponent<Button>();
        readyButton.onClick.AddListener(delegate { LobbyManager.Instance.CmdToggleReady(); });

        nameInput.text = teamName;
        for (int i = 0; i < LobbyManager.colors.Length; i++)
        {
            if (teamColor == LobbyManager.colors[i])
            {
                colorOptions.value = i;
                break;
            }
        }
        teamColor = LobbyManager.colors[colorOptions.value];
    }

    [Command]
    public void CmdChangeName()
    {
        teamName = nameInput.text;
    }
    public void ChangeName(string newName)
    {
        if (nameInput)
            nameInput.text = newName;
    }

    [Command]
    public void CmdChangeColor()
    {
        teamColor = LobbyManager.colors[colorOptions.value];
    }
    public void ChangeColor(Color newColor)
    {
        if (colorOptions)
        {
            for (int i = 0; i < LobbyManager.colors.Length; i++)
            {
                if (LobbyManager.colors[i] == newColor)
                    colorOptions.value = i;
            }
        }
    }
}
