﻿using UnityEngine;

[System.Serializable]
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
            Command tempCmd = Instantiate(commandPrefab, unit.transform.position, unit.transform.rotation, GameObject.Find("Commands").transform).GetComponent<Command>();
            unit.actionsRemaining -= tempCmd.actionPoints;
            tempCmd.unit = unit;
            tempCmd.Selected();
        }
    }

    public virtual void Action(Vector3 direction) { }
    public virtual void Deselected() { }
    public virtual void Cancel() { }
}