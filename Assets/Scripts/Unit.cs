using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, ISelectable,IDamageable
{
    public Team playerTeam;
    public float speed;
    public float health;
    public float maxMoveDistance;

    public bool hasFlag = false;

    private PlayerTeam team;
    private GameObject Model;

    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    GameObject grenadePrefab;
    [SerializeField]
    Transform bulletSpawn;

    [Space(20)]
    public List<Command> orders;

    private void Start ()
    {
        team = GameManager.teams[(int)playerTeam];
        team.units.Add(this);
        orders = new List<Command>();

        Model = transform.GetChild(1).gameObject;
        Model.GetComponent<Renderer>().material.color = team.teamColor;
    }

    // ISelectable
    public virtual void Selected()
    {
        GameManager.instance.Selection = this;
        ToggleCommands();
    }

    public virtual void Action(Vector3 point)
    {
        Deselected();
    }

    public virtual void Deselected()
    {
        ToggleCommands();
        GameManager.instance.Selection = null;
    }

    public void ToggleCommands()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);

        for (int i = 0; i < orders.Count; i++)
            orders[i].gameObject.SetActive(orders[i].gameObject.activeSelf);
    }

    //IDamageable
    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
            Die();
    }
    public virtual void Die()
    {
        StopAllCoroutines();
        GameManager.unitsMoving.Remove(this);
        team.units.Remove(this);
        Destroy(gameObject);
    }

    //Actions
    public virtual IEnumerator InvokeCommands()
    {
        GameManager.unitsMoving.Add(this);

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
            orders[0].Remove();
        }
        GameManager.unitsMoving.Remove(this);
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
        Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(direction));
        yield return new WaitForSeconds(0.2f);
    }
    public IEnumerator Grenade(Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
        yield return new WaitForSeconds(0.2f);
        Instantiate(grenadePrefab, bulletSpawn.position, Quaternion.LookRotation(direction + Vector3.up * 4));
        yield return new WaitForSeconds(0.2f);
    }
}
