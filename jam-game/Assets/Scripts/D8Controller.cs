using System.Collections;
using UnityEngine;

public class D8Controller : MonoBehaviour
{
    private PlayerController player;
    private Rigidbody2D d8Rigidbody;
    private bool attacking = false;
    private Animator animator;
    private bool attackOnCooldown = false;

    [SerializeField]
    private float detectionDistance, coolDownPeriodInSeconds, speed;
    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform firePoint;
    [SerializeField]
    private float bulletRange;

    void Start()
    {
        d8Rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player)
        {
            bool playerIsToTheRight = (transform.position.x < player.transform.position.x);

            Vector3 newRotation = transform.localScale;
            transform.localScale = new Vector3(playerIsToTheRight ? 2 : -2, newRotation.y, newRotation.z);

            if (Vector2.Distance(transform.position, player.transform.position) > detectionDistance && !attacking)
            {
                Vector3 playerDirection = player.transform.position - transform.position;
                d8Rigidbody.velocity = playerDirection.normalized * speed;
            }
            else
            {
                d8Rigidbody.velocity = Vector2.zero;
                if (!attackOnCooldown)
                {
                    attacking = true;
                }
            }
        }
        else
        {
            player = FindObjectOfType<PlayerController>();
            d8Rigidbody.velocity = Vector2.zero;
        }

        animator.SetBool("Attacking", attacking);
    }

    IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(coolDownPeriodInSeconds);
        attackOnCooldown = false;
    }

    public void OnAnimationFinished()
    {
        AudioManager.instance.Play("AttackD8");
        if (player)
        {
            Vector3 playerDirection = player.transform.position - firePoint.position;
            GameObject projectile = Instantiate(bullet, firePoint.position, Quaternion.identity);
            projectile.transform.up = Quaternion.Euler(0,0,90) * playerDirection.normalized;
            projectile.GetComponent<Rigidbody2D>().AddForce( playerDirection.normalized * 10, ForceMode2D.Impulse);
            Bullet createdBullet = projectile.GetComponent<Bullet>();
            createdBullet.source = DamageSource.enemy;
            createdBullet.bulletParams.damage = 2;
            createdBullet.bulletParams.sqrRange = bulletRange * bulletRange;
            createdBullet.origin = firePoint.position;
        }
        
        attacking = false;
        attackOnCooldown = true;

        StartCoroutine(AttackCooldown());
    }
}
