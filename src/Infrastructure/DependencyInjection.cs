using System;
using Application.Interfaces;
using Infrastructure.Auth;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.SignalR;
using Infrastructure.WhatsApp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
