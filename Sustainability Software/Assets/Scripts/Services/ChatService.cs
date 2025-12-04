using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public static class ChatService
{
    public static async Task<string> SendChatAsync(
        GameStage mode,
        string userMessage,
        string context)
    {
        //Build the system prompt based on mode
        string systemPrompt = mode == GameStage.Learning
            ? $"You are a software sustainability tutor. Base answers off the following learning context:\n\n{context}\n\nKeep explanations simple, correct, and educational. Keep your answer 1-3 sentences."
            : $"You are a stakeholder in a software sustainability meeting. Stay fully in character.\nUse ONLY the client brief:\n\nClient Brief:\n{context}\n\n Respond realistically. Keep your answer 1-3 sentences.";

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
