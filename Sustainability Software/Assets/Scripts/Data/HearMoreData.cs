using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "NewHearMore", menuName = "Customs/HearMore")]
public class HearMoreData : ScriptableObject
{
    [SerializeField] private string hearMoreName;
    public virtual string HearMoreName { get => hearMoreName; protected set => hearMoreName = value; }
    [SerializeField] private string hearMoreText;
    public virtual string HearMoreText { get => hearMoreText; protected set => hearMoreText = value; }

}
