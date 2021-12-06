using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadBar : MonoBehaviour
{
    //public Slider slider;
    public Image image;
    public Image background;
    
    public void SetBar(float value)
    {
        //slider.value = value;
        image.fillAmount = value;
    }

    public void Invisible()
    {
        background.CrossFadeAlpha(0, 0, false);
        image.CrossFadeAlpha(0, 0.2f, false);
    }

    public void Visible()
    {
        background.CrossFadeAlpha(1, 0, false);
        image.CrossFadeAlpha(1, 0.2f, false);
    }


    /*
    public void SetMaxHealth(int value)
    {
        
        slider.maxValue = value;
        slider.value = value;
    }*/
}
