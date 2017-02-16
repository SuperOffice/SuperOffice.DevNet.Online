<%@ Page Language="C#" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>CompanyNews for SuperOffice</title>
    <style>
        .app-help {
            margin-left: 10%;
            margin-right: 10%;
            text-align: left;
        }

        .app-help a {
            color: #333;
            outline: none;
            padding-left: 3px;
            padding-right: 3px;
            text-decoration: underline;
        }

            .app-help a:link, .app-help a:visited,
            .app-help a:active, .app-help a:hover {
                color: #333;
            }

            .app-help a:hover {
                background-color: #c7d1d6;
            }
    </style>
</head>
<body>
    <div class="app-help" role="article" align="center">
        <h2>About the App</h2>
        <p>
            This application adds a web panel to the company screen showing news about the current company you are looking at in your SuperOffice CRM Online. It will add a link to the search engine BING.com/News/Search and add the company name and department into the search field, always giving you the latest news for the company card.
        </p>
        <h2>Install</h2>
        <p>When you click to install this app you will have to approve it as an administrator of your SuperOffice CRM Online database, this means that your user in SuperOffice CRM Online Settings & Maintenance must have a role with minimum list admin as a functional right.</p>
        
        <p><img src="style/help/install.png" /></p>
        <p>Approve the application as an administrator (minimum list admin)</p>
        <p><img src="style/help/approve.png" /></p>
        <p>An email will be sent to your administrator that this application has been approved access to your SuperOffice CRM Online database.</p>
        <h2>Use the app from SuperOffice CRM Online</h2>
        <p>Once installed you will find a new web panel called news below the company card. When you click on it a search is performed giving back news linked to Company name + company department</p>
        <p><img src="style/help/webpanel.png"/></p>
        <h2>Uninstall</h2>
        <p>Make sure you are logged in to SuperOffice CRM Online as a user with minimum List admin as functional right. Then use the following URL to uninstall: https://sod.superoffice.com/apps/CompanyNewsTrunk/uninstall.aspx</p>
        <h2>Troubleshooting</h2>
        <p>An error occurred. Authorization error:</p>
        <p><img src="style/help/loginerror.png"/></p>
        <p>You are logged in to SuperOffice CRM Online with a user who do not have sufficient functional rights to add this app. The app requires that you in SuperOffice CRM Online Settings & Maintenance have a role with minimum list admin as a functional right.</p>
        <h2>Support</h2>
        <p>If you have problems using the web panel, you may contact <a href="https://support.superoffice.com">SuperOffice Support.</a></p>
        <p>You will find more information about web panels and how to <a href="http://devnet.superoffice.com/Help-Center/Sales--Marketing-Admin/?id=2052&l=en">edit</a> them in our help files</p>
        <p>If you do have a problem regarding using this as an example app for developing new apps for CRM Online and the SuperOffice App Store, please send an email to <b>sdk@superoffice.com</b></p>
    </div>
</body>
</html>
