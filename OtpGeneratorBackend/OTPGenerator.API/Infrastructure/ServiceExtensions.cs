using OTPGenerator.Data.Access;
using OTPGenerator.Data.Contracts;
using OTPGenerator.Data.Contracts.Helpers.DTO;
using OTPGenerator.Services.Business;
using OTPGenerator.Services.Business.Helpers;
using OTPGenerator.Services.Contracts;
using OTPGenerator.Services.Quartz;
using Quartz;

namespace OTPGenerator.API.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IOTPRepository, OTPRepository>();

        services.AddScoped<IOTPService, OTPService>();
        services.AddScoped<IValidator<OTPDTO>, OTPValidator>();

        services.AddAutoMapper(typeof(Mapper));

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey("DeleteExpiredOTPsJob");
            q.AddJob<DeleteExpiredOTPsJob>(j => j.WithIdentity(jobKey));

            q.AddTrigger(t => t
                .ForJob(jobKey)
                .WithIdentity("DeleteUnmappedSkillsJobTrigger")
                .WithCronSchedule("0 * * ? * *"));
        });

        services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

        services.AddCors(options => options.AddPolicy(
            name: "Origins",
            policy => {
                policy.WithOrigins("https://localhost:4200").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
            }));

        return services;
    }
}
