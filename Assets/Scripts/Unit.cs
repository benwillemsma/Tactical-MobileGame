using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, IClickable
{
    [SerializeField]
    Command[] commands;
    Vector3 commandPosition;

	void Start ()
    {
		
	}
	void Update ()
    {
		
	}

    //Interface Functinos
    public virtual void HasBeenClicked() { }
    public virtual void HasBeenDoubleClicked() { }
}
