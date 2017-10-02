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
    [SyncVar]
    protected Color teamColor = Color.grey;

    public List<Unit> capturingUnits = new List<Unit>();

    private void Start()
    {
        if (isServer)
            GameManager.instance.s_captures.Add(this);
        GetComponent<Renderer>().material.color = teamColor;
    }

    [Command]
    public virtual void CmdCalculateCapture()
    {
        Team checkTeam = Team.Neutral;

        if (capturingUnits.Count > 0)
        {
            checkTeam = capturingUnits[0].team;

            for (int i = 1; i < capturingUnits.Count; i++)
            {
                if (checkTeam != capturingUnits[i].GetComponent<Unit>().team)
                    return;
            }

            if (checkTeam != teamAssosiation)
            {
                captureProgress += capturingUnits.Count;
                if (captureProgress >= 3)
                {
                    captureProgress = 0;
                    teamAssosiation = checkTeam;
                    teamColor = GameManager.instance.s_teams[(int)teamAssosiation].teamColor;
                    RpcChangeTeams(teamColor);
                }
            }
        }
        if (teamAssosiation != Team.Neutral)
            GameManager.instance.s_teams[(int)teamAssosiation].score++;
    }

    [ClientRpc]
    void RpcChangeTeams(Color newColor)
    {
        GetComponent<Renderer>().material.color = newColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit temp = other.GetComponent<Unit>();
        if (temp)
            capturingUnits.Add(temp);
    }

    private void OnTriggerExit(Collider other)
    {
        Unit temp = other.GetComponent<Unit>();
        if (temp)
            capturingUnits.Remove(temp);
    }
}
