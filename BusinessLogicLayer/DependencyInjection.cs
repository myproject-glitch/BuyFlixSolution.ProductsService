using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.ServiceContracts;
using BusinessLogicLayer.Services;
using BusinessLogicLayer.Validators;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;



namespace BusinessLogicLayer
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessLogicLayer(this
            IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ProductAddRequestToProductMappingProfile).Assembly);
            services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();
            services.AddScoped<IProductsService, ProductsService>();
            return services;
        }

    }
}
