using AutoMapper;
using Store.Core.Dtos.Auth;
using Store.Core.Dtos.Bakets;
using Store.Core.Entities;
using Store.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Core.Mapping.Auth
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<Address, AddressDto>().ReverseMap();

        }
    }
    
}
