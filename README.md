# eStoreJA – Order Fulfillment System
CIT4009 Enterprise Computing I – Group Project (Scenario 4: eStoreJA → VISA, DHL)

A service-oriented web application where a user logs in, places an order, pays by credit card
(via the Credit Card API) and ships it (via the Shipping API).

## Tech Stack
- **Target:** .NET Framework 4.8.1
- ASP.NET Web Forms (website) + ASP.NET Web API 2 (services)
- ASP.NET Identity (login)
- Entity Framework 6 + SQL Server LocalDB
- Bootstrap

## Projects
| Project | Type | URL | Purpose |
|---|---|---|---|
| eStoreJA.Web | Web Forms | https://localhost:44351/ | Customer website (login, orders, history, tracking) |
| CreditCard.Api | Web API | https://localhost:44352/ | Authorizes card payments (approve/decline) |
| Shipping.Api | Web API | https://localhost:44355/ | Creates shipments and tracks packages |
| Shared.Contracts | Class Library | – | Request/response classes shared by all projects |

Each system has its **own database** and communicates only through web services.

## API Endpoints
| Method | URL | Description |
|---|---|---|
| POST | `https://localhost:44352/api/payments/authorize` | Approve or decline a card payment |
| POST | `https://localhost:44355/api/shipments` | Create a shipment, returns tracking number |
| GET | `https://localhost:44355/api/shipments/{trackingNumber}` | Get shipping status and delivery date |

## Getting Started
1. Clone the repo and open `eStoreJA.sln` in Visual Studio.
2. Build the solution (**Ctrl+Shift+B**). NuGet packages restore automatically.
3. Right-click the solution → **Configure Startup Projects** → **Multiple startup projects** →
   set eStoreJA.Web, CreditCard.Api and Shipping.Api to **Start**.
4. Press **F5**. All three projects should open in the browser.

> API URLs used by the website are set in `eStoreJA.Web/Web.config` under `<appSettings>`.

## Database Setup
Each system has its own database on SQL Server LocalDB (`(localdb)\MSSQLLocalDB`).

### Option A – EF Migrations (recommended)
In Visual Studio: **Tools → NuGet Package Manager → Package Manager Console**, set **Default project** to `eStoreJA`, then run:
```
Update-Database
```
This creates `eStoreJA_DB`, builds the tables and seeds the products and test users.

### Option B – SQL script
1. In SSMS, connect to `(localdb)\MSSQLLocalDB`
2. Open `Database/eStoreJA_DB.sql` and execute it (F5)

### Test logins
| Email | Password |
|---|---|
| john@estoreja.com | Password1! |
| mary@estoreja.com | Password1! |
| admin@estoreja.com | Admin123! (Admin role – opens `/Admin`) |

> CreditCard_DB and Shipping_DB setup steps will be added by Roles 3 and 4.

## Product images (AWS S3)
Admins upload product pictures on **Admin → Products → Edit**. Files go to the `products/` folder of an S3 bucket and the product stores the public URL.

1. Copy `eStoreJA/secrets.example.config` to `eStoreJA/secrets.config` (same folder).
2. Fill in `AwsAccessKeyId`, `AwsSecretAccessKey`, `AwsS3BucketName`, `AwsS3Region` and `AwsS3PublicBaseUrl` (e.g. `https://<bucket>.s3.<region>.amazonaws.com`).
3. Restart the site.

`secrets.config` is git-ignored – **never commit it**. Without it the site still runs; the edit page just says image upload isn't configured.
The bucket needs a policy allowing public `s3:GetObject` on `products/*`, and the IAM user only needs `s3:PutObject` + `s3:DeleteObject` on `products/*`.

## Team Workflow
- **Do not push directly to `master`.** Work on your branch and open a Pull Request.
- **Do not change `Shared.Contracts`** without telling the team. All projects depend on it.
- Pull from `master` regularly to stay up to date.
- Write clear commit messages (e.g. `Add decline logging to CreditCard API`).
