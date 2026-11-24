<%@ Page Title="Checkout" Language="C#" Async="true" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="eStoreJA.Checkout" %>
<%@ MasterType VirtualPath="~/Site.Master" %>
<%@ Import Namespace="eStoreJA.Services" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="<%: ResolveUrl("~/Content/shop.css") %>" rel="stylesheet" />

    <section class="shop-band">
        <div class="container">
            <nav class="crumbs" aria-label="Breadcrumb">
                <a href="<%: ResolveUrl("~/Cart") %>">Cart</a> / <span aria-current="page">Checkout</span>
            </nav>
            <% if (Done) { %>
            <h1 class="shop-title">Order <em>placed.</em></h1>
            <% } else { %>
            <h1 class="shop-title">Check<em>out.</em></h1>
            <% } %>
        </div>
    </section>

    <div class="shop-body">
        <%-- ===================== Form + summary ===================== --%>
        <asp:Panel ID="pnlCheckout" runat="server" CssClass="co-layout">
            <div>
                <%-- Decline / API / stock messages (built from encoded parts in ShowAlert) --%>
                <asp:Panel ID="pnlAlert" runat="server" CssClass="shop-alert" Visible="false" role="alert">
                    <asp:Literal ID="litAlert" runat="server" Mode="PassThrough" />
                </asp:Panel>

                <%-- 01 Shipping address --%>
                <section class="co-card" aria-labelledby="shipTitle">
                    <div class="co-head">
                        <span class="co-num" aria-hidden="true">01</span>
                        <h2 id="shipTitle">Shipping address</h2>
                    </div>
                    <div class="co-fields">
                        <div class="full">
                            <label for="<%= txtShipName.ClientID %>">Full name</label>
                            <asp:TextBox ID="txtShipName" runat="server" CssClass="co-input" MaxLength="100" placeholder="Name for the delivery" autocomplete="name" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtShipName" ValidationGroup="Order"
                                ErrorMessage="Full name is required." Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                        </div>
                        <div class="full">
                            <label for="<%= txtLine1.ClientID %>">Address line 1</label>
                            <asp:TextBox ID="txtLine1" runat="server" CssClass="co-input" MaxLength="100" placeholder="Street address" autocomplete="address-line1" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLine1" ValidationGroup="Order"
                                ErrorMessage="Address line 1 is required." Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                        </div>
                        <div class="full">
                            <label for="<%= txtLine2.ClientID %>">Address line 2 <small>(optional)</small></label>
                            <asp:TextBox ID="txtLine2" runat="server" CssClass="co-input" MaxLength="100" placeholder="Apartment, unit, etc." autocomplete="address-line2" />
                        </div>
                        <div>
                            <label for="<%= txtCity.ClientID %>">City / Town</label>
                            <asp:TextBox ID="txtCity" runat="server" CssClass="co-input" MaxLength="50" placeholder="e.g. Kingston" autocomplete="address-level2" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCity" ValidationGroup="Order"
                                ErrorMessage="City is required." Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                        </div>
                        <div>
                            <label for="<%= txtState.ClientID %>">Parish</label>
                            <asp:TextBox ID="txtState" runat="server" CssClass="co-input" MaxLength="50" placeholder="e.g. St. Andrew" autocomplete="address-level1" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtState" ValidationGroup="Order"
                                ErrorMessage="Parish is required." Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                        </div>
                        <div>
                            <label for="<%= txtCountry.ClientID %>">Country</label>
                            <asp:TextBox ID="txtCountry" runat="server" CssClass="co-input" MaxLength="50" placeholder="Jamaica" autocomplete="country-name" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCountry" ValidationGroup="Order"
                                ErrorMessage="Country is required." Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                        </div>
                        <div>
                            <label for="<%= txtZip.ClientID %>">Postal code</label>
                            <asp:TextBox ID="txtZip" runat="server" CssClass="co-input" MaxLength="20" placeholder="e.g. JMAAW06" autocomplete="postal-code" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtZip" ValidationGroup="Order"
                                ErrorMessage="Postal code is required." Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                        </div>
                    </div>
                </section>

                <%-- 02 Payment. Card fields: autocomplete off, no ViewState (so the number is never written back
                     into the page), cleared after a decline. Nothing here is stored. --%>
                <section class="co-card" aria-labelledby="payTitle">
                    <div class="co-head">
                        <span class="co-num" aria-hidden="true">02</span>
                        <h2 id="payTitle">Payment</h2>
                        <span class="co-note">
                            <svg class="shop-svg" viewBox="0 0 24 24" aria-hidden="true"><rect x="5" y="11" width="14" height="10" rx="2" /><path d="M8 11V8a4 4 0 0 1 8 0v3" /></svg>
                            Checked securely, never stored
                        </span>
                    </div>
                    <div class="co-fields">
                        <div class="full">
                            <label for="<%= txtFullName.ClientID %>">Name on card</label>
                            <asp:TextBox ID="txtFullName" runat="server" CssClass="co-input" MaxLength="100" placeholder="As printed on the card"
                                AutoCompleteType="Disabled" EnableViewState="false" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtFullName" ValidationGroup="Order"
                                ErrorMessage="Name on card is required." Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                        </div>
                        <div class="full">
                            <label for="<%= txtCardNumber.ClientID %>">Card number</label>
                            <asp:TextBox ID="txtCardNumber" runat="server" CssClass="co-input" MaxLength="16" placeholder="16 digits, no spaces"
                                inputmode="numeric" AutoCompleteType="Disabled" EnableViewState="false" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCardNumber" ValidationGroup="Order"
                                ErrorMessage="Card number is required." Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txtCardNumber" ValidationGroup="Order"
                                ValidationExpression="^\d{16}$" ErrorMessage="Card number must be 16 digits."
                                Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                        </div>
                        <div>
                            <label for="<%= txtExpiry.ClientID %>">Expiry (MM/YY)</label>
                            <asp:TextBox ID="txtExpiry" runat="server" CssClass="co-input" MaxLength="5" placeholder="MM/YY"
                                AutoCompleteType="Disabled" EnableViewState="false" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtExpiry" ValidationGroup="Order"
                                ErrorMessage="Expiry date is required." Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txtExpiry" ValidationGroup="Order"
                                ValidationExpression="^(0[1-9]|1[0-2])\/\d{2}$" ErrorMessage="Expiry must be in MM/YY format."
                                Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                            <asp:CustomValidator runat="server" ControlToValidate="txtExpiry" ValidationGroup="Order"
                                OnServerValidate="ValidateExpiry" ErrorMessage="This card has expired."
                                Display="Dynamic" CssClass="field-error" />
                        </div>
                        <div>
                            <label for="<%= txtCvv.ClientID %>">CVV</label>
                            <asp:TextBox ID="txtCvv" runat="server" CssClass="co-input" MaxLength="4" TextMode="Password" placeholder="123"
                                inputmode="numeric" AutoCompleteType="Disabled" EnableViewState="false" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtCvv" ValidationGroup="Order"
                                ErrorMessage="CVV is required." Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txtCvv" ValidationGroup="Order"
                                ValidationExpression="^\d{3,4}$" ErrorMessage="CVV must be 3 or 4 digits."
                                Display="Dynamic" CssClass="field-error" SetFocusOnError="true" />
                        </div>
                    </div>
                </section>
            </div>

            <%-- ===== Summary (sticky) ===== --%>
            <aside class="co-sum" aria-labelledby="orderTitle">
                <h2 class="sum-title" id="orderTitle">Your <em>order</em></h2>
                <% foreach (var line in Lines) { %>
                <div class="co-line">
                    <span class="co-pic">
                        <%: ProductImage.Render(this, line.ImageUrl, line.Name, line.CategoryName, 56, 56, "pimg-xs") %>
                        <span class="qty-bubble"><span class="visually-hidden">Quantity </span><%: line.Quantity %></span>
                    </span>
                    <strong><%: line.Name %></strong>
                    <span><%: CartService.Price(line.LineTotal) %></span>
                </div>
                <% } %>
                <div class="co-totals">
                    <p class="sum-row"><span>Subtotal</span><span><%: CartService.Price(Total) %></span></p>
                    <p class="sum-row"><span>Shipping</span><span>Tracked</span></p>
                    <div class="sum-total"><span>Total</span><strong><%: CartService.Price(Total) %></strong></div>
                </div>

                <%-- Double-click guard (same as the old Place Order page): validate, then lock the button --%>
                <asp:Button ID="btnPlaceOrder" runat="server" CssClass="btn-pink btn-place" ValidationGroup="Order"
                    OnClick="btnPlaceOrder_Click" UseSubmitBehavior="false"
                    OnClientClick="if (!Page_ClientValidate('Order')) return false; this.disabled = true; this.value = 'Processing...';" />
                <p class="co-small">The button locks after one click so you can&rsquo;t be charged twice. If shipping is briefly unavailable,
                    your order is still saved and paid, and shows <strong>Awaiting Shipment</strong> until it&rsquo;s sent.</p>
                <a class="link-pink" href="<%: ResolveUrl("~/Cart") %>">&larr; Back to cart</a>
            </aside>
        </asp:Panel>

        <%-- ===================== Order placed ===================== --%>
        <asp:Panel ID="pnlDone" runat="server" Visible="false">
            <section class="done-card" role="status">
                <span class="done-check" aria-hidden="true">
                    <svg class="shop-svg" viewBox="0 0 24 24"><path d="M5 12.5l4.5 4.5L19 7.5" /></svg>
                </span>
                <h2 class="done-title">Thank you, <em><%: DoneName %>.</em></h2>
                <p><%: DoneShipped
                        ? "Your payment was approved and your order is on its way."
                        : "Your payment was approved and your order is saved. Shipping couldn't be arranged right now – it shows Awaiting Shipment in your order history until it's sent." %></p>
                <div class="done-tiles">
                    <div><span class="mono-label">Order</span><strong>ORD-<%: DoneOrderId %></strong></div>
                    <div><span class="mono-label">Tracking number</span>
                        <% if (DoneShipped) { %><strong class="mono"><%: DoneTracking %></strong><% } else { %><strong>Coming soon</strong><% } %></div>
                    <div><span class="mono-label">Total paid</span><strong><%: CartService.Price(DoneTotal) %></strong></div>
                </div>
                <div class="done-actions">
                    <a class="btn-navy" href="<%: TrackUrl %>">Track package</a>
                    <a class="btn-outline" href="<%: ResolveUrl("~/OrderHistory") %>">Order history</a>
                    <a class="link-pink" href="<%: ResolveUrl("~/Shop") %>">Keep shopping &rarr;</a>
                </div>
            </section>
        </asp:Panel>
    </div>
</asp:Content>
