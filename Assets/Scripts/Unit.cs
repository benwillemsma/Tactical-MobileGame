using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Team
{
    Neutral,
    Team_1,
    Team_2,
    Team_3,
    Team_4
}

public class Unit : MonoBehaviour, ISelectable
{
    public Team team;
    public float speed;

    public float maxMoveDistance;
    private float moveDistanceRemaining;

    public bool hasFlag = false;
    bool turnFinished = false;

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
        GameManager.units.Add(this);
        orders = new List<Command>();

        moveDistanceRemaining = maxMoveDistance;

        Model = transform.GetChild(1).gameObject;
        Model.GetComponent<Renderer>().material.color = GameManager.teamColors[(int)team];
    }

    //Interface Functinos
    public virtual void Selected()
    {
        Debug.Log(this + ": was selected");
        GameManager.instance.Selection.Enqueue(this);
        ToggleCommands();
    }

    public virtual void Action(Vector3 point)
    {
        Debug.Log("Unit Action");
    }

    public virtual void Deselected()
    {
        Debug.Log(this + ": was deselected");
        ToggleCommands();
        GameManager.instance.Selection.Dequeue();
    }

    public void ToggleCommands()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);

        for (int i = 0; i < orders.Count; i++)
            orders[i].gameObject.SetActive(orders[i].gameObject.activeSelf);
    }

    //Actions
    public virtual IEnumerator InvokeCommands()
    {
        GameManager.unitsMoving.Add(this);

        while (moveDistanceRemaining >= 0 && orders.Count > 0)
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
            if (!turnFinished)
                orders[0].Remove();
        }
        turnFinished = false;
        moveDistanceRemaining = maxMoveDistance;

        GameManager.unitsMoving.Remove(this);
    }

    public IEnumerator Move(Vector3 destination)
    {
        transform.LookAt(destination);
        Vector3 startPos = transform.position;

        while ((transform.position - destination).magnitude > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * speed);

            moveDistanceRemaining = maxMoveDistance - (transform.position - startPos).magnitude;
            if (moveDistanceRemaining <= 0)
            {
                turnFinished = true;
                yield break;
            }

            yield return null;
        }
    }
    public IEnumerator Shoot(Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
        yield return new WaitForSeconds(0.2f);
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        bullet.GetComponent<Rigidbody>().AddForce(direction * 200);
        Destroy(bullet, 4);
        yield return new WaitForSeconds(0.2f);
    }
    public IEnumerator Grenade(Vector3 direction)
    {
        transform.LookAt(transform.position + direction);
        yield return new WaitForSeconds(0.2f);
        GameObject grenade = Instantiate(grenadePrefab, bulletSpawn.position, bulletSpawn.rotation);
        grenade.GetComponent<Rigidbody>().AddForce(direction * 50 + Vector3.up * 200);
        Destroy(grenade, 4);
        yield return new WaitForSeconds(0.2f);
    }
}
