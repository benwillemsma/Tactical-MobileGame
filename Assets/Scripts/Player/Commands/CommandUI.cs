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
        unit.player.Selection = this;
    }
    
    public virtual void Deselected()
    {
        if (unit.actionsRemaining >= commandPrefab.actionPoints)
        {
#if UNITY_STANDALONE
            Vector3 startpos  = GameManager.ScreenRay(Input.mousePosition).point;
#endif
#if UNITY_ANDROID
            Vector3 startpos = GameManager.ScreenRay(Input.GetTouch(0).position).point;
#endif

            Command tempCmd = Instantiate(commandPrefab, startpos, Quaternion.identity, GameObject.Find("Commands").transform).GetComponent<Command>();
            unit.AddOrder(tempCmd);
            unit.ToggleCommands(false);
            tempCmd.unit = unit;
            tempCmd.Selected();
        }
    }
    public virtual void Cancel() { }
}