using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    private AudioManager _am;

    public float health;
    public float maxHealth = 100f;
    private float lerpSpeed = 3f;

    public static bool _isDead = false;

    public bool _batonCharged = true;

    public bool _hasEnergyBaton = false;

    public bool _hasLobbyKey = false;

    public bool _hasEndingN = false;

    public int _radioProgress = 0;

    public static bool _hasRedKey = false;
    public static bool _hasGreenKey = false;
    public static bool _hasBlueKey = false;
    public static bool _hasAlphaKey = false;

    private float deathTimer = 5;
    private float deathDespawn = 0.3f;
    private float deathCounter = 0;

#pragma warning disable 0649
    [SerializeField] private GameObject obj;
    [SerializeField] private Image healthBar;
    [SerializeField] private GameObject DeathChunkParticle;
    [SerializeField] private GameObject DeathBloodParticle;
    [SerializeField] private MainMenu mm;
    private Rigidbody2D _rb;
#pragma warning restore 0649

    private PlayerMovement _plm;

    private void Start()
    {
        Revive();
        _am = FindObjectOfType<AudioManager>();
        _plm = GetComponent<PlayerMovement>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        lerpSpeed = 3f * Time.deltaTime;
        HealthBarFiller();
        ColorChanger();
        if (_isDead)
        {
            deathCounter += Time.deltaTime;
            if(deathCounter > deathDespawn)
            {
                _rb.constraints = RigidbodyConstraints2D.FreezePositionY;
            }
            if(deathCounter > deathTimer)
            {
                // return to main menu
                mm.StartGame();
            }
        }
    }

    public void RadioCheck()
    {
        _radioProgress++;
    }

    public void enableEnd()
    {
        _hasEndingN = true;
    }

    public void Revive()
    {
        health = maxHealth;
        _isDead = false;
        //gameObject.SetActive(true);
    }

    private void HealthBarFiller()
    {
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, health / maxHealth, lerpSpeed);
    }

    private void ColorChanger()
    {
        Color healthColor = Color.Lerp(Color.red, Color.green, (health / maxHealth));

        healthBar.color = healthColor;
    }

    public void TakeDamage(float damage)
    {
        if (!_isDead)
        {
            RoomManager.Instance.CameraShake(1f, .2f);
            health -= damage;
            HealthBarFiller();

            if (health <= 0)
            {
                RoomManager.Instance.CameraShake(8f, .3f);
                Die();
            }
        }
    }

    void Die()
    {
        _am.Play("DeathSFX");
        _isDead = true;
        Instantiate(DeathChunkParticle, transform.position, DeathChunkParticle.transform.rotation);
        Instantiate(DeathBloodParticle, transform.position, DeathBloodParticle.transform.rotation);
        //gameObject.SetActive(false);
        _plm._cc.enabled = false;
        _plm._head.enabled = false;
        _rb.velocity = new Vector2(0, 99999999);
        //Destroy(gameObject);
    }

    public void switchBatonEnergyActive()
    {
        _batonCharged = !_batonCharged;
    }

    public bool TryToOpen(char doorType)
    {
        if(doorType == 'r' || doorType == 'R')
        {
            if (_hasRedKey || _hasAlphaKey)
            {
                return true;
            }
        } else if(doorType == 'g' || doorType == 'G')
        {
            if (_hasGreenKey || _hasAlphaKey)
            {
                return true;
            }
        } else if(doorType == 'b' || doorType == 'B')
        {
            if (_hasBlueKey || _hasAlphaKey)
            {
                return true;
            }
        } else if(doorType == 'a' || doorType == 'A')
        {
            if (_hasAlphaKey)
            {
                return true;
            }
        }

        return false;
    }

    public void GetLobbyKey()
    {
        _hasLobbyKey = true;
    }

    public void getRedKey()
    {
        _hasRedKey = true;
    }

    public void getGreenKey()
    {
        _hasGreenKey = true;
    }

    public void getBlueKey()
    {
        _hasBlueKey = true;
    }

    public void getAlphaKey()
    {
        _hasAlphaKey = true;
    }

    public void getEnergyBaton()
    {
        _hasEnergyBaton = true;
        obj.SetActive(true);
    }

    public void removeEnergyBaton()
    {
        _hasEnergyBaton = false;
        obj.SetActive(false);
    }
}
