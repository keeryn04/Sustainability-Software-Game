from flask import Flask, request, jsonify
import os
import requests

app = Flask(__name__)

OPENAI_API_KEY = os.getenv("OPENAI_API_KEY")

@app.route("/generate", methods=["POST"])
def generate():
    data = request.get_json()
    user_prompt = data.get("prompt", "")

    if not user_prompt:
        return jsonify({"error": "No prompt provided"}), 400

    headers = {
        "Authorization": f"Bearer {OPENAI_API_KEY}",
        "Content-Type": "application/json"
    }
    body = {
        "model": "gpt-4o-mini",
        "messages": [
            {"role": "system", "content": "You are a helpful sustainability client."},
            {"role": "user", "content": user_prompt}
        ],
        "max_tokens": 200
    }

    response = requests.post("https://api.openai.com/v1/chat/completions",
                             headers=headers, json=body)

    if response.status_code != 200:
        return jsonify({"error": response.text}), response.status_code

    output = response.json()["choices"][0]["message"]["content"]
    return jsonify({"response": output})

if __name__ == "__main__":
    app.run(host="0.0.0.0", port=5000, debug=True)
