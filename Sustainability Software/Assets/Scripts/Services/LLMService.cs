using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public static class LLMService
{
    //Vercel function URL (primary endpoint)
    private static string apiUrl = "/api/get-data";

    public static async Task<string> SendChoiceAsync(ScenarioData scenario, string choice)
    {
        string prompt =
            "You are simulating a sustainability client meeting. " +
            $"Scenario: {scenario.clientBrief} " +
            $"Player's choice: {choice} " +
            "Respond professionally in 1–4 sentences summarizing the outcome of the player's choice. " +
            "Include both pros and cons, and suggest a follow-up action. " +
            "Then provide a short, one-sentence reflection summarizing if the choice was positive or negative for the client. " +
            "Finally, suggest 4 new actionable options the player could take next, " +
            "and provide a numeric resourceImpact value between -0.4 and 0.4 indicating the choice's sustainability impact " +
            "(negative = harmful, positive = helpful). " +
            "Return your answer strictly in this JSON format (no extra text, no numbering): " +
            "{ \"clientResponse\": \"...\", " +
            "\"reflection\": \"...\", " +
            "\"choices\": [\"choice1\", \"choice2\", \"choice3\", \"choice4\"], " +
            "\"resourceImpact\": 0.0 }";

        string jsonBody = JsonUtility.ToJson(new PromptRequest { prompt = prompt });

        using (UnityWebRequest www = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            var asyncOp = www.SendWebRequest();
            while (!asyncOp.isDone) await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    //Parse JSON response from Vercel
                    PromptResponse response = JsonUtility.FromJson<PromptResponse>(www.downloadHandler.text);
                    return response.text;
                }
                catch
                {
                    Debug.LogError("Failed to parse JSON from server: " + www.downloadHandler.text);
                    return "Error: Invalid server response.";
                }
            }
            else
            {
                Debug.LogError($"Error calling LLM service: {www.error}");
                return "Error: Failed to contact service.";
            }
        }
    }

    [System.Serializable]
    private class PromptRequest { public string prompt; }

    [System.Serializable]
    private class PromptResponse { public string text; }
}