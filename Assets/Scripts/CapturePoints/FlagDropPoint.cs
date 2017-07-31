using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagDropPoint : CapturePoint
{
    public override void CalculateCapture()
    {
        Collider[] capturingUnits = Physics.OverlapSphere(transform.position, 1.25f, LayerMask.GetMask("Selectable"));

        for (int i = 0; i < capturingUnits.Length; i++)
        {
            if (capturingUnits[i].CompareTag("Unit"))
            {
                Unit unit = capturingUnits[i].GetComponent<Unit>();
                if (unit.Team == teamAssosiation && unit.hasFlag)
                {
                    GameManager.teams[i].score++;
                    FlagPoint flag = unit.GetComponentInChildren<FlagPoint>();
                    GameManager.captures.Remove(flag);
                    Destroy(flag.gameObject);
                    unit.hasFlag = false;
                }
            }
        }
    }
}
