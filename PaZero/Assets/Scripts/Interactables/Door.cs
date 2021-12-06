using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool _isOpen = false;
    AudioManager _am;
    Collider2D col;
    Animator anim;

    public char doorType;

    private void Start()
    {
        _am = FindObjectOfType<AudioManager>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    public void OpenDoor(GameObject obj)
    {
        if (!_isOpen)
        {
            PlayerManager player = obj.GetComponent<PlayerManager>();
            /*
            if (player._hasLobbyKey)
            {
                _isOpen = true;
                col.enabled = !col.enabled;
                Debug.Log("Door is unlocked!");
            }*/

            if (player.TryToOpen(doorType))
            {
                anim.Play("key_door_open");
                _am.Play("CardUnlockSFX");
                _isOpen = true;
                col.enabled = !col.enabled;
                Debug.Log("Door was unlocked using the '" + doorType + "' key!");
            }
        }
    }

    public void switchDoor()
    {
        if (_isOpen)
        {
            anim.Play("door_closed");
        }
        else
        {
            anim.Play("door_open");
        }
        _isOpen = !_isOpen;
        col.enabled = !col.enabled;
    }
}
