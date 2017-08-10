﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Unit : NetworkBehaviour, ISelectable,IDamageable
{
    # region Initilization
    public float speed;
    public float health;
    public float maxMoveDistance;

    public bool hasFlag = false;

    public Team Team { get { return team.team; } }
    
    public PlayerTeam team;
    private GameObject Model;

    [SerializeField]
    GameObject commandUI;

    public GameObject bulletPrefab;
    public GameObject grenadePrefab;
    public Transform bulletSpawn;

    [Space(20)]
    public List<Command> orders;

    private void Start ()
    {
        orders = new List<Command>();

        team = PlayerTeam.LocalTeam;
        team.AddUnits(this);

        gameObject.layer = team.teamLayer;
        for (int i = 0; i < commandUI.transform.childCount; i++)
            commandUI.transform.GetChild(i).gameObject.layer = team.teamLayer;

        transform.parent = team.transform;

        Model = transform.GetChild(1).gameObject;
        Model.GetComponent<Renderer>().material.color = team.teamColor;
    }
    #endregion

    #region ISelectable
    public virtual void Selected()
    {
        team.Selection = this;
        ToggleCommands();
    }

    public virtual void Action(Vector3 point)
    {
        Deselected();
    }

    public virtual void Deselected()
    {
        ToggleCommands();
        team.Selection = null;
    }

    public void ToggleCommands()
    {
        commandUI.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);

        for (int i = 0; i < orders.Count; i++)
            orders[i].gameObject.SetActive(orders[i].gameObject.activeSelf);
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
                        yield return Shoot(orders[0].transform.position - transform.position);
                        break;
                    case CommandType.Grenade:
                        yield return Grenade(orders[0].transform.position - transform.position);
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

    public IEnumerator Shoot(Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
        yield return new WaitForSeconds(0.2f);
        CmdShoot(direction);
        yield return new WaitForSeconds(0.2f);
    }

    public IEnumerator Grenade(Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
        yield return new WaitForSeconds(0.2f);
        CmdGrenade(direction);
        yield return new WaitForSeconds(0.2f);
    }

    [Command]
    public void CmdUnitBuisy()
    {
        GameManager.Instance.s_unitsWithOrders[(int)team.team]++;
    }

    [Command]
    public void CmdUnitDone()
    {
        GameManager.Instance.s_unitsWithOrders[(int)team.team]--;
    }

    [Command]
    public void CmdShoot(Vector3 direction)
    {
        NetworkServer.Spawn(Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(direction)));
    }

    [Command]
    public void CmdGrenade(Vector3 direction)
    {
        NetworkServer.Spawn(Instantiate(grenadePrefab, bulletSpawn.position, Quaternion.LookRotation(direction + Vector3.up * 4)));
    }
    #endregion
}
