using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public static class LLMService
{
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
                var textResponse = response.choices[0].message.content;

                return textResponse;
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