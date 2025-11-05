using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public int maxHealth;
    public int currentHealth;
    public Transform target;

    Rigidbody rigid;
    BoxCollider boxCollider;
    Material mat;
    NavMeshAgent nav;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        mat = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        nav.SetDestination(target.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            currentHealth -= weapon.damage;
            StartCoroutine(OnDamage());
            // if (currentHealth <= 0)
            // {
            //     // Handle enemy death
            //     Destroy(gameObject);
            // }
            Debug.Log("Melee: " + currentHealth);
            // if (weapon != null)
            // {
            //     // Deal damage to the enemy
            //     currentHealth -= weapon.damage;
            //     if (currentHealth <= 0)
            //     {
            //         // Handle enemy death
            //         Destroy(gameObject);
            //     }
            // }
        }
        else if (other.tag == "Bullet")
        {
            // Deal damage to the player
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;
            StartCoroutine(OnDamage());
            // if (currentHealth <= 0)
            // {
            //     // Handle enemy death
            //     Destroy(gameObject);
            // }
            Debug.Log("Range: " + currentHealth);
            // if (bullet != null)
            // {
            //     // Assuming the player has a method to take damage
            //     Player player = other.GetComponent<Player>();
            //     if (player != null)
            //     {
            //         player.TakeDamage(bullet.damage);
            //     }
            // }
        }
    }
    IEnumerator OnDamage()
    {
        mat.color = Color.red; // Damage effect
        yield return new WaitForSeconds(0.1f);
        if (currentHealth > 0)
        mat.color = Color.white; // Reset color
        else
        {
            // Handle enemy death
            mat.color = Color.gray;
            Destroy(gameObject, 2);
        }
    }
    // void Start()
    // {
    //     currentHealth = maxHealth;
    // }
}
