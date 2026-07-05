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
        
        UpdateAdminUI();
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
            PhotonNetwork.CurrentRoom.MaxPlayers;
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

        animator.Play("QuestionPopup", 0, 0f);

        yield return null;

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(state.length + 1.5f);

        // 4. fade back in
        yield return StartCoroutine(Fade(1f, 0f, 1f));
    }
    
    /// <summary>
    /// Show the results/answer screen where only admin can proceed to next question
    /// </summary>
    public void ShowResultsPanel(Question question, bool isCorrect)
    {
        if (resultPanel == null)
            return;
            
        resultPanel.SetActive(true);
        
        // Update result information
        TMP_Text resultText = resultPanel.GetComponentInChildren<TMP_Text>();
        if (resultText != null)
        {
            resultText.text = isCorrect ? "✓ Correct Answer!" : "✗ Answer";
            if (question != null)
                resultText.text += "\n\n" + question.questionText;
        }
    }
    
    public void HideResultsPanel()
    {
        if (resultPanel != null)
            resultPanel.SetActive(false);
    }
    
    /// <summary>
    /// Admin clicks this to proceed to next question
    /// </summary>
    public void AdminNextQuestion()
    {
        if (!MainNetwork.IsAdmin)
            return;
            
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
            
        HideResultsPanel();
        GameManager.Instance.AdminEndGame();
    }
}