using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
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
    #region Initilization
    public static NetworkManager netManager;
    public static GameManager Instance;

    [SyncVar]
    public int s_unitsWithOrders = 0;
    [SyncVar]
    public int s_playersReady = 0;
    [SyncVar]
    public int s_winner = -1;
    [SyncVar]
    public int s_winningScore = 10;

    public float timerAmount;

    public List<PlayerTeam> s_teams = new List<PlayerTeam>();
    public List<CapturePoint> s_captures = new List<CapturePoint>();
    
    // GUI
    private Text[] scoreText;
    private Text timerText;
    private float turnTimer = 30;

    private void Awake()
    {
        if (!Instance)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        netManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        if (scoreText == null)
            scoreText = GameObject.Find("ScoreCanvas").GetComponentsInChildren<Text>();
        if (timerText == null)
            timerText = GameObject.Find("TurnTimer").GetComponent<Text>();

        ClientScene.AddPlayer(netManager.client.connection, 0);
    }
    
    private void OnGUI()
    {
        //if (Debug.isDebugBuild)
        //{
        //    GUI.color = Color.gray;
        //    GUI.Label(new Rect(550, 20, 300, 20), "GameManager: " + s_teams.Count);
        //    for (int i = 0; i < s_teams.Count; i++)
        //    {
        //        int y = 0;
        //        GUI.Label(new Rect(100 * (int)s_teams[i].team, y, 300, 20), "" + s_teams[i].teamName);
        //        GUI.Label(new Rect(100 * (int)s_teams[i].team + 50, y, 300, 20), "" + s_teams[i].units.Count);
        //        y += 20;
        //    }
        //}
    }
    #endregion

    #region HelperFunctions
    public void AddPlayer(PlayerTeam newTeam)
    {
        s_teams.Add(newTeam);
    }
    public void RemovePlayer(PlayerTeam Team)
    {
        s_teams.Remove(Team);
    }
    
    public static RaycastHit ScreenRay(Vector3 point, LayerMask mask)
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
    #endregion

    #region GameLoop
    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
            TogglePause();

        if (!MenuManager.training)
        {
            if (turnTimer > 0)
                turnTimer -= Time.deltaTime;
            else if (turnTimer < 0)
            {
                turnTimer = 0;
                for (int i = 0; i < s_teams.Count; i++)
                {
                    s_teams[i].RpcToggleReady();
                    s_teams[i].CmdPlayerReady();
                }
            }
            UpdateUI();
        }
    }

    public void TogglePause()
    {
        string pause = "Pause";
        if (SceneManager.GetSceneByName(pause).isLoaded)
            SceneManager.UnloadSceneAsync(pause);
        else
            SceneManager.LoadScene(pause, LoadSceneMode.Additive);
    }

    private void UpdateUI()
    {
        if (!MenuManager.training)
        {
            timerText.text = "" + (int)turnTimer;
            for (int i = 0; i < s_teams.Count; i++)
            {
                scoreText[i].enabled = true;
                scoreText[i].text = s_teams[i].teamName + ": " + s_teams[i].score;
            }
            for (int i = s_teams.Count; i < scoreText.Length; i++)
                scoreText[i].enabled = false;
        }
    }

    public void CheckReadyStates()
    {
        if (s_playersReady >= s_teams.Count)
            StartTurn();
    }

    public void CheckForWinner()
    {
        for (int i = 0; i < s_teams.Count; i++)
        {
            if (s_teams[i].score >= s_winningScore)
                s_winner = (int)s_teams[i].team;
        }

        if (s_winner != -1)
            EndGame();
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

        for (int i = 0; i < s_teams.Count; i++)
            s_teams[i].RpcToggleReady();
        s_playersReady = 0;
        turnTimer = timerAmount;
    }

    private IEnumerator WaitForUnits()
    {
        for (int i = 0; i < s_teams.Count; i++)
            s_teams[i].RpcInvokeCommands();

        yield return new WaitForEndOfFrame();

        while (s_unitsWithOrders > 0)
            yield return null;
    }
    #endregion
}
