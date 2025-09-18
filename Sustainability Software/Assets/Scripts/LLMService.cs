using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using UnityEditor.Experimental.GraphView;
using System;

public static class LLMService
{
    private static string apiKey = EnvLoader.Get("GEMINI_KEY");
    private static string apiUrl = EnvLoader.Get("GEMINI_URL");

    public static async Task<string> SendChoiceAsync(ScenarioData scenario, string choice)
    {
        string prompt =
            "You are simulating a sustainability client meeting. " +
            $"Scenario: {scenario.clientBrief} " +
            $"Player's choice: {choice} " +
            "Respond professionally in 1–4 sentences, summarizing the impact of the player's choice. " +
            "Include both pros and cons, and ask a follow-up question. " +
            "Then suggest 4 new actionable options the player could take to address the issues and question raised. " +
            "Ensure the answers have some good and some bad options. " +
            "Additionally, provide a numeric resourceImpact value between -0.5 and 0.5 that represents how the player's choice " +
            "affects sustainability (negative = harmful, positive = helpful). " +
            "Return your answer strictly in this JSON format (no extra text, no numbering in choices): " +
            "{ \"clientResponse\": \"...\", \"choices\": [\"choice1\", \"choice2\", \"choice3\", \"choice4\"], \"resourceImpact\": 0.0 }";


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

/*{
    "response": "Migrating to a region powered by renewable energy will significantly reduce your environmental impact by eliminating reliance on coal. This move will also align with your sustainability goals and enhance your company's reputation as a green technology company.",
    "choices": [
        "Invest in energy-efficient servers and data centers to reduce overall energy consumption.",
        "Implement virtualization technology to optimize server utilization and reduce the number of physical servers required.",
        "Utilize cloud services to leverage renewable energy sources and reduce the carbon footprint of your platform.",
        "Partner with green energy providers to ensure a consistent and sustainable energy source for your operations."
    ]
}*/