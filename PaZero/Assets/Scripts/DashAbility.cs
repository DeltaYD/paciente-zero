using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashAbility : MonoBehaviour
{
    private Rigidbody2D _rb;
    public float _dashSpeed;
    private float _dashTime;
    public float _startDashTime;
    private int _direction;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _dashTime = _startDashTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(_direction == 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                _direction = 1;
            } else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                _direction = 2;
            }
        }
        else
        {
            if (_dashTime <= 0)
            {
                _direction = 0;
                _dashTime = _startDashTime;
                //_rb.velocity = Vector2.zero;
            }
            else
            {
                _dashTime -= Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    _rb.velocity = _rb.velocity * _dashSpeed;
                }
            }
        }
    }
}
