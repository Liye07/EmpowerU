# EmpowerU (Final Year Group Project) 

## Live Website
The live website for EmpowerU is hosted (https://empoweru-2h40.onrender.com/). Please note that this link may go down occasionally, as it is hosted on a free-tier platform. 

### Backup Option

If the live site is unavailable, you can run the website locally using Visual Studio. Follow the steps below:

1. Clone the repository from GitHub:
    ```bash
    git clone https://github.com/Liye07/EmpowerU.git
    ```
2. Open the project in **Visual Studio 2022**.

3. **Update Database Connection**  
   The default connection string in `appsettings.json` points to the live server.  
   To run locally, update it to your own **SQL Server** or **PostgreSQL/Supabase** instance:
   
   - If using **SQL Server**, update the connection string as follows:
     ```json
     "ConnectionStrings": {
         "DefaultConnection": "Server=your_server_name;Database=your_database;User Id=your_user;Password=your_password;Trusted_Connection=True;MultipleActiveResultSets=true"
     }
     ```
   - If using **PostgreSQL/Supabase**, update it accordingly:
     ```json
     "ConnectionStrings": {
         "DefaultConnection": "Host=your_host;Port=your_port;Database=your_db;Username=your_user;Password=your_password"
     }
     ```
   - Save the file.

4. **Set Up the Database**  
   Before running the application, ensure your database is set up:
   - If using **SQL Server**, make sure it’s installed and running.
   - If using **PostgreSQL**, ensure it’s installed and accessible.
   - Open **Package Manager Console** in Visual Studio:
     - Navigate to `Tools` > `NuGet Package Manager` > `Package Manager Console`
     - Run the following command to apply migrations and generate the database schema:
       ```powershell
       Update-Database
       ```

5. **Build and Run the Application**  
   Once the database is set up, build and run the application by pressing:
   ```plaintext
   Ctrl + F5


## Project Overview

EmpowerU is a web-based platform aimed at connecting consumers with trusted service providers, empowering emerging businesses to gain visibility and grow. The platform features a centralized directory, appointment booking system, user reviews, and advanced search functionalities. EmpowerU enhances both user experience and business opportunities. Despite challenges such as API integration and database synchronization, the team overcame these hurdles through innovative problem-solving and collaboration.

---

## Tools and Technologies Used

### 1. Development Environment

**Tool:** Visual Studio 2022  
**Purpose:** Visual Studio was chosen for writing, managing, and debugging code in the ASP.NET MVC web application. It offers integrated features such as IntelliSense, project templates, and debugging tools, streamlining the development process. The built-in Git integration also simplified version control and collaboration.

### 2. Framework

**Tool:** ASP.NET MVC  
**Purpose:** ASP.NET MVC was used to build the web application with a Model-View-Controller architecture, promoting separation of concerns. This framework was chosen for its scalability, maintainability, and ease of testing, making it a reliable choice for web development.

### 3. Database

**Tool:** MySQL  
**Purpose:** MySQL was initially used to create the first version of the database due to its ERD (Entity Relationship Diagram) generation feature, which helped in visualizing the database structure. It was a quick tool to start with for designing and building the database foundation.

**Tool:** SQL Server 2019  
**Purpose:** SQL Server was selected as the primary database management system due to its better compatibility with ASP.NET MVC and seamless integration with Visual Studio. SQL Server provided robust database management capabilities and optimized querying.

**Tool:** Render  
**Purpose:** Render is used for hosting the web application, ensuring it remains accessible online. Initially, it was also used for database hosting, but due to its free-tier limitations, the database was migrated to Supabase.  

**Tool:** Supabase  
**Purpose:** The database is now hosted on Supabase, offering a scalable PostgreSQL solution. However, due to inactivity, the database may shut down occasionally, requiring a manual restore to bring it back online.  

### 4. ORM (Object-Relational Mapping)

**Tool:** Entity Framework Core  
**Purpose:** Entity Framework Core was used for mapping database tables to C# objects, facilitating easy interaction with the database through object-oriented programming. Its data migration and management features made it an efficient tool for handling database operations.

### 5. Version Control

**Tool:** Git  
**Purpose:** Git was used to track changes in the project’s source code, providing a history of all modifications. It allowed for effective version control and branching for different features and updates.

**Tool:** GitHub  
**Purpose:** GitHub served as the remote repository hosting platform. It facilitated collaboration and code sharing between team members and provided a centralized location for pushing and pulling code changes, backups, and issue tracking.

### 6. UI Design

**Tool:** Bootstrap  
**Purpose:** Bootstrap was selected for designing a responsive and user-friendly interface. Its pre-built CSS and JavaScript components helped accelerate the UI development process and ensured cross-browser compatibility.

### 7. APIs

**Tool:** OpenStreetMap API (Nominatim)  
**Purpose:** The OpenStreetMap Nominatim API was used to handle real-time address lookups and suggestions in the business registration form, providing geolocation and mapping functionalities for accurate address data input.

### 8. Version Control Integration

**Tool:** Visual Studio Git Integration  
**Purpose:** Integrated directly into Visual Studio, this tool made it easy to push, pull, and manage code with Git and GitHub. This streamlined version control activities without needing a separate application, directly from within the development environment.

### 9. Diagram and Documentation Tools

**Tool:** Lucidchart  
**Purpose:** Lucidchart was chosen for creating UML diagrams, leveraging AI-assisted generation to create initial versions of the diagrams. These diagrams visually represented system architecture, class relationships, and workflows.

**Tool:** Wondershare EdrawMax  
**Purpose:** Wondershare EdrawMax was used to create the Use Case Diagram (UCD), helping to represent user interactions with the system and ensuring key functional requirements were captured clearly.

**Additional Tools:**  
**AI Assistance**: Tools like ChatGPT were used for problem-solving, optimizing code, and gaining insights into best practices, particularly in debugging and overcoming architectural challenges.

---

## System Requirements and Specifications

### 1. Objectives and Functionality

#### 1.1 Support Emerging Businesses
- EmpowerU allows businesses to create comprehensive profiles with service offerings, pricing, contact details, and images.
- Service providers can register and manage their profiles through an intuitive dashboard.

#### 1.2 Centralized Directory
- EmpowerU includes a centralized directory of beauty and wellness service providers.
- The directory is searchable by service type, location, and customer ratings.

#### 1.3 Appointment Scheduling
- Integrated appointment scheduling allows consumers to view available time slots and book services directly.
- Businesses are notified of new appointments and can manage their schedule.

#### 1.4 Optimized Search and Discovery
- Leveraging the Google Maps API, EmpowerU allows users to search for service providers by proximity, rating, and service type.

#### 1.5 Consumer Behavior Analytics
- The platform includes analytics and reporting features to provide businesses with insights into consumer interactions and service demand.

#### 1.6 Data Security
- EmpowerU implements secure user authentication, encrypted data transmission, and regular system updates to ensure data protection.

#### 1.7 User Experience Enhancements
- The platform features an intuitive and user-friendly interface to ensure smooth navigation for consumers and service providers.

#### 1.8 User Reviews and Ratings
- EmpowerU includes a review and rating system to foster trust among users and help businesses maintain high standards.

---

## Functional Scope

### Core Features
- **Service Provider Profiles:** Businesses can create and manage detailed service profiles.
- **User Reviews and Ratings:** Consumers can leave reviews and rate businesses.
- **Search and Filter Options:** Search functionality with filters by category, location, rating, and service type.
- **Appointment Booking:** An integrated scheduling system for easy appointment booking.
- **Analytics:** Businesses can access performance metrics such as customer trends and service demand.

### Third-Party Integrations
- OpenStreetMapAPI for location-based search functionality.

---

## Installation Instructions

To run the EmpowerU project locally, follow the steps below:

### Prerequisites
- Install [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/).
- Ensure you have [SQL Server 2019](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or any compatible database system installed.

### Clone the Repository
```bash
git clone https://github.com/yourusername/empoweru.git
