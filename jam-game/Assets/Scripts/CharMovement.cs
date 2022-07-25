using UnityEngine;

public class CharMovement : MonoBehaviour
{
    [HideInInspector]
    public Vector2 movementInput;
    protected Rigidbody2D RigidBody;

    [SerializeField] 
    protected float speed = 15;

    protected virtual void Start()
    {
        RigidBody = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate()
    {
        if (CanMove())
        {
            RigidBody.velocity = movementInput.normalized * speed;
        }
    }

    protected virtual bool CanMove()
    {
        return true;
    }
}
