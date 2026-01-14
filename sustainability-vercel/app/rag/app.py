from flask_cors import CORS
from dotenv import load_dotenv
import requests
from flask import Flask, json, request, jsonify
from rag_helpers.paper_query import query_papers
import os

#Get env variables
load_dotenv()

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
CORS(app, origins=[APP_URL]) #Allows access from frontend

@app.route("/query", methods=["POST"])
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
        contexts = query_papers(query, top_k)
        context_text = "\n".join(f"({i+1}) {c.page_content}" for i, c in enumerate(contexts))

        return jsonify({"contexts": context_text})

    except Exception as e:
        print("Context Generation error:", e)
        return jsonify({"error": str(e)}), 500
