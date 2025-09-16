using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "NewScenario", menuName = "GreenCodeTycoon/Scenario")]
public class ScenarioData : ScriptableObject
{
    [TextArea] public string clientBrief; // Initial client problem
    public ChoiceData[] choices;        // Array of possible player decisions
    public string reflectionFeedback;   //General wrap-up feedback
    public SustainabilityPillar pillar; //Type of scenario
    public Sprite resourceBarSprite;    //Image for resource management
}
