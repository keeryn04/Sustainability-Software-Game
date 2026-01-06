import { NextResponse } from "next/server";

export async function POST(request: Request) {
  try {
    const { prompt } = await request.json();

    if (!prompt) {
      return NextResponse.json(
        { error: "Missing prompt" },
        { status: 400 }
      );
    }

    if (!process.env.OPENAI_API_KEY) {
        return NextResponse.json(
            { error: "OPENAI_API_KEY is missing" },
            { status: 500 }
        );
    }

    const response = await fetch(
      "https://api.openai.com/v1/chat/completions",
      {
        method: "POST",
        headers: {
          "Authorization": `Bearer ${process.env.OPENAI_API_KEY}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          model: "gpt-4o-mini",
          messages: [{ role: "user", content: prompt }],
        }),
      }
    );

    if (!response.ok) {
      throw new Error(`OpenAI error: ${response.status}`);
    }

    const data = await response.json();

    return NextResponse.json({
      text: data.choices[0].message.content,
    });
  } catch (err: unknown) {
    console.error(err);

    const message =
    err instanceof Error ? err.message : String(err);

    return NextResponse.json(
        { error: message },
        { status: 500 }
    );
  }
}