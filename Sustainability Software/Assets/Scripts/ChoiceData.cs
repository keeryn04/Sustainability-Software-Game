using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChoiceData
{
    public string choiceText;                    //Text shown to player
    [TextArea] public string clientReaction;     //How client responds to choice
    public float resourceReaction;
    [TextArea] public string reflection;         //Pillar-specific reflection feedback
}
