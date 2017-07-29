using UnityEngine;

public class FlagPoint : CapturePoint
{
    public override void CalculateCapture()
    {
        Collider[] capturingUnits = Physics.OverlapSphere(transform.position, 1.25f, LayerMask.GetMask("Selectable"));

        for (int i = 0; i < capturingUnits.Length; i++)
        {
            if (capturingUnits[i].CompareTag("Unit"))
            {
                Unit unit = capturingUnits[i].GetComponent<Unit>();
                if (unit.playerTeam != teamAssosiation)
                {
                    unit.hasFlag = true;
                    Transform temp = capturingUnits[i].transform;
                    transform.parent = temp;
                    transform.position = temp.position - temp.forward * 0.5f + temp.up;

                    enabled = false;
                }
            }
        }
    }
}
