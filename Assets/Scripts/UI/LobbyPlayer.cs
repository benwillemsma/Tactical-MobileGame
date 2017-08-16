using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class LobbyPlayer : MonoBehaviour
{
    private NetworkManager netManager;
    public Dropdown colorOptionsPrefab;
    public InputField nameInput;

    [Space(20)]
    public Team team;
    public string teamName;
    public Color teamColor;

    private Dropdown colorOptions;

    public void Start()
    {
        netManager = FindObjectOfType<NetworkManager>();

        colorOptions = LobbyManager.CreateColorOptions(colorOptionsPrefab.gameObject, transform);

        nameInput.text = teamName;
        for (int i = 0; i < LobbyManager.colors.Length; i++)
        {
            if (teamColor == LobbyManager.colors[i])
            {
                colorOptions.value = i;
                return;
            }
        }
        teamColor = LobbyManager.colors[colorOptions.value];
    }

    public void ChangeName()
    {
        teamName = nameInput.text;
    }

    public void ChangeColor()
    {
        teamColor = LobbyManager.colors[colorOptions.value];
    }
}
