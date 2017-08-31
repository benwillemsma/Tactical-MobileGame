using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnCollision : MonoBehaviour
{
    public GameObject Prefab;
    public float ObjectLifeTime = -1;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject temp = Instantiate(Prefab, transform.position, transform.rotation);
        if (ObjectLifeTime > 0)
            Destroy(temp, ObjectLifeTime);
    }
}
