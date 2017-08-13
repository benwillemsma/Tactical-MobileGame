using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.UI;

public enum CommandType
{
    Move,
    Shoot,
    Grenade
}

public class Command : MonoBehaviour, ISelectable
{
    public CommandType type;
    public int actionPoints;

    public Unit unit;

    public Transform[] visualMarkers;
    public GameObject undoButton;

    private List<Transform> lineSegments = new List<Transform>();
    protected static Vector3 groundOffset = new Vector3(0, 0.2f, 0);

    protected bool selected = false;
    protected Vector3 touchOffset = new Vector3(0.5f, 0, 1f);

    protected virtual void Start()
    {
        gameObject.layer = 2;
        unit.orders.Add(this);
    }
    protected virtual void Update()
    {
        if (unit == null)
        {
            Destroy(gameObject);
            return;
        }
        if (selected)
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    transform.position = GameManager.ScreenRay(Input.GetTouch(0).position).point;
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    Action(GameManager.ScreenRay(Input.GetTouch(0).position).point);
            }
            else
                transform.position = GameManager.ScreenRay(Input.mousePosition).point;
    }

    public virtual void Remove()
    {
        unit.orders.Remove(this);
        Destroy(gameObject);
    }

    // Interface Functions
    public virtual void Selected()
    {
        gameObject.layer = 2;
        unit.team.Selection = this;
        selected = true;
    }
    public virtual void Action(Vector3 point)
    {
        Deselected();
        selected = false;
        unit.ToggleCommands(true);
        transform.position = point;
    }
    public virtual void Deselected()
    {
        unit.ToggleCommands(true);
        gameObject.layer = unit.gameObject.layer;
        unit.team.Selection = unit;
    }

    public void Cancel()
    {
        Remove();
    }

    public void OnDestroy()
    {
        unit.orders.Remove(this);
    }

    // Visual Funciton;
    public void LineFromTo(Vector3 from, Vector3 to, int tileAmount, Transform lineSprite)
    {
        for (int i = 0; i < lineSegments.Count; i++)
            lineSegments[i].gameObject.SetActive(i < tileAmount ? true : false);

        while (lineSegments.Count < tileAmount)
            lineSegments.Add(Instantiate(lineSprite, gameObject.transform));

        for (int i = 0; i < tileAmount; i++)
        {
            if (!lineSegments[i].gameObject.activeSelf)
                break;
            lineSegments[i].position = from + ((to - from).normalized * (i + 0.5f));
            lineSegments[i].rotation = Quaternion.LookRotation(Vector3.up, (to - from));
        }
    }
}
