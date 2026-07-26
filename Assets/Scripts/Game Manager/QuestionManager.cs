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

    private void Start()
    {
        // If no questions were set up in the Inspector, use the built-in bank.
        // animalID must match the Animal component on each prefab in Resources/Animals.
        if (questionBank.Count == 0)
            questionBank = CreateDefaultQuestionBank();
    }

    private static List<Question> CreateDefaultQuestionBank()
    {
        return new List<Question>
        {
            new Question
            {
                questionID = 1,
                correctAnimalID = 1,
                questionText = "Aku adalah mamalia darat terbesar yang memiliki belalai panjang dan telinga yang lebar. Siapakah aku?",
                questionDescription = "Gajah adalah mamalia darat terbesar dengan belalai panjang."
            },
            new Question
            {
                questionID = 2,
                correctAnimalID = 2,
                questionText = "Aku memiliki leher yang sangat panjang untuk memakan daun-daun di pohon yang tinggi. Siapakah aku?",
                questionDescription = "Jerapah memiliki leher panjang untuk mencapai daun tinggi."
            },
            new Question
            {
                questionID = 4,
                correctAnimalID = 4,
                questionText = "Aku sering disebut sebagai 'Raja Hutan' dan memiliki rambut lebat di sekitar kepalaku. Siapakah aku?",
                questionDescription = "Singa dikenal sebagai raja hutan dan hidup berkelompok."
            },
            new Question
            {
                questionID = 3,
                correctAnimalID = 3,
                questionText = "Aku adalah hewan berkaki empat yang sangat cepat berlari dan sering membantu manusia menarik delman. Siapakah aku?",
                questionDescription = "Kuda adalah hewan kuat yang dapat berlari sangat cepat."
            },
            new Question
            {
                questionID = 5,
                correctAnimalID = 5,
                questionText = "Aku adalah burung berbulu indah yang terkenal pintar menirukan suara atau ucapan manusia. Siapakah aku?",
                questionDescription = "Burung beo pandai meniru suara manusia."
            },
            new Question
            {
                questionID = 6,
                correctAnimalID = 6,
                questionText = "Aku adalah burung yang sering hidup berdampingan dengan manusia di taman kota dan dahulu sering digunakan untuk mengirim surat. Siapakah aku?",
                questionDescription = "Merpati terkenal karena kemampuan kembali ke sarangnya."
            },
            new Question
            {
                questionID = 7,
                correctAnimalID = 7,
                questionText = "Aku memiliki corak hitam seperti topeng di sekitar mataku dan suka 'mencuci' makananku sebelum dimakan. Siapakah aku?",
                questionDescription = "Rakun memiliki wajah seperti memakai topeng hitam."
            },
            new Question
            {
                questionID = 8,
                correctAnimalID = 8,
                questionText = "Aku adalah burung mungil berbulu cokelat yang sangat sering melompat-lompat di halaman rumah atau atap gereja. Siapakah aku?",
                questionDescription = "Burung gereja adalah burung kecil yang sering ditemukan di sekitar rumah."
            },
            new Question
            {
                questionID = 9,
                correctAnimalID = 9,
                questionText = "Aku adalah kucing besar yang memiliki bulu berwarna oranye dengan garis-garis loreng hitam yang gagah. Siapakah aku?",
                questionDescription = "Harimau adalah kucing besar dengan loreng hitam khas."
            },
            new Question
            {
                questionID = 10,
                correctAnimalID = 10,
                questionText = "Aku adalah burung penyanyi bertubuh sangat kecil, lincah, dan memiliki kombinasi warna bulu yang cantik di kepalaku. Siapakah aku?",
                questionDescription = "Burung gelatik adalah burung kecil dengan suara merdu."
            },
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

        int randIndex = Random.Range(1, currAvailable.Count);

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
