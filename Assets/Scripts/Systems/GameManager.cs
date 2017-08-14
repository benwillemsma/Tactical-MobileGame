using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public enum Team
{
    Team_1,
    Team_2,
    Team_3,
    Team_4,
    Neutral
}

public class GameManager : NetworkBehaviour
{
    public static NetworkManager NetManager;
    public static GameManager Instance;

    public List<PlayerTeam> s_teams = new List<PlayerTeam>();
    public List<CapturePoint> s_captures = new List<CapturePoint>();
    public int[] s_unitsWithOrders;

    // Game Specific Variables
    public bool[] s_playerReady;
    private int s_teamAmount;

    public PlayerTeam s_winner = null;
    public int s_winningScore = 10;

    // GUI
    public Text[] scoreText = new Text[4];

    private void Awake ()
    {
        NetManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        //NetManager = GameObject.Find("LobbyManager").GetComponent<NetworkManager>();

        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);

        if (scoreText.Length == 0)
            scoreText = GameObject.Find("ScoreCanvas").GetComponentsInChildren<Text>();
    }
    private void Start()
    {
        if (isServer)
        {
            s_unitsWithOrders = new int[4];
            s_playerReady = new bool[4];
        }
    }
    private void Update()
    {
        if (Input.GetButtonDown("Exit"))
            Application.Quit();
        if (isServer)
        {
            s_teamAmount = NetManager.numPlayers;
            UpdateScoreUI();
        }
    }
    private void OnGUI()
    {
        int y = 0;
        for (int i = 0; i < s_teamAmount; i++)
        {
            GUI.Label(new Rect(0, y, 300, 20), "" + s_teams[i].name);
            y += 20;
            GUI.Label(new Rect(0, y, 300, 20), "" + s_teams[i].units.Count);
            y += 20;
        }
    }

    public void AddPlayer(PlayerTeam player)
    {
        s_teamAmount++;
        s_teams.Add(player);
    }
    public void RemovePlayer(PlayerTeam player)
    {
        s_teams.Remove(player);
        s_teamAmount--;
    }

    private void UpdateScoreUI ()
    {

        for (int i = 0; i < s_teamAmount; i++)
        {
            if (s_teams.Count == s_teamAmount)
            {
                scoreText[i].enabled = true;
                scoreText[i].text = s_teams[i].teamName + ": " + s_teams[i].score;
            }
        }
        for (int i = s_teamAmount; i < scoreText.Length; i++)
            scoreText[i].enabled = false;

        // Update clients
        for (int i = 0; i < scoreText.Length; i++)
            RpcUpdateScoreUI(i, scoreText[i].enabled, scoreText[i].text);
    }
    [ClientRpc]
    private void RpcUpdateScoreUI(int i, bool enabled,string text)
    {
        scoreText[i].enabled = enabled;
        scoreText[i].text = text;
    }

    public void CheckReadyStates()
    {
        for (int i = 0; i < s_teamAmount; i++)
        {
            if (!s_playerReady[i])
                return;
        }
        StartTurn();
    }
    public void CheckForWinner()
    {
        for (int i = 0; i < s_teams.Count; i++)
        {
            if (s_teams[i].units.Count <= 0)
            {
                Debug.Log("here");
                RemovePlayer(s_teams[i]);
            }

            if (s_teams[i].score >= s_winningScore)
                s_winner = s_teams[i];
        }

        if (s_teamAmount == 1)
            s_winner = s_teams[0];

        if (s_winner != null)
            EndGame();
    }

    public static RaycastHit ScreenRay(Vector3 point,LayerMask mask)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(point);
        Physics.Raycast(ray, out hit, 15, LayerMask.GetMask("Selectable", LayerMask.LayerToName(mask)));
        return hit;
    }
    public static RaycastHit ScreenRay(Vector3 point)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(point);
        Physics.Raycast(ray, out hit, 15, LayerMask.GetMask("Selectable"));
        return hit;
    }

    private void StartGame()
    {

    }
    private void EndGame()
    {

    }
    
    public void StartTurn()
    {
        StartCoroutine(RunTurn());
    }
    private IEnumerator RunTurn()
    {
        yield return WaitForUnits();
        Endturn();
    }
    private void Endturn()
    {
        for (int i = 0; i < s_captures.Count; i++)
            s_captures[i].CmdCalculateCapture();

        CheckForWinner();

        for (int i = 0; i < s_teamAmount; i++)
        {
            s_teams[i].RpcToggleReady();
            s_playerReady[i] = false;
        }
    }

    private IEnumerator WaitForUnits()
    {
        for (int i = 0; i < s_teamAmount; i++)
            s_teams[i].RpcInvokeCommands();

        //Wait for all units to stop moving
        while (UnitsWithOrdorsRemaining() > 0)
            yield return null;
    }
    private int UnitsWithOrdorsRemaining()
    {
        int temp = 0;
        for (int i = 0; i < s_unitsWithOrders.Length; i++)
            temp += s_unitsWithOrders[i];
        return temp;
    }
}
