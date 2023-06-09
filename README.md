# Welcome to the UCAB GO API! 🚗🎓
This API is designed to support the UCAB GO app, a ride-sharing platform for the UCAB community. With UCAB GO, students, faculty, and staff can easily request rides to and from campus, making transportation more convenient and accessible.

## Technologies Used 💻
- **.NET 6** - The UCAB GO API is built using .NET 6, a cross-platform framework for building modern applications.

- **Entity Framework Core** - Entity Framework Core is used as the Object-Relational Mapping (ORM) framework for the UCAB GO API. It provides a simple way to interact with the database and map database entities to C# objects.

- **MariaDB** - MariaDB is used as the database for the UCAB GO API. It is a popular open-source relational database management system that is known for its performance, scalability, and reliability.

- **JWT** - JSON Web Tokens (JWT) are used for authentication and authorization in the UCAB GO API. JWT is a standard for securely transmitting information between parties as a JSON object.

- **Azure Functions** - Azure Functions is used to host the UCAB GO API. It is a serverless compute service that allows you to run event-driven code without having to manage infrastructure.

- **Visual Studio** - Visual Studio is used as the Integrated Development Environment (IDE) for the UCAB GO API. It provides a powerful set of tools for building, testing, and debugging .NET applications.

These technologies were chosen for their reliability, performance, and ease of use. They provide a solid foundation for building a robust and scalable API that can meet the needs of the UCAB university community.

## Quick start 🚀

### 1. **Install .NET 6 and Visual Studio**

Make sure you have .NET 6 and Visual Studio installed your machine. You can download .NET 6 from the [official .NET website, and Visual Studio from the Visual Studio website](https://visualstudio.microsoft.com/).

### 2. **Clone the repository**

Clone the UCAB GO API repository to your local machine using Git.

### 3. **Create a MariaDB database**

Create an empty MariaDB database and save the connection string and use it when necessary in the text steps.

### 4. **Add the required keys to `local.settings.json`**

Add the following keys to the `local.settings.json` file. These keys are required for the API to function properly.

```json
{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    "AzureWebJobsStorage": "",
    "SqlConnectionString": "",
    "JWT_SECRET": "",
    "JWT_AUD": "",
    "JWT_ISS": "",
    "JWT_EXP": ,
    "IsAutomaticRideDeletionEnabled": true,
    "AzureSignalRConnectionString": ""
  }
}
```


### 5. **Open the project in Visual Studio**

Open the UCAB GO API project in Visual Studio by navigating to the project folder and double-clicking on the `.sln` file.

### 6. **Run Database Migrations**

Before running database migrations, it's important to understand why they are necessary. Database migrations are used to keep the database schema in sync with the application's codebase.
To sync your database with the tables that the project needs, follow these steps:
- Open the `UcabgoContext.cs` file located in `UcabGo.Infrastructure/Data`.
- Comment out the line with the connectionString variable and initialize it with your own connection string. For example:
```csharp
// var connectionString = "Server=(localdb)\\mssqllocaldb;Database=Ucabgo;Trusted_Connection=True;MultipleActiveResultSets=true";
var connectionString = "Server=myServerAddress;Database=myDataBase;User Id=myUsername;Password=myPassword;";
```
- Open the NuGet Package Manager Console.
- Run the following command to run the migrations. This will create all the tables in your database:
```
Update-Database
```
- Undo all the changes made to the `UcabgoContext.cs` file. It's important to avoid having connection strings burnt in the code, which could lead to security vulnerabilities.

**Note: You'll need to follow this process every time a database update is made.**

### 7. **Run the project**

Run the project by clicking on the "Run" button in Visual Studio. This will launch the API and allow you to start making requests.

### 8. **Start making requests**

You can go to `/api/swagger/ui` to start making requests to the UCAB GO API. Refer to our documentation for more information on how to use the API.

### It's done!
That's it! You are now ready to start using the UCAB GO API. If you have any questions or issues, please refer to our documentation or contact our support team. 📚📞
