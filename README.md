
# Image Captioning with ASP.NET Core, FastAPI, and BLIP Model

## Project Overview

This project implements an AI-powered image captioning application using a microservices architecture. The **ASP.NET Core** frontend provides a user-friendly web interface for uploading images. The backend, a **FastAPI** service, handles the heavy lifting of generating captions using the state-of-the-art Salesforce **BLIP** (Bootstrapping Language-Image Pre-training) model. Both services are containerized using **Docker** and orchestrated with **Docker Compose** for easy deployment and management.

-----

## Features

  * **Image Upload:** Users can upload images via a simple web interface.
  * **AI-Generated Captions:** Utilizes the BLIP model to generate two types of captions:
      * **Conditional Caption:** A caption generated based on a provided text prefix (e.g., "a photography of").
      * **Unconditional Caption:** A general caption describing the image without any specific prompt.
  * **Microservices Architecture:** Separates concerns into distinct, scalable services.
  * **Dockerized Deployment:** Ensures easy setup and a consistent environment across different machines.
  * **Offline Dependency Management:** Uses pre-downloaded model files and Python wheel packages for faster and more reliable Docker builds.
  * **Responsive Web Interface:** Built with ASP.NET Core MVC and Bootstrap.

-----

## Architecture

The project consists of two primary services that communicate over Docker's internal network:

1.  **`image-captioning-app` (ASP.NET Core):**

      * Serves the web user interface.
      * Handles image uploads from the user.
      * Communicates with the `captioning-service` via HTTP (at `http://captioning-service:8000`) to request captions.
      * Displays the generated captions and the uploaded image.

2.  **`captioning-service` (FastAPI):**

      * Hosts the BLIP model.
      * Receives image data from the `image-captioning-app`.
      * Processes the image using the BLIP model.
      * Returns conditional and unconditional captions as a JSON response.

-----

## Setup and Installation

Follow these steps to get the project up and running on your local machine.

### Prerequisites

  * [Git](https://git-scm.com/downloads) installed.
  * [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running (includes Docker Engine and Docker Compose).
  * [Python 3.x](https://www.python.org/downloads/) and [pip](https://pip.pypa.io/en/stable/installation/) installed.
  * [Jupyter Notebook](https://jupyter.org/install) installed (or [Anaconda](https://www.anaconda.com/products/distribution) for easier setup) to run the `download.ipynb`.

### 1\. Clone the Repository

First, clone this GitHub repository to your local machine:

```bash
git clone https://github.com/hkhajei/Image-Captioning-Project.git # Replace with your actual repo URL
cd Image-Captioning-Project # Replace with your actual repository name
```

### 2\. Download BLIP Model Files

The BLIP model is large and is not included in the Git repository. You'll use a Jupyter Notebook to download it.

1.  **Navigate to the root of your project.**
2.  **Run the Jupyter Notebook:**
    ```bash
    jupyter notebook download.ipynb
    ```
    Execute all cells in the notebook. This will download the BLIP model files from Hugging Face and save them into the `captioning-service/blip_model` directory. Ensure this directory is created and populated with files like `pytorch_model.bin`, `config.json`, `preprocessor_config.json`, etc.

### 3\. Prepare Python Dependencies (Local Wheels)

You've pre-downloaded Python package `.whl` files to ensure faster and more consistent Docker builds for the FastAPI service.

1.  **Navigate to the `captioning-service` directory:**
    ```bash
    cd captioning-service
    ```
2.  **Ensure `local_wheels` directory is populated:**
    Confirm that all required Python package `.whl` files (as specified by your `requirements.txt` for the FastAPI service) are downloaded and placed into the `captioning-service/local_wheels` directory. You likely generated these using a command like:
    ```bash
    pip download -r requirements.txt -d local_wheels
    ```
3.  **Go back to the root of your project:**
    ```bash
    cd ..
    ```

### 4\. Build and Run with Docker Compose

From the root directory of your project (where `docker-compose.yml` is located), execute the following command:

```bash
docker-compose up --build
```

This command will:

  * Build the Docker images for both `image-captioning-app` and `captioning-service`, ensuring the BLIP model and local wheels are copied into the `captioning-service` image.
  * Create and start the containers.
  * Map port `8080` on your host machine to the ASP.NET Core app (container port 80), and port `8000` to the FastAPI service.

-----

## Usage

Once Docker Compose has finished setting up (you'll see messages like "Uvicorn running on [suspicious link removed]" and the ASP.NET Core app starting), open your web browser and navigate to:

```
http://localhost:8080/api/ImageCaption
```

1.  **Upload an Image:** Use the "Select Image" button to choose an image file (JPG, PNG, GIF).
2.  **Generate Caption:** Click the "Generate Caption" button.
3.  **View Results:** The page will display the uploaded image along with the generated conditional and unconditional captions.

-----

## Troubleshooting

  * **"Connection refused" or "Internal Server Error" on the web page:**
      * Ensure all Docker containers are running correctly: `docker ps`.
      * Check the logs of the `captioning-service` for startup errors: `docker logs -f imagecaptioning-captioning-service-1`.
      * Check the logs of the `image-captioning-app` for communication errors: `docker logs -f imagecaptioning-image-captioning-app-1`.
  * **"No conditional caption generated." / "No unconditional caption generated." on the web page:**
      * This indicates the C\# app connected successfully, but the FastAPI service returned empty or null captions.
      * **Crucially, check the `captioning-service` logs carefully** (`docker logs -f imagecaptioning-captioning-service-1`). You should see lines like "Generated conditional caption: '...'" confirming the model is producing output. If not, there's a problem with the Python model's execution.
      * Ensure the BLIP model files were downloaded correctly into `captioning-service/blip_model` and are complete.
  * **`Console.WriteLine` or `_logger.LogInformation` not showing in ASP.NET Core app logs:**
      * Make sure you are checking the logs for the correct container: `docker logs -f imagecaptioning-image-captioning-app-1`.
      * Confirm your ASP.NET Core app is configured for logging to console in `appsettings.Development.json` (e.g., `"LogLevel": { "Default": "Information" }`).

-----

## Contributing

Contributions are welcome\! If you find a bug, have an idea for an improvement, or want to add a new feature, please feel free to:

1.  Fork the repository.
2.  Create a new branch (`git checkout -b feature/your-feature-name`).
3.  Make your changes.
4.  Commit your changes (`git commit -m 'Add new feature'`).
5.  Push to the branch (`git push origin feature/your-feature-name`).
6.  Open a Pull Request.

-----

## License

This project is licensed under the [MIT License](https://www.google.com/search?q=LICENSE). *(It's a good practice to create a `LICENSE` file in your repository if you don't have one, detailing the terms of use.)*

-----

## Acknowledgements

  * [Salesforce BLIP](https://huggingface.co/Salesforce/blip-image-captioning-base) for the fantastic image captioning model.
  * [Hugging Face Transformers](https://huggingface.co/docs/transformers/index) for providing easy-to-use NLP tools.
  * [FastAPI](https://fastapi.tiangolo.com/) for the modern and fast Python web framework.
  * [ASP.NET Core](https://dotnet.microsoft.com/) for the robust web application framework.
  * [Docker](https://www.docker.com/) for simplifying deployment.

-----
