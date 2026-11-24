<%@ Page Title="Product" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Product.aspx.cs" Inherits="eStoreJA.ProductPage" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Import Namespace="eStoreJA.Services" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="<%: ResolveUrl("~/Content/shop.css") %>" rel="stylesheet" />

    <% if (Item == null) { %>
    <%-- Unknown, invalid or hidden id (the response status is 404) --%>
    <section class="not-found">
        <h1 class="shop-h2">Product <em>not found.</em></h1>
        <p>This product doesn't exist or is no longer for sale.</p>
        <a class="btn-pink" href="<%: ResolveUrl("~/Shop") %>">Browse the shop &rarr;</a>
    </section>
    <% } else { %>

    <nav class="crumbs pt-3" aria-label="Breadcrumb">
        <a href="<%: ResolveUrl("~/") %>">Home</a> /
        <a href="<%: ResolveUrl("~/Shop") %>">Shop</a> /
        <a href="<%: CategoryUrl %>"><%: Item.Category.Name %></a> /
        <span aria-current="page"><%: Item.Name %></span>
    </nav>

    <div class="pd">
        <div class="pd-pic">
            <%: ProductImage.Render(this, Item.ImageUrl, Item.Name, Item.Category.Name, 720, 520, "pimg-hero") %>
        </div>

        <div>
            <div class="pd-label-row">
                <span class="mono-label"><%: Item.Category.Name %></span>
                <% if (Item.IsFeatured) { %><span class="tag-featured">Featured</span><% } %>
            </div>
            <h1 class="pd-title"><%: NameStart %><em><%: NameLast %></em></h1>
            <p class="pd-price"><%: CartService.Price(Item.UnitPrice) %></p>
            <p class="pd-desc"><%: Item.Description %></p>

            <% if (Item.StockQuantity <= 0) { %>
            <span class="stock-pill is-out">Out of stock</span>
            <% } else if (Item.StockQuantity <= 5) { %>
            <span class="stock-pill is-low">Only <%: Item.StockQuantity %> left</span>
            <% } else { %>
            <span class="stock-pill">In stock &middot; <%: Item.StockQuantity %> available</span>
            <% } %>

            <% if (Added != null && !Added.Success) { %>
            <div class="shop-alert mt-3" role="alert"><%: Added.Message %></div>
            <% } %>

            <%-- Quantity + buttons. Both buttons POST; the server re-reads the quantity, caps it and adds to the cart.
                 shop.js keeps the value inside 1…stock and updates the total on the button. --%>
            <label class="pd-qty-label" for="pdQty">Quantity</label>
            <div class="pd-buy">
                <div class="stepper stepper-lg">
                    <button type="button" id="pdMinus" aria-label="Decrease quantity" <%= SoldOut ? "disabled" : "" %>>&minus;</button>
                    <input type="number" id="pdQty" name="qty" value="1" min="1" max="<%: MaxQuantity %>" inputmode="numeric"
                        data-price="<%: Item.UnitPrice.ToString(System.Globalization.CultureInfo.InvariantCulture) %>" <%= SoldOut ? "disabled" : "" %> />
                    <button type="button" id="pdPlus" aria-label="Increase quantity" <%= SoldOut ? "disabled" : "" %>>+</button>
                </div>
                <% if (SoldOut) { %>
                <button type="button" class="btn-pink" disabled>Sold out</button>
                <button type="button" class="btn-outline" disabled>Buy now</button>
                <% } else { %>
                <button type="submit" class="btn-pink" name="addToCart" value="1">Add to cart &middot; <span id="pdTotal"><%: CartService.Price(Item.UnitPrice) %></span></button>
                <button type="submit" class="btn-outline" name="buyNow" value="1">Buy now</button>
                <% } %>
            </div>

            <% if (Added != null && Added.Success) { %>
            <div class="pd-added" role="status">
                <p><%: Added.Capped ? Added.Message : Added.QuantityAdded + " × " + Added.ProductName + " added to your cart." %></p>
                <a href="<%: ResolveUrl("~/Cart") %>">View cart &rarr;</a>
            </div>
            <% } %>

            <div class="pd-info">
                <div>
                    <span class="info-icon" aria-hidden="true"><svg class="shop-svg" viewBox="0 0 24 24"><rect x="5" y="11" width="14" height="10" rx="2" /><path d="M8 11V8a4 4 0 0 1 8 0v3" /></svg></span>
                    <p class="m-0"><strong>Secure card payment</strong><span>Your card is checked before the order is placed.</span></p>
                </div>
                <div>
                    <span class="info-icon" aria-hidden="true"><svg class="shop-svg" viewBox="0 0 24 24"><path d="M3 6h11v10H3z" /><path d="M14 9h4l3 3v4h-7" /><circle cx="7" cy="17.5" r="1.8" /><circle cx="17" cy="17.5" r="1.8" /></svg></span>
                    <p class="m-0"><strong>Tracked delivery</strong><span>A tracking number the moment your order ships.</span></p>
                </div>
            </div>
        </div>
    </div>

    <% if (More.Count > 0) { %>
    <section class="more-band" aria-labelledby="moreTitle">
        <div class="container">
            <div class="more-head">
                <h2 class="shop-h2" id="moreTitle">More from <em><%: Item.Category.Name %>.</em></h2>
                <a class="link-pink" href="<%: CategoryUrl %>">See all &rarr;</a>
            </div>
            <div class="more-grid">
                <% foreach (var m in More) { %>
                <a class="mini-card" href="<%: ResolveUrl("~/Product?id=" + m.ProductId) %>">
                    <%: ProductImage.Render(this, m.ImageUrl, m.Name, Item.Category.Name, 300, 150, "pimg-mini") %>
                    <span class="mini-foot"><strong><%: m.Name %></strong><span><%: CartService.Price(m.UnitPrice) %></span></span>
                </a>
                <% } %>
            </div>
        </div>
    </section>
    <% } %>

    <script src="<%: ResolveUrl("~/Scripts/shop.js") %>" defer></script>
    <% } %>
</asp:Content>
