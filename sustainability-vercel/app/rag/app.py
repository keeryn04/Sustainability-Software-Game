from flask_cors import CORS
from dotenv import load_dotenv
from waitress import serve
from flask import Flask, request, jsonify
from vector_search import query_papers
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
    auth = request.headers.get("Authorization")
    if API_KEY and auth != f"Bearer {API_KEY}":
        return jsonify({"error": "Unauthorized"}), 401

    data = request.json
    query_text = data.get("query")
    top_k = data.get("top_k", 5)

    if not query_text:
        return jsonify({"error": "Missing query"}), 400

    results = query_papers(query_text, top_k=top_k)

    return jsonify({
        "contexts": [
            {
                "text": doc.page_content,
                "similarity": doc.metadata["similarity"]
            }
            for doc in results
        ]
    })

if __name__ == '__main__':
    serve(app, host="0.0.0.0", port=5000, threads=6, debug=True, timeout=120)
