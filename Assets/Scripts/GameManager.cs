using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public static int[] teamPoints = new int[4];                                                                        // winning Point values for each team.
    public static Color[] teamColors = new Color[] {Color.grey, Color.blue, Color.red, Color.yellow, Color.green };

    public static List<Unit> units = new List<Unit>();                                                                  // All Units in the game
    public static List<Unit> unitsMoving = new List<Unit>();                                                            // All Units That need to Execute orders

    public static List<CapturePoint> captures = new List<CapturePoint>();                                               // All CapturePoints in the game


    public Queue<ISelectable> Selection = new Queue<ISelectable>();                                                     // Current Selection

    private void Start ()
    {

        if (!instance)
            instance = this; 
        else Destroy(gameObject);

        Cursor.lockState = CursorLockMode.Confined;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
	}

    private void Update ()
    {
        if (Input.GetButtonDown("Click"))
            CheckSelection(Input.mousePosition);

        else if (Input.GetKeyDown(KeyCode.Space))
            StartTurn(); 
    }

    // Selection Functions
    private void CheckSelection(Vector3 point)
    {
        RaycastHit hit = ScreenRay(point);
        ISelectable hitObject = hit.transform.gameObject.GetComponent<ISelectable>();

        if (hitObject != null)
        {
            Debug.Log("hitObject = " + hitObject);
            if (Selection.Count == 0)
                hitObject.Selected();
            else if (hitObject != Selection.Peek())
                hitObject.Selected();
        }

        if (Selection.Count > 0)
        {
            Debug.Log(Selection.Peek() + ": is selected");
            Selection.Peek().Action(hit.point);
        }
        else if (Selection.Count == 0)
            Debug.Log("NothingSelected");
    }

    public static RaycastHit ScreenRay(Vector3 point)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(point);
        Physics.Raycast(ray, out hit, 15, LayerMask.GetMask("Selectable"));
        return hit;
    }

    // GameTurn Functions
    public void StartTurn()
    {
        StartCoroutine(RunTurn());
    }

    private IEnumerator RunTurn()
    {
        yield return WaitForUnits();
        Endturn();
    }
    private IEnumerator WaitForUnits()
    {
        for (int i = 0; i < units.Count; i++)
            StartCoroutine(units[i].InvokeCommands());

        //Wait for all units to stop moving
        while (unitsMoving.Count > 0)
            yield return null;
    }

    private void Endturn()
    {
        for (int i = 0; i < captures.Count; i++)
            captures[i].CalculateCapture();
    }
}
