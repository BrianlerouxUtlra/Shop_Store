# Shop Store ASP.NET Core 7 Solution

This repository contains the source code for the Shop Store application built with ASP.NET Core 7. The application uses Docker for containerization.

## Prerequisites

Before you begin, ensure you have the following tools installed on your machine:

- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Getting Started

1. **Clone the repository:**

    ```bash
    git clone https://github.com/BrianlerouxUtlra/Shop_Store.git
    cd Shop_Store
    ```

2. **Configure the Database:**

    Create a new SQL Server database for the Shop Store application. If you don't have a SQL Server instance, you can use Docker to run a containerized SQL Server:

    ```bash
    docker run -e "ACCEPT_EULA=Y" -p 1433:1433 --name shopstore-db -d mcr.microsoft.com/mssql/server
    ```

3. **Configure the Connection String:**

    Open the `Shop-store.Api/appsettings.json` file and update the `DefaultConnection` in the `ConnectionStrings` section with your SQL Server connection string:

    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=shopstore-db;Database=ShoppingCart;Trusted_Connection=true;Encrypt=true;TrustServerCertificate=true;"
    }
    ```

4. **Build and Run the Application using Docker Compose:**

    ```bash
    docker-compose up --build
    ```

    This command will build the Docker images and start the containers for the web API and the database.

5. **Access the Application:**

    Open your web browser and navigate to (http://localhost:5083/swagger) to interact with the API using Swagger.

6. **Stopping the Application:**

    To stop the application and remove the containers, press `Ctrl + C` in the terminal where the `docker-compose up` command is running.

## Docker Compose Configuration

The `docker-compose.yml` file is configured to run the Shop Store application with the following services:

- **webapi:** The ASP.NET Core web API service.
- **database:** The MSSQL Server database service.

## Environment Variables

The `docker-compose.yml` file sets the following environment variables:

- `ASPNETCORE_ENVIRONMENT`: Development environment for the web API.
- `ASPNETCORE_URLS`: URLs for the web API.
- `ConnectionStrings__DefaultConnection`: Connection string for the database.

## Notes

- The database service uses the `mcr.microsoft.com/mssql/server` image with default settings for development. Modify the environment variables as needed for production.

- Ensure that the specified ports (5083 for the web API and 1433 for the database) are not in use by other processes on your machine.

- The provided Dockerfile in the `Shop-store.Api` folder builds the application and sets up the runtime environment.

- The `launchsettings.json` file in the project defines different profiles for various environments, including Docker. Adjust the configuration as needed.

