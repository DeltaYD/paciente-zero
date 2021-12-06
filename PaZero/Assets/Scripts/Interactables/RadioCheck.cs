using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RadioCheck : MonoBehaviour
{

    public UnityEvent interactAction;
    public bool isChecked = false;

    public void RadioChecked()
    {
        if (!isChecked)
        {
            isChecked = true;
            interactAction.Invoke();
        }
    }
}
