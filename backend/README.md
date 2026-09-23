# ShopFlow API

An e-commerce and delivery API built with **ASP.NET Core 10** and **Entity Framework Core 10**.

Three roles share one database:

| Role | What they can do |
| --- | --- |
| **Customer** | Browse the catalogue, fill a basket, place orders and follow them |
| **Courier** | Pick up unassigned orders and mark them as delivered |
| **Admin** | Manage categories, products, variants, prices, couriers and order status |

The API is complete on its own and can be used entirely from Swagger. A separate
front-end consumes it; see [Connecting a front-end](#connecting-a-front-end).

---

## Running it

### 1. Requirements

* .NET 10 SDK
* SQL Server (LocalDB, a full instance, or the Docker image)

### 2. Configuration

`appsettings.Development.json` carries a LocalDB connection string, a development signing
key and a seeded administrator password, so the project runs on a fresh machine with no
setup. They are development values only and reach nothing outside your own computer.

For any other environment provide the real values through user secrets:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=ShopFlow;User Id=sa;Password=<your password>;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:SigningKey" "<a random string of at least 32 characters>"
```

The application refuses to start if the signing key is missing or shorter than 32 bytes.

### 3. Start

On Windows you can double-click `start-shopflow.bat` in the repository root: it builds and
starts this API, serves the front-end and opens the browser. See the root README.

From a terminal:

```bash
cd src/ShopFlow
dotnet run
```

On start-up it applies the migrations and, on an empty database, seeds an administrator
plus a small demo catalogue. It then listens on `http://localhost:5292` and opens Swagger.

**Seeded administrator** (development only):

```
admin@shopflow.local / Admin123!
```

### 4. Try it from Swagger

1. Open <http://localhost:5292/swagger>.
2. `POST /api/account/login` → **Try it out** → send `{ "email": "admin@shopflow.local", "password": "Admin123!" }`.
3. Copy the `token` from the response.
4. Press **Authorize** at the top right, paste the token, confirm.
5. Every other endpoint now works, for example `GET /api/admin/dashboard`.

Customers register themselves through `POST /api/account/register`. Couriers are created
by an administrator.

### 5. Run the tests

```bash
dotnet test
```

---

## Endpoints

Everything except registering and signing in needs `Authorization: Bearer <token>`.

| Method | Path | Role |
| --- | --- | --- |
| POST | `/api/account/register` | – |
| POST | `/api/account/login` | – |
| POST | `/api/account/logout` | – |
| GET | `/api/account/me` | any |
| GET | `/api/shop/storefront?categoryId=&productId=` | Customer |
| POST | `/api/shop/cart/items` | Customer |
| POST | `/api/shop/cart/items/remove` | Customer |
| POST | `/api/shop/orders` | Customer |
| GET | `/api/shop/orders/{id}` | Customer |
| GET | `/api/admin/dashboard?search=` | Admin |
| POST | `/api/admin/categories` | Admin |
| POST | `/api/admin/products` | Admin |
| POST | `/api/admin/variants` | Admin |
| POST | `/api/admin/couriers` | Admin |
| PUT | `/api/admin/orders/status` | Admin |
| PUT | `/api/admin/variants/price` | Admin |
| DELETE | `/api/admin/products/{id}` | Admin |
| DELETE | `/api/admin/couriers/{id}` | Admin |
| GET | `/api/courier/console` | Courier |
| POST | `/api/courier/orders/{id}/accept` | Courier |
| POST | `/api/courier/orders/{id}/complete` | Courier |

A command answers with `{ "success": true, "message": "..." }`. A failure keeps the same
shape and adds `errors`, a field name to messages map, so a client can show each message
under the input it belongs to.

---

## Connecting a front-end

The API accepts cross-origin requests only from the addresses listed under
`Cors:AllowedOrigins` in `appsettings.json`. Out of the box that is
`http://localhost:5500`, which is the port the Live Server extension for VS Code uses.
Add your own address there if you serve your pages elsewhere.

A client signs in, keeps the returned token, and sends it on every request:

```js
const response = await fetch('http://localhost:5292/api/shop/storefront', {
    headers: { Authorization: `Bearer ${token}` }
});
```

The reference front-end for this API is a small HTML/CSS/JavaScript project kept
alongside this one.

---

## How it is put together

```
backend/
├── ShopFlow.slnx
├── src/ShopFlow/
│   ├── Controllers/    HTTP entry points, one per area
│   ├── Services/       business logic, interfaces, token service
│   ├── DAL/            DbContext, entity configurations, repositories, seeding
│   ├── Models/         EF Core entities and the interfaces they implement
│   ├── DTOModels/      request and response objects
│   ├── Mappings/       entity → DTO extension methods
│   ├── Validators/     FluentValidation rules
│   ├── Enums/          OrderStatus, UserRole
│   ├── Common/         Result<T>, ErrorType, ApiResponse
│   ├── Extensions/     DI registration and small helpers
│   ├── Web/            JWT options, validation filter, exception handler
│   └── Migrations/     EF Core migrations
└── tests/ShopFlow.Tests/
```

**Request flow**

```
Client --Bearer token--> Controller → Service → Repository / UnitOfWork → DbContext → SQL Server
                                          ↑
                                    returns Result / Result<T>
```

| Pattern | Where | Why it is there |
| --- | --- | --- |
| **Repository** | `DAL/Repositories` | Services describe *what* data they need; the query syntax stays in one place. A generic `Repository<T>` holds the shared plumbing and each repository adds only its own queries. |
| **Unit of Work** | `DAL/Repositories/UnitOfWork.cs` | All repositories share one `DbContext` and there is exactly one `SaveChangesAsync`, so a use case such as "place an order" commits as a single transaction. |
| **Result object** | `Common/Result.cs` | A service returns *why* something failed (`NotFound`, `Conflict`, `Forbidden`, `Validation`) instead of a bare `bool`, so `ApiControllerBase` can pick the right status code without exceptions being used for control flow. |
| **DTO + mapping extensions** | `DTOModels`, `Mappings` | Entities never leave the server, so the database shape and the wire shape can change independently. |
| **Abstractions for the environment** | `IDateTimeProvider`, `IPasswordHasher`, `ICurrentUser` | Time, hashing and "who is asking" sit behind interfaces, which is what makes the token service testable with a fixed clock. |

EF Core's `DbContext` is itself a repository and a unit of work, so for a project this
size the `DAL/Repositories` layer is more than strictly necessary. It is kept because it
makes each query easy to find and because the interfaces give the services something
small to depend on.

---

## Security

* Passwords are stored as PBKDF2 hashes (ASP.NET Core Identity's `PasswordHasher`), never in plain text.
* Sign-in answers with the same message for an unknown email and for a wrong password, so the endpoint cannot be used to find out which accounts exist.
* Authorization is **deny by default**: a fallback policy requires an authenticated user, and each controller states the role it needs.
* The identity of the caller is always read from the token (`ICurrentUser`). No action trusts a user id coming from the request body, which is what stops one customer from acting on another customer's basket or orders.
* Unhandled exceptions are logged with a trace id; the caller only receives a neutral message.
* The only secrets in this repository are development placeholders. Real values belong in user secrets or environment variables, which the configuration reads first.

---

## Business rules worth knowing

* An order copies the price of each variant at the moment it is placed, so later price changes never rewrite order history.
* The order total is calculated from its lines by the domain itself (`Order.RecalculateTotal`).
* Stock is checked and reduced inside the same transaction that creates the order.
* Products and variants are **soft deleted** and hidden by a global query filter, so historical orders keep their data. A product that still sits in a basket or an open order cannot be removed.
* An order can only be accepted by one courier — the domain refuses a second assignment.

---

## Database

The schema is managed by EF Core migrations in `src/ShopFlow/Migrations`.

```bash
cd src/ShopFlow
dotnet ef migrations add <Name>
dotnet ef database update
```

Main tables: `Users`, `Couriers`, `Categories`, `Products`, `ProductVariants`,
`Baskets`, `BasketItems`, `Orders`, `OrderDetails`.

---

## Tech stack

ASP.NET Core 10 · Entity Framework Core 10 (SQL Server) · FluentValidation ·
JWT bearer authentication · Swagger · xUnit
