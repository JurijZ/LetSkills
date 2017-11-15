using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using skillsBackend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Amazon.S3;
using Amazon.SimpleNotificationService;
using Hangfire;
using skillsBackend.Utilities;

namespace skillsBackend
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
            // Add database model to the container
            services.AddDbContext<SkillsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Adding Amazon S3 support
            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", "AKIAJW3QA7QH7S4NTQ6A"); // Keys should be stored in the config
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", "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");

            services.AddDefaultAWSOptions(Configuration.GetAWSOptions());   // This line consumes above keys
            services.AddAWSService<IAmazonS3>();                            // Registered S3 service for DI
            services.AddAWSService<IAmazonSimpleNotificationService>();     // SNS (https://github.com/aws/aws-sdk-net/issues/782)

            // Adding Hangfire Scheduler service
            services.AddHangfire(x => x.UseSqlServerStorage(Configuration.GetConnectionString("Hangfire")));

            // Adding JWT support
            var stsServerUrl = Configuration.GetSection("Identity:stsServer"); // reads from the appsetings.json
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.Authority = stsServerUrl.Value; //"http://www.usefullskills.com/identity"
                o.Audience = "apiApp";
                o.RequireHttpsMetadata = false;
            });

            // Adding Utility class (cannot use a Singleton hence Scheduler depends on DB context which has a Scoped lifetime)
            services.AddScoped<IMyScheduler, Scheduler>();

            // Adding CORS
            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    // http://localhost does not work as expected, I think there are some limitations
                    policy.WithOrigins("http://localhost", 
                                       "http://localhost/client", 
                                       "http://localhost/provider",
                                       "http://localhost:5002",                                       
                                       "http://localhost:5002/client",
                                       "http://localhost:5005",
                                       "http://localhost:5005/provider",
                                       "http://www.usefullskills.com",
                                       "http://www.usefullskills.com/client",
                                       "http://www.usefullskills.com/provider"
                                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Injecting services
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IMyScheduler sch)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Setup HangFire Scheduled tasks (run )
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            // Run Offer exiry processing every (web interface http://localhost/hangfire)
            //RecurringJob.AddOrUpdate(() => sch.ProcessExpiredOffers(), Cron.Minutely);
            //RecurringJob.AddOrUpdate(() => Console.WriteLine("Minutely Job executed"), Cron.Minutely);
            RecurringJob.AddOrUpdate(() => sch.ProcessExpiredOffers(), "*/1 * * * *");            

            app.UseCors("default");

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
