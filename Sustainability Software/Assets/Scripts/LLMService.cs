using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public static class LLMService
{
    private static string apiUrl = "/api/get-data"; //Vercel function

    public static async Task<string> SendChoiceAsync(ScenarioData scenario, ChoiceData choice)
    {
        string prompt = $@"
        Scenario: {scenario.clientBrief}
        Player choice: {choice.choiceText}
        Respond professionally and highlight trade-offs.
        ";

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
                return www.downloadHandler.text;

            if (www.result == UnityWebRequest.Result.Success)
            {
                //Parse JSON response from Vercel
                try
                {
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

