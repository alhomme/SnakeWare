using System;
using UnityEngine;

public abstract class RuntimeScriptableObject<T> : ScriptableObject
{
    private T mValue;
    public event Action<T> OnChanged;

    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;

    public T Value
    {
        get { return mValue; }
        set
        {
            mValue = value;
            OnChanged?.Invoke(mValue);
        }
    }
}
