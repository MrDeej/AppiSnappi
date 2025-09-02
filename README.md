# AppiSnappi Family Application
<img width="368" height="368" alt="image" src="https://github.com/user-attachments/assets/d4c6d486-c144-4a8b-bef8-ed732c211dd9" />


## Overview

Family Application is a collaborative web app designed for families to manage daily activities, events, finances, and communications. It features a shared blackboard for todos, a family events timeline, user wallets with savings goals, notifications, and user profiles. The app is built as a .NET Aspire project, utilizing Blazor for the interactive frontend, CosmosDB for data storage, and Azure infrastructure defined via Bicep templates.

This application promotes family organization by allowing members to track todos, schedule events (e.g., birthdays, vacations), manage personal savings, receive notifications, and view shared timelines. It includes real-time elements like periodic data sorting and session tracking.

## Features

- **Blackboard**: A shared todo list where family members can add, edit, and mark items as performed. Includes modals for item creation and completion.
- **Family Events & Timeline**: Create and view events like birthdays, anniversaries, vacations, medical appointments, and more. Displays a visual timeline with customizable cards for each event type.
- **User Wallets & Savings Goals**: Track personal balances, transactions, incoming funds, and savings goals. Features modals for editing balances and goals, with visual money representations.
- **Notifications**: System for sending and managing notifications (e.g., via push or in-app panel). Includes subscription management and periodic cleanup of old notifications.
- **User Profiles**: Editable profiles with Lottie animations for avatars, user types (e.g., adult/child), and notification panels.
- **Sessions**: Track active user sessions with cleanup services for security and performance.
- **Home Dashboard**: Aggregates family activities, todos, and events with a customizable engine for displaying relevant data.
- **Data Buffering & Services**: Background services for data initialization, periodic tasks (e.g., daily sorting, deleting old notifications), and scoped/global view models for state management.
- **UI Components**: Responsive Blazor components with CSS styling, Lottie animations for engaging visuals (e.g., piggy banks, family icons), and modals for interactive dialogs.
- **Infrastructure**: Azure KeyVault integration for secrets management, with Bicep modules for deployment.

## Technologies Used

- **Backend**: C# with .NET Aspire for orchestration.
- **Frontend**: Blazor (Razor components) for interactive UI, with CSS for styling.
- **Database**: Azure CosmosDB for storing family data, users, events, todos, and notifications. Includes custom JSON converters (e.g., for DateOnly).
- **Infrastructure as Code**: Bicep templates for Azure resources like KeyVault, with role assignments.
- **Animations**: Lottie files for dynamic UI elements (e.g., profiles, events, notifications).
- **Other**: Service workers for PWA capabilities, push notifications via JavaScript interop, and periodic background services.

## Getting Started

### Prerequisites

- .NET 8 SDK or later.
- Azure account (for CosmosDB and KeyVault; optional for local dev with emulators).
- Visual Studio or VS Code with C# extensions.
- Azure CLI (for deployment).

### Installation

1. Clone the repository:
   ```
   git clone https://github.com/MrDeej/AppiSnappi.git
   cd AppiSnappi/src
   ```

2. Restore NuGet packages:
   ```
   dotnet restore FamiliyApplication.AspireApp.sln
   ```

3. Configure appsettings:
   - Update `appsettings.Development.json` in relevant projects with your CosmosDB connection string, Azure KeyVault details, etc.
   - For local development, use the Azure CosmosDB emulator if needed.

### Running Locally

1. Build the solution:
   ```
   dotnet build
   ```

2. Run the app host:
   ```
   dotnet run --project FamiliyApplication.AspireApp.AppHost
   ```
   - This starts the API service, web frontend, and any orchestrated services.
   - Access the web app at `https://localhost:5001` (or check launchSettings.json for ports).

3. For testing:
   - Run unit/integration tests: `dotnet test FamiliyApplication.AspireApp.Tests`.

### Development Tips

- Use the Blazor components in `FamiliyApplication.AspireApp.Web/Components` for UI development.
- Data models are in `CosmosDb/`; extend as needed.
- Periodic services (e.g., `DailySortService.cs`) run in the background—monitor via logs.
- Lottie animations are in `wwwroot/lottie/`; add more via libman.json if required.

## Deployment

The project includes Bicep files for Azure deployment in `FamiliyApplication.AspireApp.AppHost/infra/`.

1. Install Azure CLI and log in: `az login`.

2. Deploy infrastructure:
   ```
   az deployment group create --resource-group <your-rg> --template-file infra/main.bicep --parameters infra/main.parameters.json
   ```
   - Update parameters with your subscription details.

3. Publish the app:
   - Use `dotnet publish` for the web and API projects.
   - Deploy to Azure App Service or containers via the generated `manifest.json` and `azure.yaml`.

For production, secure KeyVault roles and enable HTTPS.

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/YourFeature`.
3. Commit changes: `git commit -m 'Add YourFeature'`.
4. Push to the branch: `git push origin feature/YourFeature`.
5. Open a Pull Request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details. (If no LICENSE file exists, add one.)

## Acknowledgments

- Built with .NET Aspire for modern cloud-native apps.
- Lottie animations enhance user experience.
- Inspired by family collaboration needs.

For questions, open an issue on GitHub. Happy family organizing! 👨‍👩‍👧‍👦
