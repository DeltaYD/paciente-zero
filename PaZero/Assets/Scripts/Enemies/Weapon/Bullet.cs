using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private AudioManager _am;
    public float speed = 20f;
    public float damage = 10f;
    public Rigidbody2D rb;
    public LayerMask playerLayer;
    public LayerMask groundLayer;
    public GameObject hitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        _am = FindObjectOfType<AudioManager>();
        rb.velocity = transform.right * speed;
        _am.Play("ShootSFX");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerManager player = collision.GetComponent<PlayerManager>();
        if(collision.gameObject.CompareTag("Player"))
        {
            _am.Play("HitSFX");
            player.TakeDamage(damage);
            Instantiate(hitPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
        if(collision.gameObject.CompareTag("Ground"))
        {
            _am.Play("HitSFX");
            Instantiate(hitPrefab, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }

}
