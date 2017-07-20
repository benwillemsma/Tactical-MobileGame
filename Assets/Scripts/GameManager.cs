using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static List<Unit> units = new List<Unit>();

    public ISelectable Selection;

    private void Start ()
    {
        if (!instance)
            instance = this; 
        else Destroy(gameObject);
	}

    private void Update ()
    {
        //Touch Input
        if (Input.touchCount != 0)
        {
            Touch input = Input.GetTouch(0);

            if (input.phase == TouchPhase.Began)
            {
                CheckSelection(input.position);

                if (Selection != null)
                    Selection.Selected();
            }
        }
        //Mouse Input
        else if (Input.GetButtonDown("Fire1"))
        {
            CheckSelection(Input.mousePosition);

            if (Selection != null)
                Selection.Selected();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
            Endturn();

        if (Selection != null)
            Debug.Log(Selection);
    }

    void Endturn()
    {
        for (int i = 0; i < units.Count; i++)
            StartCoroutine(units[i].InvokeCommands());
    }    

    void CheckSelection(Vector3 point)
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
        if (Physics.Raycast(ray, out hit, 15, LayerMask.GetMask("Selectable"))) { }
        return hit;
    }
}
