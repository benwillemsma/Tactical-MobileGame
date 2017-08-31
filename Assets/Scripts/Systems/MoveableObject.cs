using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObject : MonoBehaviour, ISelectable
{
    protected bool selected = false;
    public int normalLayer;

    protected virtual void Update()
    {
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

    public virtual void Selected()
    {
        selected = true;
        gameObject.layer = 2;
    }
    public virtual void Action(Vector3 point)
    {
        Deselected();
    }
    public virtual void Deselected()
    {
        gameObject.layer = normalLayer;
        selected = false;
    }
    public virtual void Cancel() { }
}
