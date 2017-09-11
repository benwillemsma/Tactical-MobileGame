using UnityEngine;

public class CommandUI : MonoBehaviour, ISelectable
{
    [SerializeField]
    private Unit unit;

    protected GameObject cursor;
    protected GameObject line;
    public Command commandPrefab;

    //Interface Functinos
    public virtual void Selected()
    {
        if (unit.actionsRemaining >= commandPrefab.actionPoints)
        {
            Command tempCmd = Instantiate(commandPrefab, unit.transform.position, Quaternion.identity, GameObject.Find("Commands").transform).GetComponent<Command>();
            unit.AddOrder(tempCmd);
            unit.ToggleCommands(false);
            tempCmd.unit = unit;
            tempCmd.Selected();
        }
    }

    public virtual void Action(Vector3 direction) { }
    public virtual void Deselected() { }
    public virtual void Cancel() { }
}