# Role 4 – Implementation Instructions

## 1. Prerequisites

The repository uses classic ASP.NET Web Forms, ASP.NET Web API 2 and .NET Framework 4.8.1. Use a Windows computer with Visual Studio 2022 (ASP.NET and web development workload), SQL Server LocalDB/SQL Server Express, and the .NET Framework 4.8.1 Developer Pack.

**GitHub Codespaces note:** Standard Linux Codespaces cannot run .NET Framework 4.8.1 Web Forms/IIS Express. Codespaces can be used to edit and commit files, but run and test this solution on Windows with Visual Studio/IIS Express.

## 2. Get the project

1. Clone the group's GitHub repository or download its ZIP.
2. Open the solution file (`.sln`) in Visual Studio.
3. Restore NuGet packages if Visual Studio prompts you.
4. Build the solution once before applying changes to identify any pre-existing build issues.

## 3. Add the Role 4 files

Copy the contents of the Role4 package into the matching folders in the repository, preserving the folder structure. When prompted, replace only the matching Role 4 files.

Files included:
- `Shipping.Api/Controllers/ShipmentsController.cs`
- `Shipping.Api/Web.config`
- `Shared.Contracts/ShipmentRequest.cs`
- `eStoreJA/Services/ShipmentSummary.cs`
- `eStoreJA/Services/ShippingRetry.cs`
- `eStoreJA/Checkout.aspx.cs`
- `eStoreJA/OrderHistory.aspx`
- `eStoreJA/OrderHistory.aspx.cs`
- `eStoreJA/OrderHistory.aspx.designer.cs`
- `eStoreJA/TrackPackage.aspx`
- `eStoreJA/TrackPackage.aspx.cs`
- `eStoreJA/TrackPackage.aspx.designer.cs`

The existing project files already include the controller and Web Forms pages in their compile/content lists. No new C# source file needs to be added to the project file.

## 4. Configure the database

1. Start SQL Server LocalDB or the SQL Server instance used by the existing eStoreJA project.
2. Confirm the database `eStoreJA_DB` exists and that the existing `Orders` table has been created.
3. Open `Database/AddShipments.sql` in SQL Server Management Studio (SSMS) or Visual Studio SQL Server Object Explorer.
4. Execute the script against `eStoreJA_DB`.
5. Confirm that `dbo.Shipments` exists and has a foreign key to `dbo.Orders`.

The website's existing `eStoreJA/Web.config` uses a `DefaultConnection` connection string. The supplied `Shipping.Api/Web.config` contains a matching LocalDB example. If the group uses another SQL Server instance, update both connection strings so the website and Shipping API point to the same database.

Do not replace the group's existing database or rerun its full database creation script over a populated database. Run only `Database/AddShipments.sql` for this feature.

## 5. Configure the API URL

The existing website configuration has:

```xml
<add key="ShippingApiUrl" value="https://localhost:44355/" />
```

The Shipping API project's IIS Express SSL port is configured as `44355`. If Visual Studio assigns a different port, update `ShippingApiUrl` in `eStoreJA/Web.config` to match. Keep the trailing slash.

The Credit Card API must also be running for the complete checkout-to-shipping test.

## 6. Build and run

1. In Visual Studio, set the solution's multiple startup projects to start `CreditCard.Api`, `Shipping.Api`, and `eStoreJA`.
2. Build the solution.
3. Start the projects using IIS Express.
4. Confirm the Shipping API is available at `https://localhost:44355/`.
5. Sign in to eStoreJA as a customer.
6. Place an order using valid test payment details supported by the group's Credit Card API.
7. After payment approval, confirm the order is saved and a tracking number appears in the checkout confirmation.
8. Open **Order History** and confirm the signed-in customer's order appears.
9. Select the tracking number or open **Track Package** and search for it.
10. Confirm the API returns the status and estimated delivery date.

## 7. API tests

### Create shipment

Send a `POST` request to `https://localhost:44355/api/shipments` with a valid JSON request body. Use an existing `OrderId` from the database. The API should return a tracking number.

Send the same request a second time. The same tracking number should be returned because the endpoint is idempotent by order.

### Track shipment

Send a `GET` request to:

`https://localhost:44355/api/shipments/{trackingNumber}`

The API should return the stored status and estimated delivery date. A made-up tracking number should return HTTP 404.

### Test delivered status

For a controlled demonstration, update a test shipment in SQL Server:

```sql
UPDATE dbo.Shipments
SET Status = 'Delivered',
    ActualDelivery = SYSUTCDATETIME()
WHERE TrackingNumber = 'REPLACE_WITH_TEST_TRACKING_NUMBER';
```

Refresh Track Package. The actual delivery date should now be displayed.

## 8. GitHub contribution

Create a feature branch before adding the files:

```bash
git checkout -b role4-shipping-tracking
```

After adding the files and testing:

```bash
git status
git add Shipping.Api Shared.Contracts eStoreJA Database Documentation
git commit -m "Implement Role 4 shipping API and customer tracking"
git push -u origin role4-shipping-tracking
```

Open a pull request to the group's main/master branch. Ask the group leader to review and merge it.

## 9. Troubleshooting

- **HTTP 500 from Shipping API:** Check that `dbo.Shipments` exists and the API connection string points to the same database as the website.
- **HTTP 404 during tracking:** Confirm the tracking number is exact and that the shipment row exists.
- **No tracking number after checkout:** Confirm payment was approved, Shipping API is running, and the API URL/SSL port is correct. Orders that cannot reach Shipping API remain `Awaiting Shipment` and can be retried using the group's existing admin retry feature.
- **SQL foreign key error:** The supplied SQL script requires an existing `dbo.Orders` table and a valid OrderId.
- **Page compilation error:** Confirm the `.aspx`, `.aspx.cs` and `.designer.cs` files are all copied to the matching project folders.
- **Codespaces cannot launch the site:** This is expected for the .NET Framework Web Forms/IIS Express application. Use Windows and Visual Studio for runtime testing.

## 10. Assumptions and limitations

- The existing eStoreJA authentication and order database are reused.
- A shipment is created only after payment approval and order persistence.
- Each order has at most one shipment.
- The estimated delivery date is initially five days after shipment creation.
- Shipment statuses are simulated for the coursework demonstration; there is no live DHL carrier integration.
- The SQL Server database is shared by the website and Shipping API.
