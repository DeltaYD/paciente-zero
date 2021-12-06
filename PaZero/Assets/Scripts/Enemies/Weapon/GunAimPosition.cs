using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAimPosition : MonoBehaviour
{
    private FieldOfView fov;

    void Start()
    {
        fov = transform.GetComponentInParent<FieldOfView>();
    }
    
    void Update()
    {
        if (fov.canTrackPlayer)
        {
            transform.right = fov.directionToTarget;
        }
    }
}
