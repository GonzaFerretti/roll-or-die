using System;
using UnityEngine;

public class PlayerMovement : CharMovement
{
    public delegate void OnRollEvent(bool bStart);

    public OnRollEvent OnRolledEvent;
    
    private PlayerLook playerLook;

    [SerializeField]
    private float rollDistance;
    
    [SerializeField]
    private float rollCooldown;
    
    [SerializeField]
    public float rollDuration;

    private float lastRollTimestamp = -1;
    public Vector2 rollDirection;
    private Vector2 rollOrigin;
    public bool isRolling = false;
    
    [SerializeField]
    private AnimationCurve rollCurve;

    protected override void Start()
    {
        base.Start();
        playerLook = GetComponent<PlayerLook>();
    }

    public void TryRoll()
    {
        float currentTime = Time.time;
        if (!isRolling && currentTime >= lastRollTimestamp + rollCooldown)
        {
            RigidBody.velocity = Vector2.zero;
            lastRollTimestamp = currentTime;

            rollOrigin = transform.position;
            rollDirection = (movementInput == Vector2.zero) ? playerLook.GetLookDirection() : movementInput.normalized;

            isRolling = true;
            if (OnRolledEvent != null)
            {
                OnRolledEvent(true);
            }
        }
    }

    public void InterruptRoll(Vector3 NormalDirection)
    {
        isRolling = false;
        RigidBody.velocity = NormalDirection * speed;
        if (OnRolledEvent != null)
        {
            OnRolledEvent(false);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (isRolling)
        {
            float normalizedTime = Mathf.Clamp((Time.time - lastRollTimestamp) / rollDuration, 0, 1);
            float rollProgress = rollCurve.Evaluate(normalizedTime);
            Vector2 newPos = rollOrigin + rollDirection * (rollDistance * rollProgress);
            transform.position = newPos;
            
            if (normalizedTime == 1f)
            {
                isRolling = false;
                if (OnRolledEvent != null)
                {
                    OnRolledEvent(false);
                }
            }
        }
    }

    protected override bool CanMove()
    {
        return !isRolling;
    }
}
