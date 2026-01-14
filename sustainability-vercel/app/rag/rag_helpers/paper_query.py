import os
from sentence_transformers import SentenceTransformer
from database_connector import get_db_connection

db = get_db_connection()

# Point HuggingFace and transformers caches to a temp location
os.environ["TRANSFORMERS_CACHE"] = "/tmp/transformers_cache"
os.environ["HF_HOME"] = "/tmp/huggingface_cache"

# Cache the model
_model = None
def get_model():
    global _model
    if _model is None:
        _model_path = "models/all-MiniLM-L6-v2"
        _model = SentenceTransformer(_model_path, device="cpu")
    return _model

# Query Supabase using vector embeddings
def query_papers(query, top_k=5):
    model = get_model()
    query_embedding = model.encode(query).tolist()

    response = db.rpc("match_paper_chunks", {
        "query_embedding": query_embedding,
        "match_count": top_k
    }).execute()

    results = []
    for row in response.data:
        results.append({"text": row['text'], "similarity": row["similarity"]})
    return results
