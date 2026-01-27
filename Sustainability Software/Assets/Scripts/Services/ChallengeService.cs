using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class ChallengeService : MonoBehaviour
{
    private static string apiUrl = "/api/generate-test";

    public static async Task<Challenge> GenerateChallengeAsync(string topic, int numQuestions = 5)
    {
        if (string.IsNullOrEmpty(topic))
        {
            Debug.LogError("ChallengeService: Topic cannot be empty.");
            return null;
        }

        string jsonBody = JsonUtility.ToJson(new ChallengeRequest
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
                Debug.LogError($"ChallengeService: Failed to contact server: {www.error}");
                return null;
            }

            try
            {
                //Parse server response
                ChallengeResponse response = JsonUtility.FromJson<ChallengeResponse>(www.downloadHandler.text);

                if (response == null || response.challenge == null || response.challenge.questions == null)
                {
                    Debug.LogError("ChallengeService: Invalid server response: " + www.downloadHandler.text);
                    return null;
                }

                return response.challenge;
            }
            catch
            {
                Debug.LogError("ChallengeService: Failed to parse JSON: " + www.downloadHandler.text);
                return null;
            }
        }
    }

    [Serializable]
    private class ChallengeRequest
    {
        public string query;
        public int numQuestions;
    }

    [Serializable]
    public class ChallengeResponse
    {
        public Challenge challenge;
        public string error;
    }

    [Serializable]
    public class Challenge
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
