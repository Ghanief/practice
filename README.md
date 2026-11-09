# HustleHub+ - Secure Freelance Marketplace (Part 1, Part 2 and Part 3)

![Node.js](https://img.shields.io/badge/Node.js-20.x-green)
![Express](https://img.shields.io/badge/Express-5.x-blue)
![MongoDB](https://img.shields.io/badge/MongoDB-7.x-brightgreen)
![JWT](https://img.shields.io/badge/JWT-authentication-orange)
![2FA](https://img.shields.io/badge/2FA-TOTP-purple)
![HTTPS](https://img.shields.io/badge/HTTPS-enabled-success)
![License](https://img.shields.io/badge/License-ISC-blue)

---

## Table of contents

- [System overview](#system-overview)
- [Intended users](#intended-users)
- [Architecture diagram](#architecture-diagram)
- [Technology stack](#technology-stack)
- [Part 1 backend structure](#part-1-backend-structure)
- [Part 1 - Secure backend foundations](#part-1--secure-backend-foundations)
- [Part 1 security features](#part-1-security-features)
  - [Password handling](#password-handling)
  - [JWT authentication and session checks](#jwt-authentication-and-session-checks)
  - [Two-factor authentication](#two-factor-authentication)
  - [Role-based authorization and ownership](#role-based-authorization-and-ownership)
  - [Validation and sanitisation](#validation-and-sanitisation)
  - [HTTP security controls](#http-security-controls)
  - [Rate limiting](#rate-limiting)
  - [Logging and sensitive data](#logging-and-sensitive-data)
- [Additional security features](#additional-security-features)
- [Prerequisites](#prerequisites)
- [Setup instructions](#setup-instructions)
- [Environment variables](#environment-variables)
- [Key API endpoints](#key-api-endpoints)
- [Example requests and responses](#example-requests-and-responses)
- [Part 1 Postman testing](#part-1-postman-testing)
- [Part 2 - Marketplace application](#part-2--marketplace-application)
- [Part 3 - DevSecOps, monitoring and finalisation](#part-3--devsecops-monitoring-and-finalisation)
  - [SARS tax estimation](#sars-tax-estimation-and-income-dashboard)
  - [GitHub Actions and static analysis](#github-actions-and-static-analysis)
  - [Docker and Docker Compose](#docker-and-docker-compose)
  - [Logging and cloud log forwarding](#logging-and-cloud-log-forwarding)
  - [Additional security features](#part-3-additional-security-features)
  - [Final security review](#part-3-final-security-review)
  - [Part 3 testing and evidence](#part-3-testing-and-evidence)
- [Roles and workflows](#roles-and-workflows)
- [Running the project](#running-the-project)
- [Testing](#testing)
- [Email testing with MailHog](#email-testing-with-mailhog)
- [Project structure](#project-structure)
- [Known scope and limitations](#known-scope-and-limitations)
- [Assessment evidence](#assessment-evidence)
- [Part 1 screenshots and demo video](#part-1-screenshots-and-demo-video)
- [Team](#team)
- [License](#license)

---

## System overview

**HustleHub+** is a freelance marketplace where clients can discover and book services offered by freelancers. The platform records simulated transactions associated with bookings and provides freelancers with income information.

The project was developed in two parts. **Part 1** established the backend foundation, including authentication, authorization, validation, security controls, and API tests. **Part 2** extended the project with a React frontend and marketplace workflows for gigs, bookings, reviews, transactions, income, notifications, and administration.

The application uses **React and TypeScript** for the frontend, **Node.js and Express** for the REST API, and **MongoDB with Mongoose** for data storage.

Part 3 builds on the first two parts with an estimated SARS tax reserve for freelancers, Docker deployment, DevSecOps checks, cloud-capable logging, and additional security controls.

> **Project scope:** Payments are simulated. The application does not process real payments or provide tax advice or tax estimates.
>
> **Part 3 clarification:** The application now provides an approximate tax estimate from completed HustleHub+ income. It remains an educational planning estimate, not tax advice or a tax return.

## Intended users

| Role           | Description                                                                                       |
| -------------- | ------------------------------------------------------------------------------------------------- |
| **Client**     | Browses gigs, makes bookings, tracks progress, and reviews completed work.                        |
| **Freelancer** | Creates and manages gigs, handles bookings, and tracks income.                                    |
| **Admin**      | Uses restricted administration tools to review users, gigs, bookings, transactions, and activity. |

Public registration is limited to client and freelancer roles. Administrator access must be granted separately.

## Architecture diagram

The application follows the MERN stack:

<img width="449" height="980" alt="Architecture Diagram" src="https://github.com/user-attachments/assets/e0cbba16-2f52-4015-88ff-0ce6ae2d5160" />

## Technology stack

| Layer                         | Technology                                                                           |
| ----------------------------- | ------------------------------------------------------------------------------------ |
| **Frontend**                  | React, TypeScript, Vite, React Router                                                |
| **Backend runtime**           | Node.js 20+                                                                          |
| **Backend framework**         | Express 5                                                                            |
| **Database**                  | MongoDB with Mongoose                                                                |
| **Authentication**            | JSON Web Tokens                                                                      |
| **Two-factor authentication** | TOTP with QR-code setup                                                              |
| **2FA secret encryption**     | AES-256-GCM                                                                          |
| **Password hashing**          | bcryptjs                                                                             |
| **Rate limiting**             | express-rate-limit                                                                   |
| **Validation**                | Custom middleware                                                                    |
| **Logging**                   | Structured JSON file logger                                                          |
| **Security headers**          | Helmet, CORS, Content Security Policy, and related headers                           |
| **Environment configuration** | dotenv                                                                               |
| **API testing**               | Postman and Newman                                                                   |
| **Frontend testing**          | Vitest and React Testing Library                                                     |
| **CI/CD and static analysis** | GitHub Actions, CodeQL, and Prettier (workflow still to be added to this repository) |
| **Containers**                | Docker, Docker Compose, and Nginx                                                    |
| **Cloud log ingestion**       | HTTPS JSON log ingestion API; optional provider configuration                        |

## Part 1 backend structure

This is a summary of the original Part 1 backend foundation, not a complete listing of the current Part 2 codebase.

```text
backend/
├── controllers/                   # API request handlers
├── middleware/                    # Authentication, security, validation, and errors
├── models/                        # Mongoose schemas
├── routes/                        # Express API routes
├── scripts/                       # Setup and administration scripts
├── utils/                         # Shared utilities and logging
├── logs/                          # Runtime logs; ignored by Git
├── certs/                         # Local HTTPS certificates; generated locally
├── app.js                         # Express app and HTTPS server
├── package.json
└── package-lock.json
```

## Part 1 - Secure backend foundations

Part 1 focused on providing a secure backend foundation for the marketplace. Its functionality includes:

- User registration and login
- Password hashing
- JWT authentication
- Role-based access control
- Two-factor authentication
- Request validation and sanitisation
- HTTPS for local development
- Security headers and rate limiting
- Structured application logging
- MongoDB integration
- Postman API testing

## Part 1 security features

Security controls are enforced in the backend. Frontend role checks support navigation and usability, but do not replace backend authentication or authorization.

### Password handling

Passwords are hashed with `bcryptjs` using 12 salt rounds before being stored. The application does not store plain-text passwords, and password hashes are excluded from normal API responses.

Registration and login inputs are validated before they are processed. Password rules are checked by the authentication validation middleware.

### JWT authentication and session checks

After successful login, the API issues a signed JWT. Protected requests must provide it in the `Authorization: Bearer <token>` header.

The authentication middleware verifies the JWT signature and configured issuer and audience. It then retrieves the user from MongoDB and checks the account status and token version. This allows the API to reject suspended accounts and tokens invalidated by a session-version change.

Only the user information needed by downstream handlers is attached to the request. The stored password hash is not included. Tokens expire according to the configured JWT expiry.

The frontend stores the current session in browser session storage. If the API rejects a token as invalid or expired, the frontend clears the session and returns the user to the unauthenticated flow.

### Two-factor authentication

Users can set up two-factor authentication using the API’s TOTP flow. The server encrypts the stored 2FA secret using a separate encryption key. When 2FA is enabled, a successful password check may return a short-lived challenge; the user must submit a valid TOTP code to finish signing in.

Challenge tokens have a separate audience and a short expiry. They cannot be used as ordinary access tokens. The API also applies a dedicated rate limit to 2FA endpoints.

### Role-based authorization and ownership

The API distinguishes clients, freelancers, and administrators. Public registration accepts client and freelancer roles, preventing a user from making themselves an administrator through a registration request.

Protected routes enforce the required role on the server. Controllers also check ownership before permitting changes. For example, freelancers can manage their own gigs, and booking actions are checked against the client or freelancer involved in the booking. Administrative routes require an administrator account.

These checks run on the backend for each protected request. Hiding a button in the frontend is not treated as an authorization control.

### Validation and sanitisation

Request validators define allowed fields and check their values before controller logic uses them. Unexpected fields can be rejected rather than blindly accepted. Validation covers inputs such as email addresses, passwords, roles, gig prices, packages, booking status transitions, and review ratings.

Marketplace text fields have length limits and are checked for disallowed markup and control characters. JSON request bodies also have a size limit. The frontend renders user-submitted content as text instead of inserting it as raw HTML.

### HTTP security controls

The Express application uses Helmet and a Content Security Policy for API responses. Other controls include disabling the `X-Powered-By` header, content-type sniffing protection, clickjacking protection, a restrictive referrer policy, and no-store caching for sensitive responses.

CORS is restricted to configured frontend origins. HSTS is enabled for production configuration. Local HTTPS uses certificate files in the backend `certs` directory.

### Rate limiting

Rate limits restrict repeated requests and help reduce brute-force attempts. The API has a general limit, with stricter limits for authentication, two-factor authentication, password reset, and booking operations. Administrative routes have a separate limit.

When a limit is reached, the server responds with HTTP `429 Too Many Requests` and a retry interval. Automated test runs should be paced appropriately. Rate limiting should not be disabled in a deployed environment.

### Logging and sensitive data

The backend writes structured JSON events to the ignored `backend/logs` directory. Request logs include information such as the method, path, and response status. The logger avoids recording request bodies, passwords, and authorization tokens. Detailed error information is kept in server-side logs for diagnosis rather than being exposed in public error responses.

The logs directory is excluded from Git so runtime activity is not added to source control.

## Additional security features

The project includes security measures beyond basic registration and login:

| Feature                       | Purpose                                                                  |
| ----------------------------- | ------------------------------------------------------------------------ |
| **Two-factor authentication** | Requires a second authentication step for accounts with 2FA enabled.     |
| **Rate limiting**             | Restricts repeated requests, including repeated authentication attempts. |
| **QR-code setup**             | Makes it easier to add the TOTP account to an authenticator app.         |

These features strengthen the project’s security controls. They do not by themselves make the application production-ready.

## Prerequisites

- Node.js 20 or newer
- MongoDB Atlas or a local MongoDB replica set
- Git
- OpenSSL for generating local HTTPS certificates
- Postman for manual API testing
- Docker Desktop, if using MailHog

MongoDB transactions require a replica-set-capable deployment. MongoDB Atlas supports this; a standalone local MongoDB server needs to be configured as a replica set.

## Setup instructions

### 1. Clone the repository

```bash
git clone <your-repo-url>
cd <repo-folder>
```

### 2. Install backend dependencies

```bash
cd backend
npm install
```

### 3. Generate local HTTPS certificates

**Windows (Git Bash):**

```bash
mkdir -p certs
openssl req -x509 -newkey rsa:2048 -keyout certs/ca.key -out certs/ca.crt -days 365 -nodes -subj "//CN=localhost-CA"
openssl req -x509 -newkey rsa:2048 -keyout certs/cert.key -out certs/cert.crt -days 365 -nodes -subj "//CN=localhost"
```

**macOS/Linux:**

```bash
mkdir -p certs
openssl req -x509 -newkey rsa:2048 -keyout certs/ca.key -out certs/ca.crt -days 365 -nodes -subj "/CN=localhost-CA"
openssl req -x509 -newkey rsa:2048 -keyout certs/cert.key -out certs/cert.crt -days 365 -nodes -subj "/CN=localhost"
```

These are local development certificates and should not be committed.

### 4. Create the backend environment file

```bash
npm run setup
```

The setup command creates `backend/.env` if needed, generates authentication secrets, and creates the logs directory. It does not overwrite an existing database connection string or JWT secret.

If using MongoDB Atlas, put the Atlas connection string in `CONN_STRING` in `backend/.env`.

### 5. Start the backend

```bash
npm start
```

The API uses the configured port, defaulting to `3000`. The default local API address is:

```text
https://localhost:3000/api
```

### 6. Start the frontend

In a second terminal, from the project root:

```powershell
cd frontend
npm install
npm run dev
```

Open the local URL printed by Vite.

For a production build:

```powershell
npm run build
```

## Environment variables

Create `backend/.env` locally and keep it out of source control. The setup script can create the initial configuration:

```powershell
cd backend
npm run setup
```

Common settings include:

| Variable                    | Purpose                                                                                                              |
| --------------------------- | -------------------------------------------------------------------------------------------------------------------- |
| `PORT`                      | Backend port; defaults to `3000`                                                                                     |
| `CONN_STRING`               | MongoDB connection string                                                                                            |
| `JWT_SECRET`                | Secret used to sign access tokens                                                                                    |
| `JWT_EXPIRES_IN`            | Access-token lifetime                                                                                                |
| `TWO_FACTOR_ENCRYPTION_KEY` | Key used to encrypt stored 2FA secrets                                                                               |
| `CORS_ORIGINS`              | Allowed frontend origins, if configured                                                                              |
| `LOG_INGEST_URL`            | Optional HTTPS endpoint for cloud log ingestion. Leave blank to use local logs only.                                 |
| `LOG_INGEST_TOKEN`          | Optional bearer token for the configured log-ingestion source. Keep it secret and leave blank to disable forwarding. |
| Email SMTP settings         | Email provider configuration, such as MailHog                                                                        |

Use strong, unique secrets. Do not commit `.env`, show its values in screenshots, or put backend secrets in frontend environment variables.

## Key API endpoints

This table lists selected API endpoints, not every route. Check the backend route files for the complete and current route definitions.

| Method   | Endpoint                          | Description                             |
| -------- | --------------------------------- | --------------------------------------- |
| `GET`    | `/api/home`                       | Health check                            |
| `POST`   | `/api/auth/register`              | Register a client or freelancer         |
| `POST`   | `/api/auth/login`                 | Log in or start a 2FA challenge         |
| `POST`   | `/api/auth/2fa/verify-login`      | Complete 2FA login                      |
| `POST`   | `/api/auth/forgot-password`       | Request a password reset                |
| `POST`   | `/api/auth/reset-password`        | Submit a password reset token           |
| `GET`    | `/api/auth/profile`               | Get the current user profile            |
| `GET`    | `/api/gigs/search`                | Search gigs                             |
| `GET`    | `/api/gigs/categories`            | Get gig categories                      |
| `POST`   | `/api/gigs`                       | Create a gig as a freelancer            |
| `PATCH`  | `/api/gigs/:id`                   | Edit an owned gig                       |
| `DELETE` | `/api/gigs/:id`                   | Soft-delete an owned gig                |
| `GET`    | `/api/bookings`                   | List the current user’s bookings        |
| `POST`   | `/api/bookings`                   | Create a booking                        |
| `PATCH`  | `/api/bookings/:id/status`        | Update an eligible booking status       |
| `POST`   | `/api/bookings/:id/review`        | Submit an eligible review               |
| `POST`   | `/api/bookings/:id/messages`      | Add a booking message                   |
| `GET`    | `/api/bookings/:id/receipt.pdf`   | Download a booking receipt              |
| `GET`    | `/api/transactions`               | List transactions for the current user  |
| `GET`    | `/api/transactions/income`        | Get freelancer income information       |
| `GET`    | `/api/transactions/income.csv`    | Export freelancer income as CSV         |
| `GET`    | `/api/transactions/statement.pdf` | Download a transaction statement        |
| `GET`    | `/api/freelancers/:id`            | View a public freelancer profile        |
| `GET`    | `/api/notifications`              | List notifications for the current user |
| `GET`    | `/api/admin/overview`             | Get admin dashboard overview            |
| `GET`    | `/api/admin/users`                | Search or list users as an admin        |
| `GET`    | `/api/admin/gigs`                 | List gigs for admin moderation          |
| `GET`    | `/api/admin/bookings`             | List bookings as an admin               |
| `GET`    | `/api/admin/transactions`         | List transactions as an admin           |
| `GET`    | `/api/admin/activity`             | View admin activity information         |

Protected route access depends on authentication, role, and—where applicable—ownership of the related resource.

## Example requests and responses

### Register

**Request:**

```http
POST /api/auth/register
Content-Type: application/json
```

```json
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePass123!",
  "role": "freelancer"
}
```

**Example response:**

```json
{
  "message": "Registration successful.",
  "user": {
    "_id": "example-user-id",
    "username": "john_doe",
    "email": "john@example.com",
    "role": "freelancer",
    "twoFactorEnabled": false
  },
  "token": "<access-token>"
}
```

### Login with 2FA

The first login request uses the registered email and password:

```json
{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

If 2FA is enabled, the API responds with a challenge. The client then submits the challenge token and a current TOTP code to `/api/auth/2fa/verify-login`.

Do not publish real access tokens, passwords, TOTP codes, or secrets in documentation or screenshots.

## Part 1 Postman testing

Import the Part 1 collection from the path and filename that exist in your repository. In this project, the collection was identified as:

```text
backend/Postman-Collection
```

The collection covers health checks, registration, login, validation failures, 2FA, protected profile access, and rate limiting.

|   # | Request                          | Expected status |
| --: | -------------------------------- | --------------: |
|   1 | Health check                     |             200 |
|   2 | Register user                    |             201 |
|   3 | Register duplicate email         |             409 |
|   4 | Register weak password           |             400 |
|   5 | Login without 2FA                |             200 |
|   6 | Login with wrong password        |             401 |
|   7 | Set up 2FA                       |             200 |
|   8 | Enable 2FA                       |             200 |
|   9 | Request 2FA login challenge      |             200 |
|  10 | Verify 2FA login                 |             200 |
|  11 | Submit invalid 2FA code          |             401 |
|  12 | Get profile with token           |             200 |
|  13 | Get profile without token        |             401 |
|  14 | Disable 2FA                      |             200 |
|  15 | Exceed authentication rate limit |             429 |

Confirm the request names and expected statuses against the imported collection before presenting this table as a complete description of it.

## Part 2 - Marketplace application

Part 2 adds a React and TypeScript frontend that uses the backend API. The frontend follows the API’s request and response shapes, roles, and booking status transitions.

Payments and transaction amounts are simulated. The application does not process payments or transfer funds.

### Authentication and account security

- Register and log in as a client or freelancer
- Complete the 2FA challenge when enabled
- Set up, enable, disable, and verify 2FA where supported
- Request a password reset and submit a reset token
- View account details and manage the current session
- Restrict administrator pages and actions by role

### Gigs and freelancer profiles

- Browse, search, filter, and sort available gigs
- Inspect gig details, packages, deliverables, prices, and ratings
- Create, edit, activate, deactivate, and soft-delete a freelancer’s own gigs
- Offer Basic, Standard, and Premium packages where configured
- View public freelancer profiles, gigs, and reviews
- Save gigs and follow freelancers where supported

Soft-deleted gigs are hidden from normal listings while remaining available for administrative review and historical booking references.

### Bookings and reviews

- Clients can book an available gig package, track progress, and cancel when the booking status permits it
- Freelancers can view bookings, start work, and mark eligible work as complete
- Booking messages are associated with their booking
- Clients can review completed work; freelancers can review clients where supported
- Booking actions are restricted by role, ownership, and current status

### Transactions and freelancer income

- View the transactions returned for the authenticated account
- Show simulated amounts in ZAR
- View freelancer income summaries and revenue trends
- Export freelancer income history as CSV
- Download booking receipts and transaction statements as PDFs where available

### Notifications and administration

- View notifications and mark them as read
- Review overview, users, gigs, bookings, transactions, and activity in the admin dashboard
- Search, filter, and paginate users
- Suspend or reactivate users
- Moderate gigs, including removed gigs

Administrative actions are restricted to authorized administrator accounts. Public registration cannot create administrator accounts.

## Roles and workflows

| Role              | Main workflow                                                                                                                                    |
| ----------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Freelancer**    | Register -> create and publish a gig -> receive a booking -> start work -> mark it complete -> view completed income                             |
| **Client**        | Register -> browse and filter gigs -> inspect packages and prices -> book a gig -> track progress -> cancel if eligible or review completed work |
| **Administrator** | Sign in to the restricted dashboard -> review users, gigs, bookings, transactions, and activity -> apply moderation actions                      |

### Promote an administrator account

Register a normal account first. From the `backend` folder, run:

```powershell
node -r dotenv/config scripts/adminSetup.js admin@example.com
```

Replace the example email with the registered account’s email. Sign out and sign in again so the account receives a token with the updated role.

## Running the project

Use separate terminals for the backend and frontend. The local-development commands are below. Part 3 also adds a Docker Compose option; run that from the repository root.

### Docker Compose (Part 3)

Before starting the containers, create `backend/.env` with `npm run setup` and generate the local HTTPS certificates in `backend/certs` as described in [Setup instructions](#setup-instructions). The backend container mounts the certificates read-only and mounts `backend/logs` so runtime logs remain available on the host.

From the repository root, run:

```powershell
docker compose up --build
```

Open the frontend at `http://localhost:8080`, the API at `https://localhost:3000/api`, and the local MailHog inbox at `http://localhost:8025`. The backend and frontend run as separate services on the `hustlehub-bridge` bridge network. The allowed browser origins are configured through `CORS_ORIGINS`. The local API certificate is self-signed, so a browser warning is expected during development.

Useful commands, also from the repository root:

```powershell
docker compose ps
docker compose logs -f backend
docker compose down
```

The compose file uses the backend `.env` for application secrets. It does not include a real payment gateway. Never add `.env` values or private certificates to screenshots or source control.

### Backend

```powershell
cd backend
npm install
npm run setup
npm start
```

### Frontend

In a second terminal from the project root:

```powershell
cd frontend
npm install
npm run dev
```

Open the local URL printed by Vite.

### Database requirement

Set `CONN_STRING` to a MongoDB connection string. Booking and transaction operations that use MongoDB transactions require a replica-set-capable deployment, such as MongoDB Atlas or a correctly configured local replica set.

## Testing

### Frontend tests

From the `frontend` directory:

```powershell
npm test
```

To run a specific test file:

```powershell
npm test -- src/pages/BookingsPage.test.tsx
```

To run the production type check and build:

```powershell
npm run build
```

Frontend tests use Vitest and React Testing Library.

**Recorded frontend result:** 18 test files passed; 36 tests passed.

### Backend API tests in Postman

Import `backend/Postman-Collection-Part2.json` into Postman. Start the backend and set the collection’s `baseUrl` to:

```text
https://localhost:3000/api
```

For a local self-signed certificate, Postman may require SSL certificate verification to be disabled in its settings for local testing.

**Recorded Postman result:** 130 tests passed; 0 failed.

### Backend API tests with Newman

From the `backend` directory, with the backend running:

```powershell
.\node_modules\.bin\newman.cmd run .\Postman-Collection-Part2.json --env-var "baseUrl=https://localhost:3000/api" --insecure
```

`--insecure` is for the local development certificate

The collection creates test users, gigs, bookings, and related records in the configured database.

## Email testing with MailHog

MailHog captures email locally without delivering it to a real inbox. If Docker Compose is available, start the MailHog service from the `backend` directory:

```powershell
docker compose up -d mailhog
```

Open the inbox at:

```text
http://localhost:8025
```

Stop the service when finished:

```powershell
docker compose down
```

The backend email settings in `.env` must point to MailHog’s local SMTP service for captured emails to appear in its inbox.

## Project structure

```text
backend/
  controllers/       API request handlers
  middleware/        Authentication, authorization, validation, security, and limits
  models/            Mongoose models
  routes/            Express route definitions
  scripts/           Setup and administration scripts
  utils/             Logging and shared backend utilities
  logs/              Runtime logs; ignored by Git
  certs/             Local HTTPS certificates; generated locally
  Postman-Collection-Part2.json

frontend/
  src/
    components/      Shared navigation and route components
    context/         Authentication context and provider
    models/          Frontend data types
    pages/           Application pages
    services/        API client and request helpers
    test/            Frontend test setup and utilities
    styles/
  vite.config.ts
  package.json

README.md
```

## Known scope and limitations

- Payments are simulated; no real payment processing is implemented.
- The project does not provide professional tax advice or prepare a tax return. **Part 3 adds an approximate estimate based only on completed HustleHub+ platform income; it does not account for a person’s other income, deductions, credits, or payments.**
- Email delivery depends on the configured SMTP provider. MailHog captures messages locally for development.
- Local HTTPS certificates are for development. A deployed environment needs managed certificates and production configuration.
- Frontend controls follow the backend’s available endpoints and booking status rules.
- Newman tests create persistent records in the configured database. The existing `reset-data` script deletes all users, not only test users.

## Assessment evidence

| Check                     | Recorded result                       |
| ------------------------- | ------------------------------------- |
| Newman Part 2 collection  | 130 passed, 0 failed                  |
| Frontend automated tests  | 18 test files passed; 36 tests passed |
| Frontend production build | Passed                                |

## Part 1 screenshots and demo video

The following evidence is for Part 1: screenshots of the Postman requests and the Part 1 demonstration video.

### Part 1 screenshots

| Screenshot                                                                                                                                          | Description                 |
| --------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------- |
| <img width="1089" height="785" alt="Health Check" src="https://github.com/user-attachments/assets/6aa57439-281e-48eb-aebc-bf56fcae3472" />          | Health check (200)          |
| <img width="1087" height="867" alt="Registration" src="https://github.com/user-attachments/assets/37d9a90f-3cc9-4a5f-a8b5-ac185721bfbf" />          | Registration (201)          |
| <img width="1070" height="993" alt="Duplicate email" src="https://github.com/user-attachments/assets/bba02403-a32c-4aba-8885-7bd70fd3c28e" />       | Duplicate email (409)       |
| <img width="1093" height="840" alt="Weak password" src="https://github.com/user-attachments/assets/e7db3a40-13b2-4cab-b47a-536f09e2d33c" />         | Weak password (400)         |
| <img width="1088" height="990" alt="Login" src="https://github.com/user-attachments/assets/77b49b25-32c3-456c-8697-5d4679985bc2" />                 | Login (200)                 |
| <img width="1071" height="1002" alt="Wrong password" src="https://github.com/user-attachments/assets/1c519e4e-2a58-4c39-99c5-2735afeb6666" />       | Wrong password (401)        |
| <img width="1078" height="921" alt="2FA setup" src="https://github.com/user-attachments/assets/dd61ca54-b30a-433c-9d45-2a79a92c2017" />             | 2FA setup (200)             |
| <img width="1086" height="997" alt="2FA enable" src="https://github.com/user-attachments/assets/214118f3-5258-4957-97f4-49339ec326dc" />            | 2FA enable (200)            |
| <img width="1079" height="1012" alt="2FA challenge" src="https://github.com/user-attachments/assets/e942359b-58f0-4e6a-8482-e37ae21a6505" />        | 2FA challenge (200)         |
| <img width="1086" height="993" alt="2FA verify" src="https://github.com/user-attachments/assets/c3b2cd1e-51ee-4c89-90ec-84e6cde70482" />            | 2FA verification (200)      |
| <img width="1063" height="1013" alt="Invalid 2FA" src="https://github.com/user-attachments/assets/d42ff851-1c33-48a4-945c-20cce0ae2606" />          | Invalid 2FA code (401)      |
| <img width="1087" height="988" alt="Profile" src="https://github.com/user-attachments/assets/2ef8905f-2fe5-4fc2-97b2-640badf7fe9c" />               | Profile (200)               |
| <img width="1086" height="989" alt="Profile without token" src="https://github.com/user-attachments/assets/494c075e-2c1e-452c-9ff5-a343a95022a8" /> | Profile without token (401) |
| <img width="1106" height="1017" alt="Rate limit" src="https://github.com/user-attachments/assets/1c00e366-060c-4374-ae7d-a53d0aba7702" />           | Rate limit (429)            |
| <img width="1092" height="990" alt="2FA disabled" src="https://github.com/user-attachments/assets/a99211b8-ebf0-4aa7-a5b6-01df5cb6809a" />          | 2FA disable (200)           |

### Part 1 demonstration video

[Watch the Part 1 demonstration](https://youtu.be/q97q4UyS2wY)

The video shows the server starting, Postman requests, JWT authentication, and HTTPS. The remaining 2FA flow is documented with screenshots because the time-based code expires quickly.

## Part 2 assessment evidence

### Part 2 demonstration video

[Watch the Part 2 demonstration](https://youtu.be/UG0RwO8ZNvc)

### Part 2 backend test evidence

![Part 2 backend Postman tests: 130 passed, 0 failed](https://github.com/user-attachments/assets/776fdbeb-752a-4c6c-b9fb-5181103430db)

### Part 2 frontend test evidence

![Part 2 frontend tests: 18 test files and 36 tests passed](https://github.com/user-attachments/assets/854bdd94-6f64-4eed-8f7f-ce01f0db6e18)


## 🚀 Part 3 - DevSecOps, monitoring and finalisation

Part 3 extends the existing Part 1 backend and Part 2 marketplace. It adds an indicative freelancer tax view, security controls, container deployment, and cloud-capable logging. Payments remain simulated.

### 💰 SARS tax estimation and income dashboard

The freelancer income response includes an estimated tax reserve for the current SARS tax year. The backend applies the individual marginal tax brackets for the schedule implemented in `backend/utils/taxEstimator.js`, calculates tax progressively across the brackets, applies the primary rebate, and returns the estimated tax, effective rate, estimated income after tax, and bracket breakdown. The income used is completed HustleHub+ platform income recorded during the tax year. The frontend presents these values in the freelancer income area and labels the tax year and ZAR amounts. The implementation follows the [SARS individual income-tax rates](https://www.sars.gov.za/tax-rates/income-tax/rates-of-tax-for-individuals/).

This is an estimate for planning and demonstration only. It assumes an individual under 65 and does not include other income, allowable expenses, deductions, credits, or provisional-tax payments. It is not tax advice and must not be treated as the user’s final tax liability. Tax information is shown with freelancer income; clients do not have platform income to estimate.

---

### 🔍 GitHub Actions and static code analysis

Static code analysis examines source code without running the application. It helps identify potentially insecure coding patterns, programming mistakes, and maintainability problems before those changes reach users. This is especially useful for HustleHub+ because the backend handles authentication, account permissions, bookings, and transaction records.

Part 3 distinguishes the following checks rather than treating them as interchangeable:

| Check | Purpose in HustleHub+ | Current repository status |
| --- | --- | --- |
| **🛡️ CodeQL security analysis (SAST)** | Analyse JavaScript/TypeScript code for recognised vulnerabilities and suspicious data flows, such as untrusted inputs reaching sensitive operations. Findings need developer review because automated alerts can be false positives. | **Not yet evidenced.** No CodeQL workflow or scan report is included in the supplied repository. |
| **🧩 Backend syntax analysis** | `npm run build` runs `backend/scripts/checkSyntax.js`, which uses Node.js `--check` on backend `.js` files to catch syntax errors without starting the API. | Implemented; this is a syntax check, **not** a vulnerability scan. |
| **🔠 Frontend TypeScript check** | `npm run build` runs `tsc -b` before Vite builds the frontend, identifying TypeScript type errors. | Configured in `frontend/Package.json`; this does not replace security analysis. |
| **🎨 Prettier formatting check** | `npm run format:check` checks selected files for consistent code formatting. | Configured in the package scripts; formatting alone does not prove code is secure. |
| **🧪 Automated tests** | Backend Node tests and frontend Vitest tests check expected application behaviour, including security-control tests. | Test files and scripts exist; test results should be recorded from the current revision. |

**⚙️ How the planned DevSecOps workflow should work:** A GitHub Actions workflow at `.github/workflows/ci.yml` should run when code is pushed or a pull request is opened. It should install dependencies with `npm ci`, execute backend/frontend tests, run their builds and formatting checks, run CodeQL for the supported languages, and build both Docker images. If a required check fails, the workflow should report a failure. Publishing images to Docker Hub should happen **only after** the required checks pass, using the repository secrets `DOCKERHUB_USERNAME` and `DOCKERHUB_TOKEN`. CodeQL result upload requires an appropriately configured repository and the required `security-events` permission.

**🛠️ Reviewing static-analysis findings:** A developer should inspect each CodeQL alert, note its severity and affected file, trace the potentially unsafe input or operation, and decide whether it is a real problem. A confirmed issue should be fixed in code, checked again, and recorded with the relevant test or pull request. A false positive should be documented with a reason rather than silently ignored. High-risk findings should be resolved before a release. A passing CodeQL scan would reduce risk, but it would not guarantee that the application has no vulnerabilities.

**⚠️ Current limitation:** The supplied ZIP does not contain `.github/workflows/ci.yml` or a CodeQL report. Therefore, this README does **not** claim that GitHub Actions, CodeQL, or Docker Hub publishing has succeeded. The backend formatting script also references the missing workflow file, so the complete formatting check may not pass until the workflow is added. Add the actual run URL, scan configuration, alert summary, and any fixes once the pipeline has been run; results from another project copy are not proof for this repository.

---

### 🐳 Docker and Docker Compose

The root `compose.yml` defines separate `backend`, `frontend`, and local `mailhog` services. The backend and frontend are built from their own Dockerfiles, connected through the `hustlehub-bridge` bridge network, and publish ports 3000 and 8080 respectively. The backend receives its settings from `backend/.env`, uses the configured `CORS_ORIGINS`, mounts local certificates read-only, and maps `backend/logs` to the host. MailHog exposes its web inbox on localhost port 8025 and SMTP inside the compose network.

Run the stack from the repository root:

```powershell
docker compose up --build
```

Check service state and backend logs with `docker compose ps` and `docker compose logs -f backend`. Stop the stack with `docker compose down`. Docker requires a populated `backend/.env` and the local certificate files under `backend/certs`; do not commit either. The frontend uses the build-time `VITE_API_URL` value.

---

### 📊 Logging and cloud log forwarding

**🎯 Purpose of logging:** Logging creates a record of what the application did and when it happened. In HustleHub+, these records help developers investigate failures, confirm that important marketplace operations occurred, and trace security-relevant activity without relying only on what a user sees on screen. This matters for **monitoring** because a sequence of log events can reveal a failing endpoint, recurring errors, or an interrupted booking workflow. It matters for **security** because suspicious patterns, such as repeated failed logins, invalid tokens, or unexpected administrator actions, can be investigated later.

The backend uses the structured logger in `backend/utils/logger.js`. Each JSON entry contains a UTC timestamp, a severity level (`INFO`, `WARN`, or `ERROR`), an event name, and selected safe details. Examples from the application include:

| Logged event | Why it is useful |
| --- | --- |
| 🌐 `api_request` | Records HTTP method, route path, and response status to help identify errors and unusual request activity. Query strings and request bodies are not added by this request logger. |
| 🔑 `login_failed`, `token_validation_failed`, `two_factor_login_failed` | Help investigate failed authentication attempts and possible unauthorised access. |
| 🛡️ `compromised_password_rejected`, `password_screening_unavailable` | Show when password safety checks reject a password or cannot reach the screening service. |
| 🔐 `two_factor_enabled`, `admin_user_status_changed`, `admin_gig_moderated` | Provide a record of important account-security and administrative actions. |
| 💳 `booking_confirmed`, `transaction_recorded` | Help trace the booking process and confirm that a **simulated** transaction was recorded after the database transaction completed. |
| 🚨 `unhandled_error` | Helps diagnose server-side failures without sending internal error details to the client. |

**🗂️ Local storage and monitoring:** Events are written as JSON lines to `backend/logs/app.log`, a directory excluded from Git. When the log reaches approximately 5 MB, the logger rotates it to `app.log.1`, keeping a limited amount of local history. It also maintains a bounded recent-event list and supplies recent entries for the restricted administrator activity view. Docker Compose mounts the log directory so local files remain available on the host. To follow backend output while using containers, run `docker compose logs -f backend` from the repository root.

**🔐 Protecting sensitive information:** The logger sanitises sensitive field names and values before writing or forwarding events. Passwords, tokens, bearer credentials, email addresses, MongoDB connection strings, reset links, and 2FA-related fields are redacted where recognised. The request logger does not capture request bodies or query strings. This reduces the risk of leaking personal information or secrets, although log access still needs to be restricted and sanitisation should be tested when new event fields are introduced. The redaction tests are in `backend/test/logger.test.js`.

**☁️ Optional cloud forwarding:** The backend can also send sanitised JSON events via HTTPS to a configured log-ingestion service using `LOG_INGEST_URL` and `LOG_INGEST_TOKEN` in `backend/.env`. The endpoint must be HTTPS; requests have a three-second timeout. If these variables are not set, events remain local. If cloud delivery fails, the logger reports the failure and continues local logging, so a provider outage does not stop the application. The ingest token must never be committed or exposed to the frontend. Provider retention and usage limits depend on the chosen service; for example, see [Better Stack's HTTP log-ingestion documentation](https://betterstack.com/docs/logs/ingesting-data/http/logs/).

**⚠️ Monitoring limitation:** Logs provide evidence for troubleshooting and investigation, but this repository does not demonstrate a configured cloud dashboard, automatic threat alerting, or a long-term retention policy. For a deployed system, the administrator should review authentication failures, HTTP `401`/`403`/`429` responses, `ERROR` events, and cloud-forwarding failures, and configure alerts and retention with the chosen provider.

**📸 Evidence to capture:** A screenshot of a redacted event in `backend/logs/app.log` and, if cloud forwarding is configured, the matching event in the provider's live tail. Do not include ingestion tokens or real credentials in screenshots.

---

### 🔐 Part 3 additional security features

Part 3 adds two security controls on top of the existing password hashing, JWT authentication, role checks, and optional user 2FA. Both controls are implemented in the backend rather than relying on frontend restrictions alone.

#### 1. 🛡️ Compromised-password screening

**🎯 Purpose:** A password can meet normal length and complexity rules but still be unsafe if it has already appeared in a known data breach. Screening helps prevent users from creating or resetting an account with a password that attackers may already know. It supplements password hashing; it does not replace hashing or password validation.

**⚙️ How it works in HustleHub+:**

1. During registration (`POST /api/auth/register`) and password reset (`POST /api/auth/reset-password`), `backend/controllers/authController.js` calls the utility in `backend/utils/compromisedPassword.js` before accepting the new password.
2. The password is hashed locally with SHA-1 **for the breach lookup only**. The utility sends just the first **five hexadecimal characters** of that hash to the Have I Been Pwned (HIBP) Pwned Passwords range API. It never sends the plain-text password or full hash. The request also asks for padded results to limit information exposure from response size.
3. HIBP returns possible matching hash suffixes. The backend compares the remaining portion of the local hash against this list and checks for a positive breach count. This technique is called **k-anonymity**: the service receives a hash prefix, not the user's exact password. Password storage continues to use the application's existing bcrypt hashing.
4. If a match is found, the API returns **HTTP 400** with `PASSWORD_COMPROMISED` and asks the user to choose another password. If no match is found, normal registration or reset processing continues.
5. A three-second timeout limits how long the app waits for HIBP. A network failure, timeout, or unsuccessful provider response returns **HTTP 503** with `PASSWORD_SCREENING_UNAVAILABLE`. The password change is not accepted until screening is available. This is a **fail-closed** approach: it avoids silently bypassing the check, but it means a provider outage can temporarily block new registrations and password resets.

**🧪 Testing:** `backend/test/compromisedPassword.test.js` covers a breached password, a clean password, and unavailable-service cases (network/abort and unsuccessful responses). It also checks that only the five-character hash prefix is placed in the request URL. The corresponding outcomes are logged as `compromised_password_rejected` or `password_screening_unavailable` without recording the password. See the [HIBP Pwned Passwords API](https://haveibeenpwned.com/API/v3#PwnedPasswords).

#### 2. 🔑 Admin-only multi-factor authentication (MFA) enforcement

**🎯 Purpose:** Administrator accounts can manage users, moderate gigs, and inspect administrative data. A stolen admin password could therefore expose more of the platform than a normal account. Requiring a time-based one-time password (**TOTP**) adds another authentication factor and reduces the risk of password-only compromise.

**⚙️ How it works in HustleHub+:**

1. The middleware `backend/middleware/requireAdminMfa.js` is applied to the admin routes in `backend/routes/adminRoutes.js` **after** JWT authentication and the admin-role check. An admin account with `twoFactorEnabled: false` receives **HTTP 403** and the code `ADMIN_MFA_REQUIRED` instead of access to `/api/admin/*` endpoints.
2. The React app recognises that an administrator has not enrolled in 2FA and directs them to the `/security` page. This page explains the requirement and allows setup through `/api/auth/2fa/setup` and confirmation through `/api/auth/2fa/enable`, using a QR code and an authenticator-generated TOTP code.
3. An existing admin can still sign in with their password and access the authenticated security-setup flow before MFA is enabled. Only protected administrative operations are blocked, avoiding a setup dead end. After enrollment, future logins require the normal password **and** a valid TOTP code to complete the 2FA challenge.
4. The backend also prevents admins from disabling 2FA through `/api/auth/2fa/disable`, returning `ADMIN_MFA_REQUIRED`. The requirement applies specifically to administrators; non-admin users retain the optional 2FA behaviour.

**🧪 Testing:** `backend/test/requireAdminMfa.test.js` checks that an unenrolled administrator is blocked, an enrolled administrator is allowed through, and non-admin accounts are unaffected by this middleware. Frontend tests in `frontend/src/components/ProtectedRoute.test.tsx` and `frontend/src/pages/SecurityPage.test.tsx` cover the admin setup navigation and guidance. The server-side middleware is the actual access-control boundary; a frontend redirect alone would not secure the API.

---

### 🛡️ Part 3 final security review

The final security review considers common risks to a freelance marketplace and maps each risk to controls that are present in the code. It also records what still needs to be checked before deployment. This is a **code-based security review**, not a claim that a penetration test, live attack simulation, or complete CodeQL scan has been performed.

| Risk or threat | Controls reviewed in the implementation | Residual risk and recommended follow-up |
| --- | --- | --- |
| **💉 Injection and unsafe input** | Backend request validators, permitted-field checks, request-size limits, and Mongoose models/query operations constrain input before business logic uses it. | Review every new endpoint for unexpected object fields and unsafe query construction. Complete CodeQL scanning and test malicious inputs. |
| **🌐 Cross-site scripting (XSS)** | React normally renders user content as text; backend text validation, Helmet, and Content Security Policy add defence in depth. | Do not introduce unreviewed raw HTML rendering for gigs, messages, or reviews; check any future rich-text editor separately. |
| **🔓 Weak or breached passwords** | Password validation, bcrypt storage, rate limiting, and Part 3 HIBP range-based screening on registration/reset. | HIBP availability is an external dependency. Verify that the `400` breached-password and `503` unavailable-service paths remain consistent in API-level tests. |
| **🔑 Administrator account takeover** | JWT verification, role-based controls, admin-only TOTP middleware, and protection against disabling an admin's MFA. | Test the whole login/setup/admin-route flow, including expired tokens and attempts to access admin APIs without enrollment. Protect TOTP recovery and encryption keys. |
| **🚫 Broken object-level access control** | Backend ownership and role checks restrict gig, booking, account, and administrative actions. | Re-test cross-account requests after changing endpoints; frontend visibility controls must never be the only authorisation check. |
| **🔒 Secret and personal-data exposure** | Backend configuration keeps secrets out of the frontend; logs redact identified credentials, email addresses, database URIs, and reset/2FA fields. | Review new logging fields, restrict file/provider access, and rotate exposed secrets. Redaction reduces risk but cannot replace access controls. |
| **🚨 Brute-force and abuse** | Authentication, 2FA, and API rate limits; failed authentication and token checks produce log events. | Inspect repeated failures and `429` responses. Logs do not themselves implement blocking, alerting, or an incident-response process. |
| **🐳 Deployment and transport** | Local HTTPS, security headers, CORS configuration, and separate backend/frontend containers in Docker Compose. | Local self-signed certificates are not production certificates. Harden production configuration, image dependencies, and secret management. |
| **📡 Monitoring or third-party outage** | Local JSON logging and bounded rotation continue when optional cloud delivery fails; HIBP errors cause an explicit `503` response. | Add a monitored cloud sink, alert thresholds, and documented log retention if deploying. A prolonged HIBP outage will affect registration and reset availability. |
| **⚠️ Unverified build or dependency risk** | Test scripts, syntax/type/build checks, and Dockerfiles are present. | The supplied repository has no GitHub Actions workflow or CodeQL result. Complete and record the CI/SAST checks and dependency review before claiming release readiness. |

**📋 Review outcome:** The two new Part 3 controls are traceable to their middleware/utilities and associated tests. The main outstanding security-assurance gap is missing CI/CodeQL evidence, together with deployment-specific monitoring and operational checks. No statement in this review should be interpreted as a guarantee that the system is vulnerability-free.

---

### 🧪 Part 3 testing and evidence

To reproduce the existing checks, open a terminal in `backend` and run:

```powershell
npm ci
npm test
npm run build
npm run format:check
```

Run the frontend checks from `frontend`:

```powershell
npm ci
npm test
npm run build
npm run format:check
```

The frontend build performs TypeScript checking before the Vite production build. The backend build performs JavaScript syntax checks; these are not substitutes for CodeQL security analysis. From the repository root, build the images with `docker compose build`. Newman API tests are described in Part 2 and should be executed against a test database because they create records.

**✅ Focused verification performed on the supplied code:** `node --test test/compromisedPassword.test.js test/requireAdminMfa.test.js test/logger.test.js` completed with **8 tests passed and 0 failed**. These tests cover the password-screening helper, the admin MFA middleware, and log sanitisation. This result does **not** mean that the complete backend/frontend suites, Docker build, HIBP live service, or CodeQL scan passed.

| Evidence | Status / what should be recorded |
| --- | --- |
| 🛡️ Compromised-password tests | **✅ Verified locally:** breached, clean, and unavailable screening behaviour (three tests). Add terminal output or a CI run link. |
| 🔑 Admin MFA middleware tests | **✅ Verified locally:** unenrolled admin blocked, enrolled admin allowed, non-admin unaffected (three tests). Add a screenshot of the guided enrollment flow. |
| 📝 Logging sanitisation tests | **✅ Verified locally:** field/value redaction (two tests). Add a sanitised sample from the local log. |
| 🔍 GitHub Actions and CodeQL | **⏳ Pending:** add `.github/workflows/ci.yml`, enable scanning as supported by the repository, and record the workflow URL, query setup, alerts, and fixes. |
| 🧪 Full backend/frontend tests and builds | **⚠️ Not verified in this README update:** add current test counts, build results, and output from the actual revision. |
| 🐳 Docker Compose | **⚠️ Not verified in this README update:** record backend/frontend service startup and health-check evidence. |
| ☁️ Cloud logging | **⚠️ Not verified in this README update:** add a redacted event visible in the chosen provider's live tail. |
| 🎬 Part 3 demonstration video | **⏳ Pending:** link a walkthrough showing authentication, gig creation, booking/completion, simulated transaction, income/tax display, and Part 3 security/deployment checks. |

Keep the linked evidence and results specific to the submitted repository and commit. Do not label a check as successful until the corresponding output confirms it.

---

### 📌 Conclusion

Part 3 extends the marketplace with an indicative SARS-based freelancer tax estimate, additional password and administrator authentication controls, structured local and optional cloud logging, and separate Docker services orchestrated by Compose. The project remains an educational system: payments are simulated, the tax output is approximate, and deployment readiness should be judged from the actual passing pipeline, static-analysis report, container run, and demonstration evidence—not from feature descriptions alone. The CI workflow and Part 3 evidence listed above remain to be completed in this repository.

## Team

| Name                |
| ------------------- |
| AbdulRauf Al-ahkras |
| Ghanief Batchelor   |
| Ebrahim Kadodia     |
| Qays Mahamud        |

## License

This project is for educational purposes. All rights reserved.
