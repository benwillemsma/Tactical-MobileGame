using UnityEngine;

public class Billboard : MonoBehaviour
{
	void Update ()
    {
        transform.eulerAngles = Vector3.ProjectOnPlane(new Vector3(transform.eulerAngles.x, -180, 0), Camera.main.transform.forward);
	}
}
