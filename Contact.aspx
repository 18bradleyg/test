<%@ Page Title="Contact us" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="eStoreJA.Contact" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <%-- Site.Master has no head placeholder, so the page links its own stylesheet --%>
    <link href="<%: ResolveUrl("~/Content/pages.css") %>" rel="stylesheet" />

    <%-- ===== Top: contact info + form (navy) ===== --%>
    <section class="pg-band pg-navy pg-hero-band">
        <div class="container pg-contact">
            <div class="pg-contact-info">
                <p class="pg-badge"><span class="pg-badge-dot" aria-hidden="true"></span>Support &middot; 24/7</p>
                <h1 class="pg-display pg-display-sm">How can we<br /><em>help?</em></h1>
                <p class="pg-lead">Pick a topic, tell us what's going on, and we'll reply by email.</p>

                <ul class="pg-contact-rows">
                    <li>
                        <span class="pg-row-icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24"><rect x="3" y="5" width="18" height="14" rx="2" /><path d="M3 7l9 6 9-6" /></svg>
                        </span>
                        <span><a href="mailto:support@estoreja.com">support@estoreja.com</a><small>Email us any time</small></span>
                    </li>
                    <li>
                        <span class="pg-row-icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="9" /><path d="M12 7v5l3 2" /></svg>
                        </span>
                        <span><strong>24/7 support</strong><small>Day or night, every day</small></span>
                    </li>
                    <li>
                        <span class="pg-row-icon" aria-hidden="true">
                            <svg viewBox="0 0 24 24"><path d="M12 21s-7-6.2-7-11.5A7 7 0 0 1 19 9.5C19 14.8 12 21 12 21z" /><circle cx="12" cy="9.5" r="2.5" /></svg>
                        </span>
                        <span><strong>Kingston, Jamaica</strong><small>Where eStoreJA is based</small></span>
                    </li>
                </ul>
            </div>

            <div class="pg-form-card">
                <span class="pg-form-orb" aria-hidden="true"></span>

                <%-- ===== The form ===== --%>
                <asp:Panel ID="pnlForm" runat="server" DefaultButton="btnSend">
                    <h2 class="pg-form-title">Send a <em>message.</em></h2>

                    <div class="pg-form-row">
                        <div class="pg-field">
                            <asp:Label runat="server" AssociatedControlID="txtName">Your name</asp:Label>
                            <div class="pg-input">
                                <asp:TextBox ID="txtName" runat="server" placeholder="Full name" autocomplete="name" MaxLength="100" />
                            </div>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" ValidationGroup="Contact"
                                CssClass="pg-error" Display="Dynamic" role="alert" ErrorMessage="Please enter your name." />
                        </div>
                        <div class="pg-field">
                            <asp:Label runat="server" AssociatedControlID="txtEmail">Email</asp:Label>
                            <div class="pg-input">
                                <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" placeholder="you@example.com" autocomplete="email" MaxLength="256" />
                            </div>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail" ValidationGroup="Contact"
                                CssClass="pg-error" Display="Dynamic" role="alert" ErrorMessage="Please enter your email." />
                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txtEmail" ValidationGroup="Contact"
                                ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
                                CssClass="pg-error" Display="Dynamic" role="alert" ErrorMessage="Please enter a valid email address." />
                        </div>
                    </div>

                    <%-- A real radio group (arrow keys work); the inputs are visually hidden and the labels styled as pills --%>
                    <fieldset class="pg-topics">
                        <legend>What's it about?</legend>
                        <asp:RadioButtonList ID="rblTopic" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="pg-pills">
                            <asp:ListItem Value="order" Selected="True">An order</asp:ListItem>
                            <asp:ListItem Value="delivery">A delivery</asp:ListItem>
                            <asp:ListItem Value="payment">A payment</asp:ListItem>
                            <asp:ListItem Value="account">My account</asp:ListItem>
                            <asp:ListItem Value="other">Something else</asp:ListItem>
                        </asp:RadioButtonList>
                    </fieldset>

                    <div class="pg-field">
                        <asp:Label runat="server" AssociatedControlID="txtOrder">Order number <span class="pg-optional">(optional)</span></asp:Label>
                        <div class="pg-input">
                            <asp:TextBox ID="txtOrder" runat="server" placeholder="e.g. ORD-1042" MaxLength="20" autocomplete="off" />
                        </div>
                        <%-- "ORD-1042" or "1042"; the prefix in either case (client and server regexes have no ignore-case flag) --%>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtOrder" ValidationGroup="Contact"
                            ValidationExpression="^([Oo][Rr][Dd]-)?\d+$"
                            CssClass="pg-error" Display="Dynamic" role="alert" ErrorMessage="Order numbers look like ORD-1042 (or just 1042)." />
                    </div>

                    <div class="pg-field">
                        <asp:Label runat="server" AssociatedControlID="txtMessage">Message</asp:Label>
                        <div class="pg-input pg-input-area">
                            <%-- Request validation off for this box only, so a message with "<" (e.g. "<3 items")
                                 doesn't land on the error page. The text is never written back unencoded. --%>
                            <asp:TextBox ID="txtMessage" runat="server" TextMode="MultiLine" Rows="5" placeholder="How can we help?"
                                ValidateRequestMode="Disabled" />
                        </div>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMessage" ValidationGroup="Contact"
                            CssClass="pg-error" Display="Dynamic" role="alert" ErrorMessage="Please write a message." />
                        <%-- No MaxLength on purpose: the browser would silently cut the text instead of telling the user --%>
                        <asp:RegularExpressionValidator runat="server" ControlToValidate="txtMessage" ValidationGroup="Contact"
                            ValidationExpression="^[\s\S]{0,2000}$"
                            CssClass="pg-error" Display="Dynamic" role="alert" ErrorMessage="Please keep your message under 2,000 characters." />
                    </div>

                    <asp:Button ID="btnSend" runat="server" Text="Send message" CssClass="pg-btn pg-btn-pink pg-btn-block"
                        ValidationGroup="Contact" OnClick="btnSend_Click" />
                </asp:Panel>

                <%-- ===== After a valid submit ===== --%>
                <asp:Panel ID="pnlSent" runat="server" Visible="false" CssClass="pg-sent" role="status">
                    <span class="pg-sent-icon" aria-hidden="true">
                        <svg viewBox="0 0 24 24"><path d="M5 12.5l4.5 4.5L19 7.5" /></svg>
                    </span>
                    <h2 class="pg-form-title">Message <em>sent.</em></h2>
                    <p>Thanks! We'll reply to your email as soon as we can.</p>
                    <a class="pg-btn pg-btn-outline-navy" href="<%: ResolveUrl("~/Contact") %>">Send another message</a>
                </asp:Panel>
            </div>
        </div>
    </section>

    <%-- ===== Need an answer right now? (teal, rounded top) ===== --%>
    <section class="pg-band pg-teal pg-rounded" aria-labelledby="quickTitle">
        <div class="container">
            <h2 class="pg-h2" id="quickTitle">Need an answer <em>right now?</em></h2>

            <div class="pg-links">
                <a class="pg-card pg-link-card" href="<%: ResolveUrl("~/TrackPackage") %>">
                    <span class="pg-icon" aria-hidden="true">
                        <svg viewBox="0 0 24 24"><path d="M3 6h11v9H3z" /><path d="M14 9h4l3 3v3h-7" /><circle cx="7" cy="17" r="2" /><circle cx="17" cy="17" r="2" /></svg>
                    </span>
                    <h3>Track a package</h3>
                    <p>Enter your tracking number to see where your order is.</p>
                    <span class="pg-more">Track Package &rarr;</span>
                </a>

                <a class="pg-card pg-link-card" href="<%: ResolveUrl("~/OrderHistory") %>">
                    <span class="pg-icon" aria-hidden="true">
                        <svg viewBox="0 0 24 24"><path d="M6 3h12v18l-3-2-3 2-3-2-3 2z" /><path d="M9 8h6M9 12h6" /></svg>
                    </span>
                    <h3>Order history</h3>
                    <p>See every order, its status and tracking number.</p>
                    <span class="pg-more">Order History &rarr;</span>
                </a>

                <a class="pg-card pg-card-navy pg-link-card" href="#"><!-- TODO: FAQ page -->
                    <span class="pg-icon" aria-hidden="true">
                        <svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="9" /><path d="M9.6 9.5a2.5 2.5 0 0 1 4.8 1c0 1.7-2.4 2-2.4 3.5" /><path d="M12 17h.01" /></svg>
                    </span>
                    <h3>FAQs</h3>
                    <p>Answers about payments, delivery and accounts.</p>
                    <span class="pg-more">Read the FAQs &rarr;</span>
                </a>
            </div>
        </div>
    </section>
</asp:Content>
