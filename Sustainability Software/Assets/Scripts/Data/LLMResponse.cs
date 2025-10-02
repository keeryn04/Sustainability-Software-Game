using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LLMResponse
{
    public string clientResponse;
    public string[] choices;
    public float resourceImpact;
    public string reflection;
}
