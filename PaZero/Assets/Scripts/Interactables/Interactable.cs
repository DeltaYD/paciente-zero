using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject _visualCue;
#pragma warning restore 0649
    public bool _isInRange = false;
    public KeyCode interactKey;
    public UnityEvent interactAction;

    private void Awake()
    {
        if (_visualCue != null)
        {
            _visualCue.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isInRange && !DialogueManager.GetInstance().dialogueIsPlaying)
        {
            if (Input.GetKeyDown(interactKey))
            {
                interactAction.Invoke();
            }
        }
        else
        {
            if (_visualCue != null)
            {
                _visualCue.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isInRange = true;
            if(_visualCue != null)
            {
                _visualCue.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _isInRange = false;
        }
    }
}
