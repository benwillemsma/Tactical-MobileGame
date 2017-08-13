using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera thisCamera;

    private void Start()
    {
        thisCamera = GetComponent<Camera>();
        if (!thisCamera)
            Debug.LogError("CameraScript is not attached to a camera");
    }

    private void Update ()
    {
        if (Input.GetButton("LeftClick") && (!PlayerTeam.LocalTeam || PlayerTeam.LocalTeam.Selection == null))
        {
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    transform.position -= new Vector3(Input.GetTouch(0).deltaPosition.x, 0, Input.GetTouch(0).deltaPosition.y) * Time.deltaTime / 3;
            }
            else
                transform.position -= new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y")) * 20 * Time.deltaTime;
        }
        if (Input.touchCount == 2)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
            {
                if (Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position) > Vector2.Distance(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition, Input.GetTouch(1).position))
                    thisCamera.orthographicSize += (Input.GetTouch(0).deltaPosition.magnitude + Input.GetTouch(1).deltaPosition.magnitude) * Time.deltaTime;
                else
                    thisCamera.orthographicSize -= (Input.GetTouch(0).deltaPosition.magnitude + Input.GetTouch(1).deltaPosition.magnitude) * Time.deltaTime;
            }
        }
        else thisCamera.orthographicSize += Input.GetAxis("Mouse ScrollWheel");

        Mathf.Clamp(thisCamera.orthographicSize, 5, 25);
    }
}
