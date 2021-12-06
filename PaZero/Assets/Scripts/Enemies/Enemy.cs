using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float _maxHealth = 20f;
    public float _currentHealth;
    private Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = _maxHealth;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _rb.velocity = Vector2.zero;
    }

    public void TakeDamage(float damage)
    {
        _currentHealth -= damage;

        if(_currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //die anim
        Debug.Log("Enemy died!");
        //disable enemy
        GetComponent<Collider2D>().enabled = false;
        _rb.isKinematic = true;
        this.enabled = false;
    }
}
