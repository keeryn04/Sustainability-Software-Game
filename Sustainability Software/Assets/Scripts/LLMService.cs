using System.Threading.Tasks;
using Unity.VisualScripting;

public static class LLMService
{
    private static string apiKey = "OPENAI_KEY"; //Still need to fix

    public static async Task<string> SendChoiceAsync(ScenarioData scenario, ChoiceData choice)
    {
        string prompt = $@"
        You are acting as a client in a sustainable software simulation.
        Scenario: {scenario.clientBrief}
        Player choice: {choice.choiceText}

        Respond as the client would in a professional but realistic tone.
        Highlight trade-offs (performance, cost, emissions, etc).
        End with a follow-up question if appropriate.
        ";

        // Example pseudo-call
        var response = await OpenAI.ChatAsync(prompt, apiKey);
        return response.Text;
    }
}
