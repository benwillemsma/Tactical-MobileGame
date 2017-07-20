using UnityEngine;

public class Billboard : MonoBehaviour
{
	void Update ()
    {
        transform.LookAt(Vector3.ProjectOnPlane(transform.position - Camera.main.transform.position, Camera.main.transform.forward));
	}
}
