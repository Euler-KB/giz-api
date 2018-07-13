using GIZ.API.Helpers;
using GIZ.API.Models;
using GIZ.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Web;

namespace GIZ.API
{
    public class DbInitializer : System.Data.Entity.CreateDatabaseIfNotExists<GIZContext>
    {

        class BranchInfo
        {
            public string Region { get; set; }

            public string Country { get; set; }

            public string Location { get; set; }

            public string Phone { get; set; }

            public string Comment { get; set; }

            public DateTime? EstablishmentDate { get; set; }
        }

        class DealerCompanyInfo
        {
            public string Name { get; set; }

            public string Location { get; set; }

            public string Country { get; set; }

            public string Region { get; set; }

            public string Phone { get; set; }

            public string Email { get; set; }

            public string Comment { get; set; }

            public DateTime? EstablishmentDate { get; set; }

            public IList<BranchInfo> Branches { get; set; }
        }

        class UserInfo
        {
            public string Username { get; set; }

            public string Password { get; set; }

            public string FullName { get; set; }

            public string Phone { get; set; }

            public IList<DealerCompanyInfo> Companies { get; set; }

            public DateTime? EstablishmentDate { get; set; }

            public UserAccountType AccountType { get; set; }

            public string Email { get; set; }

            public string Location { get; set; }
        }

        class ProductInfo
        {
            public string ProductType { get; set; }

            public string Name { get; set; }

            public string Dealer { get; set; }

            public ProductClassification Classification { get; set; }

            public List<string> Brand { get; set; }

            public List<string> Crops { get; set; }

            public List<string> Regions { get; set; }

            public List<LifeCycleInfo> LifeCycle { get; set; }

            public List<SolutionInfo> Solutions { get; set; }
        }

        class LifeCycleInfo
        {
            public string Name { get; set; }

            public string Description { get; set; }
        }

        class SolutionInfo
        {
            public string Statement { get; set; }
        }

        static T LoadDocument<T>(string name)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Seed", name)));
        }

        public static void RunSeed(GIZContext context)
        {
            var usersPayload = LoadDocument<IEnumerable<UserInfo>>("Users.json");
            var productsPayload = LoadDocument<IEnumerable<ProductInfo>>("Products.json");

            context.Users.AddOrUpdate(x => x.Username, usersPayload.Select(x =>
            {
                var user = new AppUser()
                {
                    AccountType = x.AccountType,
                    Email = x.Email,
                    FullName = x.FullName,
                    IsActive = true,
                    IsEmailConfirmed = true,
                    IsPhoneConfirmed = true,
                    Username = x.Username,
                    Phone = x.Phone,
                };

                if (user.AccountType == UserAccountType.Dealer && x.Companies?.Count > 0)
                {
                    user.Companies = x.Companies.Select(t =>
                    {
                        var company = new CompanyInfo()
                        {
                            Name = t.Name,
                            Phone = t.Phone,
                            Country = t.Country,
                            Email = t.Email,
                            Location = t.Location,
                            Region = t.Region,
                            EstablishmentDate = t.EstablishmentDate,
                            Comment = t.Comment
                        };

                        if (t.Branches?.Count > 0)
                        {
                            company.Branches = t.Branches.Select(k => new CompanyBranch()
                            {
                                Region = k.Region,
                                Comment = k.Comment,
                                Country = k.Country,
                                EstablishmentDate = k.EstablishmentDate,
                                Location = k.Location,
                                Phone = k.Phone
                            }).ToList();

                        }

                        return company;

                    }).ToList();
                }


                string salt;
                user.PasswordHash = PasswordHelpers.HashPassword(x.Password, out salt);
                user.PasswordSalt = salt;

                foreach (var product in productsPayload.Where(t => t.Dealer == x.Username))
                {
                    var pd = new Product()
                    {
                        Brand = string.Join(",", product.Brand),
                        CropType = string.Join(",", product.Crops),
                        Classification = product.Classification,
                        Name = product.Name,
                        ProductType = product.ProductType,
                        Dealer = user,
                    };

                    if (product.LifeCycle != null)
                    {
                        int order = 0;
                        foreach (var lc in product.LifeCycle)
                        {
                            pd.LifeCycle.Add(new ProductLifeCycle()
                            {
                                Order = order++,
                                Name = lc.Name,
                                Description = lc.Description,
                                Product = pd
                            });
                        }
                    }

                    if (product.Solutions != null)
                        foreach (var sol in product.Solutions)
                        {
                            pd.Solutions.Add(new ProductSolution()
                            {
                                Product = pd,
                                Statement = sol.Statement
                            });
                        }

                    user.Products.Add(pd);
                }

                return user;
            }).ToArray());

        }

        protected override void Seed(GIZContext context)
        {
            //  Seed Users & Products
            RunSeed(context);

            //
            base.Seed(context);
        }
    }
}