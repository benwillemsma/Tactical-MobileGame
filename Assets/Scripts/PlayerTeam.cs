using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerTeam : NetworkBehaviour
{
    public static PlayerTeam LocalTeam;
    public GameObject UnitPrefab;

    [SyncVar]
    public string teamName;
    [SyncVar]
    public Color teamColor;
    
    public Team team;
    public float score;
    public bool ready;

    private List<Unit> units = new List<Unit>();

    [Command]
    void CmdInitPlayer(GameObject team)
    {
        Debug.Log("Server");

        name = "Team: " + GameManager.teams.Count;
        teamName = name;
        teamColor = Random.ColorHSV();

        GameObject unit = Instantiate(UnitPrefab, transform.position, transform.rotation);
        NetworkServer.SpawnWithClientAuthority(unit, team.gameObject);
    }

    private void Start()
    {
        Debug.Log("Start");

        LocalTeam = this;

        team = (Team)GameManager.teams.Count;
        GameManager.teams.Add(this);

        if (isLocalPlayer)
            CmdInitPlayer(gameObject);
    }
    private void OnDestroy()
    {
        GameManager.teams.Remove(this);
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
            ready = true;

        if (units.Count <= 0)
            GameManager.teams.Remove(this);

        else if (score >= GameManager.winningScore)
            GameManager.winner = this;
    }

    public void InvokeCommands()
    {
        for (int i = 0; i < units.Count; i++)
            StartCoroutine(units[i].InvokeCommands());
    }

    public void AddUnits(params Unit[] units)
    {
        this.units.AddRange(units);
    }
    public void RemoveUnits(params Unit[] units)
    {
        for (int i = 0; i < units.Length; i++)
            this.units.Remove(units[i]);
    }
}
