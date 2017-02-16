<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LoginRedirect.aspx.cs" Inherits="SuperOffice.DevNet.Online.Maps.WebForm.LoginRedirect" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <p id="explanationText" runat="server">
            Redirecting... If you havent't been redirected within 20 seconds, click <a id="redirectLink" runat="server">here</a>.
        </p>
    <div id="redirectUrl" runat="server">
    </div>
    </form>
</body>
</html>
