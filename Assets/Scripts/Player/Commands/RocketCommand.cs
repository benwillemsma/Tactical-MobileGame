using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketCommand : Command
{
    Transform lastOrder;

    protected override void Start()
    {
        base.Start();
        blankCommand.transform.parent = transform.parent;
        blankCommand.normalLayer = normalLayer;

        if (unit.orders.Count >= 2)
            lastOrder = unit.orders[unit.orders.Count - 2].transform;
        else lastOrder = unit.transform;
    }

    protected override void Update()
    {
        base.Update();

        if (lastOrder)
        {
            if (selected)
                blankCommand.transform.position = (transform.position + lastOrder.position) / 2;
            CurveFromTo(lastOrder.position, blankCommand.transform.position, transform.position, visualMarkers[0]);
        }
    }

    public override void Deselected()
    {
        base.Deselected();

#if UNITY_STANDALONE
        blankCommand.Selected();
#endif
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Destroy(blankCommand.gameObject);
    }
}
