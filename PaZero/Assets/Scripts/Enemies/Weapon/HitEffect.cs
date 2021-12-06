using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    private float activeTime = 0;

    private void Update()
    {
        activeTime += Time.deltaTime;
        if(activeTime >= 0.05f)
        {
            Destroy(this);
        }
    }
}
