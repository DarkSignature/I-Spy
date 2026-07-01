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

    public Question GetRandomQuestion()
    {
        List<Question> available = questionBank.Where(q => !q.used).ToList();

        if(available.Count == 0)
        {
            ResetQuestions();
            available = questionBank;
        }

        Question chosen = available[Random.Range(0, available.Count)];

        chosen.used = true;

        return chosen;
    }

    void ResetQuestions()
    {
        foreach(var question in questionBank)
            question.used = false;
    }

}
