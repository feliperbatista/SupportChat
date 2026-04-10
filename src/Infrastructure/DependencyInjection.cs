using System;
using Application.Interfaces;
using Azure.Storage.Blobs;
using Infrastructure.AudioConverter;
using Infrastructure.Auth;
using Infrastructure.AzureBlob;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.SignalR;
using Infrastructure.WhatsApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config
    )
    {
        services.AddDbContext<AppDbContext>(options => 
            options.UseSqlServer(config.GetConnectionString("Default")));

        services.AddScoped<IAgentRepository, AgentRepository>();
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IConversationCategoryRepository, ConversationCategoryRepository>();

        services.AddSingleton<IAudioConverter, FFmpegAudioConverterService>();
        
        services.AddScoped<IAzureBlobService, AzureBlobService>();
        services.Configure<AzureBlobOptions>(
            config.GetSection("AzureBlob")
        );
        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AzureBlobOptions>>().Value;
            return new BlobServiceClient(options.ConnectionString);
        });

        services.Configure<WhatsAppOptions>(config.GetSection("WhatsApp"));
        services.AddHttpClient<IWhatsAppService, WhatsAppService>(client =>
        {
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", config["WhatsApp:Token"]);
        });

        services.Configure<JwtOptions>(config.GetSection("Jwt"));
        services.AddScoped<IJwtService, JwtService>();
       
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}
