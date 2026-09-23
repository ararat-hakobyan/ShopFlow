# ShopFlow front-end

The screens for the ShopFlow API: plain HTML, CSS and JavaScript. No framework, no build
step, no dependencies to install.

Every page asks the API for what it needs and draws it. The token returned by sign-in is
kept in `localStorage` and sent as an `Authorization` header on every request.

---

## Running it

The API has to be running first (see the back-end README). Then the pages have to be
served over http — a browser blocks `fetch` from a page opened straight off the disk, so
double-clicking `index.html` will not work.

On Windows the easiest way is to double-click `start-shopflow.bat` in the repository root:
it starts the API, serves this folder on `http://localhost:5500` with
`scripts/serve-frontend.ps1` and opens the browser. See the root README.

To serve the pages yourself, use the **Live Server** extension for VS Code:

1. Install *Live Server* (by Ritwick Dey) from the Extensions panel.
2. Open this `frontend` folder in VS Code.
3. Right-click `index.html` → **Open with Live Server**, or press **Go Live** in the
   status bar.
4. The browser opens on `http://127.0.0.1:5500`.

`.vscode/settings.json` pins Live Server to port 5500 and to `index.html`, so it always
opens on the sign-in page whichever file you started from. Port 5500 is the one the API's
CORS policy allows. Any other static server works too — just add its address to
`Cors:AllowedOrigins` in the API's `appsettings.json`.

Sign in with the seeded administrator:

```
admin@shopflow.local / Admin123!
```

---

## Where the API address lives

`js/config.js`:

```js
window.ShopFlowConfig = {
    apiBaseUrl: 'http://localhost:5292/api'
};
```

Change it if the API runs on a different port.

---

## The pages

| File | What it is |
| --- | --- |
| `index.html` | Sends the visitor to the page that belongs to their role. |
| `login.html`, `register.html`, `access-denied.html` | The account pages. |
| `shop.html` | The customer screen: catalogue, basket, order history. |
| `admin.html` | The administrator dashboard: orders, catalogue, couriers. |
| `courier.html` | The courier console: available, active and delivered orders. |

## The scripts

| File | What it does |
| --- | --- |
| `js/config.js` | The address of the API. |
| `js/auth.js` | Keeps the token, guards a page by role. |
| `js/api.js` | `fetch` with the token attached and one common answer shape. |
| `js/format.js` | Money, dates and status badges. |
| `js/alerts.js` | The green and red banners above a page. |
| `js/forms.js` | Puts validation messages under the field they belong to. |
| `js/layout.js` | Draws the navigation bar and the footer. |
| `js/order-details.js` | The body of the order history modal. |
| `js/shop.js`, `js/admin.js`, `js/courier.js` | One file per screen. |

Styling is Bootstrap 5 from a CDN plus `css/site.css`.
