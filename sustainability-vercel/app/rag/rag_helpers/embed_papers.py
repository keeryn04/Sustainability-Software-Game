import os
import re
import nltk
from sentence_transformers import SentenceTransformer
from langchain_community.document_loaders import UnstructuredPDFLoader
from langchain.text_splitter import SpacyTextSplitter
from database_connector import get_db_connection

nltk.download('punkt')
nltk.download('punkt_tab')
nltk.download('averaged_perceptron_tagger_eng')

db = get_db_connection()
MODEL_PATH = "models/all-MiniLM-L6-v2"
model = SentenceTransformer(MODEL_PATH, device="cpu")

#Clean unwanted text from research papers
def clean_text(text):
    text = re.sub(r"Authorized licensed use.*?Restrictions apply\.", "", text, flags=re.DOTALL) #Remove license watermark
    text = re.sub(r"\[\d+\]\s?.*?(\n|$)", "", text) #Remove inline references
    text = re.sub(r"\s+", " ", text) #Normalize whitespace
    return text.strip()

#Load PDFs, split them into chunks, and clean each chunk
def get_chunks_for_embedding(pdf_folder="papers", chunk_size=600, chunk_overlap=150):
    docs = []
    #Load each PDF in the folder
    for filename in os.listdir(pdf_folder):
        if filename.endswith(".pdf"):
            loader = UnstructuredPDFLoader(os.path.join(pdf_folder, filename), strategy="fast")
            docs.extend(loader.load())

    #Use SpacyTextSplitter to split documents into overlapping chunks
    splitter = SpacyTextSplitter(chunk_size=chunk_size, chunk_overlap=chunk_overlap)
    chunks = splitter.split_documents(docs)

    #Clean each chunk's content
    cleaned_chunks = [clean_text(chunk.page_content) for chunk in chunks]
    metadata = [chunk.metadata for chunk in chunks]

    return cleaned_chunks, metadata

#Upload chunks and their vector embeddings to Supabase
def upload_chunks_to_supabase(texts, metadata):
    for i, text in enumerate(texts):
        embedding = model.encode(text).tolist()
        db.table("paper_chunks").insert({
            "text": text,
            "embedding": embedding
        }).execute()

#Build the vector index by processing PDFs and uploading them
def build_supabase_index(pdf_folder="papers"):
    texts, metadata = get_chunks_for_embedding(pdf_folder)
    upload_chunks_to_supabase(texts, metadata)

#Run indexing pipeline when new papers pushed to GitHub
if __name__ == "__main__":
    build_supabase_index(pdf_folder="papers")