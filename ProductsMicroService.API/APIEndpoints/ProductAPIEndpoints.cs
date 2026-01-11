using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicroService.API.APIEndpoints
{
    public static class ProductAPIEndpoints
    {
        public static IEndpointRouteBuilder MapProductAPIEndpoints(this IEndpointRouteBuilder app)
        {
            //GET /api/products
            app.MapGet("/api/products", async (IProductsService productsService) =>
            {
                List<ProductResponse?> products = await productsService.GetProducts();
                return Results.Ok(products);
            });


            //GET /api/products/search/product-id/00000000-0000-0000-0000-000000000000
            app.MapGet("/api/products/search/product-id/{ProductID:guid}", async (IProductsService productsService, Guid ProductID) =>
            {
                ProductResponse? product = await productsService.GetProductByCondition
                (temp => temp.ProductID == ProductID);
                if(product == null)
                    return Results.NotFound();
                return Results.Ok(product);
            });


            //GET /api/products/search/xxxxxxxxxxxxxxx
            //⭐ FIXED — with EF.Functions.Like to prevent collation & mapping errors in MySQL
            app.MapGet("/api/products/search/{SearchString}", async (IProductsService productsService, string SearchString) =>
            {
                List<ProductResponse?> productsByProductName =
                    await productsService.GetProductsByCondition(temp => temp.ProductName != null &&
                        EF.Functions.Like(temp.ProductName!, $"%{SearchString}%"));

                List<ProductResponse?> productsByCategory =
                    await productsService.GetProductsByCondition(temp => temp.Category != null &&
                        EF.Functions.Like(temp.Category!, $"%{SearchString}%"));

                var products = productsByProductName.Concat(productsByCategory).Distinct().ToList();

                return Results.Ok(products);
            });


            //POST /api/products
            app.MapPost("/api/products", async (IProductsService productsService, IValidator<ProductAddRequest> productAddRequestValidator, ProductAddRequest productAddRequest) =>
            {
                ValidationResult validationResult = await productAddRequestValidator.ValidateAsync(productAddRequest);

                if (!validationResult.IsValid)
                {
                    Dictionary<string, string[]> errors = validationResult.Errors
                      .GroupBy(temp => temp.PropertyName)
                      .ToDictionary(grp => grp.Key,
                        grp => grp.Select(err => err.ErrorMessage).ToArray());
                    return Results.ValidationProblem(errors);
                }

                var addedProductResponse = await productsService.AddProduct(productAddRequest);
                if (addedProductResponse != null)
                    return Results.Created($"/api/products/search/product-id/{addedProductResponse.ProductID}", addedProductResponse);
                else
                    return Results.Problem("Error in adding product");
            });


            //PUT /api/products
            app.MapPut("/api/products", async (IProductsService productsService, IValidator<ProductUpdateRequest> productUpdateRequestValidator, ProductUpdateRequest productUpdateRequest) =>
            {
                ValidationResult validationResult = await productUpdateRequestValidator.ValidateAsync(productUpdateRequest);

                if (!validationResult.IsValid)
                {
                    Dictionary<string, string[]> errors = validationResult.Errors
                      .GroupBy(temp => temp.PropertyName)
                      .ToDictionary(grp => grp.Key,
                        grp => grp.Select(err => err.ErrorMessage).ToArray());
                    return Results.ValidationProblem(errors);
                }

                var updatedProductResponse = await productsService.UpdateProduct(productUpdateRequest);
                if (updatedProductResponse != null)
                    return Results.Ok(updatedProductResponse);
                else
                    return Results.Problem("Error in updating product");
            });


            //DELETE /api/products/xxxxxxxxxxxxxxxxxxx
            app.MapDelete("/api/products/{ProductID:guid}", async (IProductsService productsService, Guid ProductID) =>
            {
                bool isDeleted = await productsService.DeleteProduct(ProductID);
                if (isDeleted)
                    return Results.Ok(true);
                else
                    return Results.Problem("Error in deleting product");
            });

            return app;
        }
    }
}
