﻿using System.Collections;
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
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    transform.position -= new Vector3(Input.GetTouch(0).deltaPosition.x, 0, Input.GetTouch(0).deltaPosition.y) * Time.deltaTime;
            }
            else
                transform.position -= new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y")) * 20 * Time.deltaTime;
        }
    }
}
