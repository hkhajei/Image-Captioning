# captioning-service/Dockerfile
FROM python:3.9-slim-buster

# Install essential build tools and system dependencies required by many Python packages,
# especially those with C/C++ extensions like PyTorch.
# We use --no-install-recommends to keep the image as small as possible.
RUN apt-get update && \
    apt-get install -y --no-install-recommends \
    build-essential \
    libglib2.0-0 \
    && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy all pre-downloaded .whl files from the local_wheels sub-directory
COPY local_wheels/*.whl ./

# Copy the locally downloaded BLIP model directory
COPY blip_model ./blip_model

COPY caption_server.py .

# This line should now install the new torch wheel and other dependencies
RUN pip install --no-cache-dir *.whl --find-links . --no-index

EXPOSE 8000
# --- IMPORTANT CHANGE HERE ---
# Use uvicorn to run your FastAPI application, listening on all interfaces (0.0.0.0)
# and on port 8000. 'caption_server:app' refers to the 'app' object in 'caption_server.py'
CMD ["uvicorn", "caption_server:app", "--host", "0.0.0.0", "--port", "8000"]