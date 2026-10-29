using AutoMapper;
using Microsoft.Extensions.Configuration;
using Store.Core.Dtos.Auth;
using Store.Core.Dtos.Bakets;
using Store.Core.Dtos.Orders;
using Store.Core.Entities;

using Store.Core.Entities.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Mapping.Orders
{
    public class OrderProfile : Profile
    {

        public OrderProfile(IConfiguration configuration)
        {
            CreateMap<Order, OrderToReturnDto>()
                 .ForMember(d => d.DeliveryMethod, options => options.MapFrom(s => s.DeliveryMethod.ShortName))
                 .ForMember(d => d.DeliveryMethodCost, options => options.MapFrom(s => s.DeliveryMethod.Cost));

            CreateMap<Address, AddressDto>()
                                           .ForMember(d => d.FName, o => o.MapFrom(s => s.FirstName))
                                           .ForMember(d => d.LName, o => o.MapFrom(s => s.LastName));

            CreateMap<AddressDto, Address>()
                                            .ForMember(d => d.FirstName, o => o.MapFrom(s => s.FName))
                                            .ForMember(d => d.LastName, o => o.MapFrom(s => s.LName));

            CreateMap<OrderItem, OrderItemDto>()
               .ForMember(d => d.ProductId, options => options.MapFrom(s => s.Product.ProductId))
               .ForMember(d => d.ProductName, options => options.MapFrom(s => s.Product.ProductName))
               .ForMember(d => d.PictureUrl, options => options.MapFrom(s => $"{configuration["BASEURL"]}/{s.Product.PictureUrl}"));
            
        }
    }
}
