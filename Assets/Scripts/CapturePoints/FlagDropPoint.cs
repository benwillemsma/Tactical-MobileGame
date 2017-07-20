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
                if (unit.team == team && unit.hasFlag)
                {
                    GameManager.teamPoints[i]++;
                }
            }
        }
    }
}
