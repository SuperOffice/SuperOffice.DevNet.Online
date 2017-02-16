<%@ Page Language="C#" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Map Example for SuperOffice</title>
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
            This application adds a web panel to Company/Diary/Project/Selection screen in your SuperOffice CRM Online. It shows various maps of addresses from SuperOffice in Google maps.
        </p>
        <h2>Install</h2>
        <p>When you click to install this app you will have to approve it as an administrator of your SuperOffice CRM Online database, this means that your user in SuperOffice CRM Online Settings & Maintenance must have a role with minimum list admin as a functional right.</p>
        
        <p><img src="style/help/install.png" /></p>
        <p>When you approve you will be presented with the choice of views to add. </p>
        <p><img src="style/help/approve.png" /></p>
        <p><img src="style/help/setup.png" /></p>
        <p>After you made your choices, click Install and you will be redirected to SuperOffice CRM Online again. Depending on which views you chose, you will see the new web panels added.</p>
        <h2>Use the app from SuperOffice CRM Online</h2>
        <p>Company screen under www - Location</p>
        <p><img src="style/help/companyscreen.png"/></p>
        <p>Project screen under www – ProjectMembers Location</p>
        <p><img src="style/help/projectscreen.png"/></p>
        <p>Diary screen – Todays locations button in lower right corner pops up in a new window</p>
        <p><img src="style/help/diaryscreen.png"/></p>
        <p>Selection location button in lower right corner pops up in a new window</p>
        <p><img src="style/help/selection.png"/></p>
        <h2>Settings & Maintenance Changes</h2>
        <p>You will find the new list items in Settings and Maintenance under GUI – Web panel called Location, ProjectMembers Location, Todays Locations and Selection Locations. Note that the number of new web panels vary depending on your install choices.</p>
        <p><img src="style/help/lists.png"/></p>
        <h2>Uninstall</h2>
        <p>Make sure you are logged in to CRM Online as a user with minimum List admin as functional right. Then use the following URL to uninstall: https://sod.superoffice.com/apps/MapsExampleTrunk/uninstall.aspx</p>
        <p><img src="style/help/uninstall.png"/></p>
        <p>This will remove the web panels.</p>
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
