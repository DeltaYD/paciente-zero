using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePosition2D : MonoBehaviour
{
    //[SerializeField] private Camera mainCamera;
    Vector2 direction;
    private float time;

    private bool _facingRight = true;

    public float offset;

    private Animator _anim;

    private void Start()
    {
        _anim = this.transform.GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!PauseMenu.isPaused && !PlayerManager._isDead)
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            if ((difference.x < 0 && _facingRight) || (difference.x > 0 && !_facingRight))
            {
                transform.rotation = Quaternion.Euler(180f, 0f, -rotZ + offset);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
            }
        }
    }

    public void ChargeUp()
    {
        _anim.SetBool("hasAttacked", false);
        _anim.SetBool("isCharging", true);

    }

    public void Release()
    {
        _anim.SetBool("hasAttacked", true);
        _anim.SetBool("isCharging", false);
    }
}
