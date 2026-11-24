<%@ Page Title="Cart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="eStoreJA.Cart" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Import Namespace="eStoreJA.Services" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="<%: ResolveUrl("~/Content/shop.css") %>" rel="stylesheet" />

    <section class="shop-band">
        <div class="container shop-band-row">
            <h1 class="shop-title">Your <em>cart.</em></h1>
            <% if (Lines.Count > 0) { %><p class="shop-count"><%: ItemCountText %></p><% } %>
        </div>
    </section>

    <%-- Screen readers hear what the last button did (the page reloads after every change) --%>
    <p class="visually-hidden" role="status"><%: Status %></p>

    <div class="shop-body">
        <% if (Notices.Count > 0) { %>
        <div class="shop-alert" role="alert">
            <strong>Your cart was updated</strong>
            <ul><% foreach (string n in Notices) { %><li><%: n %></li><% } %></ul>
        </div>
        <% } %>

        <% if (Lines.Count == 0) { %>
        <%-- ===== Empty cart ===== --%>
        <section class="cart-empty">
            <span class="empty-circle" aria-hidden="true">
                <svg class="shop-svg" viewBox="0 0 24 24"><path d="M3 4h2l2.2 10.2a1.5 1.5 0 0 0 1.5 1.2h8.6a1.5 1.5 0 0 0 1.5-1.1L20.5 8H6.1" /><circle cx="9.5" cy="19.5" r="1.3" /><circle cx="17" cy="19.5" r="1.3" /></svg>
            </span>
            <h2 class="shop-h2">Your cart is <em>empty.</em></h2>
            <p>Find something you love and it&rsquo;ll show up here.</p>
            <a class="btn-pink" href="<%: ResolveUrl("~/Shop") %>">Start shopping &rarr;</a>
        </section>
        <% } else { %>

        <div class="cart-layout">
            <%-- ===== Lines. Every button is a POST (name = action, value = product id); Page_Load applies it
                 to the quantity stored in the cart (never one from the page) and redirects back. ===== --%>
            <section class="cart-card" aria-label="Items in your cart">
                <% foreach (var line in Lines) {
                       string url = ResolveUrl("~/Product?id=" + line.ProductId);
                       int max = System.Math.Min(line.StockQuantity, CartService.MaxQuantity); %>
                <div class="cart-row">
                    <a class="c-pic" href="<%: url %>" tabindex="-1" aria-hidden="true"><%: ProductImage.Render(this, line.ImageUrl, line.Name, line.CategoryName, 104, 104, "pimg-thumb") %></a>
                    <div class="c-info">
                        <span class="mono-label"><%: line.CategoryName %></span><br />
                        <a class="c-name" href="<%: url %>"><%: line.Name %></a>
                        <span class="c-each"><%: CartService.Price(line.UnitPrice) %> each</span>
                        <% if (line.Quantity >= line.StockQuantity) { %><span class="c-max">Only <%: line.StockQuantity %> in stock</span><% } %>
                    </div>
                    <div class="stepper" role="group" aria-label="Quantity of <%: line.Name %>">
                        <button type="submit" name="dec" value="<%: line.ProductId %>" aria-label="Decrease quantity of <%: line.Name %>" <%= line.Quantity <= 1 ? "disabled" : "" %>>&minus;</button>
                        <span class="stepper-value"><%: line.Quantity %></span>
                        <button type="submit" name="inc" value="<%: line.ProductId %>" aria-label="Increase quantity of <%: line.Name %>" <%= line.Quantity >= max ? "disabled" : "" %>>+</button>
                    </div>
                    <span class="c-total"><%: CartService.Price(line.LineTotal) %></span>
                    <button type="submit" class="c-remove" name="remove" value="<%: line.ProductId %>" aria-label="Remove <%: line.Name %>">
                        <svg class="shop-svg" viewBox="0 0 24 24" aria-hidden="true"><path d="M4 7h16" /><path d="M10 11v6M14 11v6" /><path d="M6 7l1 13h10l1-13" /><path d="M9 7V4h6v3" /></svg>
                    </button>
                </div>
                <% } %>
                <a class="link-pink cart-continue" href="<%: ResolveUrl("~/Shop") %>">&larr; Continue shopping</a>
            </section>

            <%-- ===== Navy summary ===== --%>
            <aside class="sum-card" aria-labelledby="sumTitle">
                <h2 class="sum-title" id="sumTitle">Order <em>summary</em></h2>
                <p class="sum-row"><span>Items (<%: ItemCount %>)</span><span><%: CartService.Price(Subtotal) %></span></p>
                <p class="sum-row"><span>Shipping</span><span>Tracked, arranged after payment</span></p>
                <div class="sum-total"><span>Total</span><strong><%: CartService.Price(Subtotal) %></strong></div>
                <a class="btn-pink" href="<%: ResolveUrl("~/Checkout") %>">Checkout &rarr;</a>
                <p class="sum-lock">
                    <svg class="shop-svg" viewBox="0 0 24 24" aria-hidden="true"><rect x="5" y="11" width="14" height="10" rx="2" /><path d="M8 11V8a4 4 0 0 1 8 0v3" /></svg>
                    Secure card payment at checkout
                </p>
            </aside>
        </div>
        <% } %>
    </div>
</asp:Content>
