using UnityEngine;
using System.Collections;

public class IdleAnimator : MonoBehaviour
{
    private enum AnimType
    {
        Developer,
        Boss
    }

    [SerializeField] private Animator animator;
    [SerializeField] private AnimType animType;

    [Tooltip("Minimum seconds between idle animations")]
    [SerializeField] private float minWait = 3f;

    [Tooltip("Maximum seconds between idle animations")]
    [SerializeField] private float maxWait = 8f;

    private void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        StartCoroutine(PlayRandomIdle());
    }

    private IEnumerator PlayRandomIdle()
    {
        while (true)
        {
            float waitTime = Random.Range(minWait, maxWait);
            yield return new WaitForSeconds(waitTime);

            if (animType == AnimType.Boss)
            {
                animator.SetTrigger("BossIdle");
            } else
            {
                animator.SetTrigger("IdleBlink"); //Generic Idle
            }
        }
    }
}
