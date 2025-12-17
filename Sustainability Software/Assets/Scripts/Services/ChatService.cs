using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class ChatService
{
    public static async Task<string> SendChatAsync(
        GameStage mode,
        string userMessage,
        string context)
    {
        string baseContext = @"
        Software sustainability involves designing, developing, and maintaining software
        in ways that minimize negative environmental, economic, technical and social impacts.
        Focus on efficient code, maintainable architecture, long-term resource use,
        and ethical considerations in software development.";

        //Use the provided context if available, otherwise use the base context
        string effectiveContext = string.IsNullOrWhiteSpace(context) ? baseContext : context;

        //Build the system prompt based on mode
        string systemPrompt = mode switch
        {
            GameStage.Learning =>
                $"You are a software sustainability tutor. Base answers ONLY on the following learning context:\n\n{effectiveContext}\n\n" +
                "Keep explanations simple, correct, and educational. Keep your answer 1-3 sentences. " +
                "Do not answer questions unrelated to software sustainability. If a question is off-topic, politely remind the user that only software sustainability questions can be answered.",

            GameStage.Reflection =>
                $"You are guiding the player through reflective analysis. Use ONLY the reflection context below:\n\nReflection Context:\n{effectiveContext}\n\n" +
                "Your goal is to help the player understand the impact of their previous choices. Provide thoughtful, concise insights in 1-3 sentences. " +
                "Do not go off-topic or discuss unrelated subjects. If a question is unrelated, remind the player to focus on software sustainability.",

            _ =>
                $"You are a stakeholder in a software sustainability meeting. Stay fully in character. " +
                $"Use ONLY the client brief:\n\nClient Brief:\n{effectiveContext}\n\n" +
                "Respond realistically, keeping your answer 1-3 sentences. " +
                "Do not discuss anything outside of software sustainability. Politely decline unrelated questions."
        };

        //Build the message list
        List<ChatMessage> messages = new List<ChatMessage>();

        messages.Add(new ChatMessage { role = "system", content = systemPrompt });

        messages.Add(new ChatMessage { role = "user", content = userMessage });

        //Convert to request
        ChatRequest chatRequest = new ChatRequest
        {
            model = "gpt-3.5-turbo",
            messages = messages.ToArray(),
            max_tokens = 300
        };

        string jsonBody = JsonUtility.ToJson(chatRequest);

        //Send to OpenAI API
        using (UnityWebRequest www =
               new UnityWebRequest("https://api.openai.com/v1/chat/completions", "POST"))
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

                try
                {
                    var response = JsonUtility.FromJson<OpenAIResponse>(rawJson);
                    return response.choices[0].message.content;
                }
                catch
                {
                    Debug.LogError("ChatService JSON parse error: " + rawJson);
                    return "Error: Invalid response format.";
                }
            }
            else
            {
                Debug.LogError($"ChatService error: {www.responseCode} - {www.error}");
                Debug.LogError("Response body: " + www.downloadHandler.text);
                return "Error: Chat request failed.";
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

    //Request classes
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
