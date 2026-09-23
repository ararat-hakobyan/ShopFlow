# ShopFlow

An e-commerce and delivery application in two halves that talk to each other over HTTP:

* **`backend/`** — an ASP.NET Core 10 Web API with Entity Framework Core and SQL Server.
* **`frontend/`** — the screens, written in plain HTML, CSS and JavaScript.

Each half has its own README with the detail. This file is only about how they fit
together.

---

## How they connect

```
frontend  http://localhost:5500  ──fetch + Bearer token──>  backend  http://localhost:5292
```

They are two separate applications on two different ports, which means two things have to
be arranged for them to work together, and both are already configured:

**CORS.** A browser refuses a request from one origin to another unless the server allows
it. The API lists the addresses it accepts under `Cors:AllowedOrigins` in
`backend/src/ShopFlow/appsettings.json`; `http://localhost:5500` is there because that is
the port Live Server uses.

**The token.** There is no session cookie. `POST /api/account/login` returns a JWT, the
front-end keeps it in `localStorage`, and `js/api.js` attaches it to every later request
as `Authorization: Bearer <token>`. When the API answers 401 the front-end drops the token
and goes back to the sign-in page.

The API address the pages call is in `frontend/js/config.js`.

---

## Quick start on Windows: one double-click

Double-click **`start-shopflow.bat`** in the repository root. It works the same on a fresh
clone and on a copy that is already set up. It:

1. checks that the .NET 10 SDK is installed;
2. checks that SQL Server LocalDB is installed (the API starts `MSSQLLocalDB` by itself);
3. builds the API (this also restores the NuGet packages);
4. starts the API in a window called **ShopFlow API** and waits for `http://localhost:5292`;
5. serves the `frontend` folder on `http://localhost:5500` in a window called
   **ShopFlow Frontend** (`scripts/serve-frontend.ps1`, plain PowerShell, nothing to install);
6. opens the browser on the sign-in page.

If port 5292 or 5500 is already in use, that half is assumed to be running and is not
started again. The first run takes longer because it creates the database.

**To stop**, close the *ShopFlow API* and *ShopFlow Frontend* windows.

Requirements: Windows, the .NET 10 SDK and SQL Server LocalDB (installed with Visual Studio's
*Data storage and processing* workload).

---

## Starting both by hand, step by step

**1. Start the API.** Open `backend/ShopFlow.slnx` in Visual Studio and press **F5**,
or from a terminal:

```bash
cd backend/src/ShopFlow
dotnet run
```

Either way, wait for `Now listening on: http://localhost:5292`; Swagger opens in the
browser. On the first run it creates the database, applies the migrations and seeds an
administrator and a demo catalogue. Leave it running.

**2. Serve the pages.** Open the `frontend` folder in VS Code and press **Go Live**
(the Live Server extension). The browser opens on `http://127.0.0.1:5500`.

**3. Sign in.**

```
admin@shopflow.local / Admin123!
```

Customers create their own account from the *Create account* page. Couriers are created
by an administrator from the dashboard.

---

## Using the API on its own

The API does not need the front-end. With it running, <http://localhost:5292/swagger>
lists every endpoint and lets you call them by hand — sign in, copy the token, press
**Authorize**, and the rest of the endpoints open up. The back-end README walks through it.
