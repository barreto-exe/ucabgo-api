# Welcome to the UCAB GO API! 🚗🎓
This API is designed to support the UCAB GO app, a ride-sharing platform for the UCAB community. With UCAB GO, students, faculty, and staff can easily request rides to and from campus, making transportation more convenient and accessible.

# Technologies Used 💻
- **.NET 6** - The UCAB GO API is built using .NET 6, a cross-platform framework for building modern applications.

- **Entity Framework Core** - Entity Framework Core is used as the Object-Relational Mapping (ORM) framework for the UCAB GO API. It provides a simple way to interact with the database and map database entities to C# objects.

- **MariaDB** - MariaDB is used as the database for the UCAB GO API. It is a popular open-source relational database management system that is known for its performance, scalability, and reliability.

- **JWT** - JSON Web Tokens (JWT) are used for authentication and authorization in the UCAB GO API. JWT is a standard for securely transmitting information between parties as a JSON object.

- **Azure Functions** - Azure Functions is used to host the UCAB GO API. It is a serverless compute service that allows you to run event-driven code without having to manage infrastructure.

- **Visual Studio** - Visual Studio is used as the Integrated Development Environment (IDE) for the UCAB GO API. It provides a powerful set of tools for building, testing, and debugging .NET applications.

These technologies were chosen for their reliability, performance, and ease of use. They provide a solid foundation for building a robust and scalable API that can meet the needs of the UCAB university community.

# Quick start 🚀
1. **Install .NET 6 and Visual Studio**

Make sure you have .NET 6 and Visual Studio installed your machine. You can download .NET 6 from the [official .NET website, and Visual Studio from the Visual Studio website](https://visualstudio.microsoft.com/).

2. **Clone the repository**

Clone the UCAB GO API repository to your local machine using Git.

3. **Create a MariaDB database**

Create a MariaDB database that matches the Entity Framework models of the project. You can use the Entity Framework Core tools to generate the database schema from the models.

4. **Update the connection string**

Update the connection string in the `local.settings.json` file to point to your MariaDB database.

5. **Add the required keys to `local.settings.json`**

Add the following keys to the `local.settings.json` file: `SqlConnectionString`, `JWT_SECRET`, `JWT_ISS`, `JWT_AUD`. These keys are required for the API to function properly.

6. **Open the project in Visual Studio**

Open the UCAB GO API project in Visual Studio by navigating to the project folder and double-clicking on the `.sln` file.

7. **Run the project**

Run the project by clicking on the "Run" button in Visual Studio. This will launch the API and allow you to start making requests.

8. **Start making requests**

You can go to `/api/swagger/ui` to start making requests to the UCAB GO API. Refer to our documentation for more information on how to use the API.

That's it! You are now ready to start using the UCAB GO API. If you have any questions or issues, please refer to our documentation or contact our support team. 📚📞
