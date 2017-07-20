using UnityEngine;

[System.Serializable]
public class CommandUI : MonoBehaviour, ISelectable
{
    [SerializeField]
    private Unit unit;

    protected GameObject cursor;
    protected GameObject line;
    public Command CommandPoint;

    //Interface Functinos
    public virtual void Selected()
    {
        Command tempCmd = Instantiate(CommandPoint, unit.transform.position, unit.transform.rotation, GameObject.Find("Commands").transform).GetComponent<Command>();
        tempCmd.unit = unit;
        tempCmd.Selected();
    }

    public void Action(Vector3 direction)
    {
        unit.Deselected();
    }

    public void DoubleClicked()
    {

    }

    public virtual void Deselected()
    {
        GameManager.instance.Selection = unit;
    }
}