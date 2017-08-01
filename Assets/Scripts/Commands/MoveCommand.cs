using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    private List<MoveCommand> moves = new List<MoveCommand>();
    private float remainingMove;

    protected override void Start()
    {
        base.Start();

        for (int i = 0; i < unit.orders.Count; i++)
        {
            if (unit.orders[i].GetType() == typeof(MoveCommand))
                moves.Add((MoveCommand)unit.orders[i]);
        }
    }

    protected override void Update ()
    {
        if (unit == null)
        {
            Destroy(gameObject);
            return;
        }
        base.Update();

        visualMarkers[0].gameObject.SetActive(selected ? true : false);

        float availableMove = unit.maxMoveDistance;
        Vector3 start = unit.transform.position;
        Vector3 direction = start - transform.position;

        for (int i = 0; i < moves.Count; i++)
        {
            if (i != 0 && moves[i - 1])
            {
                availableMove = moves[i - 1].remainingMove;
                if (availableMove < 0.1f)
                {
                    Deselected();
                    Remove();
                    unit.ToggleCommands();
                    break;
                }

                start = moves[i - 1].transform.position;
                direction = start - transform.position;
                direction = Vector3.ClampMagnitude(direction, availableMove);

                remainingMove = moves[i - 1].remainingMove - direction.magnitude;
                visualMarkers[0].localScale = Vector3.one * availableMove;
                transform.position = start - direction;
            }
            else
            {
                remainingMove = unit.maxMoveDistance - direction.magnitude;
                visualMarkers[0].localScale = Vector3.one * unit.maxMoveDistance;
                direction = Vector3.ClampMagnitude(direction, unit.maxMoveDistance);
                transform.position = start - direction;
            }
            LineFromTo(start, transform.position, Mathf.RoundToInt(direction.magnitude), visualMarkers[1]);
            visualMarkers[0].position = start + groundOffset;
        }
	}
}
