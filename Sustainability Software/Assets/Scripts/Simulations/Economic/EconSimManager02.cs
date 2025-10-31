using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EconSimManager02 : MonoBehaviour
{
    private float fairness;
    private float resilience;
    private float profit;

    public void UpdateBucketTotals(float newFairness, float newResilience, float newProfit)
    {
        fairness = newFairness;
        resilience = newResilience;
        profit = newProfit;

        EvaluateSustainability();
    }

    void EvaluateSustainability()
    {
        float imbalance = Mathf.Abs((fairness + resilience) - profit);

        if (imbalance > 10f)
        {
            Debug.Log("System Collapse: imbalance too large!");
        }
        else if (imbalance > 5f)
        {
            Debug.Log("Warning: System becoming unstable...");
        }
        else
        {
            Debug.Log("Balanced and sustainable economy!");
        }

        Debug.Log($"Fairness: {fairness}, Resilience: {resilience}, Profit: {profit}");
    }

    public void ClearBucket(List<Token> tokensInBucket)
    {
        foreach (Token token in tokensInBucket)
        {
            token.gameObject.SetActive(true);
            token.ResetToken();
        }
    }
}
