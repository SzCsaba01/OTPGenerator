# OTP Generator

This project is a One-Time Password (OTP) generator application built using **Angular** for the frontend and **.NET** for the backend. The OTPs are designed to be unpredictable, securely encrypted during transmission, and validated against stored hashes. The project also includes unit tests to ensure reliability.

## Features

- **OTP Generation**: Generates a random 6-digit password using uppercase letters and numbers.
- **Hashing & Security**:
  - Uses a generated salt and a static pepper for added security.
  - Hashes the OTP with a static key before storing it in the database.
  - Saves the hash, expiration date, and salt in the database.
- **OTP Validation**:
  - Uses user input, OTP ID, and salt to generate a new hash.
  - Compares the newly generated hash with the stored hash.
  - Validates the OTP only if it matches and is not expired.
- **Automatic Expiration Cleanup**:
  - A **cron job** runs every minute to delete expired OTPs from the database.
- **Encryption**:
  - Uses **HTTPS** with a self-signed certificate included in the project.
- **Testing**:
  - Uses **xUnit** and **Moq** for unit testing.

## Running the Project

### Frontend (Angular)
To run the frontend part of the application, use the following command:

```sh
npm start
```

This will start the application with HTTPS enabled.

### Backend (.NET)
To run the backend part of the application:
1. Open the project in **Visual Studio**.
2. Run the application using the HTTPS configuration.
3. Ensure that the database connection is properly specified in the `appsettings.json` file.
4. Run the following command in the **Package Manager Console** to generate the database in SQL Server:

```sh
update-database
```

## Security Considerations
- The OTPs are hashed before storage, ensuring they cannot be reversed.
- HTTPS is enforced for secure communication between frontend and backend.
- A salt is used to ensure each OTP hash is unique, even if the same password is generated multiple times.


