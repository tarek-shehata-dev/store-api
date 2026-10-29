# Store API

An e-commerce backend built with **ASP.NET Core 8 Web API**, following a layered (clean-architecture style) structure. It covers the full shopping flow: browsing products, a guest-friendly basket, authentication, orders, and card payments with Stripe.

## Features

- **Authentication & authorization** with ASP.NET Core Identity and JWT bearer tokens (register, login, current user, saved address, account lockout after repeated failed logins).
- **Products** with search, filtering by brand and type, sorting by price/name, and pagination.
- **Basket** stored in **Redis** (30-day expiry). Guests can build a basket without logging in; login is required at checkout.
- **Orders**: server-side price calculation from the database (client-sent prices are never trusted), delivery methods, order history per user.
- **Stripe payments**: PaymentIntent creation/update per basket, plus a signed webhook that updates the order status (`Payment Received` / `Payment Failed`).
- **Response caching** for product listings using a custom `[Cached]` action filter backed by Redis.
- **Global exception middleware** and consistent JSON error responses (including validation errors).
- Swagger UI in Development.

## Tech stack

| Area | Technology |
|---|---|
| Framework | .NET 8, ASP.NET Core Web API |
| Database | SQL Server, Entity Framework Core 8 (code-first migrations, seeding, Fluent API configurations) |
| Cache / Basket | Redis (StackExchange.Redis) |
| Auth | ASP.NET Core Identity, JWT Bearer |
| Payments | Stripe.net |
| Mapping | AutoMapper |
| Docs | Swagger / Swashbuckle |

## Architecture

```
Store.sln
├── Store.APIs         Controllers, middleware, filters, DI/startup configuration
├── Store.Service      Business logic (Product, Basket, Order, Payment, Token, User, Cache services)
├── Store.Repository   EF Core DbContexts, migrations, Generic Repository, Unit of Work, seeding
└── Store.Core         Entities, DTOs, contracts (interfaces), specifications, AutoMapper profiles
```

Patterns used: **Generic Repository**, **Unit of Work**, **Specification pattern** (filtering, sorting, paging, includes), **Dependency Injection**, DTO mapping with AutoMapper.

## API overview

🔒 = requires `Authorization: Bearer <token>`

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/Accounts/register` | Create an account, returns a JWT |
| POST | `/api/Accounts/login` | Log in, returns a JWT |
| GET | `/api/Accounts/GetCurrentUser` 🔒 | Current user info |
| GET / PUT | `/api/Accounts/Address` 🔒 | Get / create-or-update the user's address |
| GET | `/api/Products` 🔒 | List products. Query: `search`, `sort` (`priceAsc`, `priceDesc`), `brandId`, `typeId`, `pageIndex`, `pageSize` |
| GET | `/api/Products/{id}` | Product details |
| GET | `/api/Products/types` 🔒 | Product types |
| GET | `/api/Products/brands` 🔒 | Product brands |
| GET | `/api/Basket?id={basketId}` | Get a basket (empty basket if none exists) |
| POST | `/api/Basket` | Create or update a basket |
| DELETE | `/api/Basket?id={basketId}` | Delete a basket |
| GET | `/api/Orders/DeliveryMethod` | Available delivery methods |
| POST | `/api/Orders` 🔒 | Create an order from a basket |
| GET | `/api/Orders` 🔒 | Orders of the current user |
| GET | `/api/Orders/{orderId}` 🔒 | One order of the current user |
| POST | `/api/Payment/{basketId}` 🔒 | Create/update the Stripe PaymentIntent for a basket |
| POST | `/api/Payment/webhook` | Stripe webhook (verified by signature) |

Typical checkout flow: **fill basket → (optional) create PaymentIntent → create order → pay with the returned `clientSecret` → webhook marks the order as paid.**

## Getting started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or full)
- Redis running on `localhost:6379`
- A [Stripe](https://stripe.com) account in **test mode** (API keys + a webhook secret)

### 1. Clone

```bash
git clone https://github.com/tarek-shehata-dev/store-api.git
cd Store
```

### 2. Configure secrets

Non-sensitive settings live in `Store.APIs/appsettings.json`. Sensitive values are **not** committed; use [user-secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets). See `Store.APIs/appsettings.Example.json` for every key the app expects.

```bash
cd Store.APIs
dotnet user-secrets set "ConnectionStrings:DefaultConnection"  "Server=YOUR_SERVER;Database=Store.App;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "ConnectionStrings:IdentityConnection" "Server=YOUR_SERVER;Database=Store.Identity;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "JWT:Key"                "a-long-random-string-of-at-least-32-characters"
dotnet user-secrets set "Stripe:SecretKey"       "sk_test_..."
dotnet user-secrets set "Stripe:PublishableKey"  "pk_test_..."
dotnet user-secrets set "Stripe:WebhookSecret"   "whsec_..."
```

### 3. Run

```bash
cd Store.APIs
dotnet run --launch-profile https
```

On startup the app applies the EF Core migrations to both databases and seeds products, brands, types, delivery methods and a demo user. Open Swagger at `https://localhost:7154/swagger`.

> **Note:** the seed files are read using a path relative to the `Store.APIs` folder (Windows-style), so run the app from `Store.APIs` (or from Visual Studio).

**Demo account:** `admin@store.com` / `P@ssW0rd`

### Stripe webhook (local)

Install the [Stripe CLI](https://stripe.com/docs/stripe-cli) and forward events to the API:

```bash
stripe listen --forward-to https://localhost:7154/api/Payment/webhook
```

Use the signing secret it prints as `Stripe:WebhookSecret`. Test card: `4242 4242 4242 4242`.

## Design notes

- **Guest basket:** baskets are identified by a client-generated, unguessable id (use a GUID). Authentication is only required from checkout on (orders and payments).
- **Server-side pricing:** order totals and PaymentIntent amounts are computed from database prices, not from the values in the basket.
- **Idempotent checkout:** creating an order for a basket that already has a PaymentIntent reuses it and replaces the previous pending order for that intent.
- **Input validation:** orders are rejected (400) for empty baskets, unknown products or delivery methods, and non-positive quantities.

## Roadmap

- Unit and integration tests
- Docker Compose for SQL Server + Redis
- Role-based authorization and an admin area for managing products
- Refresh tokens

## Screenshots

<!-- Add Swagger / Postman screenshots here -->

## Author

**Tarek Shehata** · [GitHub](https://github.com/tarek-shehata-dev)
