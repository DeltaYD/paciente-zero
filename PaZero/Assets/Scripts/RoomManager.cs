using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }
    CinemachineVirtualCamera cinemachineVirtualCamera;
    public GameObject _virtualCam;
    private float shakeTimer;
    private float shakeTimerTotal;
    public int isFirst = 0;
    private float startingIntensity;

    private void Awake()
    {
        if(isFirst != 0)
        {
            Instance = this;
        }
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Instance = this;
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            _virtualCam.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !other.isTrigger)
        {
            _virtualCam.SetActive(false);
        }
    }

    public void CameraShake(float intensity, float time)
    {
        startingIntensity = intensity;
        shakeTimerTotal = time;

    cinemachineVirtualCamera = _virtualCam.GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        Debug.Log(cinemachineBasicMultiChannelPerlin.m_AmplitudeGain);
        shakeTimer = time;
    }

    private void Update()
    {
        if(shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                cinemachineVirtualCamera = _virtualCam.GetComponent<CinemachineVirtualCamera>();
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
            }
        }
    }

}
