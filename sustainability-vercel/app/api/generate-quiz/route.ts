import { NextResponse } from "next/server";

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
        { error: "No context available for quiz generation" },
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
        You are an educational quiz generator for a software sustainability game.

        Rules:
        - Use ONLY the provided context
        - Generate conceptual questions (not trivia)
        - Each question must have exactly 4 options
        - Only ONE option is correct
        - Include a brief explanation justifying the correct answer
        - Do NOT reference the context explicitly
        - Output ONLY valid JSON
        - No markdown, no extra text

        JSON format:
        {
        "questions": [
            {
            "question": string,
            "options": string[],
            "correctIndex": number,
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
        Generate ${numQuestions} quiz questions.
        If the context does not support ${numQuestions} questions, generate fewer.
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
    console.error("Quiz generation error:", err);

    return NextResponse.json(
      {
        error:
          err instanceof Error ? err.message : "Unknown server error",
      },
      { status: 500 }
    );
  }
}
