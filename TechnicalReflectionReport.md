# TechMove Logistics SOA, Cloud-Native and Testing Reflection Report

## 1. Service-Oriented Architecture Refactor
The original TechMove Logistics prototype was a monolithic ASP.NET Core MVC system. The MVC controllers were responsible for presenting screens, executing business logic, and communicating directly with the SQL Server database through Entity Framework Core. In the refactored version, the system is separated into two main services: an ASP.NET Core Web API backend and an ASP.NET Core MVC frontend.

The backend API is now the only application layer that communicates with the database. It exposes JSON endpoints such as `GET /api/contracts`, `POST /api/contracts`, and `PATCH /api/contracts/{id}/status`. The MVC application no longer uses Entity Framework Core for operational data. Instead, its controllers use `HttpClient` to call the backend API. This creates a clean separation between the presentation layer and the service/data layer.

This design improves maintainability because changes to business rules can be made inside the API without rewriting the MVC interface. It also improves scalability because the API and web application can be deployed and scaled separately depending on traffic.

## 2. Swagger/OpenAPI
Swagger/OpenAPI was enabled in the backend API. This allows the API to be self-documenting and testable from the browser. Developers and lecturers can open the Swagger UI and immediately see available endpoints, request bodies, response formats, and authentication requirements.

Swagger is useful in an SOA system because frontend developers can test service endpoints independently from the MVC application. It also supports faster debugging because each endpoint can be called directly.

## 3. Authentication
The backend API uses JWT bearer authentication. The MVC application requests a token from `POST /api/auth/token` by using configured API client credentials. After receiving the token, the MVC application attaches it to API requests using the `Authorization: Bearer` header.

This protects the API from anonymous calls and demonstrates that the service layer is not just publicly exposed without security. The MVC application also includes cookie-based login for the user interface.

## 4. Automated API Integration Testing
Automated integration testing is critical in a CI/CD pipeline because it confirms that the real API endpoints behave correctly after changes are pushed. Unlike unit tests, integration tests call the running service over HTTP and verify real status codes and JSON responses.

For example, the test project calls `GET /api/contracts` and asserts that the response status is `200 OK` and that the returned JSON is not empty. It also verifies that protected endpoints return `401 Unauthorized` when a JWT token is missing. These tests reduce the risk of breaking changes because the pipeline can fail before broken code reaches production.

Automated testing prevents bugs from reaching production by checking important application behaviour before deployment. If a developer accidentally changes an endpoint route, breaks authentication, or returns invalid JSON, the tests will detect the issue early.

## 5. Containerization with Docker Compose
The system is fully containerized with Docker Compose. The compose file starts three containers: `sql-server-db`, `gims-backend-api`, and `gims-frontend-web`. SQL Server stores the application data. The API container connects to SQL Server using the internal Docker service name `sql-server-db`. The MVC container connects to the API using the internal Docker service name `gims-backend-api`.

Docker solves the “it works on my machine” problem because the application runs with the same runtime, dependencies, environment variables, and networking setup on every computer. Developers, testers, and production servers can run the same containers instead of manually installing different versions of .NET, SQL Server, or configuration settings.

Docker Compose also makes the system easier to demonstrate. Instead of starting SQL Server, the API, and the MVC app manually, the full ecosystem can be started with one command: `docker compose up --build`.

## 6. DevOps Value
The refactored architecture supports DevOps practices because it can be built, tested, and deployed automatically. A CI/CD pipeline can restore packages, build the API and MVC projects, run integration tests, and build Docker images. If any step fails, deployment can be stopped automatically.

This improves software quality and reliability. The API can be tested independently from the frontend, and Docker ensures that the same tested application image is the one that gets deployed.

## 7. Conclusion
The TechMove Logistics system has been modernised from a monolithic MVC prototype into a cloud-ready Service-Oriented Architecture. The backend API owns database access and business logic, the MVC frontend consumes the API using `HttpClient`, JWT protects API endpoints, automated integration tests validate endpoint behaviour, and Docker Compose runs the full ecosystem consistently. This makes the application easier to maintain, test, deploy, and scale.
