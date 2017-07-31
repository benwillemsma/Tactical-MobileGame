using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    [SerializeField]
    protected int captureProgress = 0;

    [SerializeField]
    protected Team teamAssosiation;

    private void Start()
    {
        GameManager.captures.Add(this);
    }

    public virtual void CalculateCapture()
    {
        Collider[] capturingUnits = Physics.OverlapSphere(transform.position, 1.25f, LayerMask.GetMask("Unit"));
        Team checkTeam = Team.Neutral;

        if (capturingUnits.Length > 0)
        {
            checkTeam = capturingUnits[0].gameObject.GetComponent<Unit>().Team;

            for (int i = 1; i < capturingUnits.Length; i++)
            {
                if (checkTeam != capturingUnits[i].gameObject.GetComponent<Unit>().Team)
                    return;
            }

            if (checkTeam != teamAssosiation)
            {
                captureProgress += capturingUnits.Length;
                if (captureProgress >= 3)
                {
                    captureProgress = 0;
                    teamAssosiation = checkTeam;
                    changeTeams();
                }
            }
        }
        if (teamAssosiation != Team.Neutral)
            GameManager.teams[(int)teamAssosiation].score++;
    }

    void changeTeams()
    {
        if (teamAssosiation != Team.Neutral)
            GetComponent<Renderer>().material.color = GameManager.teams[(int)teamAssosiation].teamColor;
        else
            GetComponent<Renderer>().material.color = Color.yellow;
    }
}
