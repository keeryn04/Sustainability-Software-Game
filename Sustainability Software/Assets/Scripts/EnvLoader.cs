using System.IO;
using System.Collections.Generic;
using UnityEngine;
public static class EnvLoader
{
    private static Dictionary<string, string> envVars = new Dictionary<string, string>();

    static EnvLoader()
    {
        string path = Path.Combine(Application.dataPath, "../.env");
        if (!File.Exists(path))
        {
            Debug.LogWarning(".env file not found at " + path);
            return;
        }

        foreach (var line in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
            var split = line.Split('=', 2);
            if (split.Length == 2)
                envVars[split[0].Trim()] = split[1].Trim();
        }
    }

    public static string Get(string key)
    {
        if (envVars.ContainsKey(key)) return envVars[key];
        Debug.LogWarning($"Env key {key} not found");
        return null;
    }
}