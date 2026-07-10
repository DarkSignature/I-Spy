using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class QuestionManager : MonoBehaviour
{
    public static QuestionManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField]
    private List<Question> questionBank = new();

    private Question currentQuestion;

    public Question CurrentQuestion => currentQuestion;

    public int GetRandomQuestionID()
    {
        List<Question> currAvailable = questionBank.Where(q => !q.used).ToList();

        if(currAvailable.Count == 0)
        {
            ResetQuestions();
            currAvailable = questionBank;
        }

        int randIndex = Random.Range(0, currAvailable.Count);

        Question chosen = currAvailable[randIndex];

        chosen.used = true;

        return chosen.questionID;
    }
    
    public Question GetQuestionByID(int id)
    {
        return questionBank.FirstOrDefault(q => q.questionID == id);
    }

    void ResetQuestions()
    {
        foreach(var question in questionBank)
            question.used = false;
    }

}
