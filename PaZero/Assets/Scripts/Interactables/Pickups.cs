using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickups : MonoBehaviour
{
    public bool _isCollected = false;
    public Animator _animator;

    public void PickUp()
    {
        if (!_isCollected)
        {
            _isCollected = true;
            Debug.Log("Pickup collected.");
            Destroy(this.gameObject);
            //animator.SetBool("IsOpen", _isOpen);
        }
    }
}
