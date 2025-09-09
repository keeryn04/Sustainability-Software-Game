using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEditor.Experimental.GraphView;

public static class LLMService
{
    private static string apiKey = EnvLoader.Get("GEMINI_KEY");
    private static string apiUrl = "https://generativelanguage.googleapis.com/v1beta2/models/text-bison-001:generateText"; //EnvLoader.Get("GEMINI_URL");

    public static async Task<string> SendChoiceAsync(ScenarioData scenario, ChoiceData choice)
    {
        string prompt = $"Scenario: {scenario.clientBrief} Player choice: {choice.choiceText} Respond professionally and highlight trade-offs.";

        var chatRequest = new ChatRequest
        {
            model = "gpt-3.5-turbo",
            messages = new ChatMessage[]
    {
        new ChatMessage { role = "user", content = prompt }
    },
            max_tokens = 500
        };

        string jsonBody = JsonUtility.ToJson(chatRequest);

        //Send request to OpenAI
        using (UnityWebRequest www = new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Authorization", "Bearer " + EnvLoader.Get("OPENAI_API_KEY"));

            var asyncOp = www.SendWebRequest();
            while (!asyncOp.isDone) await Task.Yield();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string rawJson = www.downloadHandler.text;

                //Parse the text from the response
                var response = JsonUtility.FromJson<OpenAIResponse>(rawJson);
                return response.choices[0].message.content;
            }
            else
            {
                Debug.LogError($"Error calling OpenAI API: {www.responseCode} - {www.error}");
                Debug.LogError("Response body: " + www.downloadHandler.text);
                return "Error: Failed to contact service.";
            }
        }
    }

    //Response classes
    [System.Serializable]
    private class OpenAIResponse
    {
        public Choice[] choices;
    }

    [System.Serializable]
    private class Choice
    {
        public Message message;
    }

    [System.Serializable]
    private class Message
    {
        public string role;
        public string content;
    }

    [System.Serializable]
    public class ChatRequest
    {
        public string model;
        public ChatMessage[] messages;
        public int max_tokens;
    }

    [System.Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;
    }
}
