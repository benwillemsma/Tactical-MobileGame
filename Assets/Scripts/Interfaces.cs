using UnityEngine;

public interface ISelectable
{
    GameObject gameobject { get;}

    void Selected();
    void Action(Vector3 point);
    void Deselected();
}
