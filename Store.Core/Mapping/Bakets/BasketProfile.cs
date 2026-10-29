using AutoMapper;
using Store.Core.Dtos.Bakets;
using Store.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Mapping.Bakets
{
    public class BasketProfile : Profile
    {
        public BasketProfile()
        {
            CreateMap<CustomerBasket ,CustomerBasketDto>().ReverseMap();    
            
        }
    }
}
