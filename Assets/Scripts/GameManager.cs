using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public static int[] teamPoints = new int[4];

    public static List<Unit> units = new List<Unit>();
    public static List<CapturePoint> captures = new List<CapturePoint>();

    public static List<Unit> unitsMoving = new List<Unit>();

    public ISelectable Selection;

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
        // Input
        if (Input.GetButtonDown("Click"))
        {
            CheckSelection(Input.mousePosition);

            if (Selection != null)
                Selection.Selected();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
            StartTurn();

        // If somthing was selected
        if (Selection != null)
            Debug.Log(Selection);
    }

    // Selection Functions
    private void CheckSelection(Vector3 point)
    {
        RaycastHit hit = ScreenRay(point);
        ISelectable hitObject = hit.transform.gameObject.GetComponent<ISelectable>();

        if (Selection != null)
            Selection.Action(hit.point);

        Selection = hitObject;
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

        while (unitsMoving.Count > 0)
            yield return null;
    }

    private void Endturn()
    {
        for (int i = 0; i < captures.Count; i++)
            captures[i].CalculateCapture();
    }
}
