using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnDestroy : MonoBehaviour
{
    public GameObject Prefab;
    public float ObjectLifeTime = -1;

    void OnDestroy()
    {
        GameObject temp =  Instantiate(Prefab, transform.position, transform.rotation);
        if (ObjectLifeTime > 0)
            Destroy(temp, ObjectLifeTime);
    }
}
