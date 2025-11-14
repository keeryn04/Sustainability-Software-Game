using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ScalePlatform : MonoBehaviour, IDropHandler
{
    public List<Token> tokensOnPlatform = new List<Token>();
    [SerializeField] private EconSimManager02 econManager;
    private float totalFairness = 0f;
    private float totalProfit = 0f;

    public void OnDrop(PointerEventData eventData)
    {
        Token token = eventData.pointerDrag.GetComponent<Token>();
        if (token != null)
        {
            tokensOnPlatform.Add(token);

            totalFairness += token.fairnessEffect;
            totalProfit += token.profitEffect;

            token.isDraggable = false; //Token consumed

            econManager.UpdateTotals(totalFairness, totalProfit);
        }
    }

    public void ResetBucket()
    {
        econManager.ClearPlatform(tokensOnPlatform);
        tokensOnPlatform.Clear();
        totalFairness = totalProfit = 0f;
    }
}
