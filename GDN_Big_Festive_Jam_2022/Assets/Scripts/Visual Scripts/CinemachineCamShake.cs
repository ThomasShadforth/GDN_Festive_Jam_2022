using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public static class CinemachineCamShake
{

    public static IEnumerator CamShakeCo(float ampGain, CinemachineVirtualCamera cineCam)
    {
        CinemachineBasicMultiChannelPerlin cineNoise = cineCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cineNoise.m_AmplitudeGain = ampGain;
        yield return new WaitForSeconds(.6f);
        cineNoise.m_AmplitudeGain = 0f;
    }
}
