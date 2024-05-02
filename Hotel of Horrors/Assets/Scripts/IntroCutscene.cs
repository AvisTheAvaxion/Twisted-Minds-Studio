using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCutscene : MonoBehaviour
{
    [SerializeField] DialogueSystem dialogueSystem;
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] Animator varrenAnimator;
    [SerializeField] float varrenMoveSpeed = 0.8f;
    [SerializeField] string fileName;
    [SerializeField] int blockNum;
    [SerializeField] Transform startTransform;
    [SerializeField] Transform endTransform;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(IntroSequence());
    }

    IEnumerator IntroSequence()
    {
        yield return null;
        playerMovement.TogglePlayerControls(false);
        varrenAnimator.gameObject.SetActive(true);
        varrenAnimator.gameObject.transform.position = startTransform.position;
        varrenAnimator.SetBool("isWalking", true);
        varrenAnimator.SetFloat("x", 1);
        varrenAnimator.SetFloat("y", 0);
        float timer = 0;
        while(timer < 1)
        {
            varrenAnimator.gameObject.transform.position = Vector3.Lerp(startTransform.position, endTransform.position, timer);
            yield return null;
            timer += Time.deltaTime * varrenMoveSpeed;
        }
        varrenAnimator.SetBool("isWalking", false);

        dialogueSystem.StartDialogue(fileName, blockNum);
        yield return null;
        yield return new WaitUntil(() => Time.timeScale > 0.5f);

        varrenAnimator.SetFloat("x", -1);
        varrenAnimator.SetFloat("y", 0);
        varrenAnimator.SetBool("isWalking", true);
        timer = 0;
        while (timer < 1)
        {
            varrenAnimator.gameObject.transform.position = Vector3.Lerp(endTransform.position, startTransform.position, timer);
            yield return null;
            timer += Time.deltaTime * varrenMoveSpeed;
        }
        varrenAnimator.SetBool("isWalking", false);
        varrenAnimator.gameObject.SetActive(false);

        playerMovement.TogglePlayerControls(true);
    }
}
