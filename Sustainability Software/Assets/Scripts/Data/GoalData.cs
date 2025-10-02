using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewGoal", menuName = "Customs/Goal")]
public class GoalData : ScriptableObject
{
    public GoalType goalType;
    public string objective; //Objective player must meet
    public float resourceThreshold;
    public int pointThreshold;
}
public enum GoalType
{
    PointLevel,
    ResourceLevel
}