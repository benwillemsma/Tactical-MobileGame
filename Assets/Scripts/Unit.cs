using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, ISelectable
{
    public float speed;

    public float maxMoveDistance;
    private float moveDistanceRemaining;

    bool turnFinished = false;

    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    Transform bulletSpawn;

    [Space(20)]
    public List<Command> orders;

    private void Start ()
    {
        GameManager.units.Add(this);
        orders = new List<Command>();

        moveDistanceRemaining = maxMoveDistance;
    }

    //Interface Functinos
    public virtual void Selected()
    {
        GameManager.instance.Selection = this;
        ToggleCommands();
    }
    
    public virtual void Action(Vector3 point)
    {
        Deselected();
    }

    public void DoubleClicked()
    {

    }

    public virtual void Deselected()
    {
        ToggleCommands();
        GameManager.instance.Selection = null;
    }

    void ToggleCommands()
    {
        transform.GetChild(0).gameObject.SetActive(!transform.GetChild(0).gameObject.activeInHierarchy);
    }

    //Actions
    public virtual IEnumerator InvokeCommands()
    {
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
                default:
                    Debug.Log("Unknown Command:" + orders[0].type.ToString());
                    break;
            }
            if (!turnFinished)
                orders[0].Remove();
        }
        turnFinished = false;
        moveDistanceRemaining = maxMoveDistance;
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
}
