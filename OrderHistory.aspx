<%@ Page Title="Order History" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderHistory.aspx.cs" Inherits="eStoreJA.OrderHistory" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4">
        <h2 class="mb-2">My Order History</h2>
        <p class="text-muted">View your previous orders and track packages that have shipped.</p>
        <asp:Label ID="lblMessage" runat="server" CssClass="alert alert-info d-block" Visible="false" />
        <asp:Repeater ID="rptOrders" runat="server">
            <HeaderTemplate><div class="table-responsive"><table class="table table-striped table-hover align-middle">
                <thead><tr><th>Order</th><th>Date</th><th>Items</th><th>Ship to</th><th>Status</th><th>Tracking</th><th>Total</th></tr></thead><tbody>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>#<%#: Eval("OrderId") %></td>
                    <td><%#: Eval("OrderDate") %></td>
                    <td><%#: Eval("Items") %></td>
                    <td><%#: Eval("ShipTo") %></td>
                    <td><span class="badge bg-secondary"><%#: Eval("Status") %></span></td>
                    <td>
                        <%# string.IsNullOrEmpty(Convert.ToString(Eval("TrackingNumber")))
                            ? "Not available yet"
                            : "<a href=\"" + ResolveUrl("~/TrackPackage?tracking=") + System.Web.HttpUtility.UrlEncode(Convert.ToString(Eval("TrackingNumber"))) + "\">" + System.Web.HttpUtility.HtmlEncode(Convert.ToString(Eval("TrackingNumber"))) + "</a>" %>
                    </td>
                    <td><%#: Eval("Total", "{0:C}") %></td>
                </tr>
            </ItemTemplate>
            <FooterTemplate></tbody></table></div></FooterTemplate>
        </asp:Repeater>
        <asp:HyperLink ID="lnkShop" runat="server" NavigateUrl="~/Shop" CssClass="btn btn-outline-primary mt-3">Continue shopping</asp:HyperLink>
    </div>
</asp:Content>
