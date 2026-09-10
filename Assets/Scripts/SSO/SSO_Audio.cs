using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SSO_Audio", menuName = "SnakeSSO/SSO Audio")]
public class SSO_Audio : StaticScriptableObject<List<AudioElement>>
{
    private Dictionary<string, AudioClip> m_Cache;

    public void InitCache()
    {
        m_Cache = new Dictionary<string, AudioClip>();
        foreach (AudioElement elem in this.Value)
        {
            m_Cache[elem.Key] = elem.Clip;
        }
    }

    public AudioClip GetAudioClip(string key)
    {
        if (m_Cache.ContainsKey(key))
            return m_Cache[key];

        return null;
    }
}

[Serializable]
public struct AudioElement
{
    public string Key;
    public AudioClip Clip;
}
