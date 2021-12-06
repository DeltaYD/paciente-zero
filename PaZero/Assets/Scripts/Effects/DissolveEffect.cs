using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveEffect : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private Material mat;
#pragma warning restore 0649

    private float dissolveAmount;
    public bool isDissolving = true;

    private void Update()
    {
        if (isDissolving)
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount + Time.deltaTime);
            mat.SetFloat("_DissolveAmount", dissolveAmount);
        }
        else
        {
            dissolveAmount = Mathf.Clamp01(dissolveAmount - Time.deltaTime);
            mat.SetFloat("_DissolveAmount", dissolveAmount);
        }
    }
}
