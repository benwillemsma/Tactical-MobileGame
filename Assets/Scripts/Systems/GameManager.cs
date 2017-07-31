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
    public static ISelectable Selection;

    public static List<PlayerTeam> teams = new List<PlayerTeam>();
    public static List<Unit> unitsMoving = new List<Unit>();
    public static List<CapturePoint> captures = new List<CapturePoint>();

    // Game Specific Variables
    public static PlayerTeam winner = null;
    public static int winningScore = 10;
    private static int teamAmount;
    private bool RecieveInput = true;

    // GUI
    public Text[] ScoreText;
    public Button TurnButton;

    private void Start ()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    private void Update ()
    {
        teamAmount = teams.Count;
        if (RecieveInput)
        {
            if (Input.GetButtonDown("Click"))
                CheckSelection(Input.mousePosition);

            for (int i = 0; i < teamAmount; i++)
            {
                ScoreText[i].enabled = true;
                ScoreText[i].text = teams[i].teamName + ": " + teams[i].score;
            }
            for (int i = teamAmount; i < ScoreText.Length; i++)
                ScoreText[i].enabled = false;
        }

        if (teamAmount > 0)
            CheckReadyStates();
    }
    private void CheckReadyStates()
    {
        for (int i = 0; i < teams.Count; i++)
        {
            if (!teams[i].ready)
                return;
        }
        StartTurn();
    }

    private void CheckForWinner()
    {
        if (teams.Count == 1)
            winner = teams[0];

        if (winner != null)
            EndGame();
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
        Physics.Raycast(ray, out hit, 15, LayerMask.GetMask("Selectable","Unit"));
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
        for (int i = 0; i < teamAmount; i++)
            teams[i].InvokeCommands();

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

        CheckForWinner();
    }
}
