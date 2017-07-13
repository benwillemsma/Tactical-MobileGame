using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public enum CommandType
{
    Move,
    Shoot
}

[System.Serializable]
public class CommandUI : MonoBehaviour, IClickable
{
    public CommandType type;

    protected GameObject cursor;
    protected GameObject line;
    protected Command cmd;

    //Interface Functinos
    public virtual void HasBeenClicked()
    {
        cmd = new Command(transform.parent.GetComponent<Unit>(), type, transform.parent.position);
        GameManager.instance.commandSelected = true;
    }
    public virtual void HasBeenDoubleClicked() { }
}

public class Command
{
    static GameObject imagePrefab;

    protected UnityEvent command;
    protected Unit unit;

    protected GameObject cursor;
    protected GameObject line;

    protected Collider col;

    protected Vector3 startPos;
    protected Vector3 actionPos;

    public Command(Unit unit, CommandType type, Vector3 start)
    {
        if (!imagePrefab)
            imagePrefab = GameObject.Find("blankImage");

        this.unit = unit;
        startPos = start;
        actionPos = start;

        Image template = imagePrefab.GetComponent<Image>();
        template.sprite = GameManager.instance.cursors[(int)type];
        cursor = Object.Instantiate(imagePrefab, actionPos, imagePrefab.transform.rotation) as GameObject;
        col = cursor.AddComponent<BoxCollider>();

        template.sprite = GameManager.instance.cursors[(int)type];
        line = Object.Instantiate(imagePrefab, (startPos + actionPos) / 2, Quaternion.FromToRotation(Vector3.up, actionPos - startPos)) as GameObject;
        line.transform.localScale = new Vector3((actionPos - startPos).magnitude, 1, 1);
    }

    public void moveCursor(Vector3 newPos)
    {
        cursor.transform.position = newPos;
        line.transform.position = (newPos + startPos) / 2;
        Quaternion.FromToRotation(Vector3.up, actionPos - startPos);
        line.transform.localScale = new Vector3((newPos - startPos).magnitude, 1, 1);
    }

    public void placeCursor(Vector3 newPos)
    {
        GameManager.instance.commandSelected = true;
    }

    public void Invoke()
    {
        command.Invoke();
    }

    public void Remove()
    {
        Object.Destroy(cursor);
        Object.Destroy(line);
    }
}
