# Finanzauto Monorepo

This monorepo contains all the services and applications for the Finanzauto project.

## Project Structure

- `UserMicroservice/` - User management service (.NET 8)
- `PublicationMicroservice/` - Publication management service (.NET 8)
- `finanzautos-frontend/` - Next.js frontend application (TypeScript)

## Technology Stack

- **Backend**: .NET 8 microservices
- **Frontend**: Next.js 15 with TypeScript
- **Database**: SQL Server
- **Containerization**: Docker

## Getting Started

### Prerequisites

- Node.js 22+
- .NET 8 SDK
- Docker and Docker Compose

### Installation

1. Clone the repository:

```bash
git clone <repository-url>
cd finanzauto
```

2. Install frontend dependencies:

```bash
cd finanzautos-frontend
npm install
cd ..
```

3. Start all services using Docker:

```bash
npm run start:all
```

### Development

#### Frontend (Next.js)

```bash
npm run start:frontend
```

#### User Microservice (.NET)

```bash
npm run start:user
```

#### publication Microservice (.NET)

```bash
npm run start:publication
```

### Building

```bash
npm run build:all
```

## Docker Support

The entire application can be run using Docker Compose:

```bash
npm run start:all
```

To stop all services:

```bash
npm run stop:all
```

## Environment Variables

### Frontend

- `NEXT_PUBLIC_USER_MICROSERVICE_URL`: URL for the User Microservice
- `NEXT_PUBLIC_PUBLICATION_MICROSERVICE_URL`: URL for the Publication Microservice

### Backend

- `DB_CONNECTION_STRING`: Connection string for the SQL Server database

## API Documentation

### User Microservice

- `POST /register` - Register a new user
- `POST /login` - Authenticate a user
- `GET /me` - Get current user information
- `PUT /` - Update user information

### Publcation Microservice

- `GET /user` - Get publications for current user
- `POST /` - Create a new publication
- `DELETE /{id}` - Delete a publication
