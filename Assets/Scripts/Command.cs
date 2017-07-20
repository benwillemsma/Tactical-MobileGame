using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public enum CommandType
{
    Move,
    Shoot
}

public class Command : MonoBehaviour, ISelectable
{
    public CommandType type;

    public Unit unit;

    private bool selected = false;

    private void Start()
    {
        unit.orders.Add(this);
    }

    private void Update()
    {
        if (selected)
            transform.position = GameManager.ScreenRay(Input.mousePosition).point;
    }

    public void Remove()
    {
        Debug.Log("removed");
        unit.orders.Remove(this);
        Destroy(gameObject);
    }

    //Interface Functinos
    public virtual void Selected()
    {
        GameManager.instance.Selection = this;
        selected = true;
    }

    public void Action(Vector3 point)
    {
        selected = false;
        transform.position = point;
        Deselected();
    }

    public void DoubleClicked()
    {

    }

    public virtual void Deselected()
    {
        GameManager.instance.Selection = unit;
    }
}
