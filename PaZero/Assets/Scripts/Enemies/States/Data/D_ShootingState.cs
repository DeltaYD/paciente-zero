using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newShootingStateData", menuName = "Data/State Data/Shooting State")]
public class D_ShootingState : ScriptableObject
{
    public float fireAccuracy = 1f;
    public float fireRate = 1f;
    public float reloadTime = 1.5f;
    public int maxAmmo = 30;
    public float beginHuntActionTimer = 2f;
    public GameObject bulletPrefab;
}
