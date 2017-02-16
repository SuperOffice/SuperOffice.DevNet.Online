<%@ Page Title="Welcome to Company News" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SuperOffice.DevNet.Online.News.WebForm.Welcome" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <style>

        .ButtonRow {
            padding-top: 1em;
        }

        .InstallButton {
            height: 4em;
            width: 20em;
        }
    </style>
    
    <asp:Button Text="Install Company News" CssClass="InstallButton" ToolTip="Installs Company News" runat="server" ID="InstallButton" OnClick="InstallButton_Click"></asp:Button>

</asp:Content>
