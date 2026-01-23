from flask_cors import CORS
from dotenv import load_dotenv
import requests
from waitress import serve
from flask import Flask, json, request, jsonify
from supabase import create_client
from vector_search import query_papers
import os

#Get env variables
load_dotenv()
SUPABASE_URL = os.getenv("SUPABASE_URI")
SUPABASE_SERVICE_KEY = os.getenv("SUPABASE_SERVICE_KEY")
OPENAI_KEY = os.getenv("OPENAI_API_KEY")
OPENAI_URL = "https://api.openai.com/v1/chat/completions"

if not SUPABASE_URL or not SUPABASE_SERVICE_KEY:
    raise ValueError("Missing Supabase connection info")

supabase = create_client(SUPABASE_URL, SUPABASE_SERVICE_KEY)

#Set up app with Flask
app = Flask(__name__)

FLASK_SECRET_KEY = os.getenv("FLASK_SECRET_KEY")
APP_URL = os.getenv('APP_URL')
API_KEY = os.getenv('RAG_API_KEY')

#Configure Flask session for HTTPS hosting
app.config["SESSION_TYPE"] = "filesystem"
app.config["SESSION_COOKIE_SAMESITE"] = None  #Allows cross-site cookies
app.config["SESSION_COOKIE_SECURE"] = True  #Only over HTTPS
app.secret_key = FLASK_SECRET_KEY
CORS(app) #Allows access from frontend

@app.route("/", methods=["GET"])
def test():
    return jsonify({"status": "All Good!"})

@app.route("/api/generate-quiz", methods=["POST"])
def query():
    data = request.json
    query_text = data.get("query")
    top_k = data.get("top_k", 5)
    num_questions = data.get("numQuestions", 5)

    if not query_text:
        return jsonify({"error": "Missing query"}), 400

    docs = query_papers(query_text, top_k=top_k)
    if not docs:
        return jsonify({"error": "No context found"}), 404

    context_text = "\n".join([f"({i+1}) {d.page_content}" for i, d in enumerate(docs)])

    messages = [
        {
            "role": "system",
            "content": f"""
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
        },
        {
            "role": "user",
            "content": f"Generate {num_questions} quiz questions. If context is insufficient, generate fewer."
        }
    ]

    headers = {"Authorization": f"Bearer {OPENAI_KEY}", "Content-Type": "application/json"}
    payload = {"model": "gpt-4o-mini", "messages": messages, "temperature": 0.2}
    response = requests.post(OPENAI_URL, json=payload, headers=headers)

    if not response.ok:
        return jsonify({"error": f"OpenAI failed: {response.status_code}"}), 500

    try:
        quiz_json_text = response.json()["choices"][0]["message"]["content"]
        quiz = json.loads(quiz_json_text)
    except Exception as e:
        return jsonify({"error": f"Failed to parse LLM output: {str(e)}"}), 500

    return jsonify({"quiz": quiz})

@app.route("/api/generate-challenge", methods=["POST"])
def challenge():
    data = request.json
    query_text = data.get("query")
    top_k = data.get("top_k", 5)
    num_questions = data.get("numQuestions", 5)

    if not query_text:
        return jsonify({"error": "Missing query"}), 400

    docs = query_papers(query_text, top_k=top_k)
    if not docs:
        return jsonify({"error": "No context found"}), 404

    context_text = "\n".join([f"({i+1}) {d.page_content}" for i, d in enumerate(docs)])

    system_prompt = """
    You are an educational challenge generator for a software sustainability game.

    Your role is to generate boss-style challenges that test strategic understanding of sustainability pillars.

    Developers and their pillars:
    - Environmental: focuses on environmental sustainability
    - Social: focuses on social sustainability
    - Economic: focuses on economic sustainability
    - Technical: focuses on software and technical sustainability

    Rules:
    - Use ONLY the provided context
    - Generate conceptual, scenario-based boss questions (not trivia)
    - Each question represents a challenge posed by a boss (Use first person)
    - For each question, generate EXACTLY TWO response options
    - The player must choose:
    1) ONE developer
    2) ONE strategy (Attack or Defend)
    - Only ONE developer + strategy combination is correct
    - Ensure the question focuses on one developer type
    - Include a brief explanation justifying the correct choice
    - Do NOT reference the context explicitly
    - Output ONLY valid JSON
    - No markdown, no extra text

    JSON format:
    {
    "questions": [
        {
        "bossQuestion": string,
        "strategies": [
            { "id": "Attack", "description": string },
            { "id": "Defend", "description": string }
        ],
        "correctDeveloper": "environmental" | "social" | "economic" | "technical",
        "correctStrategyId": "Attack" | "Defend",
        "explanation": string
        }
    ]
    }

    Context:
    {context}
    """.strip()

    system_content = system_prompt.replace("{context}", context_text)

    messages = [
        {
            "role": "system",
            "content": system_content
        },
        {
            "role": "user",
            "content": f"Generate {num_questions} boss challenges. If context is insufficient, generate fewer."
        }
    ]

    headers = {"Authorization": f"Bearer {OPENAI_KEY}", "Content-Type": "application/json"}
    payload = {"model": "gpt-4o-mini", "messages": messages, "temperature": 0.2}
    response = requests.post(OPENAI_URL, json=payload, headers=headers)

    if not response.ok:
        return jsonify({"error": f"OpenAI failed: {response.status_code}"}), 500

    try:
        quiz_json_text = response.json()["choices"][0]["message"]["content"]
        quiz = json.loads(quiz_json_text)
    except Exception as e:
        return jsonify({"error": f"Failed to parse LLM output: {str(e)}"}), 500

    return jsonify({"quiz": quiz})

if __name__ == "__main__":
    from waitress import serve
    serve(app, host="127.0.0.1", port=5000, threads=4)
