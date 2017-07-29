using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    [SerializeField]
    protected int captureProgress = 2;

    [SerializeField]
    protected Team teamAssosiation;

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
                captureProgress -= capturingUnits[i].GetComponent<Unit>().playerTeam == Team.Team_1 ? 1 : -1;
        }

        captureProgress = Mathf.Clamp(captureProgress, 0, 4);

        if (captureProgress <= 0)
            teamAssosiation = Team.Team_1;

        else if (captureProgress > 0 && captureProgress < 4)
            teamAssosiation = Team.Neutral;

        else if (captureProgress >= 4)
            teamAssosiation = Team.Team_2;

        changeTeams(teamAssosiation);
        if (teamAssosiation != Team.Neutral)
            GameManager.teams[(int)teamAssosiation].score++;
    }

    void changeTeams(Team teamNumber)
    {
        GetComponent<Renderer>().material.color = GameManager.teams[(int)teamNumber].teamColor;
    }
}
