using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Rocket : Projectile
{
    private Vector3 startPos;
    private Vector3 newPos;
    private float elapsedTime;
    
    public Vector3[] guidePoints;
    [ClientRpc]
    public void RpcSetGuides(Vector3[] points)
    {
        if(points != null)
            guidePoints = points;
    }

    private void Start()
    {
        startPos = transform.position;
        newPos = startPos;

        for (int i = 0; i < guidePoints.Length; i++)
            guidePoints[i].y = 1.8f;
    }

    void Update ()
    {
        transform.position = newPos;
        if (guidePoints.Length > 0)
        {
            newPos = Vector3.LerpUnclamped(startPos, guidePoints[0], elapsedTime);
            for (int i = 1; i < guidePoints.Length; i++)
            {
                Vector3 temp = Vector3.LerpUnclamped(guidePoints[i - 1], guidePoints[i], elapsedTime);
                newPos = Vector3.LerpUnclamped(newPos, temp, elapsedTime);
            }
            transform.LookAt(newPos);
            elapsedTime += Time.deltaTime;
        }
    }
}
