using ApiWorker;
using DataAccess;
using Entities.Repositories;
using Hangfire;
using Logic;
using MongoDB.Driver;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using MongoDB.Driver.Core.Operations;
using Hangfire.Mongo.Migration.Strategies.Backup;

public class Startup
{

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        var mongoDBConnectionString = "mongodb+srv://egallo:7xbmi5ZVYZLF8Vhi@cluster0.2r4lurh.mongodb.net/";

        // Crea el cliente de MongoDB directamente con la URL de conexión
        var mongoClient = new MongoClient(mongoDBConnectionString);
        services.AddSingleton<IMongoClient>(mongoClient);

        // Agrega la configuración de Hangfire y MongoDB
        services.AddScoped<IWorkerLogic, WorkerLogic>();
        services.AddScoped<IWorkerRepository, WorkerRepository>();
        services.AddScoped<IEventoLogic, EventoLogic>();
        services.AddScoped<IEventoRepository, EventoRepository>();
        services.AddScoped<CronJob>();
        //Cron
        services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseMongoStorage(mongoDBConnectionString, "HangfireCron"));
        services.AddHangfireServer();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
        });

    }

    public async void Configure(IApplicationBuilder app, IWebHostEnvironment env, CronJob cronJob)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        // Configuraciones de middleware y rutas
        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
        
     RecurringJob.AddOrUpdate("mi-trabajo-recurrente", () => cronJob.ExecuteAsync(), "*/10 * * * * *");
    }
}
