from flask_cors import CORS
from dotenv import load_dotenv
from flask import Flask, request, jsonify
from rag_helpers.paper_query import query_papers
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
CORS(app, origins=[APP_URL])

@app.route("/", methods=["GET"])
def root():
    return {"status": "ok"}

@app.route("/", methods=["GET"])
def test():
    return jsonify({"status": "All Good!"})

@app.route("/api/generate-test", methods=["POST"])
def query():
    try:
        auth = request.headers.get("Authorization")
        if API_KEY and auth != f"Bearer {API_KEY}":
            return jsonify({"error": "Unauthorized"}), 401

        data = request.json
        query_text = data.get("query")
        top_k = data.get("top_k", 5)

        if not query_text:
            return jsonify({"error": "Missing query"}), 400

        #Get RAG context
        contexts = query_papers(query_text, top_k)

        return jsonify({"contexts": contexts})

    except Exception as e:
        print("Context Generation error:", e)
        return jsonify({"error": str(e)}), 500
