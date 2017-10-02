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
        {
#if UNITY_STANDALONE
            transform.position = GameManager.ScreenRay(Input.mousePosition).point;
#endif
#if UNITY_ANDROID
            transform.position = GameManager.ScreenRay(Input.GetTouch(0).position).point;
#endif
        }
    }

    public virtual void Selected()
    {
        selected = true;
        gameObject.layer = 2;
    }
    public virtual void Deselected()
    {
        PlayerTeam.localTeam.Selection = null;
        gameObject.layer = normalLayer;
        selected = false;
    }
    public virtual void Cancel() { }
}
