using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    [SerializeField]
    protected int captureProgress = 3;

    [SerializeField]
    protected Team teamAssosiation;
    protected Team team
    {
        get {return teamAssosiation; }
        set
        {
            teamAssosiation = value;
            if (teamAssosiation == value)
                changeTeams(value);
        }
    }

    private void Start()
    {
        GameManager.captures.Add(this);
    }

    public virtual void CalculateCapture()
    {
        Collider[] capturingUnits = Physics.OverlapSphere(transform.position, 1.25f, LayerMask.GetMask("Selectable"));

        for (int i = 0; i < capturingUnits.Length; i++)
        {
            if (capturingUnits[i].CompareTag("Unit"))
                captureProgress -= capturingUnits[i].GetComponent<Unit>().team == Team.Team_1 ? 1 : -1;
        }

        captureProgress = Mathf.Clamp(captureProgress, 0, 6);

        if (captureProgress <= 0)
            team = Team.Team_1;

        else if (captureProgress > 0 && captureProgress < 6)
            team = Team.Neutral;

        else if (captureProgress >= 6)
            team = Team.Team_2;
    }

    void changeTeams(Team teamNumber)
    {
        //change graphics representation
    }
}
