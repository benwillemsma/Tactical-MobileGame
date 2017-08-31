using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public enum CommandType
{
    Move,
    Shoot,
    Grenade,
    Rocket,
    Melee
}

public class Command : MoveableObject
{
    public CommandType type;
    public int actionPoints;

    public Unit unit;

    public static GameObject blankCommand;
    public GameObject spawnObject;
    public Transform[] visualMarkers;

    private List<Transform> lineSegments = new List<Transform>();
    protected static Vector3 groundOffset = new Vector3(0, 0.2f, 0);

    protected virtual void Start()
    {
        normalLayer = unit.gameObject.layer;
        unit.actionsRemaining -= actionPoints;
        unit.orders.Add(this);
    }
    protected override void Update()
    {
        if (unit == null)
        {
            Destroy(gameObject);
            return;
        }
        base.Update();
    }

    public void LineFromTo(Vector3 from, Vector3 to, Transform lineSprite)
    {
        int tileAmount = Mathf.RoundToInt((from - to).magnitude);

        for (int i = 0; i < lineSegments.Count; i++)
            lineSegments[i].gameObject.SetActive(i < tileAmount ? true : false);

        while (lineSegments.Count < tileAmount) {
            Transform line = Instantiate(lineSprite, gameObject.transform);
            line.localScale = Vector3.one / line.parent.localScale.x; // not needed with real art assets/everything scaled properly
            lineSegments.Add(line);
        }

        for (int i = 0; i < tileAmount; i++)
        {
            if (!lineSegments[i].gameObject.activeSelf)
                break;
            lineSegments[i].position = from + ((to - from).normalized * (i + 0.5f)) + groundOffset;
            lineSegments[i].rotation = Quaternion.LookRotation(Vector3.up, (to - from));
        }
    }
    public void CurveFromTo(Vector3 from, Vector3 midpoint, Vector3 to, Transform lineSprite)
    {
        int tileAmount1 = Mathf.RoundToInt((from - midpoint).magnitude);
        int tileAmount2 = Mathf.RoundToInt((midpoint - to).magnitude);

        for (int i = 0; i < lineSegments.Count; i++)
            lineSegments[i].gameObject.SetActive(i < tileAmount1 + tileAmount2 ? true : false);

        while (lineSegments.Count < tileAmount1 + tileAmount2)
        {
            Transform line = Instantiate(lineSprite, gameObject.transform);
            line.localScale = Vector3.one / line.parent.localScale.x; // not needed with real art assets/everything scaled properly
            lineSegments.Add(line);
        }

        for (int i = 0; i < tileAmount1; i++)
        {
            if (!lineSegments[i].gameObject.activeInHierarchy)
                break;
            lineSegments[i].position = from + ((midpoint - from).normalized * (i + 0.5f)) + groundOffset;
            lineSegments[i].rotation = Quaternion.LookRotation(Vector3.up, (midpoint - from));
        }
        for (int i = tileAmount1; i < tileAmount1 + tileAmount2; i++)
        {
            if (!lineSegments[i].gameObject.activeInHierarchy)
                break;
            lineSegments[i].position = midpoint + ((to - midpoint).normalized * (i - tileAmount1 + 0.5f)) + groundOffset;
            lineSegments[i].rotation = Quaternion.LookRotation(Vector3.up, (to - midpoint));
        }
    }

    public virtual void Remove()
    {
        unit.actionsRemaining += actionPoints;
        unit.orders.Remove(this);
        Destroy(gameObject);
    }
    public virtual void OnDestroy()
    {
        unit.orders.Remove(this);
    }

    // Interface Functions
    public override void Selected()
    {
        base.Selected();
        unit.team.Selection = this;
    }
    public override void Action(Vector3 point)
    {
        base.Action(point);
        unit.ToggleCommands(true);
        transform.position = point;
    }
    public override void Deselected()
    {
        base.Deselected();
        unit.ToggleCommands(true);
        unit.team.Selection = unit;
    }

    public override void Cancel()
    {
        Remove();
    }
}
