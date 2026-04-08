Overview
This repository contains a full‑stack application built with a .NET backend and an Angular frontend. 
The system integrates multiple AI capabilities, including chat, embeddings, retrieval‑augmented generation, image generation, speech‑to‑text, and text‑to‑speech. 
The backend exposes REST endpoints, while the frontend provides a unified interface for interacting with these services.

The project is organized into two main folders:
/API     → .NET backend
/APP     → Angular frontend

Both applications run independently but communicate through HTTP.

Backend (.NET 10 API)
The backend is located under:
API/src/

It follows a layered architecture:
AiWorkbench.Api            → API layer (controllers, DI, configuration)
AiWorkbench.Application    → business logic and service layer
AiWorkbench.Domain         → domain entities and value objects
AiWorkbench.Infrastructure → EF Core, Azure Blob Storage, external AI providers

Technologies Used:
.NET 10
Entity Framework Core 10
SQL Server
Azure Blob Storage
Azure OpenAI (Chat, Embeddings, Images, Audio)
Stability AI (Image generation)
FluentValidation
JWT Authentication
Scalar for API documentation

Entity Framework Core
The AiWorkbench.Infrastructure project contains:
  AiWorkbenchDbContext
  Entity configurations implementing IEntityTypeConfiguration<T>
  Migrations folder
  Repository and data access logic

To add a migration:
  cd API/src/AiWorkbench.Api
  dotnet ef migrations add MigrationName

To apply migrations:
  dotnet ef database update  

The connection string is configured in appsettings.json under:
  "ConnectionStrings": {
    "DefaultConnection": "..."
  }

External Services Configuration
All external API keys and endpoints are stored in:
  API/src/AiWorkbench.Api/appsettings.json

The structure is:
  "ExternalServices": {
    "AzureOpenAI": {
      "ApiKey": "",
      "Endpoint": "",
      "ChatDeployment": "",
      "EmbeddingDeployment": "",
      "ImageDeployment": "",
      "AudioDeployment": {
        "AudioTranscriptionDeployment": "",
        "AudioSpeechDeployment": ""
      },
      "ChatModeSystemMessage": ""
    },
    "AzureBlobStorage": {
      "ConnectionString": "",
      "ContainerName": ""
    },
    "RagService": {
      "ApiKey": ""
    }
  }

How to Obtain API Keys

Azure OpenAI:
  Open Azure Portal.
  Navigate to your Azure OpenAI resource.
  Go to Keys and Endpoint.
  Copy the key and endpoint.
  Paste them into appsettings.json under ExternalServices.AzureOpenAI.

Stability AI: 
  Go to https://ai.azure.com/resource/deployments (selecting your resource)
  Open your account settings.
  Copy your API key.
  Add it to appsettings.json under ExternalServices.StabilityAI.

Keys should never be committed to Git.
The .gitignore file excludes all development configuration files.

Frontend (Angular)
The frontend is located under:

APP/

The structure includes:
  src/app/
      auth/
      chat/
          chat-text/
          chat-images/
          chat-embeddings/
          chat-speech/
          shell-layout/
      guards/
      interceptors/
      interfaces/
      services/

Key Features
  Authentication (login, register)
  Chat with streaming responses
  Image generation (Azure OpenAI and Stability AI)
  Speech‑to‑text and text‑to‑speech
  Document upload and RAG
  Real‑time session updates via SignalR
  Modular chat modes (text, images, embeddings, speech)

Angular Configuration:
The Angular build configuration is defined in:
  angular.json

The application uses:
  SCSS styling
  Standalone components
  Angular build system
  Services for API communication
  Interceptors for JWT authentication

Environment Configuration
Environment files are stored under:
  APP/src/environments/ 
(You have to create it, probably add dev env and prod env, up to you)

These files contain:
  API base URL
  Feature flags
  Any client‑side configuration

They are excluded from Git.

Running the Project:

Backend
  cd API/src/AiWorkbench.Api
  dotnet run

The API will start on the port defined in launchSettings.json.

Frontend
  cd APP
  npm install
  npm start

The Angular app will run on http://localhost:4200

Folder Structure Summary

AzureOpenAiFullStackApp/
│
├── API/
│   └── src/
│       ├── AiWorkbench.Api/
│       ├── AiWorkbench.Application/
│       ├── AiWorkbench.Domain/
│       └── AiWorkbench.Infrastructure/
│
└── APP/
    ├── node_modules/
    ├── src/
    ├── angular.json
    └── package.json

Notes:
  All secrets must remain in appsettings.Development.json or environment variables.
  The .gitignore file excludes all sensitive and generated files.
  The backend and frontend can be deployed independently.
