from flask_cors import CORS
from dotenv import load_dotenv
import requests
from waitress import serve
from flask import Flask, json, request, jsonify
from vector_search import query_papers
import os

#Get env variables
load_dotenv()

#Set up app with Flask
app = Flask(__name__)

API_KEY = os.getenv('RAG_API_KEY')
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")

#Configure Flask session for HTTPS hosting
app.config["SESSION_TYPE"] = "filesystem"
app.config["SESSION_COOKIE_SAMESITE"] = None  #Allows cross-site cookies
app.config["SESSION_COOKIE_SECURE"] = True  #Only over HTTPS
CORS(app) #Allows access from frontend

@app.route("/", methods=["GET"])
def base():
    return jsonify({"success": "Backend Active!"}), 200

@app.route("/api/generate-quiz", methods=["POST"])
def generate_quiz():
    try:
        auth = request.headers.get("Authorization")
        if API_KEY and auth != f"Bearer {API_KEY}":
            return jsonify({"error": "Unauthorized"}), 401
    
        payload = request.get_json()
        query = payload.get("query")
        num_questions = payload.get("numQuestions", 5)

        if not query:
            return jsonify({"error": "Missing query"}), 400

        #Get RAG context
        contexts = query_papers("Software sustainability", top_k=5)
        context_text = "\n".join(f"({i+1}) {c.page_content}" for i, c in enumerate(contexts))

        #Build prompt messages
        system_message = f"""
        You are an educational quiz generator for a software sustainability game.

        Rules:
        - Base questions and answers on the provided context
        - Generate conceptual questions (not trivia)
        - Each question must have exactly 4 options
        - Only ONE option is correct
        - Include a brief explanation justifying the correct answer
        - Do NOT reference the context explicitly
        - Output ONLY valid JSON
        - No markdown, no extra text

        JSON format:
        {{
            "questions": [
                {{
                    "question": string,
                    "options": string[],
                    "correctIndex": number,
                    "explanation": string
                }}
            ]
        }}

        Context:
        {context_text}
        """.strip()

        user_message = f"""
        Generate {num_questions} quiz questions.
        """.strip()

        #Call OpenAI
        headers = {
            "Content-Type": "application/json",
            "Authorization": f"Bearer {OPENAI_API_KEY}",
        }

        openai_body = {
            "model": "gpt-4o-mini",
            "messages": [
                {"role": "system", "content": system_message},
                {"role": "user", "content": user_message},
            ],
            "temperature": 0.2,
        }

        openai_response = requests.post(
            "https://api.openai.com/v1/chat/completions",
            headers=headers,
            json=openai_body
        )

        if not openai_response.ok:
            raise Exception(f"OpenAI failed: {openai_response.status_code}")

        openai_data = openai_response.json()
        quiz_json_text = openai_data["choices"][0]["message"]["content"]

        try:
            quiz = json.loads(quiz_json_text)
        except Exception:
            raise Exception("LLM returned invalid JSON")

        return jsonify({"quiz": quiz})

    except Exception as e:
        print("Quiz generation error:", e)
        return jsonify({"error": str(e)}), 500

if __name__ == '__main__':
    serve(app, host="127.0.0.1", port=5000, threads=4)
