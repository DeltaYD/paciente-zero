using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float radius;
    [Range(0,360)] public float angle;
    public GameObject playerRef;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public bool canSeePlayer = false;
    public bool canTrackPlayer = false;
    public Vector2 directionToTarget;
    public Vector2 lastSeenPos;
    float distanceToTarget;
    public Transform target;

    private void Start()
    {
        //playerRef = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(FOVRoutine());
    }

    private IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.01f);

        while (true)
        {
            yield return wait;

            FieldOfViewCheck();

        }

    }

    private void FieldOfViewCheck()
    {
        Collider2D[] rangeChecks = Physics2D.OverlapCircleAll(transform.position, radius, targetMask);

        if(rangeChecks.Length != 0)
        {
            target = rangeChecks[0].transform;
            directionToTarget = (target.position - transform.position + new Vector3(0,0.5f)).normalized;
            distanceToTarget = Vector2.Distance(transform.position, target.position);

            if (Vector2.Angle(transform.right, directionToTarget) < angle / 2)
            {
                if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    canSeePlayer = true;
                }
                else
                {
                    canSeePlayer = false;
                }
            }
            else
            {
                canSeePlayer = false;
            }

            if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstructionMask))
            {
                canTrackPlayer = true;
                lastSeenPos = target.position;
            }
            else
            {
                canTrackPlayer = false;
            }
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
            canTrackPlayer = false;
        }

    }

}
