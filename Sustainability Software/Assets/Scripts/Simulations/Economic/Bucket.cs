using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Bucket : MonoBehaviour, IDropHandler
{
    public List<Token> tokensInBucket = new List<Token>();
    [SerializeField] private EconSimManager02 econManager;
    private float totalFairness = 0f;
    private float totalResilience = 0f;
    private float totalProfit = 0f;

    public void OnDrop(PointerEventData eventData)
    {
        Token token = eventData.pointerDrag.GetComponent<Token>();
        if (token != null)
        {
            tokensInBucket.Add(token);

            totalFairness += token.fairnessEffect;
            totalResilience += token.resilienceEffect;
            totalProfit += token.profitEffect;

            token.gameObject.SetActive(false); //Token consumed

            econManager.UpdateBucketTotals(totalFairness, totalResilience, totalProfit);
        }
    }

    public void ResetBucket()
    {
        econManager.ClearBucket(tokensInBucket);
        tokensInBucket.Clear();
        totalFairness = totalResilience = totalProfit = 0f;
    }
}
