using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeEnemy : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;
    public float rotateSpeed = 0.25f;
    public Rigidbody2D rb;
    public GameObject bulletPrefab;

    public float distanceToShoot = 5f;
    public float distanceToStop = 3f;

    public float fireRate;
    private float timeToFire;

    public int maxHealth = 3;
    public int currentHealth;

    public HealthBar healthBar;

    public Transform firingPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
    }

    private void Update()
    {
        if (!target)
        {
            GetTarget();
        }
        else
        {
            RotateTowardsTarget();
        }

        if(target != null && Vector2.Distance(target.position, transform.position) <= distanceToShoot)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if(timeToFire <= 0f)
        {
            Instantiate(bulletPrefab, firingPoint.position, firingPoint.rotation);
            timeToFire = fireRate;
        }
        else
        {
            timeToFire -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if(target != null)
        {
            if (Vector2.Distance(target.position, transform.position) >= distanceToStop)
            {
                rb.velocity = transform.up * speed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        } 
    }

    private void RotateTowardsTarget()
    {
        Vector2 targetDirection = target.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.localRotation = Quaternion.Slerp(transform.localRotation, q, rotateSpeed);
    }

    private void GetTarget()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void TakeDamage(int damage, GameObject enemy)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);

        CheckIfDead(enemy);
    }

    private void CheckIfDead(GameObject enemy)
    {
        if (currentHealth <= 0)
        {
            LevelManager.manager.IncreaseScore(3);
            Destroy(enemy);

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BulletWeapon1"))
        {
            Destroy(collision.gameObject);
            TakeDamage(2, gameObject);
        }
        else if (collision.gameObject.CompareTag("BulletWeapon2"))
        {
            Destroy(collision.gameObject);
            TakeDamage(1, gameObject);
        }
        else if (collision.gameObject.CompareTag("BulletWeapon3"))
        {
            Destroy(collision.gameObject);
            TakeDamage(4, gameObject);
        }
    }
}
