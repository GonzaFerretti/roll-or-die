using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
//using UnityEditor.MPE;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class FollowPlayer : MonoBehaviour
{
    private PlayerController player;
    private Camera sceneCamera;
    
    [SerializeField] private float followSpeed = 0.125f;
    [SerializeField] private float maxDistanceForFollow;

    [SerializeField] private Vector2 maxPanDistance = Vector2.one;
    [SerializeField] private Vector2 panDistanceScalar = Vector2.one;
    [SerializeField] private Vector2 deadZoneSize = Vector2.zero;

    private void Start()
    {
        sceneCamera = Camera.main;
    }

    void FixedUpdate()
    {
        if (!player)
        {
            player = FindObjectOfType<PlayerController>();
        }

        if (player)
        {
            Vector3 mousePosition = sceneCamera.ScreenToWorldPoint(Input.mousePosition);
            Vector3 playerPosition = player.transform.position;
            Vector3 mouseDelta = mousePosition - playerPosition;

            Vector3 panDeltaVector = Vector3.zero;
            float centerDistance = (mouseDelta.x * mouseDelta.x) / (deadZoneSize.x * deadZoneSize.x) +
                              (mouseDelta.y * mouseDelta.y) / (deadZoneSize.y * deadZoneSize.y);

            float deadZoneScalar = Mathf.Clamp(centerDistance, 0, 1);
            
            Vector3 v1 = new Vector3(mouseDelta.x / maxPanDistance.x, mouseDelta.y / maxPanDistance.y);

            panDeltaVector = (v1).normalized;
            panDeltaVector.Scale(maxPanDistance);
            panDeltaVector.Scale(panDistanceScalar * deadZoneScalar);
            
            Vector3 targetPosition = playerPosition + panDeltaVector + new Vector3(0, 1, -5);
            float sqrDistance = (targetPosition - transform.position).sqrMagnitude;
            float finalSpeed = followSpeed;
            
            if (sqrDistance <= maxDistanceForFollow * maxDistanceForFollow)
            {
                finalSpeed *= Mathf.Sqrt(sqrDistance) / maxDistanceForFollow;
            }
            transform.position = Vector3.Slerp(transform.position, targetPosition, finalSpeed);
        }
    }
}
