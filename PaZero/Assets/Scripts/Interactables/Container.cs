using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : MonoBehaviour
{

    public bool _isOpen = false;
    public Animator _animator;

    public void OpenContainer()
    {
        if (!_isOpen)
        {
            _isOpen = true;
            Debug.Log("Container has been opened.");
            //animator.SetBool("IsOpen", _isOpen);
        }
    }
}
