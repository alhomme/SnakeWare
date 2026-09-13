using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private AudioSource m_AudioSourceMusic;
    [SerializeField] private AudioSource m_AudioSourceSFX;

    [SerializeField] private SSO_Audio m_MusicSSO;
    [SerializeField] private SSO_Audio m_SfxSSO;

    [SerializeField] private RSO_Speed m_SpeedRSO;
    [SerializeField] private RSE_PlayMusic m_PlayMusicRSE;
    [SerializeField] private RSE_PlaySFX m_PlaySFXRSE;

    private void OnEnable()
    {
        m_SpeedRSO.OnChanged += OnSpeedChanged;
        m_PlayMusicRSE.Event += OnPlayMusic;
        m_PlaySFXRSE.Event += OnPlaySFX;

        m_MusicSSO.InitCache();
        m_SfxSSO.InitCache();

    }

    private void OnDisable()
    {
        m_SpeedRSO.OnChanged -= OnSpeedChanged;
        m_PlayMusicRSE.Event -= OnPlayMusic;
        m_PlaySFXRSE.Event -= OnPlaySFX;
    }

    private void OnPlayMusic(string music)
    {
        AudioClip clip = m_MusicSSO.GetAudioClip(music);

        if (clip != null && m_AudioSourceMusic.clip != clip)
        {
            m_AudioSourceMusic.Stop();
            m_AudioSourceMusic.clip = clip;
            m_AudioSourceMusic.Play();
        }
    }

    private void OnPlaySFX(string sfx)
    {
        if (sfx == "Stop")
        {
            m_AudioSourceSFX.Stop();
        }
        else
        {
            AudioClip clip = m_SfxSSO.GetAudioClip(sfx);
            if (clip != null)
            {
                m_AudioSourceSFX.clip = clip;
                m_AudioSourceSFX.Play();
            }
        }
    }

    private void OnSpeedChanged(float speed)
    {
        // Adjust pitch
        float newPitch = 1 + ((speed - 1) / 10);
        Utils.Log("MusicManager.OnSpeedChanged: New pitch = " + newPitch);
        m_AudioMixer.SetFloat("MusicPitch", newPitch);
    }
}
