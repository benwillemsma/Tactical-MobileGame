using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketCommand : Command
{
    protected override void Start()
    {
        base.Start();
        blankCommand.transform.parent = transform.parent;
        blankCommand.normalLayer = normalLayer;
    }

    protected override void Update()
    {
        base.Update();

        if (unit.orders.Count >= 2)
        {
            Transform lastOrder = unit.orders[unit.orders.Count - 2].transform;
            if (selected)
                blankCommand.transform.position = (transform.position + lastOrder.transform.position) / 2;
            CurveFromTo(lastOrder.transform.position, blankCommand.transform.position, transform.position, visualMarkers[0]);
        }
        else
        {
            if (selected)
                blankCommand.transform.position = (transform.position + unit.transform.position) / 2;
            CurveFromTo(unit.transform.position, blankCommand.transform.position, transform.position, visualMarkers[0]);
        }
    }

    public override void Action(Vector3 point)
    {
        base.Action(point);
        blankCommand.Selected();
        unit.team.Selection = blankCommand;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(blankCommand.gameObject);
    }
}
