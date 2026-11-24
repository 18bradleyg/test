<%@ Page Title="About us" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="eStoreJA.About" %>
<%@ MasterType VirtualPath="~/Site.Master" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <%-- Site.Master has no head placeholder, so the page links its own stylesheet --%>
    <link href="<%: ResolveUrl("~/Content/pages.css") %>" rel="stylesheet" />

    <%-- ===== Hero (navy) ===== --%>
    <section class="pg-band pg-navy pg-hero-band">
        <div class="container pg-hero">
            <div class="pg-hero-text">
                <p class="pg-badge"><span class="pg-badge-dot" aria-hidden="true"></span>About eStoreJA &middot; Kingston, Jamaica</p>
                <h1 class="pg-display">Everyday goods.<br /><em>Zero guesswork.</em></h1>
                <p class="pg-lead">We're an online store built around three promises: things you actually use, payments you can trust, and deliveries you can follow from start to finish.</p>
                <div class="pg-buttons">
                    <a class="pg-btn pg-btn-pink" href="<%: ResolveUrl("~/Shop") %>">Start shopping &rarr;</a>
                    <a class="pg-btn pg-btn-ghost" href="<%: ResolveUrl("~/TrackPackage") %>">Track a package</a>
                </div>
            </div>

            <%-- Decorative orbs (pure CSS) with the white logo mark; hidden on phones --%>
            <div class="pg-hero-art" aria-hidden="true">
                <span class="pg-orb-blue"></span>
                <span class="pg-orb-pink"></span>
                <span class="pg-glass"><img src="<%: ResolveUrl("~/Images/estoreja-mark-white.svg") %>" alt="" width="92" height="92" /></span>
            </div>
        </div>
    </section>

    <%-- ===== What makes us different (teal, rounded top) ===== --%>
    <section class="pg-band pg-teal pg-rounded" aria-labelledby="diffTitle">
        <div class="container">
            <h2 class="pg-h2" id="diffTitle">What makes us <em>different.</em></h2>

            <div class="pg-bento">
                <article class="pg-card pg-card-navy pg-card-wide">
                    <h3 class="pg-mono">Our story</h3>
                    <p class="pg-story">We started eStoreJA so that buying online in Jamaica feels <em>easy</em>: find what you need, pay with confidence, and always know where your package is.</p>
                </article>

                <article class="pg-card">
                    <span class="pg-icon" aria-hidden="true">
                        <svg viewBox="0 0 24 24"><rect x="5" y="11" width="14" height="10" rx="2" /><path d="M8 11V8a4 4 0 0 1 8 0v3" /></svg>
                    </span>
                    <h3>Secure payments</h3>
                    <p>Cards are checked by our payment partner. We never store your card details.</p>
                </article>

                <article class="pg-card">
                    <span class="pg-icon" aria-hidden="true">
                        <svg viewBox="0 0 24 24"><path d="M3 6h11v9H3z" /><path d="M14 9h4l3 3v3h-7" /><circle cx="7" cy="17" r="2" /><circle cx="17" cy="17" r="2" /></svg>
                    </span>
                    <h3>Tracked delivery</h3>
                    <p>Every order gets a tracking number the moment it ships.</p>
                </article>

                <%-- Count and names come from the Categories table (About.aspx.cs), so they stay correct --%>
                <a class="pg-card pg-card-pink" href="<%: ResolveUrl("~/Shop") %>">
                    <span class="pg-big-number"><%: DepartmentCount %></span>
                    <h3>Departments</h3>
                    <p><%: DepartmentNames %></p>
                </a>

                <article class="pg-card">
                    <span class="pg-icon" aria-hidden="true">
                        <svg viewBox="0 0 24 24"><circle cx="12" cy="12" r="9" /><path d="M12 7v5l3 2" /></svg>
                    </span>
                    <h3>Order history, 24/7</h3>
                    <p>Every past order, its status and tracking, in your account any time.</p>
                </article>
            </div>
        </div>
    </section>

    <%-- ===== From click to doorstep (ice) ===== --%>
    <section class="pg-band pg-ice" aria-labelledby="journeyTitle">
        <div class="container">
            <p class="pg-mono">The journey of an order</p>
            <h2 class="pg-h2" id="journeyTitle">From click to <em>doorstep.</em></h2>

            <div class="pg-journey">
                <%-- Dashed line runs behind the number circles (desktop only) --%>
                <span class="pg-journey-line" aria-hidden="true"></span>
                <ol class="pg-steps">
                    <li>
                        <span class="pg-step-num" aria-hidden="true">01</span>
                        <h3>Pick your products</h3>
                        <p>Browse <%: DepartmentCountWords %> departments and choose what you need.</p>
                    </li>
                    <li>
                        <span class="pg-step-num" aria-hidden="true">02</span>
                        <h3>Pay by card</h3>
                        <p>Your card is checked before the order is placed.</p>
                    </li>
                    <li>
                        <span class="pg-step-num" aria-hidden="true">03</span>
                        <h3>We ship it</h3>
                        <p>You get a tracking number the moment it leaves.</p>
                    </li>
                    <li>
                        <span class="pg-step-num" aria-hidden="true">04</span>
                        <h3>Follow it home</h3>
                        <p>Check the status any time in <a href="<%: ResolveUrl("~/TrackPackage") %>">Track Package</a>.</p>
                    </li>
                </ol>
            </div>
        </div>
    </section>

    <%-- ===== Closing CTA (navy) ===== --%>
    <section class="pg-band pg-navy pg-cta" aria-labelledby="ctaTitle">
        <div class="container">
            <h2 class="pg-h2" id="ctaTitle">Got a question? <em>We're here.</em></h2>
            <p class="pg-lead">Our team helps with orders and deliveries any time, day or night.</p>
            <div class="pg-buttons">
                <a class="pg-btn pg-btn-pink" href="<%: ResolveUrl("~/Contact") %>">Contact us</a>
                <a class="pg-btn pg-btn-ghost" href="#">Read the FAQs</a><!-- TODO: FAQ page -->
            </div>
        </div>
    </section>
</asp:Content>
