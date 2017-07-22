using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public enum CommandType
{
    Move,
    Shoot,
    Grenade
}

public class Command : MonoBehaviour, ISelectable
{
    public CommandType type;

    public Unit unit;

    protected bool selected = false;
    protected Vector3 pointOffset = new Vector3(0.5f, 0, 1f);

    private void Start()
    {
        unit.orders.Add(this);
    }

    private void Update()
    {
        if (selected)
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    transform.position = GameManager.ScreenRay(Input.GetTouch(0).position).point + pointOffset;
                if (Input.GetTouch(0).phase == TouchPhase.Ended)
                    Action(GameManager.ScreenRay(Input.GetTouch(0).position).point + pointOffset);
            }
            else
                transform.position = GameManager.ScreenRay(Input.mousePosition).point;
    }

    public void Remove()
    {
        unit.orders.Remove(this);
        Destroy(gameObject);
    }

    //Interface Functions
    public virtual void Selected()
    {
        Debug.Log(this + ": was selected");
        GameManager.instance.Selection.Enqueue(this);
        Debug.Log(GameManager.instance.Selection.Peek());
        gameObject.layer = 2;
        selected = true;
    }

    public virtual void Action(Vector3 point)
    {
        Debug.Log("Command: action");
        Deselected();
        selected = false;
        unit.ToggleCommands();
        transform.position = point;
    }

    public virtual void Deselected()
    {
        Debug.Log(this + ": was deselected");
        gameObject.layer = 8;
        GameManager.instance.Selection.Dequeue();
    }
}
