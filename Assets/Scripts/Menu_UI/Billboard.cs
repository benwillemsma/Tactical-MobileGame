using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Camera targetCamera;

    private void Start()
    {
        if(!targetCamera)
            targetCamera = Camera.main;
    }

    private void Update ()
    {
        transform.eulerAngles = Vector3.ProjectOnPlane(new Vector3(transform.eulerAngles.x, -180, 0), targetCamera.transform.forward);
        transform.localScale = Vector3.one * Mathf.Clamp(Mathf.Sqrt(targetCamera.orthographicSize - 4), 1, 5) / 50;
	}
}
