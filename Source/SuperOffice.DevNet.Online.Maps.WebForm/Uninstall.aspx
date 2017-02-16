<%@ Page Title="Uninstall Maps" Language="C#" MasterPageFile="~/MapsExample.Master" AutoEventWireup="true" CodeBehind="Uninstall.aspx.cs" Inherits="SuperOffice.DevNet.Online.Maps.WebForm.Uninstall" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .ThisWill {
            font-family: sans-serif;
            font-size: smaller;
            margin-left: -84px; /* margin + padding */
        }

        .WebPanelList {
            font-family: sans-serif;
            font-size: smaller;
            margin-left: -84px; /* margin + padding */
        }

         .Login {
            padding-top: 0.5em;
            font-family: sans-serif;
            font-size: smaller;
            margin-left: -84px; /* margin + padding */
        }

        .ButtonBottom {
            width: 100%;
            padding-top: 5em;
        }
    </style>

    <style type="text/css">
        .UninstallButton {
            -moz-box-shadow: inset 0px 1px 0px 0px #f5978e;
            -webkit-box-shadow: inset 0px 1px 0px 0px #f5978e;
            box-shadow: inset 0px 1px 0px 0px #f5978e;
            background: -webkit-gradient( linear, left top, left bottom, color-stop(0.05, #f24537), color-stop(1, #c62d1f) );
            background: -moz-linear-gradient( center top, #f24537 5%, #c62d1f 100% );
            filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#f24537', endColorstr='#c62d1f');
            background-color: #f24537;
            -webkit-border-top-left-radius: 0px;
            -moz-border-radius-topleft: 0px;
            border-top-left-radius: 0px;
            -webkit-border-top-right-radius: 0px;
            -moz-border-radius-topright: 0px;
            border-top-right-radius: 0px;
            -webkit-border-bottom-right-radius: 0px;
            -moz-border-radius-bottomright: 0px;
            border-bottom-right-radius: 0px;
            -webkit-border-bottom-left-radius: 0px;
            -moz-border-radius-bottomleft: 0px;
            border-bottom-left-radius: 0px;
            text-indent: 0;
            border: 1px solid #d02718;
            display: inline-block;
            color: #ffffff;
            font-family: Arial;
            font-size: 15px;
            font-weight: bold;
            font-style: normal;
            height: 3.5em;
            line-height: 3.5em;
            width: 15em;
            text-decoration: none;
            text-align: center;
            text-shadow: 1px 1px 0px #810e05;
            float: right;
        }


        .UninstallButton:hover {
            background: -webkit-gradient( linear, left top, left bottom, color-stop(0.05, #c62d1f), color-stop(1, #f24537) );
            background: -moz-linear-gradient( center top, #c62d1f 5%, #f24537 100% );
            filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#c62d1f', endColorstr='#f24537');
            background-color: #c62d1f;
        }

        .UninstallButton:active {
            position: relative;
            top: 1px;
        }
    </style>
    <div class="ThisWill">This will uninstall the following web panels:</div>
    <asp:BulletedList CssClass="WebPanelList" runat="server" ID="_webPanelList" />
    <div class="Login" runat="server" id="_loginRow">from <span runat="server" id="_company">bbb</span> (Logged in as <span runat="server" id="_username">aaa</span>)</div>
    <div class="ButtonBottom">
        <asp:Button Text="Uninstall" CssClass="UninstallButton" runat="server" ID="UninstallButton" OnClick="UninstallButton_Click"></asp:Button>
    </div>

</asp:Content>
