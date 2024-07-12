using AutoMapper;
using Domain.Entities;
using Application.DTOs.DisplayDTOs;
using Application.DTOs.InsertDTOs;
using Application.DTOs.UpdateDTOs;
using Microsoft.Data.SqlClient;

namespace Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, DisplayOrderDTO>()
                .ForMember(dest => dest.MerchantName, opt => opt.MapFrom(src => src.merchant.user.FullName))
                .ForMember(dest => dest.GovernorateName, opt => opt.MapFrom(src => src.governorate.name))
                .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.city.name))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.branch.name))
                .ForMember(dest => dest.RepresentativeName, opt => opt.MapFrom(src => src.representative.user.FullName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
            CreateMap<InsertOrderDTO, Order>()
                .ForMember(dest => dest.ShippingCost, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Products.Sum(p => p.Price * p.Quantity)))
                .ForMember(dest => dest.TotalWeight, opt => opt.MapFrom(src => src.Products.Sum(p => p.Weight * p.Quantity)));
            CreateMap<UpdateOrderDTO, Order>()
                .ForMember(dest => dest.ShippingCost, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.Products != null ? src.Products.Sum(p => p.Price * p.Quantity) : 0))
                .ForMember(dest => dest.TotalWeight, opt => opt.MapFrom(src => src.Products != null ? src.Products.Sum(p => p.Weight * p.Quantity) : 0 ));

            // Product Mappings
            CreateMap<Product, DisplayProductDTO>()
                .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.order.ClientName))
                .ForMember(dest => dest.ProductStatus, opt => opt.MapFrom(src => src.ProductStatus));
            CreateMap<InsertProductDTO, Product>();
            CreateMap<UpdateProductDTO, Product>();


            // Settings Mappings
            CreateMap<Settings, SettingsDTO>();
            CreateMap<SettingsInsertDTO, Settings>();
            CreateMap<SettingsUpdateDTO, Settings>();

            // Special Packages
            CreateMap<SpecialPackages, SpecialPackageDTO>()
                .ForMember(dest => dest.governorateName, opt => opt.MapFrom(src => src.governoratePackages.name))
                .ForMember(dest => dest.cityName, opt => opt.MapFrom(src => src.cityPackages.name))
                .ForMember(dest => dest.MerchantName, opt => opt.MapFrom(src => src.merchantSpecialPackage.user.FullName));

        }
    }
}
