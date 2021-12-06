using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardSprites : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private GameObject blue;
    [SerializeField] private GameObject red;
    [SerializeField] private GameObject green;
#pragma warning restore 0649

    private void Update()
    {
        if (PlayerManager._hasBlueKey)
        {
            ActivateBlue();
        }
        else
        {
            blue.SetActive(false);
        }
        if (PlayerManager._hasRedKey)
        {
            ActivateRed();
        }
        else
        {
            red.SetActive(false);
        }
        if (PlayerManager._hasGreenKey)
        {
            ActivateGreen();
        }
        else
        {
            green.SetActive(false);
        }
    }

    public void ActivateBlue()
    {
        blue.SetActive(true);
    }

    public void ActivateRed()
    {
        red.SetActive(true);
    }

    public void ActivateGreen()
    {
        green.SetActive(true);
    }
}
