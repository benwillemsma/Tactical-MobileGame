using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    Rigidbody rb;
    public float lifetime;
    public float initialForce;
    
	void Start ()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * initialForce, ForceMode.Impulse);
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
