using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;   // Image, Button, Slider, Toggle, etc.
public class WaitingRoomUI : MonoBehaviourPunCallbacks
{
    public GameObject revealPanel;
    public GameObject waitingRoomPanel;
    public GameObject questionPanel;
    public GameObject resultPanel;
    public Button nextQuestionButton;
    public Button endGameButton;
    public Button startGameButton;
    public TMP_Text adminStatusText;
    public TMP_Text playerCountText;
    
    public static WaitingRoomUI Instance;
     public Image fadeImage;
     public Animator animator;
     public GameObject question;
     public TMP_Text questionText;
     public TMP_Text questionNumber;

    [Header("Answer Timer (Phase 6)")]
    public GameObject timerPanel;
    public TMP_Text timerText;
    [Tooltip("Optional radial/bar image. Image Type must be set to Filled.")]
    public Image timerFillImage;
    public Color timerNormalColor = Color.white;
    public Color timerWarningColor = Color.red;
    [Tooltip("Timer turns to the warning color when this many seconds remain.")]
    public float timerWarningThreshold = 3f;

    // Result Page
    public TMP_Text correctText;
    public TMP_Text correctAnimal;
    public TMP_Text correctDescription;

    void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        revealPanel.SetActive(false);
        waitingRoomPanel.SetActive(true);
        questionPanel.SetActive(true);
        
        if (resultPanel != null)
            resultPanel.SetActive(false);

        if (timerPanel != null)
            timerPanel.SetActive(false);

        UpdateAdminUI();
        UpdatePlayerCount();
    }

    public void StartGameUI()
    {
        waitingRoomPanel.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerCount();
        
        // Update admin UI for waiting room
        if (WaitingRoomUI.Instance != null)
            WaitingRoomUI.Instance.UpdateAdminUI();
    }

    void UpdatePlayerCount()
    {
            if (playerCountText == null)
                return;
            
        playerCountText.text =
            "Player(s) - "
            +
            (PhotonNetwork.CurrentRoom.PlayerCount - 1)
            + " / " +
            "4";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerCount();
    }

    public void ShowTimer(float duration)
    {
        timerText.text = duration.ToString();
        if (timerPanel != null)
            timerPanel.SetActive(true);

        UpdateTimer(duration, duration);
    }

    public void UpdateTimer(float remaining, float duration)
    {
        remaining = Mathf.Max(0f, remaining);

        bool warning = remaining <= timerWarningThreshold;
        Color color = warning ? timerWarningColor : timerNormalColor;

        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(remaining).ToString();
            timerText.color = color;
        }

        if (timerFillImage != null)
        {
            timerFillImage.fillAmount = duration > 0f ? remaining / duration : 0f;
            timerFillImage.color = color;
        }
    }

    public void HideTimer()
    {
        if (timerPanel != null)
            timerPanel.SetActive(false);
    }
    
    public void UpdateAdminUI()
    {
        // Update admin status display
        if (adminStatusText != null)
        {
            adminStatusText.text = MainNetwork.IsAdmin ? "👑 ADMIN" : "PLAYER";
        }
        
        // Enable/disable admin buttons based on role
        if (startGameButton != null)
            startGameButton.interactable = MainNetwork.IsAdmin;
            
        if (nextQuestionButton != null)
            nextQuestionButton.interactable = MainNetwork.IsAdmin;
            
        if (endGameButton != null)
            endGameButton.interactable = MainNetwork.IsAdmin;
    }

    public void ShowQuestion(Question currentQuestion)
    {   
        questionNumber.text = "Q" + GameManager.Instance.curRoundNumber;
        questionText.text = currentQuestion.questionText;
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
        animator.Play("DefaultQuestion");

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("DefaultQuestion"))
        {
            yield return null;
        }

        if(!MainNetwork.IsAdmin){
            animator.Play("QuestionPopup", 0, 0f);
        }else{
            animator.Play("QuestionPopupAdmin", 0, 0f);
        }

        yield return null;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length + 1.5f);

        // 4. fade back in
        yield return StartCoroutine(Fade(1f, 0f, 1f));
    }
    
    /// <summary>
    /// Show the results/answer screen where only admin can proceed to next question
    /// </summary>
    public void ShowResultsPanel(Question question, bool isCorrect, Animal animal)
    {
        if (resultPanel == null)
            return;
            
        resultPanel.SetActive(true);
        
        // Update result information
        if (correctText != null)
        {
            // Debug.Log(correctAnimal);
            // Debug.Log(animal);
            // Debug.Log(animal.animalName);
            correctText.text = isCorrect || MainNetwork.IsAdmin ? "✓ Correct Answer!" : "✗ Answer";
            correctAnimal.text = animal.animalName;
            correctDescription.text = question.questionDescription;
        }
    }
    
    public void HideResultsPanel()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }

    public void HideQuestionPanel(){
        if (question != null){
            question.SetActive(false);
        }
    }
    
    /// <summary>
    /// Admin clicks this to proceed to next question
    /// </summary>
    public void AdminNextQuestion()
    {
        if (!MainNetwork.IsAdmin)
            return;
        HideQuestionPanel();
        HideResultsPanel();
        GameManager.Instance.AdminNextQuestion();
    }
    
    /// <summary>
    /// Admin clicks this to end the game
    /// </summary>
    public void AdminEndGame()
    {
        if (!MainNetwork.IsAdmin)
            return;
        GameManager.Instance.AdminEndGame();
    }

    public void ResetGame(){
        HideQuestionPanel();
        HideResultsPanel();
        waitingRoomPanel.SetActive(true);
    }
}