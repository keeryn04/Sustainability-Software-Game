using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeveloperAppearanceSwapper : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private DeveloperAppearanceSet[] appearanceSets;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        if (MenuManager.Instance != null)
        {

        }
        ApplyAppearance(MenuManager.Instance.CurrentPillar);
    }

    private void ApplyAppearance(SustainabilityPillar pillar)
    {
        foreach (var set in appearanceSets)
        {
            if (set.pillar == pillar)
            {
                animator.runtimeAnimatorController = set.animatorOverride;
                return;
            }
        }

        Debug.LogWarning($"No appearance set found for pillar {pillar}");
    }
}
