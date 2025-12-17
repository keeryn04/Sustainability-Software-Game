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

        string prompt = $"{systemPrompt}\n\nUser Message: {userMessage}";

        string apiUrl = "/api/get-data";

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