import os
from sentence_transformers import SentenceTransformer
from database_connector import get_db_connection

db = get_db_connection()

# Point HuggingFace and transformers caches to a temp location
os.environ["TRANSFORMERS_CACHE"] = "/tmp/transformers_cache"
os.environ["HF_HOME"] = "/tmp/huggingface_cache"

# Cache the model
MODEL_PATH = "models/all-MiniLM-L6-v2"
model = SentenceTransformer(MODEL_PATH, device="cpu")

# Query Supabase using vector embeddings
def query_papers(query, top_k=5):
    query_embedding = model.encode(query).tolist()

    response = db.rpc("match_paper_chunks", {
        "query_embedding": query_embedding,
        "match_count": top_k
    }).execute()

    results = []
    for row in response.data:
        results.append({"text": row['text'], "similarity": row["similarity"]})
    return results
