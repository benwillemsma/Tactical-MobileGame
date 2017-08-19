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
    private bool showGUI = false;

    private void OnGUI()
    {
        int y = 0;
        GUI.Label(new Rect(100 * (int)team + 30, y, 100 * (int)team + 100, 20), "");
    }

    #region Player initialization
    private void Start()
    {
        LocalTeam = this;

        if (isLocalPlayer)
        {
            TurnButton = GameObject.Find("ReadyButton").GetComponent<Button>();
            TurnButton.onClick.AddListener(delegate { ToggleReady(); });

            CmdInitPlayer();

            GameManager.Instance.AddPlayer(this);
        }
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log(teamName);
    }

    [Command]
    private void CmdInitPlayer()
    {
        NetworkManager netManager = FindObjectOfType<NetworkManager>();
        team = (Team)netManager.numPlayers - 1;
        teamLayer = 9 + (int)team;

        LobbyManager lobbyManager = FindObjectOfType<LobbyManager>();
        teamName = lobbyManager.GetLobbyPlayer((int)team).teamName;
        teamColor = lobbyManager.GetLobbyPlayer((int)team).teamColor;

        RpcInitPlayer(team, teamName, teamColor, teamLayer);
        
        CmdCreateUnit();
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
    public void CmdCreateUnit()
    {
        GameObject unit = Instantiate(UnitPrefab, transform.position, transform.rotation);
        NetworkServer.SpawnWithClientAuthority(unit, connectionToClient);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance)
            GameManager.Instance.s_teams[(int)team] = null;
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
        if (hit.collider != null)
        {
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
    }

    private void CancelSelection(Vector3 point)
    {
        tapCooldown = 0;
        RaycastHit hit = GameManager.ScreenRay(point, gameObject.layer);
        if (hit.collider != null)
        {
            ISelectable hitObject = hit.transform.gameObject.GetComponent<ISelectable>();
            if (hitObject != null)
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
        if (GameManager.Instance)
        {
            GameManager.Instance.s_playerReady[(int)team] = true;
            GameManager.Instance.CheckReadyStates();
        }
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
