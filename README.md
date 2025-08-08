# GetInitialFunctions

## Overview

This project targets .NET 8 and provides functionality to retrieve initials from a given context using Azure Functions.

## Architecture

The solution consists of two Azure Functions:

## Rebuild Solutions before running the project.

### Function A

- **Trigger:** HTTP request with a JSON payload containing `FirstName` and `LastName`.
- **Actions:**
  - Saves `FirstName` and `LastName` to a database.
  - Publishes a message to an Azure Storage Queue with the same properties.
  - Returns HTTP 200 OK if successful.

### Function B

- **Trigger:** Azure Storage Queue message (from Function A).
- **Actions:**
  - Reads `FirstName` and `LastName` from the queue message.
  - Calls the external API: `https://tagdiscovery.com/api/get-initials?name=<FULL NAME>` where `<FULL NAME>` is the concatenation of `FirstName` and `LastName`.
  - Saves the SVG response to the database, associated with the original names.

## How to Use

1. **Clone the Repository**
2. **Build the Project**
3. **Run the Project**
4. **Use in Your Code**
   - Reference the project or compiled DLL in your .NET 8 solution.
   - Call the provided methods or deploy the Azure Functions as needed.

## Running Tests

There is a Xunit test project. You could run the tests. 

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Azure Functions Core Tools (for local development)
- Access to Azure Storage and a database (e.g., Azure SQL, Cosmos DB)

## Contributing

Feel free to open issues or submit pull requests for improvements.
