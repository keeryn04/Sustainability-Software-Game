export async function POST(request: Request) {
  try {
    const { query, numQuestions = 5 } = await request.json();

    //RAG context from AWS
    const ragResponse = await fetch(process.env.RAG_API_URL!, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${process.env.RAG_API_KEY}`,
      },
      body: JSON.stringify({
        query,
        top_k: 5,
      }),
    });

    if (!ragResponse.ok) {
      throw new Error(`RAG service failed: ${ragResponse.status}`);
    }

    const ragData = await ragResponse.json();

    if (!ragData.contexts || ragData.contexts.length === 0) {
      return NextResponse.json(
        { error: "No context available for challenge generation" },
        { status: 400 }
      );
    }

    const contextText = ragData.contexts
      .map((c: any, i: number) => `(${i + 1}) ${c.text}`)
      .join("\n");

    //Quiz prompt
    const messages = [
      {
        role: "system",
        content: `
        You are an educational challenge generator for a software sustainability game.

        Your role is to generate boss-style challenges that test strategic understanding of sustainability pillars.

        Developers and their pillars:
        - environmental: focuses on environmental sustainability
        - social: focuses on social sustainability
        - economic: focuses on economic sustainability
        - technical: focuses on software and technical sustainability

        Rules:
        - Use ONLY the provided context
        - Generate conceptual, scenario-based boss questions (not trivia)
        - Each question represents a challenge posed by a boss (Use first person)
        - For each question, generate EXACTLY TWO strategic response options
        - Strategies should represent different ways of responding to the boss challenge
        - The player must choose:
        1) ONE developer
        2) ONE strategy (A or B)
        - Only ONE developer + strategy combination is correct
        - Include a brief explanation justifying why the correct choice works
        - Do NOT reference the context explicitly
        - Output ONLY valid JSON
        - No markdown, no extra text

        JSON format:
        {
        "questions": [
            {
            "bossQuestion": string,
            "strategies": [
                { "id": "A", "description": string },
                { "id": "B", "description": string }
            ],
            "correctDeveloper": "environmental" | "social" | "economic" | "technical",
            "correctStrategyId": "A" | "B",
            "explanation": string
            }
        ]
        }

        Context:
        ${contextText}
            `.trim(),
        },
        {
            role: "user",
            content: `
        Generate ${numQuestions} boss challenges.
        If the context does not support ${numQuestions} challenges, generate fewer.
            `.trim(),
        },
      ];

    //OpenAI
    const openaiResponse = await fetch(
      "https://api.openai.com/v1/chat/completions",
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${process.env.OPENAI_API_KEY}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          model: "gpt-4o-mini",
          messages,
          temperature: 0.2,
        }),
      }
    );

    if (!openaiResponse.ok) {
      throw new Error(`OpenAI failed: ${openaiResponse.status}`);
    }

    const openaiData = await openaiResponse.json();
    const quizJsonText = openaiData.choices[0].message.content;

    //Parse quiz response
    let quiz;
    try {
      quiz = JSON.parse(quizJsonText);
    } catch {
      throw new Error("LLM returned invalid JSON");
    }

    return NextResponse.json({ quiz });
  } catch (err: unknown) {
    console.error("Chalenge generation error:", err);

    return NextResponse.json(
      {
        error:
          err instanceof Error ? err.message : "Unknown server error",
      },
      { status: 500 }
    );
  }
}