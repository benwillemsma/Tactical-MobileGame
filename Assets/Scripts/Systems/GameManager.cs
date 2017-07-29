using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public enum Team
{
    Neutral,
    Team_1,
    Team_2,
    Team_3,
    Team_4
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool RecieveInput = true;
    public ISelectable Selection;

    public static List<PlayerTeam> teams = new List<PlayerTeam>();
    public static List<Unit> unitsMoving = new List<Unit>();
    public static List<CapturePoint> captures = new List<CapturePoint>();

    // Game Specific Variables
    private PlayerTeam winner = null;
    private int teamAmount = 2;
    private int winningScore = 10;

    // GUI
    public Text[] ScoreText;
    public Button TurnButton;

    private void Start ()
    {
        if (!instance)
            instance = this; 
        else Destroy(gameObject);

        Cursor.lockState = CursorLockMode.Confined;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        // Initalize Teams
        teamAmount = 2;
        teams.Add(new PlayerTeam("Nutral", Color.yellow));

        for (int i = 0; i < teamAmount; i++)
            teams.Add(new PlayerTeam("Team " + (i + 1), Random.ColorHSV()));
        
        for (int i = teamAmount; i < ScoreText.Length; i++)
            ScoreText[i].enabled = false;
    }
    private void Update ()
    {
        if (RecieveInput)
        {
            if (Input.GetButtonDown("Click"))
                CheckSelection(Input.mousePosition);

            else if (Input.GetKeyDown(KeyCode.Space))
                StartTurn();

            for (int i = 0; i < teamAmount; i++)
                ScoreText[i].text = " Team " + (i + 1) + ": " + teams[i].score;
        }

        CheckTeamStates();

        if (winner != null)
            EndGame();
    }
    private void CheckTeamStates()
    {
        for (int i = 0; i < teamAmount + 1; i++)
        {
            if(i != 0 && teams[i].units.Count <= 0)
            {
                teams.Remove(teams[i]);
                // Check if only one Team remains (and nutral).
                if (teams.Count == 2)
                {
                    winner = teams[0];
                    return;
                }
            }

            // Check if Team has Won.
            if(teams[i].score >= winningScore)
            {
                winner = teams[i];
                return;
            }

            // Check it teams are ready.
            if (!teams[i].ready)
                return;
        }

        // If game is not over and ALL Teams ready...
        StartTurn();
    }

    // Selection Functions
    private void CheckSelection(Vector3 point)
    {
        RaycastHit hit = ScreenRay(point);
        ISelectable hitObject = hit.transform.gameObject.GetComponent<ISelectable>();

        if (hitObject != null)
        {
            if (Selection != null)
            {
                Selection.Action(hit.point);
                hitObject.Selected();
            }
            else if (hitObject != Selection)
                hitObject.Selected();
        }
        else if (Selection != null)
            Selection.Action(hit.point);
    }
    public static RaycastHit ScreenRay(Vector3 point)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(point);
        Physics.Raycast(ray, out hit, 15, LayerMask.GetMask("Selectable"));
        return hit;
    }

    // GameLoopFunctions
    private void StartGame()
    {

    }
    private void EndGame()
    {
        Debug.Log(winner);
    }

    // GameTurn Functions
    public void StartTurn()
    {
        RecieveInput = false;
        TurnButton.interactable = false;
        Selection = null;
        StartCoroutine(RunTurn());
    }
    private IEnumerator RunTurn()
    {
        yield return WaitForUnits();
        Endturn();
    }
    private IEnumerator WaitForUnits()
    {
        for (int i = 0; i < teamAmount + 1; i++)
        {
            for (int u = 0; u < teams[i].units.Count; u++)
                StartCoroutine(teams[i].units[u].InvokeCommands());
        }

        //Wait for all units to stop moving
        while (unitsMoving.Count > 0)
            yield return null;
    }
    private void Endturn()
    {
        for (int i = 0; i < captures.Count; i++)
            captures[i].CalculateCapture();

        TurnButton.interactable = true;
        RecieveInput = true;
    }
}
