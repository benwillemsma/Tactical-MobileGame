using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class PlayerTeam : NetworkBehaviour
{
    #region Player initialization
    public static PlayerTeam localTeam;
    private Button TurnButton;
    [SerializeField]
    private GameObject[] UnitPrefabs;

    public List<Unit> units = new List<Unit>();
    public ISelectable Selection;
    
    [SyncVar, Space(10)]
    public Team team;
    [SyncVar, HideInInspector]
    public string teamName;
    [SyncVar, HideInInspector]
    public Color teamColor;
    [SyncVar,HideInInspector]
    public int teamLayer;

    [SyncVar, Space(10)]
    public float score;

    [HideInInspector]
    public bool canInput = true;
    private bool showGUI = false;

    private void OnGUI()
    {
        if (Debug.isDebugBuild && showGUI)
        {
            int y = -1;
            GUI.color = teamColor;
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

    private IEnumerator Start()
    {
        while (!GameManager.instance)
            yield return null;
        GameManager.instance.AddPlayer(this);
        if (isLocalPlayer)
            showGUI = true;

        if (isServer)
        {
            team = (Team)GameManager.instance.s_teams.Count - 1;
            teamLayer = 9 + (int)team;
            teamName = LobbyPlayer.players[(int)team].teamName;
            teamColor = LobbyPlayer.players[(int)team].teamColor;
            GameManager.instance.Output(team + ": " + teamName);

            while (!connectionToClient.isReady)
                yield return null;
            CmdCreateUnit();
            if ((GameManager.instance.s_playersInit += 1) == NetworkServer.connections.Count)
                GameManager.instance.PlayersInititilized = true;
        }
    }
    [Command]
    private void CmdCreateUnit()
    {
        GameObject unitObject = Instantiate(UnitPrefabs[PlayerPrefs.GetInt("UnitPrefab", 0)], transform.position, transform.rotation);
        NetworkServer.SpawnWithClientAuthority(unitObject, connectionToClient);
        Unit unit = unitObject.GetComponent<Unit>();
        unit.team = team;
    }

    private void OnDestroy()
    {
        if (GameManager.instance)
            GameManager.instance.s_teams.Remove(this);
    }
    #endregion

    #region Player Selection/Controls
    private void Update()
    {
        if (gameObject.layer != teamLayer)
            gameObject.layer = teamLayer;

        if (isLocalPlayer && canInput)
        {
            if (Input.GetButtonDown("LeftClick"))
                CheckSelection(Input.mousePosition);

            #if UNITY_ANDROID
            if (Selection != null && Input.GetButtonUp("LeftClick"))
                CheckSelection(Input.mousePosition);
            #endif
        }
    }
    
    private void CheckSelection(Vector3 point)
    {
        RaycastHit hit = GameManager.ScreenRay(point, gameObject.layer);
        if (hit.collider != null)
        {
            ISelectable hitObject = hit.transform.gameObject.GetComponent<ISelectable>();

            if (Selection != null && !Selection.Equals(null))
                Selection.Deselected();
            if (hitObject != null)
                hitObject.Selected();
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

    [Command]
    public void CmdPlayerReady()
    {
        GameManager.instance.ReadyPlayers++;
        GameManager.instance.CheckReadyStates();
    }

    [ClientRpc]
    public void RpcToggleReady()
    {
        if (isLocalPlayer)
        {
            if (Selection != null)
                Selection.Deselected();
            canInput = !canInput;
            TurnButton.gameObject.SetActive(canInput);
        }
    }
    #endregion
}

