using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class QuizService
{
    private static readonly string apiUrl = EnvLoader.Get("QUIZ_API_URL");

    public static async Task<Quiz> GenerateQuizAsync(string topic, int numQuestions = 5)
    {
        if (string.IsNullOrEmpty(topic))
        {
            Debug.LogError("QuizService: Topic cannot be empty.");
            return null;
        }

        var quizRequest = new QuizRequest
        {
            query = topic,
            numQuestions = numQuestions
        };

        string jsonBody = JsonUtility.ToJson(quizRequest);

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + EnvLoader.Get("RAG_API_KEY"));

            var asyncOp = www.SendWebRequest();
            while (!asyncOp.isDone) await Task.Yield();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"QuizService: Failed to contact server: {www.responseCode} - {www.error}");
                Debug.LogError("Response body: " + www.downloadHandler.text);
                return null;
            }

            try
            {
                QuizResponse response = JsonUtility.FromJson<QuizResponse>(www.downloadHandler.text);
                if (response == null || response.quiz == null || response.quiz.questions == null)
                {
                    Debug.LogError("QuizService: Invalid server response: " + www.downloadHandler.text);
                    return null;
                }

                return response.quiz;
            }
            catch (Exception ex)
            {
                Debug.LogError("QuizService: Failed to parse JSON: " + www.downloadHandler.text);
                Debug.LogException(ex);
                return null;
            }
        }
    }

    [Serializable]
    private class QuizRequest
    {
        public string query;
        public int numQuestions;
    }

    [Serializable]
    public class QuizResponse
    {
        public Quiz quiz;
        public string error;
    }

    [Serializable]
    public class Quiz
    {
        public List<QuizQuestion> questions;
    }

    [Serializable]
    public class QuizQuestion
    {
        public string question;
        public List<string> options;
        public int correctIndex;
        public string explanation;
    }
}