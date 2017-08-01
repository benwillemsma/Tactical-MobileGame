using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CapturePoint : NetworkBehaviour
{
    [SyncVar,SerializeField]
    protected int captureProgress = 0;

    [SyncVar, SerializeField]
    protected Team teamAssosiation;

    private void Start()
    {
        if (isServer)
        {
            GameManager.Instance.s_captures.Add(this);
            NetworkServer.Spawn(gameObject);
        }
    }

    public virtual void CalculateCapture()
    {
        if (!isServer)
            return;

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
            GameManager.Instance.s_teams[(int)teamAssosiation].score++;
    }

    void changeTeams()
    {
        if (teamAssosiation != Team.Neutral)
            GetComponent<Renderer>().material.color = GameManager.Instance.s_teams[(int)teamAssosiation].teamColor;
        else
            GetComponent<Renderer>().material.color = Color.yellow;
    }
}
