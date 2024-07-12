using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Application.Interfaces.ApplicationServices;
using Application.Services;
using Domain.Entities;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Mappings.MappingProfile));

            services.AddScoped<IPaginationService<Governorate, GovernorateDTO, GovernorateInsertDTO, GovernorateUpdateDTO, int>, GovernorateService>();
            services.AddScoped<IPaginationService<Representative, RepresentativeDisplayDTO, RepresentativeInsertDTO, RepresentativeUpdateDTO, string>, RepresentativeService>();
            services.AddScoped<IPaginationService<RolePowers, RolePowersDTO, RolePowersInsertDTO, RolePowersUpdateDTO, string>, RolePowersService>();
            services.AddScoped<IPaginationService<Merchant, MerchantResponseDto, MerchantAddDto, MerchantUpdateDto, string>, MerchantService>();
            services.AddScoped<IPaginationService<Employee, EmployeeReadDto, EmployeeAddDto, EmployeeupdateDto, string>, EmployeeService>();
            services.AddScoped<IGenericService<Product, DisplayProductDTO, InsertProductDTO, UpdateProductDTO, int>, ProductService>();
            services.AddScoped<IGenericService<Settings, SettingsDTO, SettingsInsertDTO, SettingsUpdateDTO, int>, SettingsService>();

            services.AddScoped<IPaginationService<Order, DisplayOrderDTO, InsertOrderDTO, NewOrderUpdateDTO, int>, Application.Services.OrderService>();
            services.AddScoped<IPaginationService<City, CityDisplayDTO, CityInsertDTO, CityUpdateDTO, int>, CityService>();
            services.AddScoped<IPaginationService<Branch, BranchDisplayDTO, BranchInsertDTO, BranchUpdateDTO, int>, BranchService>();

            services.AddScoped<IDropDownOptionsService<Governorate, int>, GovernorateService>();
            services.AddScoped<IDropDownOptionsService<City, int>, CityService>();
            services.AddScoped<IDropDownOptionsService<Branch, int>, BranchService>();
            services.AddScoped<IDropDownOptionsService<Representative, string>, RepresentativeService>();
            services.AddScoped<IDropDownOptionsService<ApplicationRoles, string>, RolePowersService>();

            services.AddScoped<IUpdateOrderService, OrderService>();

            return services;
        }
    }
}
