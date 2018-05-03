# SuperOffice Integration Service

## ONLINE QUOTE CONNECTOR

*If you experience any problems, please provide as much details about your situation in an email to SDK@SUPEROFFICE.COM.  The more details you provide, the less email ping-pong there will be for follow-up questions requesting missing information.*

## Example Quote Connector for SuperOffice Online

This example contains two Visual Studio projects that make up a minimalistic working Quote Connector. 
One project is the implementation Quote Connector, called JsonQuoteConnector. The JsonQuoteConnector is a minimalistic hard-coded example that implements all of the required interfaces for a Quote Connector.
The second project is an MVC web application that serves the following purpose(s).

1. Allows the web site to mimic a user installing the application via a Login button, the same as if done via the App Store Install or Get It! button. This will redirect the use to login to SuperOffice Login using the connectors registered Application Id. Once authenticated, the Consent page is presented where the administrator must “Allow” your connector to interact with the tenants web services.

2. The callback routine uses EntityFramework to store the tenant administrators’ claims, which are sent and validated to the callback controller, in an auto-generated database for subsequent quote connector invocations.

3. The Quote Connector web service resides in the Services folder, and is the endpoint you must specify when registering a new quote connector application. By default the web service endpoint is called QuoteConnector.svc, but it can be changed it to any name, just as long as it’s the same as the endpoint name used for registering the quote connector application.

### Prerequisites

1. Register a new SOD application.
    - a. Define it as a Quote Connector and provide a URL that is both accessible and you are able to debug.

2. Download the Example Application: IntegrationServerExample.zip
    - https://community.superoffice.com/online-sdk-downloads 

3. Update the following settings in the Web.Config:
    - a. ApplicationId in App Settings
    - b. ApplicationToken in SuperOffice Services
    - c. PrivateKey file: ~/App_Data/LocalhostPrivateKey.xml
        - i. If you replace the file, make sure to update ApplicationKeyFile appSetting

4. Change the web application properties to create a virtual directory with a new name

### Contents:

There are two projects in the solution:

1. JsonQuoteConnector
2. MvcIntegrationServerApp

![ProjectImage](/media/online-integration-server-project.png)

The JsonQuoteConnector doesn’t actually do much, and is only included to demonstrate the new generic QuoteConnector Interface API:

```csharp
public class QuoteConnector : OnlineQuoteConnector<SuperOffice.JsonQuoteConnector>
```

The sample Quote Connector service can be seen in the MVC solution under the Service folder.

All of the usual code that validate claims and establishing an SoSession, that are normally located in the SuperOffice.DevNet.Online.Login library have been placed inside this web site – in the Auth folder, so that you can have only the necessary components all in one project.

### Dependencies on EntityFramework

This sample does depend on the EntityFramework for storing customer information, for later use when necessary. All of the dependent types are located in the Models folder. This becomes useful to demonstrate use in a multitenant environment.

#### Multitenantcy

Note the code that overrides the Execute method in the connector service. This code ensures isolation between tenants.

```csharp
protected override TResponse Execute<TRequest, TResponse>(TRequest request, Action<IQuoteConnector, TResponse> action)
{
    using (SoDatabaseContext.EnterDatabaseContext(request.ContextIdentifier))
    {
        var systemUserToken = SystemUserManager.GetSystemUserToken(request.ContextIdentifier);
        SuperOffice.Configuration.ConfigFile.WebServices.RemoteBaseURL = systemUserToken.NetserverUrl;

        using (SoSession session = SoSession.Authenticate(new SoCredentials() { Ticket = systemUserToken.Ticket }))
        {
            return base.Execute<TRequest, TResponse>(request, action);
        }
    }
}
```

The following SuperOffice packages are available www.nuget.org.

```xml
  <package id="SuperOffice.Crm.Online.Core" version="4.0.6194.115" targetFramework="net451" />
  <package id="SuperOffice.Crm.Online.IntegrationServices" version="8.0.6144.342" targetFramework="net451" />
```

These packages will be updated on NuGet as changes occur, but contracts will remain backwards compatible to avoid any future dependencies or project recompilations.

You if experience and issues with restoring packages inside Visual Studio, try opening the Package Manager Console and issuing the Update-Package –ReInstall command. Alternatively, use the commend ```UnInstall-Package [packageName]``` to remove the packages and then ```Install-Package [packageName]``` to reinstall them.

Open the MVC Application.

When you run the application from within Visual Studio (as Administrator), Visual Studio will automatically create a web site for you in the Default Web Site.

Change the Web or Virtual Directory settings in the application properties window:

When you chose to create a virtual directory this way, or let Visual studio create it for you when run the first time, you will then be able to see it in IIS.

When running, the website will present you with a login button.

Why Login?

This step simulates when a customer installs your connector from the App Store. When you successfully authenticate and SuperOffice posts a request to the callback controller, the system user token is saved for use in subsequent requests to the quote connector.

The login button will check to see if you are currently logged into SuperOffice online. If not, you will be presented with a login screen.

Upon successfully signing in, you will be presented with the applications Approval screen. You must approve the application before the application is able to access your online tenant resources, and before the online operations center is able to invoke your Quote connector.

Upon approval, you will be redirected to the MCV application and see a Logout button.

You should now be able to setup your online quote connector in SuperOffice. Open SuperOffice Settings and Maintenance page and navigate to the Quote/Sync Settings.

Select the +Add button to add a new ERP Connection, and ensure the Quote Connector “Other Online Quote Connectors” is selected. Specify a name, and type in the Application Id associated with your Online Quote Connector.
Click the OK button, and this will invoke the initialize the call to your connector to obtain the fields.

The next time you open the settings dialog, the Quote Connector will be invoked and you will see a new field labeled “Json files folder name:”. While this field is present, it doesn’t actually do anything in this sample application, so the text can be anything.

## Summary

This example demonstrates the concepts necessary to build and use a SuperOffice Quote Connector in SuperOffice CRM Online.

What is left is adapting the included dummy connector with an implementation that calls back to the SuperOffice web services for additional data.

Suggestions for enhancements or improvements are welcome to sdk@superoffice.com.