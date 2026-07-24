using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Question
{
    public int questionID;
    public string questionText;

    public int correctAnimalID;
    public string questionDescription;
    public bool used;

    // Future additions
    // public QuestionType type;
    // public AudioClip narration;
}
