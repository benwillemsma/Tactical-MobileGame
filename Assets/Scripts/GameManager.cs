using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Sprite[] cursors;
    public Sprite[] lines;


    void Start ()
    {
        if (!instance)
            instance = this;
        else Destroy(gameObject);
        
	}
	
	void Update ()
    {
        Command temp = null;
        Touch input = Input.GetTouch(0);
        if (input.phase == TouchPhase.Began)
             temp = new Command(0, 0, Vector3.zero, input.position);
        if (input.phase == TouchPhase.Moved)
            temp.moveCursor(input.position);
    }
}
