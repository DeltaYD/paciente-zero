using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementSounds : MonoBehaviour
{
    public AudioManager _am;

    public void Footstep()
    {
        _am.Play("FootstepSFX");
    }

    public void ClimbStairs()
    {
        _am.Play("StairSFX");
    }
}
