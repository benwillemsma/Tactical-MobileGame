using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerTeam : NetworkBehaviour
{
    public static PlayerTeam localTeam;
    [SerializeField]
    private Button TurnButton;
    [SerializeField]
    private GameObject UnitPrefab;

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
        int y = 2;
        GUI.color = teamColor;
        if (showGUI)
        {
            GUI.Label(new Rect(100 * (int)team, y, 300, 20), "" + teamName);
            GUI.Label(new Rect(100 * (int)team + 50, y, 300, 20), "" + units.Count);
        }
    }

    public override void OnStartLocalPlayer()
    {
        localTeam = this;
        TurnButton = GameObject.Find("ReadyButton").GetComponent<Button>();
        TurnButton.onClick.AddListener(delegate { PlayerReady(); });
    }

    #region Player initialization
    private void Start()
    {
        if(isLocalPlayer)
            showGUI = true;

        LobbyManager lobbyManager = FindObjectOfType<LobbyManager>();
        GameManager.Instance.AddPlayer(this);

        if (isServer)
        {
            team = (Team)GameManager.Instance.s_teams.IndexOf(this);

            teamLayer = 9 + (int)team;

            teamName = lobbyManager.GetLobbyPlayer((int)team).teamName;
            teamColor = lobbyManager.GetLobbyPlayer((int)team).teamColor;

            CmdCreateUnit();
        }
    }
    [Command]
    private void CmdCreateUnit()
    {
        GameObject unitObject = Instantiate(UnitPrefab, transform.position, transform.rotation);
        NetworkServer.SpawnWithClientAuthority(unitObject, connectionToClient);
        RpcCreateUnit(unitObject);
    }
    [ClientRpc]
    private void RpcCreateUnit(GameObject unitObject)
    {
        Debug.Log("RpcCreateUnit");
        Unit unit = unitObject.GetComponent<Unit>();
        unit.team = this;
        unit.Team = team;
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
        if (tapCooldown > 0)
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

            if (Input.GetKeyDown(KeyCode.Space))
                PlayerReady();
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

    public void PlayerReady()
    {
        canInput = !canInput;
        TurnButton.gameObject.SetActive(canInput);

        CmdPlayerReady();
    }

    [ClientRpc]
    public void RpcToggleReady()
    {
        if (isLocalPlayer)
        {
            canInput = !canInput;
            TurnButton.gameObject.SetActive(canInput);
        }
    }

    [Command]
    public void CmdPlayerReady()
    {
        GameManager.Instance.s_playersReady++;
        GameManager.Instance.CheckReadyStates();
    }
    #endregion
}
