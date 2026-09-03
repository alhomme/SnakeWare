using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private AudioMixer m_AudioMixer;
    [SerializeField] private AudioSource m_AudioSource;
    [SerializeField] private AudioClip m_AudioClipDefault;
    [SerializeField] private AudioClip m_AudioClipMG;

    [SerializeField] private RSO_Speed m_SpeedRSO;
    [SerializeField] private RSE_LoadScene m_LoadScene;

    private void OnEnable()
    {
        m_SpeedRSO.OnChanged += OnSpeedChanged;
        m_LoadScene.Event += OnLoadScene;
    }

    private void OnDisable()
    {
        m_SpeedRSO.OnChanged -= OnSpeedChanged;
        m_LoadScene.Event -= OnLoadScene;
    }

    private void OnSpeedChanged(float speed)
    {
        // Adjust pitch
        float newPitch = 1 + ((speed - 1) / 10);
        Debug.Log("MusicManager.OnSpeedChanged: New pitch = " + newPitch);
        m_AudioMixer.SetFloat("MusicPitch", newPitch);
    }

    private void OnLoadScene(string scene)
    {
        Debug.Log("MusicManager.OnLoadScene");

        if (scene.StartsWith("MiniGame"))
        {
            m_AudioSource.Stop();
            m_AudioSource.clip = m_AudioClipMG;
            m_AudioSource.Play();
        }
        else
        {
            if (m_AudioSource.clip != m_AudioClipDefault)
            {
                m_AudioSource.Stop();
                m_AudioSource.clip = m_AudioClipDefault;
                m_AudioSource.Play();
            }
        }
    }
}
