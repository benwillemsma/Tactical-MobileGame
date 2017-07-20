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

    public GameObject gameobject { get { return gameObject; } }

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
        gameObject.layer = 2;
        GameManager.instance.Selection = this;
        selected = true;
    }

    public virtual void Action(Vector3 point)
    {
        Deselected();
        selected = false;
        transform.position = point;
    }

    public virtual void Deselected()
    {
        GameManager.instance.Selection = null;
        gameObject.layer = 8;
    }
}
