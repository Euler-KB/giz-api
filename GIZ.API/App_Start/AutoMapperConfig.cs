using GIZ.API.Models;
using GIZ.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GIZ.API
{
    public static class AutoMapperConfig
    {
        static readonly string MediaUri = "/media/stream";

        public static void Config()
        {
            AutoMapper.Mapper.Initialize(Initialize);
        }

        static void Initialize(AutoMapper.IMapperConfigurationExpression config)
        {
            config.CreateMap<AppUser, UserModel>();
            config.CreateMap<AppUser, RegisterUserModel>().ReverseMap();
            config.CreateMap<CompanyInfo, DealerCompanyInfo>();
            config.CreateMap<CompanyBranch, CompanyBranchInfo>();

            config.CreateMap<CompanyInfo, RegisterCompanyInfo>().ReverseMap();
            config.CreateMap<CompanyBranch, RegisterBranchInfo>().ReverseMap();
            config.CreateMap<AppUser, ProductDealerInfo>().ReverseMap();

            config.CreateMap<RegisterUserModel, AppUser>();

            config.CreateMap<ProductSolution, CreateProductSolutionModel>().ReverseMap();
            config.CreateMap<ProductLifeCycle, CreateLifeCycleModel>().ReverseMap();
            config.CreateMap<ProductProperty, CreateProductProperty>().ReverseMap();
            config.CreateMap<LifeCycleTag, CreateLifeCycleTagModel>().ReverseMap();

            config.CreateMap<CreateProductModel, Product>()
                .ForMember(x => x.CropType, x => x.ResolveUsing(t => t.Crops?.Count > 0 ? string.Join(",", t.Crops) : null))
                .ForMember(x => x.Brand, x => x.ResolveUsing(t => t.Brands?.Count > 0 ? string.Join(",", t.Brands) : null));

            config.CreateMap<ProductLifeCycle, ProductLifeCycleModel>();
            config.CreateMap<ProductProperty, ProductPropertyModel>();
            config.CreateMap<LifeCycleTag, LifeCycleTagModel>();
            config.CreateMap<ProductSolution, ProductSolutionModel>();

            config.CreateMap<Media, MediaModel>()
                .ForMember(x => x.OriginalPath, x => x.ResolveUsing(t => $"{MediaUri}/{t.OriginalPath}"))
                .ForMember(x => x.ThumbnailPath, x => x.ResolveUsing(t => $"{MediaUri}/{t.ThumbnailPath}"));

            config.CreateMap<Product, ProductModel>()
                .ForMember(x => x.View, x => x.ResolveUsing(t => new ProductViewModel()
                {
                    TotalViews = t.Views.Count,
                    UserViews = t.Views.Select(f => new UserView()
                    {
                        Timestamp = f.Timestamp,
                        UserId = f.UserId
                    }).ToList()
                }))
                .ForMember(x => x.Brands, x => x.ResolveUsing(t => (t.Brand ?? string.Empty).Split(',').ToList()))
                .ForMember(x => x.Crops, x => x.ResolveUsing(t => (t.CropType ?? string.Empty).Split(',').ToList()));
        }
    }
}