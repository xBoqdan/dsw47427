using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    [SerializeField] private GameObject bulletPrefabWeapon1;
    [SerializeField] private GameObject bulletPrefabWeapon2;
    [SerializeField] private GameObject bulletPrefabWeapon3;
    [SerializeField] private GameObject towerPrefab;
    [SerializeField] private Transform firingPoint;
    [SerializeField] private int usedWeapon = 1;

    private Rigidbody2D rb;
    private float moveX;
    private float moveY;

    Vector2 moveDirection;
    Vector2 mousePosition;

    public TextMeshProUGUI weapon1Text;
    public TextMeshProUGUI weapon2Text;
    public TextMeshProUGUI weapon3Text;
    public TextMeshProUGUI superWeaponText;

    [Range(0.1f, 2f)]
    [SerializeField] private float fireRate = 0.5f;

    private float fireTimer;

    public int maxEnergy = 100;
    public int currentEnergy;

    public int maxHealth = 100;
    public int currentHealth;

    public HealthBar healthBar;
    public EnergyBar energyBar;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        currentEnergy = 0;
        energyBar.SetMaxEnergy(maxEnergy);
        energyBar.SetEnergy(currentEnergy);

        weapon1Text.color = Color.green;
    }
    // Update is called once per frame
    void Update()
    {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        moveDirection = new Vector2(moveX, moveY).normalized;

        Vector2 aimDirection = mousePosition - rb.position;
        float aimAngle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = aimAngle;

        if (Input.GetMouseButton(0) && fireTimer <= 0f)
        {
            fireTimer = fireRate;
            Shoot();

        }
        else
        {
            fireTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeWeapon(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeWeapon(3);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentEnergy >= 25)
            {
                currentEnergy -= 25;
                energyBar.SetEnergy(currentEnergy);

                Instantiate(towerPrefab, transform.position, Quaternion.identity);

                if (currentEnergy < 25)
                {
                    superWeaponText.color = Color.white;
                }
            }
        }
    }

    private void ChangeWeapon(int selectedWeapon)
    {
        if(selectedWeapon == 1)
        {
            usedWeapon = 1;
            fireRate = 0.5f;

            weapon1Text.color = Color.green;
            weapon2Text.color = Color.white;
            weapon3Text.color = Color.white;
        }
        else if(selectedWeapon == 2) 
        {
            usedWeapon = 2;
            fireRate = 0.1f;

            weapon1Text.color = Color.white;
            weapon2Text.color = Color.green;
            weapon3Text.color = Color.white;
        }
        else if(selectedWeapon == 3)
        {
            usedWeapon = 3;
            fireRate = 1f;

            weapon1Text.color = Color.white;
            weapon2Text.color = Color.white;
            weapon3Text.color = Color.green;
        }
    }
    private void Shoot()
    {
        if(usedWeapon == 1) 
        {
            Instantiate(bulletPrefabWeapon1, firingPoint.position, firingPoint.rotation);
        }
        else if(usedWeapon == 2)
        {
            Instantiate(bulletPrefabWeapon2, firingPoint.position, firingPoint.rotation);
        }
        else if(usedWeapon == 3)
        {
            Instantiate(bulletPrefabWeapon3, firingPoint.position, firingPoint.rotation);
        }  
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(currentHealth);

        CheckIfDead();
    }

    private void CheckIfDead()
    {
        if(currentHealth <= 0)
        {
            LevelManager.manager.GameOver();
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveDirection.x * speed, moveDirection.y * speed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("EnemyBullet"))
        {
            Destroy(collision.gameObject);
            TakeDamage(20);
        }
        else if (collision.gameObject.CompareTag("Enemy")) {
            TakeDamage(5);
        }
        else if (collision.gameObject.CompareTag("EnergyBoost")){
            Destroy(collision.gameObject);
            currentEnergy += 25;
            energyBar.SetEnergy(currentEnergy);
            superWeaponText.color = Color.yellow;
        }
    }
}
