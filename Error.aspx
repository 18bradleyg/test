﻿<%@ Page Title="Error" Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="eStoreJA.Error" %>

<%-- Standalone on purpose (no Site.Master): just the message, centred on the screen, and it still
     renders even if the navbar/master page is what failed. --%>
<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <meta name="robots" content="noindex" />
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin />
    <link href="https://fonts.googleapis.com/css2?family=DM+Sans:opsz,wght@9..40,300..700&display=swap" rel="stylesheet" />
    <%-- Site-absolute paths: the visitor's URL can be any depth (e.g. /Account/x/y), so relative links would break --%>
    <asp:PlaceHolder runat="server">
        <link href="<%: ResolveUrl("~/favicon.ico") %>" rel="shortcut icon" type="image/x-icon" />
        <link href="<%: ResolveUrl("~/Content/bootstrap.min.css") %>" rel="stylesheet" />
        <link href="<%: ResolveUrl("~/Content/Site.css") %>" rel="stylesheet" />
    </asp:PlaceHolder>
</head>
<body>
    <main class="error-screen">
        <div class="page-card text-center" role="alert" aria-labelledby="title">
            <span class="eyebrow d-block">Error <%: StatusCode %></span>
            <h1 id="title"><%: Heading %></h1>
            <p class="lead-sm"><%: Message %></p>
            <div class="d-flex flex-wrap justify-content-center gap-2">
                <a class="btn btn-cta" href="<%: ResolveUrl("~/") %>">Back to home</a>
                <%-- Goes to the previous page; with no history (opened directly / new tab) the link just goes home --%>
                <a class="btn btn-outline-nav" href="<%: ResolveUrl("~/") %>"
                   onclick="if (window.history.length > 1) { window.history.back(); return false; }">Go back</a>
            </div>
        </div>
    </main>
</body>
</html>
