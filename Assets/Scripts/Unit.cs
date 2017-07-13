using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IClickable
{
    [SerializeField]
    CommandUI[] commands;

    [SerializeField]
    GameObject bulletPrefab;
    [SerializeField]
    Transform bulletSpawn;

    bool TurnDone = false;
    bool commandsVisible = false;

    void Start ()
    {
		
	}
	void Update ()
    {
		
	}

    //Interface Functinos
    public virtual void HasBeenClicked()
    {
        ToggleCommands();
    }
    public virtual void HasBeenDoubleClicked()
    {
        TurnDone = true;
    }

    void ToggleCommands()
    {
        GameManager.instance.Selection = gameObject;
        for (int i = 0; i < commands.Length; i++)
            commands[i].GetComponent<Animator>().SetBool("visible", commandsVisible);
    }

    //Actions
    public void Move(Vector3 destination)
    {
        //Pathfinding.MoveToLocation(destination);
    }

    public void Shoot(Vector3 direction)
    {
        Instantiate(bulletPrefab,bulletSpawn.position,bulletSpawn.rotation);
    }
}
