using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[CreateAssetMenu(fileName = "SSO_Items", menuName = "SnakeSSO/SSO Items")]
public class SSO_Items : StaticScriptableObject<List<GameObject>>
{
}