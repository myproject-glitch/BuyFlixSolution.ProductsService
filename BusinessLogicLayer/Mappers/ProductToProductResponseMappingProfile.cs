using AutoMapper;
using BusinessLogicLayer.DTO;
using DataAccessLayer.Entities;
using System;

namespace BusinessLogicLayer.Mappers
{
    public class ProductToProductResponseMappingProfile : Profile
    {
        public ProductToProductResponseMappingProfile()
        {
            CreateMap<Product, ProductResponse>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => ParseCategory(src.Category)))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice))
                .ForMember(dest => dest.QuantityInStock, opt => opt.MapFrom(src => src.QuantityInStock))
                .ForMember(dest => dest.ProductID, opt => opt.MapFrom(src => src.ProductID));
        }

        // Helper method for safe parsing
        private static CategoryOptions ParseCategory(string? category)
        {
            return Enum.TryParse<CategoryOptions>(category, out var result) ? result : default;
        }
    }
}
