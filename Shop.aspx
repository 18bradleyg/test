<%@ Page Title="Shop" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Shop.aspx.cs" Inherits="eStoreJA.Shop" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Register Src="~/Admin/Controls/Pager.ascx" TagPrefix="uc" TagName="Pager" %>
<%@ Import Namespace="eStoreJA.Services" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <%-- Site.Master has no head placeholder, so the page links its own stylesheet --%>
    <link href="<%: ResolveUrl("~/Content/shop.css") %>" rel="stylesheet" />

    <%-- ===== Teal title band ===== --%>
    <section class="shop-band">
        <div class="container">
            <nav class="crumbs" aria-label="Breadcrumb">
                <a href="<%: ResolveUrl("~/") %>">Home</a> / <span aria-current="page">Shop</span>
            </nav>

            <% if (Search.Length > 0) { %>
            <h1 class="shop-title">Results for <em>&ldquo;<%: Search %>&rdquo;</em></h1>
            <p class="shop-sub"><%: CountText %><a href="<%: ShopUrl(Category, null, Sort) %>">Clear search</a></p>
            <% } else if (Category != null) { %>
            <h1 class="shop-title">Shop <em><%: Category.ToLowerInvariant() %>.</em></h1>
            <p class="shop-sub"><%: CountText %></p>
            <% } else { %>
            <h1 class="shop-title">Shop <em>everything.</em></h1>
            <p class="shop-sub"><%: CountText %></p>
            <% } %>
        </div>
    </section>

    <%-- ===== Toolbar: departments, search, sort (all plain GET links / a redirect, never a POST) ===== --%>
    <div class="shop-toolbar">
        <div class="container shop-toolbar-inner">
            <nav class="dept-pills" aria-label="Departments">
                <a class="dept-pill <%: Category == null ? "is-active" : "" %>" href="<%: ShopUrl(null, Search, Sort) %>" <%= Category == null ? "aria-current=\"page\"" : "" %>>All</a>
                <% foreach (string name in Departments) { bool on = name == Category; %>
                <a class="dept-pill <%: on ? "is-active" : "" %>" href="<%: ShopUrl(name, Search, Sort) %>" <%= on ? "aria-current=\"page\"" : "" %>><%: name %></a>
                <% } %>
            </nav>

            <%-- shop.js sends this to data-search-url + q (keeps department and sort); Enter doesn't post the page --%>
            <div class="shop-search" id="shopSearch" data-search-url="<%: ShopUrl(Category, null, Sort) %>">
                <label for="shopSearchInput" class="visually-hidden">Search products</label>
                <input type="search" id="shopSearchInput" maxlength="100" autocomplete="off" placeholder="Search products" value="<%: Search %>" />
                <button type="button" id="shopSearchGo" aria-label="Search">
                    <svg class="shop-svg" viewBox="0 0 24 24" aria-hidden="true"><circle cx="11" cy="11" r="6.5" /><path d="M20 20l-4.2-4.2" /></svg>
                </button>
            </div>

            <nav class="sort-seg" aria-label="Sort products">
                <a href="<%: ShopUrl(Category, Search, "featured") %>" <%= SortCurrent("featured") %>>Featured</a>
                <a href="<%: ShopUrl(Category, Search, "price-asc") %>" <%= SortCurrent("price-asc") %>>Price &uarr;<span class="visually-hidden"> (low to high)</span></a>
                <a href="<%: ShopUrl(Category, Search, "price-desc") %>" <%= SortCurrent("price-desc") %>>Price &darr;<span class="visually-hidden"> (high to low)</span></a>
                <a href="<%: ShopUrl(Category, Search, "name") %>" <%= SortCurrent("name") %>>Name</a>
            </nav>
        </div>
    </div>

    <%-- ===== Product grid ===== --%>
    <div class="shop-body">
        <% if (Cards.Count == 0) { %>
        <p class="shop-empty">No products match. <a class="link-pink" href="<%: ResolveUrl("~/Shop") %>">Show all products</a></p>
        <% } else { %>
        <div class="shop-grid-cards">
            <% foreach (var card in Cards) { string url = ResolveUrl("~/Product?id=" + card.ProductId); %>
            <article class="p-card">
                <%-- Picture repeats the name link, so it is skipped by Tab and screen readers --%>
                <a class="p-pic" href="<%: url %>" tabindex="-1" aria-hidden="true"><%: ProductImage.Render(this, card.ImageUrl, card.Name, card.CategoryName, 400, 190, "pimg-card") %></a>
                <span class="mono-label"><%: card.CategoryName %></span>
                <h2 class="p-name"><a href="<%: url %>"><%: card.Name %></a></h2>
                <p class="p-desc"><%: card.Description %></p>
                <div class="p-foot">
                    <div>
                        <span class="p-price"><%: CartService.Price(card.UnitPrice) %></span>
                        <% if (card.StockQuantity <= 0) { %><span class="p-stock">Out of stock</span><% }
                           else if (card.StockQuantity <= 5) { %><span class="p-stock">Only <%: card.StockQuantity %> left</span><% } %>
                    </div>
                    <% if (card.StockQuantity > 0) { %>
                    <%-- POST (anti-XSRF checked by Site.Master); Page_Load adds 1 and redirects back with the toast --%>
                    <button type="submit" class="btn-add" name="addProduct" value="<%: card.ProductId %>" aria-label="Add to cart: <%: card.Name %>">Add to cart</button>
                    <% } else { %>
                    <button type="button" class="btn-add" disabled>Sold out</button>
                    <% } %>
                </div>
            </article>
            <% } %>
        </div>

        <div class="shop-pager">
            <uc:Pager ID="pager" runat="server" />
        </div>
        <% } %>
    </div>

    <script src="<%: ResolveUrl("~/Scripts/shop.js") %>" defer></script>
</asp:Content>
