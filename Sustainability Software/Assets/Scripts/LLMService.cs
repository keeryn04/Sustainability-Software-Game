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

            Debug.LogError("LLM call failed: " + www.error);
            return "Error contacting LLM service.";
        }
    }

    [System.Serializable]
    private class PromptRequest { public string prompt; }
}

