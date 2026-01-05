using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class QuizService : MonoBehaviour
{
    public int numQuestions = 5;
    private static string apiUrl = "/api/generate-quiz";

    public Action<Quiz> OnQuizLoaded;
    public Action<string> OnQuizError;

    public static async Task<Quiz> GenerateQuizAsync(string topic, int numQuestions = 5)
    {
        if (string.IsNullOrEmpty(topic))
        {
            Debug.LogError("QuizService: Topic cannot be empty.");
            return null;
        }

        string jsonBody = JsonUtility.ToJson(new QuizRequest
        {
            query = topic,
            numQuestions = numQuestions
        });

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            var asyncOp = www.SendWebRequest();
            while (!asyncOp.isDone) await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"QuizService: Failed to contact server: {www.error}");
                return null;
            }

            try
            {
                //Parse server response
                QuizResponse response = JsonUtility.FromJson<QuizResponse>(www.downloadHandler.text);

                if (response == null || response.quiz == null || response.quiz.questions == null)
                {
                    Debug.LogError("QuizService: Invalid server response: " + www.downloadHandler.text);
                    return null;
                }

                return response.quiz;
            }
            catch
            {
                Debug.LogError("QuizService: Failed to parse JSON: " + www.downloadHandler.text);
                return null;
            }
        }
    }

    //Request payload class
    [System.Serializable]
    private class QuizRequest
    {
        public string query;
        public int numQuestions;
    }

    //Response wrapper classes
    [System.Serializable]
    public class QuizResponse
    {
        public Quiz quiz;
        public string error;
    }

    [System.Serializable]
    public class Quiz
    {
        public List<QuizQuestion> questions;
    }

    [System.Serializable]
    public class QuizQuestion
    {
        public string question;
        public List<string> options;
        public int correctIndex;
        public string explanation;
    }
}