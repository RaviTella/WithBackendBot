// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.Cosmos;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using WithBackendBot.Bots;
using WithBackendBot.Cosmos;
using WithBackendBot.Dialogs;

namespace WithBackendBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            services.AddSingleton<IStorage, MemoryStorage>();

            // Use partitioned CosmosDB for storage, instead of in-memory storage.
            /*            services.AddSingleton<IStorage>(
                            new CosmosDbPartitionedStorage(
                                new CosmosDbPartitionedStorageOptions
                                {
                                    CosmosDbEndpoint = Configuration.GetValue<string>("CosmosDbEndpoint"),
                                    AuthKey = Configuration.GetValue<string>("CosmosDbAuthKey"),
                                    DatabaseId = Configuration.GetValue<string>("CosmosDbDatabaseId"),
                                    ContainerId = Configuration.GetValue<string>("CosmosDbContainerId"),
                                    CompatibilityMode = false,
                                }));*/

            services.AddSingleton<ConversationState>();
            services.AddSingleton<QueryDialog>();
            services.AddSingleton<MainDialog>();
            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            services.AddTransient<IBot, SmartBot>();
            services.AddSingleton<ICosmosService>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

            // app.UseHttpsRedirection();
        }

        /// <summary>
        /// Creates a Cosmos DB database and a container with the specified partition key. 
        /// This will be used to pull the FlowCal repo information 
        /// </summary>
        /// <returns></returns>
        private static async Task<ICosmosService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;
            CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            ICosmosService cosmosService = new CosmosService(client, databaseName, containerName);
            Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/appName");

            return cosmosService;
        }
    }
}
