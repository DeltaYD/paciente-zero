using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lever : MonoBehaviour
{
    public bool _active = false;
    [SerializeField] private GameObject obj;
    Animator anim;

    private AudioManager _am;
    public UnityEvent interactAction;
    public UnityEvent doorAction;
    //public UnityEvent doorActivation;

    private void Start()
    {
        _am = FindObjectOfType<AudioManager>();
        anim = this.GetComponent<Animator>();
        transform.GetChild(0).gameObject.SetActive(_active);
        anim.SetBool("_active", _active);
        if (!_active)
        {
            doorAction.Invoke();
        }
        //interactAction.Invoke();
    }

    public void Activate(bool batonHasEnergy)
    {
        if (batonHasEnergy && _active == false)
        {
            _am.Play("LeverSFXCharge");
            _active = !_active;
            transform.GetChild(0).gameObject.SetActive(_active);
            anim.SetBool("_active", _active);
            Debug.Log(_active);
            //obj.GetComponent<Door>().switchDoor();
            //doorActivation.Invoke();

            interactAction.Invoke();
            doorAction.Invoke();
        }
        else if(_active == true && !batonHasEnergy)
        {
            _am.Play("LeverSFXUncharge");
            _active = !_active;
            transform.GetChild(0).gameObject.SetActive(_active);
            anim.SetBool("_active", _active);
            Debug.Log(_active);
            //obj.GetComponent<Door>().switchDoor();

            interactAction.Invoke();
            doorAction.Invoke();
        }
    }
}
