using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconSimManager02 : MonoBehaviour
{
    [SerializeField] private RectTransform scaleTop;
    [SerializeField] private float sensitivity = 100f;
    private float fairness;
    private float profit;
    private float targetAngle;

    public void UpdateTotals(float newFairness, float newProfit)
    {
        fairness = newFairness;
        profit = newProfit;

        EvaluateSustainability();
    }

    void EvaluateSustainability()
    {
        float imbalance = fairness - profit;

        //Calculate target angle based on imbalance
        targetAngle = Mathf.Atan2(imbalance, 1f) * Mathf.Rad2Deg * sensitivity;
        targetAngle = Mathf.Clamp(targetAngle, -135f, 135f);

        //Smoothly tilt toward target angle
        float currentZ = scaleTop.localEulerAngles.z;
        if (currentZ > 180f) currentZ -= 360f;

        float targetZ = Mathf.Lerp(currentZ, targetAngle, Time.deltaTime * 5f);
        scaleTop.localRotation = Quaternion.Euler(0f, 0f, targetZ);

        float absImbalance = Mathf.Abs(imbalance);
        if (absImbalance > 8f)
        {
            Debug.Log("System unstable! Economy is tilting too far!");
        }
        else
        {
            Debug.Log("Balanced and sustainable economy!");
        }

        Debug.Log($"Fairness: {fairness:F1}, Profit: {profit:F1}");
    }

    public void ClearPlatform(List<Token> tokensInBucket)
    {
        foreach (Token token in tokensInBucket)
        {
            token.isDraggable = true;
            token.ResetToken();
        }

        targetAngle = 0f;
        scaleTop.localRotation = Quaternion.identity;
    }
}
