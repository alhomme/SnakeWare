using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_AudioClipDefault;
    [SerializeField] private AudioClip m_AudioClipMG;

    [SerializeField] private Dictionary<string, AudioClip> m_AudioClipDict;

    [SerializeField] private RSO_Speed m_SpeedRSO;
    [SerializeField] private RSE_SceneLoaded m_SceneLoadedRSE;

    private void OnEnable()
    {
        m_SpeedRSO.OnChanged += OnSpeedChanged;
        m_SceneLoadedRSE.Event += OnSceneLoaded;

        m_AudioClipDict = new Dictionary<string, AudioClip>();
        m_AudioClipDict.Add("MainMenu", m_AudioClipDefault);
        m_AudioClipDict.Add("Arena", m_AudioClipDefault);
        m_AudioClipDict.Add("MiniGame_Bar", m_AudioClipMG);
    }

    private void OnDisable()
    {
        m_SpeedRSO.OnChanged -= OnSpeedChanged;
        m_SceneLoadedRSE.Event -= OnSceneLoaded;
    }

    private void OnSpeedChanged(float speed)
    {
        // Adjust pitch
        float newPitch = 1 + ((speed - 1) / 10);
        Debug.Log("MusicManager.OnSpeedChanged: New pitch = " + newPitch);
        m_AudioMixer.SetFloat("MusicPitch", newPitch);
    }

    private void OnSceneLoaded(string scene)
    {
        Debug.Log("MusicManager.OnSceneLoaded");

        if (m_AudioSource.clip != m_AudioClipDict[scene])
        {
            m_AudioSource.Stop();
            m_AudioSource.clip = m_AudioClipDict[scene];
            m_AudioSource.Play();
        }
    }
}
