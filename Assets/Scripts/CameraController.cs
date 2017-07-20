using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update ()
    {
        if (Input.GetButton("Click") && GameManager.instance.Selection == null)
        {
            if (Input.touchCount > 0)
            {
                Touch input = Input.GetTouch(0);
                if (input.phase == TouchPhase.Began)
                    rb.velocity = Vector3.zero;
                else if (input.phase == TouchPhase.Moved)
                    rb.velocity -= new Vector3(Input.GetTouch(0).deltaPosition.x, 0, Input.GetTouch(0).deltaPosition.y) * Time.deltaTime;
            }
            else // not working
                transform.position += (GameManager.ScreenRay(Input.mousePosition).point - GameManager.ScreenRay(new Vector3(Screen.width, 0, Screen.height)).point / 2) * Time.deltaTime;
        }
    }
}
