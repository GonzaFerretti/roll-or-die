using System.Collections;
using UnityEngine;

public class D20Controller : MonoBehaviour
{
    [SerializeField] private float damage;
    
    private PlayerController player;
    private Rigidbody2D d20Rigidbody;
    private float speed = 0;
    private bool charging = false;
    private Animator animator;

    [SerializeField]
    private float normalSpeed, chargeSpeed, detectionDistance, stopChargeDistance;

    void Start()
    {
        d20Rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        speed = normalSpeed;
    }

    void Update()
    {
        animator.SetBool("Charging", charging);

        if (player)
        {
            if (!charging)
            {
                if (Vector2.Distance(transform.position, player.transform.position) > detectionDistance)
                {
                    speed = normalSpeed;
                    Vector3 playerDirection = player.transform.position - transform.position;
                    d20Rigidbody.velocity = playerDirection.normalized * speed;
                }
                else
                {
                    d20Rigidbody.velocity = Vector2.zero;

                    charging = true;
                    StartCoroutine(PassiveMe(1));
                }

            }
            else
            {
                if (Vector2.Distance(transform.position, player.transform.position) > stopChargeDistance)
                {
                    charging = false;
                }
            }
        }
        else
        {
            player = FindObjectOfType<PlayerController>();
            d20Rigidbody.velocity = Vector2.zero;
        }
    }

    IEnumerator PassiveMe(int secs)
    {
        yield return new WaitForSeconds(secs);

        Vector3 playerDirection = player.transform.position - transform.position;
        speed = chargeSpeed;
        d20Rigidbody.velocity = playerDirection.normalized * speed;
        AudioManager.instance.Play("RollD20");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        charging = false;
        HealthContainer container = collision.gameObject.GetComponent<HealthContainer>();
        if (container)
        {
            container.Hit(damage, DamageSource.enemy);
        }
    }
}
