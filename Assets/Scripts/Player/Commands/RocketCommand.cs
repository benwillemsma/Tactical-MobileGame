using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketCommand : Command
{
    public MoveableObject blank;

    protected override void Start()
    {
        base.Start();
        blank.transform.parent = transform.parent;
        blank.normalLayer = normalLayer;
    }

    protected override void Update()
    {
        base.Update();

        if (unit.orders.Count >= 2)
        {
            Transform lastOrder = unit.orders[unit.orders.Count - 2].transform;
            if (selected)
                blank.transform.position = (transform.position + lastOrder.transform.position) / 2;
            CurveFromTo(lastOrder.transform.position, blank.transform.position, transform.position, visualMarkers[0]);
        }
        else
        {
            if (selected)
                blank.transform.position = (transform.position + unit.transform.position) / 2;
            CurveFromTo(unit.transform.position, blank.transform.position, transform.position, visualMarkers[0]);
        }
    }

    public override void Action(Vector3 point)
    {
        base.Action(point);
        blank.Selected();
        unit.team.Selection = blank;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(blank.gameObject);
    }
}
