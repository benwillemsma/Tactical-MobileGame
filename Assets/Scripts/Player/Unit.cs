using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Networking;

public class Unit : NetworkBehaviour, ISelectable,IDamageable
{
    # region Initilization
    private NavMeshAgent agent;
    public NavMeshAgent Pathfinding { get { return agent; } }

    public float speed;
    public float health;
    public float maxMoveDistance;
    public int actionsRemaining = 1;

    public bool hasFlag = false;
    
    [SyncVar(hook = "InitUnit"), Space(20)]
    public Team Team;
    public PlayerTeam team;

    [Space(20)]
    public List<Command> orders;

    private GameObject Model;
    private GameObject commandUI;
    private Transform objectSpawn;

    private void Start ()
    {
        agent = GetComponent<NavMeshAgent>();
        orders = new List<Command>();
        commandUI = transform.GetChild(0).gameObject;
        Model = transform.GetChild(1).gameObject;
        objectSpawn = Model.transform.GetChild(0);
    }
    public void InitUnit(Team newTeam)
    {
        gameObject.layer = team.teamLayer;
        transform.parent = team.transform;

        for (int i = 0; i < commandUI.transform.childCount; i++)
            commandUI.transform.GetChild(i).gameObject.layer = team.teamLayer;
        
        Model.GetComponent<Renderer>().material.color = team.teamColor;

        team.AddUnits(this);
    }

    public void AddOrder(Command newCommand,int index = -1)
    {
        if (index == -1)
            orders.Add(newCommand);
        else orders.Insert(index, newCommand);
    }

    #endregion

    #region ISelectable
    public virtual void Selected()
    {
        team.Selection = this;
        ToggleCommands(true);
    }

    public virtual void Action(Vector3 point)
    {
        Deselected();
    }

    public virtual void Deselected()
    {
        ToggleCommands(false);
        team.Selection = null;
    }

    public virtual void Cancel() { Deselected(); }

    public void ToggleCommands(bool active)
    {
        commandUI.SetActive(active);
    }
    #endregion

    #region IDamageable
    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            Die();
    }

    public virtual void Die()
    {
        if (orders.Count > 0)
        {
            StopAllCoroutines();
            orders.Clear();
            CmdUnitDone();
        }

        team.RemoveUnits(this);
        Destroy(gameObject);
    }

    public void OnDestroy()
    {
        StopAllCoroutines();
        team.RemoveUnits(this);
    }
    #endregion

    #region Unit Actions
    public virtual IEnumerator InvokeCommands()
    {
        if (orders.Count > 0)
        {
            CmdUnitBuisy();

            while (orders.Count > 0)
            {
                switch (orders[0].type)
                {
                    case CommandType.Move:
                        yield return Move(orders[0].transform.position);
                        break;
                    case CommandType.Shoot:
                        yield return Shoot(orders[0].spawnObject, orders[0].transform.position - transform.position);
                        break;
                    case CommandType.Grenade:
                        yield return Grenade(orders[0].spawnObject, orders[0].transform.position - transform.position);
                        break;
                    case CommandType.Rocket:
                        yield return Rocket(orders[0].spawnObject, (orders[0] as RocketCommand).blankCommand.transform.position - transform.position, orders[0]);
                        break;
                    case CommandType.Melee:
                        yield return Melee(orders[0].spawnObject,orders[0].transform.position - transform.position);
                        break;
                    default:
                        Debug.Log("Command No Implimented:" + orders[0].type);
                        break;
                }
                if (orders.Count > 0)
                    orders[0].Remove();
            }
            CmdUnitDone();
        }
        actionsRemaining = 2;
    }

    [Command]
    public void CmdUnitBuisy()
    {
        GameManager.Instance.s_unitsWithOrders++;
    }

    [Command]
    public void CmdUnitDone()
    {
        GameManager.Instance.s_unitsWithOrders--;
    }

    public IEnumerator Move(Vector3 destination)
    {
        transform.LookAt(destination);
        Vector3 startPos = transform.position;

        while ((transform.position - destination).magnitude > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);
            yield return null;
        }
    }

    public IEnumerator Shoot(GameObject spawnObject, Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
        yield return new WaitForSeconds(0.2f);
        CmdSpawnObject(spawnObject);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator Grenade(GameObject spawnObject, Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
        yield return new WaitForSeconds(0.2f);
        CmdSpawnObject(spawnObject);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator Rocket(GameObject spawnObject, Vector3 direction,Command rocketCmd)
    {
        transform.LookAt(transform.position + direction);
        yield return new WaitForSeconds(0.2f);
        CmdSpawnRocket(spawnObject, (rocketCmd as RocketCommand).blankCommand.transform.position, rocketCmd.transform.position);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator Melee(GameObject spawnObject, Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
        yield return new WaitForSeconds(0.2f);
        CmdSpawnObject(spawnObject);
        yield return new WaitForSeconds(0.2f);
    }

    [Command]
    public void CmdSpawnObject(GameObject spawnObject)
    {
        NetworkServer.Spawn(Instantiate(spawnObject, objectSpawn.position, objectSpawn.rotation));
    }

    [Command]
    public void CmdSpawnRocket(GameObject spawnObject,params Vector3[] guidePoints)
    {
        GameObject temp = Instantiate(spawnObject, objectSpawn.position, objectSpawn.rotation);
        temp.GetComponent<Rocket>().guidePoints = guidePoints;
        NetworkServer.Spawn(temp);
    }
    #endregion
}
