
You’re seeing the default Azure “waiting for your content” page after a successful deployment, which means Azure App Service isn’t finding your app’s startup file or content in the deployed directory. Here are the most common causes and troubleshooting steps:
1. Check Deployment Folder Contents

    ASP.NET Core apps typically do not have an index.html unless they are SPA (React, Angular, etc.), or you have static files in your wwwroot folder.
    For API projects, the homepage may not be a web page; the default Azure page appears if no web route is available.

Action:

    Go to Azure Portal > Your App Service > Advanced Tools (Kudu) > Debug Console > site/wwwroot
    Verify your published files are present (DLLs, web.config, etc.)
        Is TechHiveAPI.dll there?
        Is web.config there?
        Is there a wwwroot folder with static content?

2. App Type

    If your project is an API (ASP.NET Core Web API), browsing to the root URL won’t show a homepage unless you created one. It will show the Azure default message.
    Try accessing /swagger or any API route you have defined.
    If you expect a landing page, make sure your project is ASP.NET Core MVC or includes static files.

3. Startup File and Routing

    If you want to serve static files, you must use app.UseStaticFiles() in Startup.cs/Program.cs.
    For minimal API templates, you may need to add a landing endpoint.

4. web.config Issues

    Azure uses web.config to route requests. If this file is missing or malformed, the app won’t start.

5. Publish Settings

    Confirm your publish step is copying everything to ./publish.
    If it’s an MVC or Razor app, you should see .dll, .pdb, web.config, and possibly a wwwroot folder with your static files.

6. App Service Startup Errors

    Check the Log Stream in Azure Portal for startup errors.
    If your app crashes or fails to run, you’ll see errors there.

Quick Checklist

    Do you see your DLLs and web.config in site/wwwroot?
    Is your app an API or a web app?
    Are you expecting an index.html or a Razor/MVC landing page?
    Can you access /swagger or /api/values (try API endpoints)?
    Are there errors in the Azure Log Stream?

Next Steps

    Check Kudu (Debug Console):
        List all files inside site/wwwroot and post here what you see.
    Share your project type:
        Is it ASP.NET Core Web API, MVC, Razor Pages, or a SPA?
    Try accessing a known endpoint:
        If API: Try /swagger or /api/values.
        If MVC: Try root / or /home/index.

Why this happens:
For ASP.NET Core Web API projects, there is no homepage by default. If you want a landing page, you need to add it (MVC, Razor, or static file in wwwroot). The Azure default page appears if nothing is there to serve at /.

Let me know:

    What files do you see in site/wwwroot?
    What’s your project type?
    Any errors in Log Stream?

And I’ll guide you step-by-step to resolve it!
