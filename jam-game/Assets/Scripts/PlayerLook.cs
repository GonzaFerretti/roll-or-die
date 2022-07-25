using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    private Camera sceneCamera;
    void Start()
    {
        sceneCamera = Camera.main;
    }

    // Update is called once per frame
    public float GetLookAngle()
    {
        Vector3 mousePosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 relativePos = mousePosition - transform.position;
        return Mathf.Atan2(relativePos.y, relativePos.x);
    }

    public Vector2 GetLookDirection()
    {
        float lookAngle = GetLookAngle();
        return new Vector2(Mathf.Cos(lookAngle), Mathf.Sin(lookAngle));
    }
}
