using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using eStoreJA.Models;

namespace eStoreJA
{
    public partial class OrderHistory : SecurePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Master.BodyCssClass = "page-teal";
            if (!IsPostBack)
                LoadOrders();
        }

        private void LoadOrders()
        {
            string userId = User.Identity.GetUserId();
            using (var db = new ApplicationDbContext())
            {
                var orders = db.Orders
                    .Include(o => o.Address)
                    .Include(o => o.OrderItems.Select(i => i.Product))
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();

                var rows = orders.Select(o => new OrderHistoryRow
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate.ToString("dd MMM yyyy"),
                    Items = string.Join(", ", o.OrderItems.Select(i =>
                        (i.Product == null ? "Product" : i.Product.Name) + " × " + i.Quantity)),
                    ShipTo = o.Address == null ? "—" :
                        o.Address.City + ", " + o.Address.State + ", " + o.Address.Country,
                    Status = o.Status ?? "Pending",
                    TrackingNumber = o.TrackingNumber,
                    Total = o.OrderItems.Sum(i => i.Quantity * i.UnitPrice)
                }).ToList();

                rptOrders.DataSource = rows;
                rptOrders.DataBind();

                if (rows.Count == 0)
                {
                    lblMessage.Text = "You have not placed any orders yet.";
                    lblMessage.Visible = true;
                }
            }
        }

        private class OrderHistoryRow
        {
            public int OrderId { get; set; }
            public string OrderDate { get; set; }
            public string Items { get; set; }
            public string ShipTo { get; set; }
            public string Status { get; set; }
            public string TrackingNumber { get; set; }
            public decimal Total { get; set; }
        }
    }
}
