<%@ Page Language="C#" MasterPageFile="~/MapsExample.Master" AutoEventWireup="true" CodeBehind="Configuration.aspx.cs" Inherits="SuperOffice.DevNet.Online.Maps.WebForm.Configuration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style>
        .CheckBoxList {
            font-size: larger;
            margin-left: 0em;
            padding: 0em;
        }

        .Header {
            font-size: larger;
            color: dodgerblue;
        }

        .Header2 {
            color: dodgerblue;
            font-size: large;
        }

        .description {
            font-size: smaller;
            color: black;
            font-family: sans-serif;
            width: 40em;
        }

        .Price {
            text-align: right;
        }

        .Indented {
            padding-left: 60px;
        }

        .sumRow {
            color: black;
            font-weight: bold;
        }

        .sumCell {
            padding-top: 1em;
            text-align: right;
        }

        .BuyButton, .DelButton {
            width: 10em;
            height: 2.3em;
            margin-left: 1em;
            margin-bottom: 0.5em;
        }
    </style>

    <asp:ObjectDataSource
        ID="_webPanelsSource"
        runat="server"
        TypeName="SuperOffice.MapsExample.Product"
        SelectMethod="GetProducts">
    </asp:ObjectDataSource>
    
    <asp:Repeater runat="server" ID="_repeater" 
        >
        <HeaderTemplate>
            <table cellpadding="0em" cellspacing="0em">
                <thead>
                    <th></th>
                </thead>
                <tbody>
                    <tr class='CheckBoxList' bgcolor="#E5F0FB">
                        <td class="Header">Get all!</td>
                        <td>
                            <asp:CheckBox ID="_buyAll" runat="server" OnCheckedChanged="AllCheckChanged" AutoPostBack="True" />
                        </td>
                    </tr>
                    <tr title="<%#Eval("Description")%>" bgcolor="#E5F0FB">
                        <td class='description'>Gives you all of the views.</td>
                        <td colspan="2"></td>
                    </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr class='CheckBoxList Indented' title="<%#Eval("Description")%>">
                <td class='Header2 Indented'><%#Eval("Name")%></td>
                <td id='CheckBoxCell_<%#Eval("ID")%>'>
                    <asp:CheckBox ID="selectPanel" Checked='<%# Eval("IsSelected")%>' runat="server" OnCheckedChanged="CheckChanged" AutoPostBack="True" />
                    <asp:HiddenField runat="server" ID="hiddenID" Value='<%#Eval("ID")%>' />
                </td>
            </tr>
            <tr title="<%#Eval("Description")%>">
                <td class='description Indented'><%#Eval("Description")%></td>
                <td colspan="2"></td>
            </tr>
        </ItemTemplate>
        <AlternatingItemTemplate>
            <tr class='CheckBoxList' title="<%#Eval("Description")%>" bgcolor="#E5F0FB">
                <td class='Header2 Indented'><%#Eval("Name")%></td>
                <td>
                    <asp:CheckBox ID="selectPanel"  Checked='<%# Eval("IsSelected")%>' runat="server" OnCheckedChanged="CheckChanged" AutoPostBack="True" />
                    <asp:HiddenField runat="server" ID="hiddenID" Value='<%#Eval("ID")%>' />
                </td>
            </tr>
            <tr title="<%#Eval("Description")%>" bgcolor="#E5F0FB">
                <td class='description Indented'><%#Eval("Description")%></td>
                <td colspan="2"></td>
            </tr>
        </AlternatingItemTemplate>
        <FooterTemplate>
            <tr class='CheckBoxList sumRow'>
                <td class="sumCell" colspan="2">
                    <asp:Button ID="_doItButton" runat="server" CssClass="BuyButton" OnClick="Install_Click" Text="Install" /><br />
                    <asp:Label runat="server" ID="_installFeedback" Visible="False"/>
                </td>
            </tr>
            </tbody></table>
        </FooterTemplate>
    </asp:Repeater>

</asp:Content>
