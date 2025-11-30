
# Mos3ef API Project

`Mos3ef` is a .NET 8 Web API platform designed to connect patients with hospital services. It provides a RESTful API for registering patients and hospitals, searching for medical services, managing patient profiles, and submitting reviews.

## Demo
Check out the project demo: [Watch on YouTube](https://youtu.be/Bp7vdTkjM6g)

## Features

  * **User Registration:** Separate registration endpoints for Patients and Hospitals.
  * **Authentication:** Secured with JWT Bearer tokens.
  * **Role-Based Authorization:** Distinct permissions for `Patient`, `Hospital`, and `Admin` roles.
  * **Service Management:** Hospitals can add, update, and delete their services.
  * **Service Discovery:** Patients can search, filter, and compare hospital services.
  * **Patient Profiles:** Patients can manage their profile and save favorite services.
  * **Reviews System:** Patients can add, update, and delete reviews for services.
  * **Hospital Dashboard:** Hospitals can view stats like service count, review count, and average rating.

## Project Structure

The solution follows a clean 3-tier architecture:

  * **`Mos3ef.Api`**: The main web project.
      * Contains Controllers for handling API requests.
      * `Program.cs` for service registration (DI, Auth, DB).
      * `appsettings.json` for configuration.
  * **`Mos3ef.BLL` (Business Logic Layer)**:
      * Contains `Managers` for business logic.
      * `Dtos` (Data Transfer Objects) for API requests/responses.
      * `Mapping` profiles for AutoMapper.
  * **`Mos3ef.DAL` (Data Access Layer)**:
      * Contains `Models` (database entities like `Hospital`, `Patient`, `Service`).
      * `ApplicationDbContext` for Entity Framework Core.
      * `Repositories` for data access patterns.
      * `Migrations` for database schema.
  * **`Documentation`**:
      * Contains detailed project documentation (Project Management, Requirements, Database Design, UI/UX).

## Documentation

For detailed project documentation, please refer to the [Documentation](./Documentation) folder:

*   **[Project Management](./Documentation/Project_Management)**: Proposal, Project Plan, Roles, Risk Assessment, KPIs.
*   **[Requirements Analysis](./Documentation/Requirements_Analysis)**: Stakeholder Analysis, Functional & Non-Functional Requirements.
*   **[Database Design](./Documentation/Database_Design)**: ERD, Schema, and Tech Stack.
*   **[UI/UX Design](./Documentation/UI_UX_Design)**: Design Guidelines and Mockups.

## Setup and Installation

1.  **Clone the repository:**

    ```sh
    git clone <https://github.com/maiatef18/DepiProject.git>
    ```

2.  **Configure Connection String:**
    Open `Mos3ef/appsettings.json` and modify the `DefaultConnection` string to point to your local SQL Server instance.

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=Mos3efDB;Trusted_Connection=True;TrustServerCertificate=True;"
    }
    ```

3.  **Run Database Migrations:**
    Use the .NET CLI to apply the database migrations.

    ```sh
    cd Mos3ef.DAL
    dotnet ef database update
    ```

    (Or in Visual Studio, open the Package Manager Console and run `Update-Database`).

4.  **Run the Project:**
    You can run the project from Visual Studio or by using the .NET CLI from the root folder:

    ```sh
    dotnet run --project Mos3ef/Mos3ef.Api.csproj
    ```

5.  **Admin User:**
    The database is automatically seeded with a default Admin user.

      * **Email:** `admin@mos3ef.com`
      * **Password:** `Admin@123`

## API Endpoints

Once running, the API is available at `http://localhost:5000` (or as specified in `launchSettings.json`). Swagger is enabled at `http://localhost:5000/swagger`.

### Authentication (`/api/Account`)

  * `POST /api/Account/register/patient`: Register a new patient.
  * `POST /api/Account/register/hospital`: Register a new hospital.
  * `POST /api/Account/login`: Log in to get a JWT token.
  * `POST /api/Account/logout`: (Auth required) Revoke the current token.
  * `POST /api/Account/change-password`: (Auth required) Change the current user's password.
  * `POST /api/Account/create-role`: (Admin required) Create a new role.
  * `POST /api/Account/assign-role`: (Admin required) Assign a role to a user.
  * `POST /api/Account/create-user`: (Admin required) Create a new user manually.
  * `PUT /api/Account/update-user`: (Admin required) Update an existing user.

### Patient (`/api/Patients`)

  * `GET /api/Patients/{id}`: (Admin/Self required) Get patient details by ID.
  * `GET /api/Patients/my-profile`: (Patient required) Get the logged-in patient's profile.
  * `PUT /api/Patients/my-profile`: (Patient required) Update the logged-in patient's profile.
  * `GET /api/Patients/my-saved-services`: (Patient required) Get a paged list of the patient's saved services.
  * `POST /api/Patients/my-saved-services/{serviceId}`: (Patient required) Save a service to the patient's list.
  * `DELETE /api/Patients/my-saved-services/{serviceId}`: (Patient required) Remove a service from the patient's list.

### Hospital (`/api/Hospital`)

  * `GET /api/Hospital/GetAll`: (Patient required) Get a list of all hospitals.
  * `GET /api/Hospital/Get/{id}`: (Patient required) Get a hospital by its ID.
  * `POST /api/Hospital/Add`: (Hospital required) Add a new hospital profile.
  * `PUT /api/Hospital/Update/Id`: (Hospital required) Update the logged-in hospital's profile.
  * `DELETE /api/Hospital/Delete/Id`: (Hospital required) Delete the logged-in hospital's profile.
  * `POST /api/Hospital/AddService`: (Hospital required) Add a new service for the hospital.
  * `PUT /api/Hospital/UpdateService/{id}`: (Hospital required) Update an existing service.
  * `DELETE /api/Hospital/DeleteService/{id}`: (Hospital required) Delete a service.
  * `GET /api/Hospital/GetServicesReviews/{id}`: (Hospital required) Get all reviews for all services offered by the hospital.
  * `GET /api/Hospital/GetDashboardStats/{id}`: (Hospital required) Get dashboard statistics.

### Services (`/api/Services`)

  * `GET /api/Services`: Get a paged list of all available services.
  * `GET /api/Services/{id}`: Get a service by its ID.
  * `GET /api/Services/search`: Search services by keyword, location, etc.
  * `POST /api/Services/compare`: Compare two services side-by-side.
  * `GET /api/Services/{id}/reviews`: Get all reviews for a specific service.
  * `GET /api/Services/{id}/hospital`: Get the hospital information for a specific service.

### Review (`/api/Review`)

  * `GET /api/Review/{serviceId}`: Get all reviews for a specific service.
  * `POST /api/Review`: (Patient required) Add a new review to a service.
  * `PUT /api/Review/{Id}`: (Patient required) Update one of the patient's existing reviews.
  * `DELETE /api/Review/{Id}`: (Patient required) Delete one of the patient's reviews.
