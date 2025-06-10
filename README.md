# Image Captioning with ASP.NET Core, FastAPI, and BLIP Model

## Project Overview

This project implements an AI-powered image captioning application using a microservices architecture. The frontend is built with ASP.NET Core, providing a user-friendly web interface for uploading images. The backend, a FastAPI service, handles the heavy lifting of generating captions using the state-of-the-art Salesforce BLIP (Bootstrapping Language-Image Pre-training) model. Both services are containerized using Docker and orchestrated with Docker Compose for easy deployment and management.

## Features

* **Image Upload:** Users can upload images via a simple web interface.
* **AI-Generated Captions:** Utilizes the BLIP model to generate two types of captions:
    * **Conditional Caption:** A caption generated based on a provided text prefix (e.g., "a photography of").
    * **Unconditional Caption:** A general caption describing the image without any specific prompt.
* **Microservices Architecture:** Separates concerns into distinct, scalable services.
* **Dockerized Deployment:** Easy setup and consistent environment using Docker.
* **Responsive Web Interface:** Built with ASP.NET Core MVC and Bootstrap.

## Technologies Used

* **Frontend:**
    * ASP.NET Core MVC ([C#](https://dotnet.microsoft.com/))
    * Bootstrap (for styling)
* **Backend (AI Service):**
    * [Python](https://www.python.org/)
    * [FastAPI](https://fastapi.tiangolo.com/) (Web framework)
    * [Transformers](https://huggingface.co/docs/transformers/index) by Hugging Face (for BLIP model)
    * [PyTorch](https://pytorch.org/) (Deep learning framework)
    * [Pillow (PIL Fork)](https://python-pillow.org/) (Image processing)
* **Containerization & Orchestration:**
    * [Docker](https://www.docker.com/)
    * [Docker Compose](https://docs.docker.com/compose/)

## Architecture

The project consists of two primary services:

1.  **`image-captioning-app` (ASP.NET Core):**
    * Serves the web UI.
    * Handles image uploads from the user.
    * Communicates with the `captioning-service` via HTTP to request captions.
    * Displays the generated captions and the uploaded image.

2.  **`captioning-service` (FastAPI):**
    * Hosts the BLIP model.
    * Receives image data from the `image-captioning-app`.
    * Processes the image using the BLIP model.
    * Returns conditional and unconditional captions as a JSON response.

Both services are connected via Docker's internal network, allowing them to communicate using their service names (e.g., `http://captioning-service:8000`).

## Setup and Installation

Follow these steps to get the project up and running on your local machine.

### Prerequisites

* [Git](https://git-scm.com/downloads) installed.
* [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running (includes Docker Engine and Docker Compose).

### 1. Clone the Repository

First, clone this GitHub repository to your local machine:

```bash
git clone [Your_GitHub_Repo_URL]
cd [Your_Repository_Name_Folder]
```

2. Download the BLIP Model
The BLIP model is quite large and should not be committed to Git. You need to download it separately and place it in the correct location.

Create a folder for the model:
Inside your cloned repository, navigate into the captioning-service directory:

Bash
```
cd captioning-service
mkdir blip_model
cd .. # Go back to the root of the project
```
This creates the blip_model directory inside your captioning-service folder.

Download the model files:
You need to download the "Salesforce/blip-image-captioning-base" model files from Hugging Face. There are two main ways:

Option A: Using Python (Recommended)
Create a temporary Python script (e.g., download_model.py) in the captioning-service directory with the following content:

Python
```
# download_model.py
from transformers import AutoProcessor, AutoModelForSeq2SeqLM
import os

model_name = "Salesforce/blip-image-captioning-base"
save_directory = "./blip_model"

if not os.path.exists(save_directory):
    os.makedirs(save_directory)

print(f"Downloading processor to {save_directory}...")
processor = AutoProcessor.from_pretrained(model_name)
processor.save_pretrained(save_directory)
print("Processor downloaded.")

print(f"Downloading model to {save_directory}...")
model = AutoModelForSeq2SeqLM.from_pretrained(model_name)
model.save_pretrained(save_directory)
print("Model downloaded.")

print("BLIP model and processor downloaded successfully!")
```
Then, run this script from inside the captioning-service folder:

Bash
```
cd captioning-service
pip install transformers torch
python download_model.py
cd .. # Go back to the root of the project
```
This will download all necessary files (like pytorch_model.bin, config.json, preprocessor_config.json, etc.) into the blip_model folder.

Option B: Manual Download (Advanced / Less Recommended)
Navigate to the Salesforce/blip-image-captioning-base page on Hugging Face. You would need to manually download all files listed under "Files and versions" (e.g., pytorch_model.bin, config.json, preprocessor_config.json, vocab.txt, tokenizer.json, etc.) and place them directly into the captioning-service/blip_model folder. Ensure you get all files, including .json, .bin, .txt, and .h5 files.

3. Build and Run with Docker Compose
From the root directory of your project (where docker-compose.yml is located), execute the following command:

Bash
```
docker-compose up --build
docker-compose up: Starts all services defined in docker-compose.yml.
```
--build: Forces Docker to rebuild the images for your services (important for reflecting code changes and ensuring the BLIP model is copied into the captioning-service image).
This command will:

Build the Docker images for both image-captioning-app and captioning-service.
Create and start the containers.
Map port 8080 on your host machine to the ASP.NET Core app, and port 8000 to the FastAPI service.
Usage
Once Docker Compose has finished setting up (you'll see messages like "Uvicorn running on [suspicious link removed]" and the ASP.NET Core app starting), open your web browser and navigate to:

http://localhost:8080/api/ImageCaption
Upload an Image: Use the "Select Image" button to choose an image file (JPG, PNG, GIF).
Generate Caption: Click the "Generate Caption" button.
View Results: The page will display the uploaded image along with the generated conditional and unconditional captions.
Troubleshooting
"Connection refused" or "Internal Server Error" on the web page:
Ensure all Docker containers are running (docker ps).
Check the logs of the captioning-service for startup errors:
Bash
```
docker logs -f imagecaptioning-captioning-service-1
```
Check the logs of the image-captioning-app for communication errors:
Bash
```
docker logs -f imagecaptioning-image-captioning-app-1
```
"No conditional caption generated." / "No unconditional caption generated." on the web page:
This means the C# app connected, but the FastAPI service returned empty/null captions.
Check captioning-service logs carefully for any Python errors during image processing or if the model failed to generate text. The logs should show Generated conditional caption: '...'.
Ensure the BLIP model files were downloaded correctly into the captioning-service/blip_model directory and that all necessary files are present.
Contributing
Contributions are welcome! If you find a bug or want to add a new feature, please:

Fork the repository.
Create a new branch (git checkout -b feature/your-feature-name).
Make your changes.
Commit your changes (git commit -m 'Add new feature').
Push to the branch (git push origin feature/your-feature-name).
Open a Pull Request.
License
This project is licensed under the MIT License. (It's a good practice to create a LICENSE file in your repository if you don't have one).

Acknowledgements
Salesforce BLIP for the fantastic image captioning model.
Hugging Face Transformers for providing easy-to-use NLP tools.
FastAPI for the modern and fast Python web framework.
ASP.NET Core for the robust web application framework.
Docker for simplifying deployment.
