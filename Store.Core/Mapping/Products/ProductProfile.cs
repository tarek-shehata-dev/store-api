using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Store.Core.Dtos.Products;
using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Mapping.Products
{
    public class ProductProfile : Profile
    {
        public ProductProfile(IConfiguration configuration)
        {
            CreateMap<Product, ProductDto>()
                .ForMember(d => d.BrandName, options => options.MapFrom(s => s.Brand.Name))
                .ForMember(d => d.TypeName, options => options.MapFrom(s => s.Type.Name))
                .ForMember(d => d.PictureURL, options => options.MapFrom(s => $"{configuration["BASEURL"]}/{s.PictureURL}"));

            CreateMap<ProductBrand, TypeBrandDto>();
            CreateMap<ProductType, TypeBrandDto>();
        }

    }
}
