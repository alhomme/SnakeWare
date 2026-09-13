using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static void Log(string message)
    {
        //Debug.Log(message);
    }

    public static void LogList(List<int> sequence)
    {
        string str = "Sequence = { ";

        foreach (var i in sequence)
        {
            str += i.ToString();
            str += " ";
        }
        str += "}";

        Log(str);
    }
}
