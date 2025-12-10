# Agent Instructions

This document provides instructions for AI coding agents working on this repository.

## Project Overview

This is a C# .NET project for the Ed-Fi Sample Data Generator (SDG), which produces realistic, cohesive, and fictional datasets for use in demonstrations and testing.

## Development Environment

### Prerequisites

- .NET 6.0 SDK or later
- A C# compatible IDE (Visual Studio, Visual Studio Code, or JetBrains Rider)

### Setup Steps

1. Clone the repository
2. Navigate to the `src` directory
3. Run `dotnet restore Ed-Fi-SDG.sln` to restore dependencies
4. Run `dotnet build` to build the solution
5. Run `dotnet test` to execute tests

## C# Coding Standards

### General Guidelines

- Follow the existing code style and conventions in the repository
- Adhere to the `.editorconfig` file settings for formatting and style
- Use meaningful variable and method names
- Keep methods small and focused on a single responsibility
- Write XML documentation comments for public APIs
- Prefer composition over inheritance where appropriate

### Naming Conventions

- Use PascalCase for class names, method names, and public properties
- Use camelCase for local variables and private fields
- Use UPPER_CASE for constants
- Prefix interface names with 'I' (e.g., `IService`)
- Use async suffix for asynchronous methods (e.g., `GetDataAsync`)

### Code Organization

- Keep related classes in appropriate namespaces
- One class per file (except for small, tightly coupled helper classes)
- Place unit tests in corresponding test projects
- Use dependency injection for loose coupling

### Error Handling

- Use exceptions for exceptional conditions, not flow control
- Catch specific exceptions rather than generic Exception
- Provide meaningful error messages
- Clean up resources properly using `using` statements or try-finally blocks

### Testing

- Write unit tests for new functionality
- Follow the Arrange-Act-Assert pattern in tests
- Use descriptive test method names that explain what is being tested
- Ensure tests are isolated and can run independently

## Building and Testing

### Build the Solution

```bash
cd src
dotnet build Ed-Fi-SDG.sln
```

### Run Tests

```bash
cd src
dotnet test
```

### Run Specific Tests

```bash
cd src
dotnet test --filter "FullyQualifiedName~YourTestNamespace"
```

## License

This project is licensed under the Apache License, Version 2.0. See the LICENSE file for details.
