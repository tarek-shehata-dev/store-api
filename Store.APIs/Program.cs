
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.APIs.Controllers;
using Store.APIs.Errors;
using Store.APIs.Middlewares;
using Store.Core;
using Store.Core.Mapping.Products;
using Store.Core.Services.Contract;
using Store.Repository;
using Store.Repository.Data;
using Store.Repository.Data.Contexts;
using Store.Service.Services.Products;
using Store.APIs.Helper;

namespace Store.APIs
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDependency(builder.Configuration);

            var app = builder.Build();

            await app.ConfigureMiddlewareAsync();

            app.Run();
        }
    }
}
