# Role 4 – Shipping Entity Relationship Diagram

```mermaid
erDiagram
    ORDERS ||--o| SHIPMENTS : "has one shipment"

    ORDERS {
        int OrderId PK
        string UserId FK
        int AddressId FK
        datetime OrderDate
        string Status
        string TrackingNumber
    }

    SHIPMENTS {
        int ShipmentId PK
        int OrderId FK,UK
        string CustomerName
        string ProductName
        int Quantity
        string AddressLine1
        string AddressLine2
        string City
        string State
        string Country
        string ZipCode
        string TrackingNumber UK
        string Status
        datetime EstimatedDelivery
        datetime ActualDelivery
        datetime CreatedAtUtc
    }
```

## Relationship

- One `Orders` record can have zero or one `Shipments` record.
- `Shipments.OrderId` is a foreign key to `Orders.OrderId`.
- `Shipments.OrderId` is unique, preventing more than one shipment record for the same order.
- `Shipments.TrackingNumber` is unique and is the public lookup key used by the tracking endpoint.
- The existing `Orders.TrackingNumber` field is retained because the current checkout and order-history workflow already uses it. The same tracking number is saved in `Shipments.TrackingNumber` by the Shipping API.

## Normalization notes

Shipment details are stored in a separate table rather than adding all delivery fields to the Orders table. The shipment row is associated with its order using `OrderId`. Product name and quantity are stored as the shipment request summary required by the assignment; the authoritative individual order lines remain in the existing `OrderItems` table.
