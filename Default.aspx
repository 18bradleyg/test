<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="eStoreJA._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <main>
               <section class="hero">
            <%-- Bootstrap carousel: auto-switches every 5 s, pauses on hover --%>
            <div id="heroCarousel" class="carousel slide carousel-fade" data-bs-ride="carousel" data-bs-interval="5000">
                <div class="carousel-inner">

                    <%-- Slide 1: Fashion --%>
                    <div class="carousel-item active">
                        <div class="container">
                            <div class="hero-inner">
                                <div>
                                    <span class="eyebrow">Fashion</span>
                                    <h1>A fresher <strong>wardrobe.</strong></h1>
                                    <p>Linen shirts, everyday sneakers and sunglasses, delivered island-wide.</p>
                                    <div class="hero-actions">
                                        <a class="btn btn-cta" href="<%: CategoryUrl("Fashion") %>">Shop fashion &rarr;</a>
                                        <%-- Shop by department (moved here from the navbar). Popper "fixed" so the
                                             carousel's overflow:hidden can't clip the menu, 16px clear of the screen edges; site-nav.js pauses the slides while it's open --%>
                                        <div class="dropdown">
                                            <button type="button" class="hero-shop-btn dropdown-toggle" data-bs-toggle="dropdown"
                                                data-bs-popper-config='{"strategy":"fixed","modifiers":[{"name":"preventOverflow","options":{"padding":16}},{"name":"offset","options":{"offset":[0,8]}}]}' aria-expanded="false"
                                                aria-label="Shop by department" title="Shop by department">
                                                <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 9.5V20h16V9.5" /><path d="M3 4h18l-1.5 5.5a2.5 2.5 0 0 1-4.75 0 2.5 2.5 0 0 1-4.75 0 2.5 2.5 0 0 1-4.75 0z" /><path d="M10 20v-5h4v5" /></svg>
                                            </button>
                                            <div class="dropdown-menu shop-menu hero-shop-menu">
                                                <div class="shop-grid">
                                                    <% foreach (string name in ShopCategoryNames) { %>
                                                    <a class="shop-item" href="<%: CategoryUrl(name) %>">
                                                        <span class="shop-code" aria-hidden="true"><%: eStoreJA.SiteMaster.CategoryCode(name) %></span>
                                                        <span><strong><%: name %></strong><small>Shop <%: name %></small></span>
                                                    </a>
                                                    <% } %>
                                                </div>
                                                <a class="shop-all" href="<%: ResolveUrl("~/Shop") %>">
                                                    <span class="mono-label">All products</span>
                                                    <span class="shop-all-title">Everything in <strong>one place.</strong></span>
                                                    <span class="shop-all-link">Shop all &rarr;</span>
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="hero-art">
                                    <img src="<%= ResolveUrl("~/Content/Images/hero.jpg") %>" alt="" />
                                    <div class="hero-badge" aria-hidden="true">
                                        <svg viewBox="0 0 48 48" fill="none" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M17 8l-9 6 4 7 5-3v22h14V18l5 3 4-7-9-6c-1 3-4 5-7 5s-6-2-7-5z"/></svg>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <%-- Slide 2: Home & Living --%>
                    <div class="carousel-item">
                        <div class="container">
                            <div class="hero-inner">
                                <div>
                                    <span class="eyebrow">Home &amp; Living</span>
                                    <h1>A calmer <strong>living room.</strong></h1>
                                    <p>Plants, pots and small pieces that make a space feel like home.</p>
                                    <div class="hero-actions">
                                        <a class="btn btn-cta" href="<%: CategoryUrl("Home & Living") %>">Shop home &rarr;</a>
                                        <%-- Shop by department (moved here from the navbar). Popper "fixed" so the
                                             carousel's overflow:hidden can't clip the menu, 16px clear of the screen edges; site-nav.js pauses the slides while it's open --%>
                                        <div class="dropdown">
                                            <button type="button" class="hero-shop-btn dropdown-toggle" data-bs-toggle="dropdown"
                                                data-bs-popper-config='{"strategy":"fixed","modifiers":[{"name":"preventOverflow","options":{"padding":16}},{"name":"offset","options":{"offset":[0,8]}}]}' aria-expanded="false"
                                                aria-label="Shop by department" title="Shop by department">
                                                <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 9.5V20h16V9.5" /><path d="M3 4h18l-1.5 5.5a2.5 2.5 0 0 1-4.75 0 2.5 2.5 0 0 1-4.75 0 2.5 2.5 0 0 1-4.75 0z" /><path d="M10 20v-5h4v5" /></svg>
                                            </button>
                                            <div class="dropdown-menu shop-menu hero-shop-menu">
                                                <div class="shop-grid">
                                                    <% foreach (string name in ShopCategoryNames) { %>
                                                    <a class="shop-item" href="<%: CategoryUrl(name) %>">
                                                        <span class="shop-code" aria-hidden="true"><%: eStoreJA.SiteMaster.CategoryCode(name) %></span>
                                                        <span><strong><%: name %></strong><small>Shop <%: name %></small></span>
                                                    </a>
                                                    <% } %>
                                                </div>
                                                <a class="shop-all" href="<%: ResolveUrl("~/Shop") %>">
                                                    <span class="mono-label">All products</span>
                                                    <span class="shop-all-title">Everything in <strong>one place.</strong></span>
                                                    <span class="shop-all-link">Shop all &rarr;</span>
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="hero-art">
                                    <img src="<%= ResolveUrl("~/Content/Images/hero.jpg") %>" alt="" />
                                    <div class="hero-badge" aria-hidden="true">
                                        <svg viewBox="0 0 48 48" fill="none" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M24 30V18"/><path d="M24 18c-6 0-9-3-9-9 6 0 9 3 9 9z"/><path d="M24 22c0-5 3-8 9-8 0 6-3 8-9 8z"/><path d="M16 30h16l-2 12H18z"/></svg>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <%-- Slide 3: Electronics --%>
                    <div class="carousel-item">
                        <div class="container">
                            <div class="hero-inner">
                                <div>
                                    <span class="eyebrow">Electronics</span>
                                    <h1>A clearer <strong>sound.</strong></h1>
                                    <p>Noise-cancelling headphones and the tech you use every day.</p>
                                    <div class="hero-actions">
                                        <a class="btn btn-cta" href="<%: CategoryUrl("Electronics") %>">Shop tech &rarr;</a>
                                        <%-- Shop by department (moved here from the navbar). Popper "fixed" so the
                                             carousel's overflow:hidden can't clip the menu, 16px clear of the screen edges; site-nav.js pauses the slides while it's open --%>
                                        <div class="dropdown">
                                            <button type="button" class="hero-shop-btn dropdown-toggle" data-bs-toggle="dropdown"
                                                data-bs-popper-config='{"strategy":"fixed","modifiers":[{"name":"preventOverflow","options":{"padding":16}},{"name":"offset","options":{"offset":[0,8]}}]}' aria-expanded="false"
                                                aria-label="Shop by department" title="Shop by department">
                                                <svg viewBox="0 0 24 24" aria-hidden="true"><path d="M4 9.5V20h16V9.5" /><path d="M3 4h18l-1.5 5.5a2.5 2.5 0 0 1-4.75 0 2.5 2.5 0 0 1-4.75 0 2.5 2.5 0 0 1-4.75 0z" /><path d="M10 20v-5h4v5" /></svg>
                                            </button>
                                            <div class="dropdown-menu shop-menu hero-shop-menu">
                                                <div class="shop-grid">
                                                    <% foreach (string name in ShopCategoryNames) { %>
                                                    <a class="shop-item" href="<%: CategoryUrl(name) %>">
                                                        <span class="shop-code" aria-hidden="true"><%: eStoreJA.SiteMaster.CategoryCode(name) %></span>
                                                        <span><strong><%: name %></strong><small>Shop <%: name %></small></span>
                                                    </a>
                                                    <% } %>
                                                </div>
                                                <a class="shop-all" href="<%: ResolveUrl("~/Shop") %>">
                                                    <span class="mono-label">All products</span>
                                                    <span class="shop-all-title">Everything in <strong>one place.</strong></span>
                                                    <span class="shop-all-link">Shop all &rarr;</span>
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="hero-art">
                                    <img src="<%= ResolveUrl("~/Content/Images/hero.jpg") %>" alt="" />
                                    <div class="hero-badge" aria-hidden="true">
                                        <svg viewBox="0 0 48 48" fill="none" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M10 28v-4a14 14 0 0 1 28 0v4"/><rect x="8" y="28" width="7" height="11" rx="3"/><rect x="33" y="28" width="7" height="11" rx="3"/></svg>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>

                <%-- Bottom row: department links (from the DB) + slide indicators --%>
                <div class="container hero-bottom">
                    <nav class="hero-cats" aria-label="Departments">
                        <asp:Repeater ID="rptCategories" runat="server" ItemType="eStoreJA.Models.Category">
                            <ItemTemplate>
                                <a href="<%#: CategoryUrl(Item.Name) %>"><%#: Item.Name %></a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </nav>
                    <div class="carousel-indicators hero-dots">
                        <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="0" class="active" aria-current="true" aria-label="Slide 1: Fashion"></button>
                        <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="1" aria-label="Slide 2: Home and Living"></button>
                        <button type="button" data-bs-target="#heroCarousel" data-bs-slide-to="2" aria-label="Slide 3: Electronics"></button>
                    </div>
                </div>
            </div>
        </section>

        <div class="container">
            <div class="section-head">
                <h2>Picked with a <em>clear eye.</em></h2>
                <a href="<%: ResolveUrl("~/Shop") %>">Browse every department &rarr;</a>
            </div>

            <div class="product-grid">

                <%-- One card per featured product, loaded from the database in Default.aspx.cs --%>
                <asp:Repeater ID="rptFeatured" runat="server" ItemType="eStoreJA.Models.Product">
                    <ItemTemplate>
                        <article class="product-card">
                            <%-- Picture and name open the product page --%>
                            <a class="product-tile" href="<%#: ResolveUrl("~/Product?id=" + Item.ProductId) %>" tabindex="-1" aria-hidden="true"><span class="dot"></span>
                                <%-- Real picture (local file now, S3 link later) --%>
                                <asp:PlaceHolder runat="server" Visible="<%# !string.IsNullOrEmpty(Item.ImageUrl) %>">
                                    <img src="<%#: ImageSrc(Item.ImageUrl) %>" alt="" />
                                </asp:PlaceHolder>
                                <%-- No picture yet: generic shopping-bag icon --%>
                                <asp:PlaceHolder runat="server" Visible="<%# string.IsNullOrEmpty(Item.ImageUrl) %>">
                                    <svg viewBox="0 0 48 48" fill="none" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M10 16h28l-2 24H12z"/><path d="M18 16v-3a6 6 0 0 1 12 0v3"/></svg>
                                </asp:PlaceHolder>
                            </a>
                            <div class="product-cat"><%#: Item.Category.Name %></div>
                            <h3 class="product-name"><a href="<%#: ResolveUrl("~/Product?id=" + Item.ProductId) %>"><%#: Item.Name %></a></h3>
                            <div class="product-foot">
                                <span><%#: string.Format("J${0:N0}", Item.UnitPrice) %></span>
                                <%-- Add to cart: a plain submit button (POST, checked by the master's anti-XSRF token);
                                     Page_Load adds 1 and redirects back with the toast. Out of stock: no button. --%>
                                <asp:PlaceHolder runat="server" Visible="<%# Item.StockQuantity > 0 %>">
                                    <button type="submit" class="btn btn-order" name="addProduct" value="<%#: Item.ProductId %>"
                                        aria-label="Add to cart: <%#: Item.Name %>">Add to cart</button>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder runat="server" Visible="<%# Item.StockQuantity <= 0 %>">
                                    <span class="out-of-stock">Out of stock</span>
                                </asp:PlaceHolder>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>

            </div>

            <div class="features">
                <div><div class="num">01</div><h3>Secure card payments</h3><p>Your card is checked by our payment partner before any order is placed.</p></div>
                <div><div class="num">02</div><h3>Tracked delivery</h3><p>A tracking number arrives the moment your order ships.</p></div>
                <div><div class="num">03</div><h3>Your order history</h3><p>Every past order, its status and tracking in one place.</p></div>
            </div>
        </div>
    </main>

</asp:Content>