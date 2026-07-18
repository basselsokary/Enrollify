# Course Enrollment API

A RESTful backend API for managing course enrollments with integrated payment processing. Built with ASP.NET Core following Clean Architecture and CQRS principles, with PostgreSQL for writes and MongoDB for reads.

---

## Features

- **Course Management** — browse catalog, view available seats, filter by type
- **Enrollment Flow** — enroll in free or premium courses
- **Payment Processing** — Stripe PaymentIntents with idempotency and webhook handling
- **Refunds** — full and partial refund support
- **CQRS with Dual Stores** — PostgreSQL for transactional writes, MongoDB for optimized reads
- **Outbox Pattern** — domain events persisted atomically with writes, processed asynchronously via a `BackgroundService`
- **Webhook Verification** — Stripe signature validation on all incoming events

---

## Tech Stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 10 |
| Architecture | Clean Architecture + CQRS + DDD |
| Write Database | PostgreSQL + EF Core |
| Read Database | MongoDB |
| Payments | Stripe.net |
| ORM | Entity Framework Core |
| Outbox Processor | .NET BackgroundService |

---

## Architecture Overview

```
src/
├── API/                        # Controllers, middleware, DI registration
├── Application/                # Commands, queries, handlers, interfaces
│   ├── Courses/
│   │   ├── Commands/
│   │   └── Queries/
│   └── Enrollments/
│       ├── Commands/
│       └── Queries/
├── Domain/                     # Aggregates, entities, domain events
└── Infrastructure/             # EF Core, MongoDB, Stripe, repositories
    └── Persistence/
    │   ├── WriteContext/                # EF Core DbContext, write repositories
    │   └── ReadContext/            # Read models, read repositories
    └── BackgroundJobs/
        └── OutboxProcessor.cs  # BackgroundService that drains the outbox
```

### CQRS Flow

```
Command (enroll, pay, refund)
    → Handler writes to PostgreSQL
    → Domain events saved to OutboxMessages (same transaction)
    → BackgroundService picks up unprocessed outbox messages
    → Projects read models to MongoDB
 
Query (catalog, user enrollments)
    → Handler reads directly from MongoDB
```

---

## Endpoints

### Courses

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/courses` | Get full course catalog |
| `GET` | `/courses/{id}` | Get course details with available seats |

### Enrollments

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/enrollments/enroll` | Enroll in a course (free or premium) |
| `GET` | `/enrollments` | Get all enrollments for a user |
| `DELETE` | `/enrollments/{id}` | Drop a course |

### Payments

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/payments` | Get all payments for a user |
| `POST` | `/payments/refund/{enrollmentId}` | Issue full or partial refund |

### Webhooks

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/webhooks/stripe` | Handle Stripe events (signature verified) |

---

## Payment Flow

```
POST /enrollments (premium course)
    → Enrollment created (status: AwaitingPayment) → PostgreSQL
    → PaymentIntent created → Stripe
    → clientSecret returned to client

Client confirms payment via Stripe.js

Stripe sends POST /webhooks/stripe
    → Signature verified
    → payment_intent.succeeded
        → Enrollment activated → PostgreSQL
        → Read model updated → MongoDB
    → payment_intent.payment_failed
        → Enrollment marked failed → PostgreSQL
```

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- PostgreSQL
- MongoDB
- Stripe account (test mode)
- [Stripe CLI](https://stripe.com/docs/stripe-cli) (for local webhook testing)

### Configuration

```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "WriteDb": "Host=localhost;Port=5432;Database=Enrollify;Username=postgres;Password=****;",
    "ReadDb": "mongodb://localhost:27017/"
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  }
}
```

### Run the API

```bash
# Run the API (MongoDB collections are auto-created on first write + EF Core migrations)
dotnet run --project src/API
```

### Test Webhooks Locally

```bash
# Forward Stripe events to your local API
stripe listen --forward-to http://localhost:5143/webhooks/stripe
```

---

## Key Implementation Details

### Idempotency

Every Stripe PaymentIntent is created with an idempotency key equal to the `enrollmentId`. Duplicate requests for the same enrollment return the existing PaymentIntent instead of creating a new charge.

```csharp
var requestOptions = new RequestOptions
{
    IdempotencyKey = enrollmentId.ToString()
};
await service.CreateAsync(options, requestOptions);
```

### Webhook Signature Verification

All incoming Stripe webhook events are verified against the webhook secret before processing. Invalid signatures return `400 Bad Request` immediately.

### Outbox Pattern
 
Domain events are persisted to an `OutboxMessages` table in the **same PostgreSQL transaction** as the write, guaranteeing they are never lost even if the process crashes before MongoDB is updated.
 
```
Command Handler
    ├── Save aggregate (PostgreSQL)
    └── Save OutboxMessage (PostgreSQL) ← same transaction
 
OutboxProcessor (BackgroundService)
    ├── Poll for unprocessed OutboxMessages
    ├── Deserialize and dispatch domain event
    ├── Project read model → MongoDB
    └── Mark message as processed
```
 
The `OutboxProcessor` runs as a hosted `BackgroundService`, polling PostgreSQL on a configurable interval via a `Channel<T>` for in-process signaling. This eliminates the need for an external message broker while still decoupling the write from the MongoDB projection.

### Read Model Projection

MongoDB read models are projected inline within command handlers — no message broker required. On any write, the corresponding MongoDB document is upserted in the same handler after the SQL transaction completes.

---

## Stripe Test Cards

| Scenario | Card Number |
|---|---|
| Successful payment | `4242 4242 4242 4242` |
| Card declined | `4000 0000 0000 0002` |
| Requires authentication (3DS) | `4000 0025 0000 3155` |

Use any future expiry date, any 3-digit CVC, and any billing ZIP.
