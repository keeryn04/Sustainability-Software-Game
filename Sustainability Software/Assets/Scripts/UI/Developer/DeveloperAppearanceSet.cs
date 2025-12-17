using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeveloperAppearanceSet", menuName = "Sustainability/DeveloperAppearanceSet")]
public class DeveloperAppearanceSet : ScriptableObject
{
    public SustainabilityPillar pillar;

    public AnimatorOverrideController animatorOverride;
}

