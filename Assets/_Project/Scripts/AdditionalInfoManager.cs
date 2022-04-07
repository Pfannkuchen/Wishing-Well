using System;
using UnityEngine;

public static class AdditionalInfoManager
{
    public static Action InfoUpdated;
    public static void OnInfoUpdated() => InfoUpdated?.Invoke();
    
    public enum Parameter
    {
        Undefined = 0,
        
        DatabaseSize = 1,
        DatabaseLoadTime = 2,
        CoinManifests = 3,
        
        Sessions = 10,
        CoinsLoaded = 11,
        CoinsThrown = 12
    }

    private static string ToPlayerPrefs(this Parameter parameter)
    {
        return parameter switch
        {
            Parameter.Undefined => "",
            Parameter.DatabaseSize => "DatabaseSize",
            Parameter.DatabaseLoadTime => "DatabaseLoadTime",
            Parameter.CoinManifests => "CoinManifests",
            Parameter.Sessions => "Sessions",
            Parameter.CoinsLoaded => "CoinsLoaded",
            Parameter.CoinsThrown => "CoinsThrown",
            _ => ""
        };
    }

    public static void SetValue(this Parameter parameter, int value)
    {
        PlayerPrefs.SetInt(parameter.ToPlayerPrefs(), value);
        OnInfoUpdated();
    }

    public static void AddValue(this Parameter parameter, int value)
    {
        int currentValue = parameter.GetValue();
        currentValue += value;
        
        parameter.SetValue(currentValue);
    }

    public static int GetValue(this Parameter parameter)
    {
        return PlayerPrefs.GetInt(parameter.ToPlayerPrefs());
    }
}
