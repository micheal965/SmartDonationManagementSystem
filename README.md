> ⚠️ **NOTICE: All API keys, credentials, and secrets found in this repository are for testing and development purposes only. Do NOT use them in production.**

# 🤝 Smart Donation Management System

A full-stack donation management platform built with **Angular** (frontend) and **.NET / C#** (backend), featuring real-time notifications, secure payment processing, and a modern web interface.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Features](#features)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Backend Setup](#backend-setup)
  - [Frontend Setup](#frontend-setup)
- [Environment Variables](#environment-variables)
- [Payment Integration](#payment-integration)
- [Real-Time Notifications](#real-time-notifications)
- [API Reference](#api-reference)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

The Smart Donation Management System is designed to streamline the process of managing donations, donors, and campaigns. It provides administrators with a real-time dashboard, donors with a smooth payment experience, and supports notifications across the platform.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Angular, TypeScript, RxJS |
| Backend | .NET / C#, Minimal APIs |
| Real-Time | SignalR |
| Payments | Paymob (iframe & mobile wallet) |
| Database | SQL Server / Entity Framework Core |
| Auth | JWT Bearer Tokens |

---

## Features

- 🔔 **Real-time notifications** via SignalR
- 💳 **Payment processing** via Paymob (card iframe + mobile wallet)
- 👥 **Donor & campaign management**
- 🔐 **JWT-based authentication & authorization**
- 📊 **Admin dashboard** with live updates
- 🔊 **Audio notification support** for new donation events
- 📱 **Mobile-friendly** UI

---

## Project Structure

```
SmartDonationManagementSystem/
├── SmartDonationSystemClient/     # Angular frontend
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/              # Services, guards, interceptors
│   │   │   ├── features/          # Feature modules (donations, campaigns, etc.)
│   │   │   └── shared/            # Shared components & utilities
│   │   └── environments/
│   └── angular.json
│
└── SmartDonationAPI/              # .NET backend
    ├── Controllers/ (or Endpoints/)
    ├── Hubs/                      # SignalR hubs
    ├── Services/
    ├── Models/
    └── appsettings.json
```

---

## Getting Started

### Prerequisites

- [Node.js](https://nodejs.org/) v18+
- [Angular CLI](https://angular.io/cli) v17+
- [.NET SDK](https://dotnet.microsoft.com/) 8.0+
- SQL Server (local or remote)

---

### Backend Setup

```bash
# Clone the repository
git clone https://github.com/michealghobrial/SmartDonationManagementSystem.git
cd SmartDonationManagementSystem/SmartDonationAPI

# Restore packages
dotnet restore

# Apply migrations
dotnet ef database update

# Run the API
dotnet run
```

The API will start at `https://localhost:7000` by default.

---

### Frontend Setup

```bash
cd SmartDonationSystemClient

# Install dependencies
npm install

# Start the dev server
ng serve
```

The app will be available at `http://localhost:4200`.

---

## Environment Variables

### Backend — `appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=SmartDonationDB;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "YOUR_TEST_JWT_SECRET_KEY",
    "Issuer": "SmartDonationAPI",
    "Audience": "SmartDonationClient"
  },
  "Paymob": {
    "ApiKey": "YOUR_TEST_PAYMOB_API_KEY",
    "IntegrationId": "YOUR_TEST_INTEGRATION_ID",
    "IframeId": "YOUR_TEST_IFRAME_ID",
    "HmacSecret": "YOUR_TEST_HMAC_SECRET"
  }
}
```

### Frontend — `src/environments/environment.ts`

```typescript
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7000/api',
  signalRUrl: 'https://localhost:7000/hubs'
};
```

> ⚠️ Never commit real secrets. Use environment-specific config or a secrets manager in production.

---

## Payment Integration

This project integrates **Paymob** for payment processing:

- **Card payments** — via Paymob iframe embed
- **Mobile wallet** — direct API integration
- **Webhook validation** — HMAC signature verification on all incoming callbacks

The payment flow:
1. Authenticate with Paymob → get auth token
2. Register order → get order ID
3. Generate payment key → get iframe token
4. Render iframe or trigger wallet API
5. Receive & validate webhook callback

---

## Real-Time Notifications

Real-time features are powered by **ASP.NET Core SignalR**:

- Clients connect to `/hubs/notifications` on app startup
- Donation events broadcast live to connected admins
- Audio notifications play on new donation events (browser autoplay policy handled)
- Infinite scroll on notification dropdown with scoped `IntersectionObserver`

---

## API Reference

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Authenticate user |
| GET | `/api/donations` | List all donations |
| POST | `/api/donations` | Create a new donation |
| GET | `/api/campaigns` | List campaigns |
| POST | `/api/payments/initiate` | Start Paymob payment flow |
| POST | `/api/payments/webhook` | Handle Paymob callback |

Full Swagger docs available at: `https://localhost:7000/swagger`

---

## Contributing

1. Fork the repo
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m 'Add your feature'`
4. Push to the branch: `git push origin feature/your-feature`
5. Open a Pull Request

---

## License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

---

> Built by [Micheal Ghobrial](https://github.com/michealghobrial)
