using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerTeam : NetworkBehaviour
{
    public static PlayerTeam LocalTeam;

    public Button TurnButton;
    public GameObject UnitPrefab;

    public List<Unit> units = new List<Unit>();
    public ISelectable Selection;

    [SyncVar, Space(10)]
    public Team team;
    [SyncVar]
    public string teamName;
    [SyncVar]
    public Color teamColor;
    [SyncVar,HideInInspector]
    public int teamLayer;

    [SyncVar, Space(10)]
    public float score;

    [HideInInspector]
    public bool canInput = true;

    private float tapCooldown;

    #region Player initialization
    private void Start()
    {
        LocalTeam = this;

        if (isLocalPlayer)
        {
            TurnButton = GameObject.Find("TurnButton").GetComponent<Button>();
            TurnButton.onClick.AddListener(delegate { ToggleReady(); });

            CmdInitPlayer();
        }
    }
    [Command]
    private void CmdInitPlayer()
    {
        team = (Team)GameManager.Instance.s_teams.Count;
        teamName = "Team" + (GameManager.Instance.s_teams.Count + 1);
        teamColor = Random.ColorHSV();
        teamLayer = 9 + GameManager.Instance.s_teams.Count;

        RpcInitPlayer(team, teamName, teamColor, teamLayer);

        GameManager.Instance.AddPlayer(this);
        CmdCreateUnit(transform.position, transform.rotation);
    }
    [ClientRpc]
    private void RpcInitPlayer(Team team,string teamName,Color teamColor,int teamLayer)
    {
        this.teamLayer = teamLayer;
        this.team = team;
        this.teamName = teamName;
        this.teamColor = teamColor;
    }

    [Command]
    public void CmdCreateUnit(Vector3 spawnPoint,Quaternion spawnRotation)
    {
        GameObject unit = Instantiate(UnitPrefab, spawnPoint, spawnRotation);
        NetworkServer.SpawnWithClientAuthority(unit, gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.RemovePlayer(this);
    }
    #endregion

    #region Player Selection/Controls
    private void Update()
    {
        if(tapCooldown > 0)
            tapCooldown -= Time.deltaTime;

        if (gameObject.layer != teamLayer)
            gameObject.layer = teamLayer;

        if (isLocalPlayer && canInput)
        {
            if (Input.GetButtonDown("LeftClick") && tapCooldown <= 0)
            {
                tapCooldown = 0.2f;
                CheckSelection(Input.mousePosition);
            }
            else if
                (Input.GetButtonDown("LeftClick") && tapCooldown > 0)
            {
                if(Input.touchCount > 0)
                    CancelSelection(Input.mousePosition);
                else
                    CancelSelection();
            }

            else if (Input.GetButtonDown("RightClick"))
                CancelSelection(Input.mousePosition);

            if (Input.GetKeyDown(KeyCode.Space))
            ToggleReady();
        }
    }
    
    private void CheckSelection(Vector3 point)
    {
        RaycastHit hit = GameManager.ScreenRay(point, gameObject.layer);
        ISelectable hitObject = hit.transform.gameObject.GetComponent<ISelectable>();

        if (hitObject != null)
        {
            if (Selection != null)
            {
                Selection.Action(hit.point);
                hitObject.Selected();
            }
            else if (hitObject != Selection)
                hitObject.Selected();
        }
        else if (Selection != null)
            Selection.Action(hit.point);
    }

    private void CancelSelection(Vector3 point)
    {
        tapCooldown = 0;
        RaycastHit hit = GameManager.ScreenRay(point, gameObject.layer);
        ISelectable hitObject = hit.transform.gameObject.GetComponent<ISelectable>();
        if (hitObject != null)
        {
            hitObject.Cancel();
        }
    }
    private void CancelSelection()
    {
        tapCooldown = 0;
        if (Selection != null)
            Selection.Cancel();
        Selection = null;
    }

    #endregion

    #region Player Commands
    [ClientRpc]
    public void RpcInvokeCommands()
    {
        if (isLocalPlayer)
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

    public void ToggleReady()
    {
        canInput = false;
        TurnButton.gameObject.SetActive(false);
        CmdIsReady();
    }
    [Command]
    public void CmdIsReady()
    {
        GameManager.Instance.s_playerReady[(int)team] = true;
        GameManager.Instance.CheckReadyStates();
    }
    [ClientRpc]
    public void RpcToggleReady()
    {
        canInput = true;
        if (isLocalPlayer)
            TurnButton.gameObject.SetActive(true);
    }
    #endregion
}
