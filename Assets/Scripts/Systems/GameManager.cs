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
    public Text[] s_scoreText;

    private void Start ()
    {
        NetManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();

        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);

        s_unitsWithOrders = new int[4];
        s_playerReady = new bool[4];

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    private void Update()
    {
        s_teamAmount = NetManager.numPlayers;
    }

    public void AddPlayer(PlayerTeam player)
    {
        s_teamAmount++;
        s_teams.Add(player);
    }
    public void RemovePlayer(PlayerTeam player)
    {
        s_teamAmount--;
        s_teams.Remove(player);
        CheckForWinner();
    }

    private void UpdateScoreUI ()
    {
        for (int i = 0; i < s_teamAmount; i++)
        {
            s_scoreText[i].enabled = true;
            s_scoreText[i].text = s_teams[i].teamName + ": " + s_teams[i].score;
        }
        for (int i = s_teamAmount; i < s_scoreText.Length; i++)
            s_scoreText[i].enabled = false;
    }
    public void CheckReadyStates()
    {
        for (int i = 0; i < s_teamAmount; i++)
        {
            Debug.Log(i + ": " + !s_playerReady[i]);
            if (!s_playerReady[i])
                return;
        }
        StartTurn();
    }
    public void CheckForWinner()
    {
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
            s_captures[i].CalculateCapture();

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
        while (UnitsWithOrdrsRemaining() > 0)
            yield return null;
    }
    private int UnitsWithOrdrsRemaining()
    {
        int temp = 0;
        for (int i = 0; i < s_unitsWithOrders.Length; i++)
        {
            temp += s_unitsWithOrders[i];
        }
        return temp;
    }
}
