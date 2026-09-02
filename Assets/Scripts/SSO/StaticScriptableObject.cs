using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public abstract class StaticScriptableObject<T> : ScriptableObject
{
    [SerializeField] private T mValue;

    private void OnEnable() => hideFlags = HideFlags.DontUnloadUnusedAsset;

    public T Value
    {
        get { return mValue; }
    }
}