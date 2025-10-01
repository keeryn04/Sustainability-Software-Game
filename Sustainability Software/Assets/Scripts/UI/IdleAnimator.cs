using UnityEngine;
using System.Collections;

public class IdleAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

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
            //Wait a random time
            float waitTime = Random.Range(minWait, maxWait);
            yield return new WaitForSeconds(waitTime);

            //Pick a random idle animation
            int randomIdle = Random.Range(0, 1); //REMEMBER TO ADJUST FOR MORE IDLE ANIMS

            switch (randomIdle)
            {
                case 0:
                    animator.SetTrigger("IdleBlink");
                    break;
            }
        }
    }
}
