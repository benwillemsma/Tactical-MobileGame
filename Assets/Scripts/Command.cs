using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CommandType
{
    Move,
    Shoot
}

[System.Serializable]
public class Command:MonoBehaviour, IClickable
{
    static GameObject imagePrefab;

    protected GameObject cursor;
    protected GameObject line;

    protected Vector3 startPos;  
    protected Vector3 actionPos;

    public Command(int newCursor, int newLine, Vector3 start, Vector3 end)
    {
        if (!imagePrefab)
            imagePrefab = GameObject.Find("blankImage");

        startPos = start;
        actionPos = end;

        Image template = imagePrefab.GetComponent<Image>();
        template.sprite = GameManager.instance.cursors[newCursor];
        cursor = Instantiate(imagePrefab, actionPos, imagePrefab.transform.rotation) as GameObject;

        template.sprite = GameManager.instance.cursors[newLine];
        line = Instantiate(imagePrefab, (startPos + actionPos) / 2, Quaternion.FromToRotation(Vector3.up, actionPos - startPos)) as GameObject;
        line.transform.localScale = new Vector3((actionPos - startPos).magnitude, 1, 1);
    }

    public void moveCursor(Vector3 newPos)
    {
        cursor.transform.position = newPos;
        line.transform.position = (newPos + startPos) / 2;
        Quaternion.FromToRotation(Vector3.up, actionPos - startPos);
        line.transform.localScale = new Vector3((newPos - startPos).magnitude, 1, 1);
    }

    //Interface Functinos
    public virtual void HasBeenClicked() { }
    public virtual void HasBeenDoubleClicked() { }

}
