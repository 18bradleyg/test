<%@ Page Title="Track Package" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TrackPackage.aspx.cs" Inherits="eStoreJA.TrackPackage" Async="true" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="container py-4" style="max-width: 760px;">
        <h2 class="mb-2">Track Your Package</h2>
        <p class="text-muted">Enter the tracking number from your order confirmation or order history.</p>
        <div class="card shadow-sm">
            <div class="card-body">
                <div class="form-group mb-3">
                    <label for="<%= txtTrackingNumber.ClientID %>">Tracking number</label>
                    <asp:TextBox ID="txtTrackingNumber" runat="server" CssClass="form-control"
                        MaxLength="30" placeholder="e.g. ESTJ261123A1B2C3D4" />
                </div>
                <asp:Button ID="btnTrack" runat="server" Text="Track package"
                    CssClass="btn btn-primary" OnClick="btnTrack_Click" />
                <asp:Label ID="lblMessage" runat="server" CssClass="d-block mt-3" />
            </div>
        </div>

        <asp:Panel ID="pnlResult" runat="server" Visible="false" CssClass="card mt-4">
            <div class="card-body">
                <h4>Shipment details</h4>
                <dl class="row mb-0">
                    <dt class="col-sm-5">Tracking number</dt>
                    <dd class="col-sm-7"><asp:Label ID="lblTracking" runat="server" /></dd>
                    <dt class="col-sm-5">Current status</dt>
                    <dd class="col-sm-7"><asp:Label ID="lblStatus" runat="server" /></dd>
                    <dt class="col-sm-5">Estimated delivery</dt>
                    <dd class="col-sm-7"><asp:Label ID="lblEstimated" runat="server" /></dd>
                    <dt class="col-sm-5">Actual delivery</dt>
                    <dd class="col-sm-7"><asp:Label ID="lblActual" runat="server" /></dd>
                </dl>
            </div>
        </asp:Panel>
        <asp:HyperLink ID="lnkOrders" runat="server" NavigateUrl="~/OrderHistory" CssClass="btn btn-link mt-3">Back to order history</asp:HyperLink>
    </div>
</asp:Content>
