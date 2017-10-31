using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking.Match;
using UnityEngine.Networking;

public class MatchList : MonoBehaviour
{
    public static NetworkLobbyManager netManager;

    public InputField matchNameText;
    public Text matchListText;
    private int matchIndex;

    private float refreshTime;

    private void Awake()
    {
        if (!netManager)
            netManager = FindObjectOfType<NetworkLobbyManager>();
    }

    private void Start ()
    {
        matchNameText.text = PlayerPrefs.GetString("UserName") + "'s Game";
    }

    private void Update()
    {
        if (refreshTime > 1)
        {
            refreshTime = 0;
            ListMatches();
        }
        else refreshTime += Time.deltaTime;
    }

#if ENABLE_UNET
    #region MatchMaking
    public void Refresh()
    {
        ListMatches();
    }

    public void CreateMatch()
    {
        if (!NetworkServer.active && !NetworkClient.active)
        {
            if (netManager.matchMaker != null && netManager.matchInfo == null)
            {
                netManager.matchName = matchNameText.text;
                netManager.matchMaker.CreateMatch(netManager.matchName, netManager.matchSize, true, "", "", "", 0, 1, netManager.OnMatchCreate);
            }
        }
    }

    public void ListMatches()
    {
        if (!NetworkServer.active && !NetworkClient.active && netManager.matchMaker != null)
        {
            if (netManager.matches == null)
                netManager.matchMaker.ListMatches(0, 20, "", true, 1, 1, netManager.OnMatchList);
            else
            {
                MenuManager.ClearOutput();
                matchListText.text = "";
                for (int i = 0; i < netManager.matches.Count; i++)
                {
                    MenuManager.Output(i + ":" + netManager.matches[i].name);
                    matchListText.text += netManager.matches[i].name + "\n";
                }
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
