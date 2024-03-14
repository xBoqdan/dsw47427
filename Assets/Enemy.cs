using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform target;
    public float speed = 3f;
    public float rotateSpeed = 0.025f;
    public Rigidbody2D rb;

    public int maxHealth = 4;
    public int currentHealth;

    public HealthBar healthBar;

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
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.up * speed;
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
            LevelManager.manager.IncreaseScore(1);
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
