using UnityEngine;

public interface ISelectable
{
    void Selected();
    void Action(Vector3 point);
    void DoubleClicked();
    void Deselected();
}
