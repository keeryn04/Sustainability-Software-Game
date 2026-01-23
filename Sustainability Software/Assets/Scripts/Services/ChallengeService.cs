using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ChallengeService : MonoBehaviour
{
    private static readonly string apiUrl = EnvLoader.Get("CHALLENGE_API_URL");
    private const bool USE_MOCK_QUIZ = true;

    public static async Task<Quiz> GenerateChallengeAsync(string topic, int numQuestions = 5)
    {
        if (USE_MOCK_QUIZ)
        {
            await Task.Yield(); // keeps async behavior consistent
            return CreateMockQuiz(numQuestions);
        }

        if (string.IsNullOrEmpty(topic))
        {
            Debug.LogError("ChallengeService: Topic cannot be empty.");
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
                Debug.LogError($"ChallengeService: Failed to contact server: {www.responseCode} - {www.error}");
                Debug.LogError("Response body: " + www.downloadHandler.text);
                return null;
            }

            try
            {
                QuizResponse response = JsonUtility.FromJson<QuizResponse>(www.downloadHandler.text);
                if (response == null || response.quiz == null || response.quiz.questions == null)
                {
                    Debug.LogError("ChallengeService: Invalid server response: " + www.downloadHandler.text);
                    return null;
                }

                return response.quiz;
            }
            catch (Exception ex)
            {
                Debug.LogError("ChallengeService: Failed to parse JSON: " + www.downloadHandler.text);
                Debug.LogException(ex);
                return null;
            }
        }
    }

    private static Quiz CreateMockQuiz(int numQuestions)
    {
        var quiz = new Quiz
        {
            questions = new List<BossQuestion>()
        };

        quiz.questions.Add(new BossQuestion
        {
            bossQuestion = "The boss is worried that the new feature will increase server costs. How should the team respond?",
            strategies = new Strategy[]
            {
            new Strategy
            {
                id = "Attack",
                description = "Profile the feature’s resource usage and optimize hot paths to reduce unnecessary computation."
            },
            new Strategy
            {
                id = "Defend",
                description = "Scale up infrastructure immediately to avoid any performance issues."
            }
            },
            correctDeveloper = "Economic",
            correctStrategyId = "Attack",
            explanation = "Optimizing resource usage reduces costs and improves sustainability without over-provisioning infrastructure."
        });

        quiz.questions.Add(new BossQuestion
        {
            bossQuestion = "Users report slow load times on older devices. What is the most sustainable approach?",
            strategies = new Strategy[]
            {
            new Strategy
            {
                id = "Attack",
                description = "Implement lightweight assets and lazy loading to improve performance on low-end devices."
            },
            new Strategy
            {
                id = "Defend",
                description = "Recommend users upgrade to newer hardware."
            }
            },
            correctDeveloper = "Technical",
            correctStrategyId = "Attack",
            explanation = "Supporting older devices reduces electronic waste and makes the software more inclusive."
        });

        quiz.questions.Add(new BossQuestion
        {
            bossQuestion = "The codebase is becoming hard to maintain as the team grows. What should be prioritized?",
            strategies = new Strategy[]
            {
            new Strategy
            {
                id = "Attack",
                description = "Continue adding features to stay competitive."
            },
            new Strategy
            {
                id = "Defend",
                description = "Refactor the codebase and establish clear coding standards."
            }
            },
            correctDeveloper = "Technical",
            correctStrategyId = "Defend",
            explanation = "Maintainable code reduces long-term technical debt and energy spent on fixes."
        });

        quiz.questions = quiz.questions.Take(numQuestions).ToList();

        return quiz;
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
        public List<BossQuestion> questions;
    }

    [Serializable]
    public class Strategy
    {
        public string id;          // "Attack" or "Defend"
        public string description;
    }

    [System.Serializable]
    public class BossQuestion
    {
        public string bossQuestion;
        public Strategy[] strategies;
        public string correctDeveloper;
        public string correctStrategyId;
        public string explanation;
    }
}
