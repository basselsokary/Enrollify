using Infrastructure;
using Application;
using Serilog;

namespace API;

public class Program
{
    static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddPresentation(builder.Host, builder.Configuration)
            .AddApplication()
            .AddInfrastructure(builder.Configuration);

        var app = builder.Build();

        await app.Services.SeedAsync();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        #region Middlewares
        
        app.UseExceptionHandler();

        app.UseSerilogRequestLogging(opts =>
        {
            opts.MessageTemplate = "{RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
            opts.EnrichDiagnosticContext = (ctx, httpCtx) =>
            {
                ctx.Set("UserId", httpCtx.User?.FindFirst("sub")?.Value);
                ctx.Set("ClientIP", httpCtx.Connection.RemoteIpAddress);
            };
        });

        app.UseRouting();
        
        app.UseCors();

        app.UseAuthentication();

        app.UseAuthorization();
        
        app.UseRateLimiter();

        app.MapControllers();
        #endregion

        app.Run();
    }
}
