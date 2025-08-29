## API Documentation
Interactive API documentation is provided via Swagger UI when the application is running. The GUIDE.md file contains detailed information on the project structure and all available endpoints.

## Project Structure
This solution follows Clean Architecture principles, organized into multiple projects:

WebAPI: The presentation layer (controllers, middleware).

Application: Business logic, CQRS commands/queries, interfaces.

Domain: Core entities, value objects, and domain exceptions.

Infrastructure: Data persistence, external service implementations, caching.

See GUIDE.md for an in-depth explanation.

## Contributing
Fork the project.

Create your feature branch (git checkout -b feature/AmazingFeature).

Commit your changes (git commit -m 'Add some AmazingFeature').

Push to the branch (git push origin feature/AmazingFeature).

Open a Pull Request.