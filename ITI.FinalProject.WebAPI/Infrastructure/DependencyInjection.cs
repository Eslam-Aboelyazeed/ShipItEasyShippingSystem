using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Application.Interfaces;


namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ShippingContext>(options => options.UseSqlServer(configuration.GetConnectionString("con")));

            services.AddIdentity<ApplicationUser, ApplicationRoles>().AddEntityFrameworkStores<ShippingContext>().AddDefaultTokenProviders();

            services.AddScoped<SignInManager<ApplicationUser>>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
