namespace CoreVsApp
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[{DateTime.Now}]: New request to http://{context.Request.Host.Value + context.Request.Path}");
                await next.Invoke();
            });

            app.Use(async (context, next) =>
            {
                string logMessage = $"[{DateTime.Now}]: New request to http://{context.Request.Host.Value + context.Request.Path}{Environment.NewLine}";

                string logFilePath = Path.Combine(env.ContentRootPath, "Logs", "RequestLog.txt");

                await File.AppendAllTextAsync(logFilePath, logMessage);

                await next.Invoke();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync($"Welcome to the {env.ApplicationName}!");
                });
            });

            app.Map("/about", About);

            app.Map("/config", Config);

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync($"Page not found)");
            });
        }

        public static async void About(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                await context.Response.WriteAsync($"{env.ApplicationName} - ASP.Net Core tutorial project");
            });
        }

        private static void Config(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var env = app.ApplicationServices.GetRequiredService<IWebHostEnvironment>();
                await context.Response.WriteAsync($"App name: {env.ApplicationName}. Environment: {env.EnvironmentName}");
            });
        }
    }
}
