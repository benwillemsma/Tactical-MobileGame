using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject Selection;

    public Sprite[] cursors;
    public Sprite[] lines;

    public bool commandSelected;

    void Start ()
    {
        if (!instance)
            instance = this; 
        else Destroy(gameObject);
	}
	
	void Update ()
    {
        if (!commandSelected)
        {
            Touch input = Input.GetTouch(0);

            if (input.phase == TouchPhase.Began)
                Selection = Checkselection(input.position);
        }
    }

    GameObject Checkselection(Vector3 point)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(point);

        if (Physics.Raycast(ray, out hit))
            return hit.transform.gameObject;
        return null;
    }
}
