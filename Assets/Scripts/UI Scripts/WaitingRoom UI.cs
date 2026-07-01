using Photon.Pun;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;   // Image, Button, Slider, Toggle, etc.
public class WaitingRoomUI : MonoBehaviourPunCallbacks
{
    public GameObject revealPanel;
    public GameObject waitingRoomPanel;
    public GameObject questionPanel;
    public static WaitingRoomUI Instance;
     public Image fadeImage;
     public Animator animator;
     public GameObject question;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        revealPanel.SetActive(false);
        waitingRoomPanel.SetActive(false);
        questionPanel.SetActive(true);
    }

    public void ShowQuestion()
    {
        StartCoroutine(ShowQuestionRoutine());
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float time = 0f;

        Color c = fadeImage.color;
        c.a = startAlpha;
        fadeImage.color = c;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadeImage.color = c;

            yield return null;
        }

        c.a = endAlpha;
        fadeImage.color = c;
    }

    private IEnumerator ShowQuestionRoutine()
    {
        // 1. fade to black
        yield return StartCoroutine(Fade(0f, 1f, 1f));

        // 2. show UI at black screen
        question.SetActive(true);
        animator.Play("QuestionPopup");

        // 3. wait until animation finishes
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // 4. fade back in
        yield return StartCoroutine(Fade(1f, 0f, 1f));
    }
}