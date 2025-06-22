# Kottatar - Sheet Music Management API

## Overview

Kottatar is a .NET-based API for managing sheet music and instrument audio files. The application provides a comprehensive solution for storing, retrieving, updating, and deleting musical compositions along with their associated instrument tracks.

## Features

### Music Management

- **Create Music**: Add new musical compositions with title and sheet music file references
- **Retrieve Music**: Get a list of all available music or fetch a specific composition by ID
- **Update Music**: Modify existing musical compositions
- **Delete Music**: Remove compositions from the system

### Instrument Management

- **Add Instruments**: Associate instrument tracks with sheet music
- **Link to Music**: Each instrument is connected to a specific music composition
- **Type Classification**: Organize instruments by their types (e.g., piano, violin, etc.)
- **Audio Files**: Store references to audio files for each instrument

## Technical Architecture

The application follows a clean architecture approach with the following components:

### Projects Structure

1. **Kottatar.Entities**
   - Contains all entity models and DTOs
   - Defines the domain objects like Music and Instrument

2. **Kottatar.Data**
   - Handles database operations through a generic Repository pattern
   - Uses Entity Framework Core with SQL Server database

3. **Kottatar.Logic**
   - Contains business logic for Music and Instrument management
   - Implements data transformation using AutoMapper

4. **Kottatar.Endpoint**
   - Exposes RESTful API endpoints
   - Uses ASP.NET Core Web API with Swagger documentation

### Key Technologies

- **.NET 8**: Latest framework for optimal performance
- **Entity Framework Core**: For data access and ORM
- **AutoMapper**: For mapping between entities and DTOs
- **Swagger/OpenAPI**: For API documentation and testing

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or standard instance)
- Visual Studio 2022 or another compatible IDE

### Setup

1. Clone the repository
2. Ensure your SQL Server instance is running
3. The application is configured to use LocalDB with the connection string:Server=(localdb)\\MSSQLLocalDB;Database=KottatarDb;Trusted_Connection=True;TrustServerCertificate=True4. Run the application from Visual Studio or using `dotnet run` command

### API Usage

Once running, access the Swagger UI to explore and test all available endpoints:
- Navigate to `/swagger` when the application is running
- Available endpoints include:
  - GET/POST/PUT/DELETE operations for music compositions
  - POST operations for adding instruments

## Data Models

### Music
- **Id**: Unique identifier for the music piece
- **Title**: Name of the musical composition
- **SheetMusicFile**: Reference to the sheet music file
- **Instruments**: Collection of associated instruments

### Instrument
- **Id**: Unique identifier for the instrument
- **MusicId**: Reference to the associated music piece
- **Type**: Type of instrument (e.g., piano, violin)
- **AuidoFile**: Reference to the audio file for this instrument (note: property name in code has a typo)