using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : Command
{
    private List<MoveCommand> moves = new List<MoveCommand>();
    private float remainingMove = 10;
    
    protected override void Start()
    {
        base.Start();
        GetMoves();
    }

    protected override void Update()
    {
        if (unit == null)
        {
            Remove();
            return;
        }

        base.Update();

        visualMarkers[0].gameObject.SetActive(selected ? true : false);

        float availableMove = unit.maxMoveDistance;
        Vector3 start = unit.transform.position;
        Vector3 direction = start - transform.position;

        int index = moves.IndexOf(this);
        if (index != 0 && moves[index - 1])
        {
            availableMove = moves[index - 1].remainingMove;

            start = moves[index - 1].transform.position;
            direction = start - transform.position;
            direction = Vector3.ClampMagnitude(direction, availableMove);

            remainingMove = moves[index - 1].remainingMove - direction.magnitude;
            visualMarkers[0].localScale = Vector3.one * availableMove;

            transform.position = ClosestPointOnNavmesh(start, direction);
        }
        else
        {
            visualMarkers[0].localScale = Vector3.one * unit.maxMoveDistance;
            direction = Vector3.ClampMagnitude(direction, unit.maxMoveDistance);
            remainingMove = unit.maxMoveDistance - direction.magnitude;
            transform.position = ClosestPointOnNavmesh(start, direction);
        }

        if (availableMove < 0.2f)
        {
            Remove();
            return;
        }
        LineFromTo(start, transform.position, visualMarkers[1]);
        visualMarkers[0].position = start + groundOffset;
	}

    public override void Remove()
    {
        base.Remove();
    }

    private void GetMoves()
    {
        for (int i = 0; i < unit.orders.Count; i++)
        {
            if (unit.orders[i].GetType() == typeof(MoveCommand))
                moves.Add((MoveCommand)unit.orders[i]);
        }
    }

    public override void Action(Vector3 point)
    {
        base.Action(point);
    }

    private Vector3 ClosestPointOnNavmesh(Vector3 start,Vector3 direction)
    {
        LayerMask mask = LayerMask.GetMask("Level");
        Vector3 point = start - direction;
        RaycastHit rayHit;
        if (Physics.SphereCast(start + Vector3.up, 0.3f, -direction + Vector3.up, out rayHit, direction.magnitude, mask))
            return rayHit.point;

        NavMeshHit meshHit;
        if (!NavMesh.SamplePosition(point, out meshHit, 0.1f, 1))
            if (NavMesh.SamplePosition(point, out meshHit, 5f, 1))
                return meshHit.position;
        return point;
    }
}