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

        // If no questions were set up in the Inspector, use the built-in bank.
        // Done in Awake so the bank is ready before buffered RPCs arrive on scene load.
        // animalID must match the Animal component on each prefab in Resources/Animals.
        if (questionBank.Count == 0)
            questionBank = CreateDefaultQuestionBank();
    }

    [SerializeField]
    private List<Question> questionBank = new();

    private static List<Question> CreateDefaultQuestionBank()
    {
        return new List<Question>
        {
            new Question { questionID = 1,  correctAnimalID = 1,  questionText = "Aku adalah mamalia darat terbesar yang memiliki belalai panjang dan telinga yang lebar. Siapakah aku?" },                                          // Elephant (Gajah)
            new Question { questionID = 2,  correctAnimalID = 2,  questionText = "Aku memiliki leher yang sangat panjang untuk memakan daun-daun di pohon yang tinggi. Siapakah aku?" },                                             // Giraffe (Jerapah)
            new Question { questionID = 3,  correctAnimalID = 3,  questionText = "Aku sering disebut sebagai 'Raja Hutan' dan memiliki rambut lebat di sekitar kepalaku. Siapakah aku?" },                                           // Lion (Singa)
            new Question { questionID = 4,  correctAnimalID = 4,  questionText = "Aku adalah hewan berkaki empat yang sangat cepat berlari dan sering membantu manusia menarik delman. Siapakah aku?" },                             // Horse (Kuda)
            new Question { questionID = 5,  correctAnimalID = 5,  questionText = "Aku adalah burung berbulu indah yang terkenal pintar menirukan suara atau ucapan manusia. Siapakah aku?" },                                        // Parrot (Burung Beo)
            new Question { questionID = 6,  correctAnimalID = 6,  questionText = "Aku adalah burung yang sering hidup berdampingan dengan manusia di taman kota dan dahulu sering digunakan untuk mengirim surat. Siapakah aku?" },  // Pigeon (Burung Merpati)
            new Question { questionID = 7,  correctAnimalID = 7,  questionText = "Aku memiliki corak hitam seperti topeng di sekitar mataku dan suka 'mencuci' makananku sebelum dimakan. Siapakah aku?" },                          // Racoon (Rakun)
            new Question { questionID = 8,  correctAnimalID = 8,  questionText = "Aku adalah burung mungil berbulu cokelat yang sangat sering melompat-lompat di halaman rumah atau atap gereja. Siapakah aku?" },                   // Sparrow (Burung Gereja)
            new Question { questionID = 9,  correctAnimalID = 9,  questionText = "Aku adalah kucing besar yang memiliki bulu berwarna oranye dengan garis-garis loreng hitam yang gagah. Siapakah aku?" },                           // Tiger (Harimau)
            new Question { questionID = 10, correctAnimalID = 10, questionText = "Aku adalah burung penyanyi bertubuh sangat kecil, lincah, dan memiliki kombinasi warna bulu yang cantik di kepalaku. Siapakah aku?" },             // Tit (Burung Gelatik)
        };
    }

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
