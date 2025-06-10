# caption_server.py
from fastapi import FastAPI, UploadFile, File
from PIL import Image
from transformers import BlipProcessor, BlipForConditionalGeneration
import io
import os
import traceback # Add this import

app = FastAPI()

# Specify the local path where you downloaded the model
local_model_path = "./blip_model"

# --- MODEL LOADING (KEEP THIS AS IS) ---
print(f"Attempting to load model from: {os.path.abspath(local_model_path)}")
if not os.path.exists(local_model_path):
    print(f"ERROR: Model directory NOT FOUND at {os.path.abspath(local_model_path)}")
else:
    print(f"Model directory found at {os.path.abspath(local_model_path)}")
    print(f"Contents: {os.listdir(local_model_path)}")

try:
    processor = BlipProcessor.from_pretrained(local_model_path, local_files_only=True)
    model = BlipForConditionalGeneration.from_pretrained(local_model_path, local_files_only=True)
    print("BLIP model and processor loaded successfully.")
except Exception as e:
    print(f"ERROR: Failed to load BLIP model or processor: {e}")
    traceback.print_exc()
    # If model loading fails, the app will likely not function correctly.
    # Consider raising the exception or exiting if critical for startup.
    # For now, let's just print to logs.
# --- END MODEL LOADING ---


@app.post("/caption")
async def generate_caption(image: UploadFile = File(...)):
    print(f"Received image: {image.filename} (Type: {image.content_type}, Size: {image.size} bytes)") # Add this print
    try:
        image_bytes = await image.read()
        raw_image = Image.open(io.BytesIO(image_bytes)).convert("RGB")
        print("Image successfully opened with PIL.") # Add this print

        # Conditional image captioning
        text = "a photography of"
        inputs = processor(raw_image, text, return_tensors="pt")
        out = model.generate(**inputs)
        caption_conditional = processor.decode(out[0], skip_special_tokens=True)
        print(f"Generated conditional caption: '{caption_conditional}'") # Add this print

        # Unconditional image captioning
        inputs = processor(raw_image, return_tensors="pt")
        out = model.generate(**inputs)
        caption_unconditional = processor.decode(out[0], skip_special_tokens=True)
        print(f"Generated unconditional caption: '{caption_unconditional}'") # Add this print

        return {
            "caption_conditional": caption_conditional,
            "caption_unconditional": caption_unconditional
        }
    except Exception as e:
        print(f"ERROR during caption generation: {e}") # Add this print
        traceback.print_exc() # Print full stack trace for runtime errors
        # If an error occurs, you might want to return an appropriate error response
        # to the client instead of raising an internal server error.
        # For now, let's return nulls to match the current behavior and see the logs.
        return {
            "caption_conditional": None,
            "caption_unconditional": None
        }