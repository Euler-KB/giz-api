using GIZ.API.Helpers;
using GIZ.Models;
using GIZ.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using Z.EntityFramework.Plus;
using System.Configuration;
using GIZ.Models.Responses;
using GIZ.API.Models;
using System.Text;
using System.Security.Claims;
using GIZ.API.Services;

namespace GIZ.API.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : BaseController
    {
        internal enum PrimaryActivationMode
        {
            Phone,
            Email
        }

        private IEmailService emailService;

        private ISMSService smsService;

        public UsersController(IEmailService emailService, ISMSService smsService)
        {
            this.emailService = emailService;
            this.smsService = smsService;
        }

        public UsersController()
        {

        }

        /// <summary>
        /// The primary method of activating user accounts
        /// </summary>
        static readonly PrimaryActivationMode UserActivationMode = PrimaryActivationMode.Phone;

        private string GetUserIdentity(AppUser user)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA256(Helpers.UserTokensHelper.Secret))
                return $"{user.Id}:{Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes($"{user.Username}_{user.DateCreated.Ticks}")))}";

        }

        private async Task<AppUser> GetUserWithIdentity(string identity)
        {
            var parts = identity.Split(':');
            long id;
            if (long.TryParse(parts.FirstOrDefault(), out id))
            {
                var user = await DB.Users.FirstOrDefaultAsync(x => x.Id == id);
                if (user != null)
                {
                    if (GetUserIdentity(user).Equals(identity))
                        return user;
                }

            }

            return null;
        }

        [Authorize]
        [Route("photo")]
        [HttpPut]
        public async Task<HttpResponseMessage> SetPhoto()
        {
            var stream = await Request.Content.ReadAsStreamAsync();
            var contentType = Request.Content.Headers.ContentType.MediaType;
            var user = await DB.Users.Include(x => x.ProfileImage).FirstOrDefaultAsync(x => x.Id == UserId);

            Media media;
            if (user.ProfileImage == null)
            {
                var path = MediaHelper.GenerateName(contentType);
                user.ProfileImage = (media = new Media()
                {
                    OriginalPath = path.Actual,
                    ThumbnailPath = path.Thumbnail,
                    Tag = ImageTags.ProfileImage
                });

            }
            else
            {
                user.ProfileImage.LastUpdated = DateTime.UtcNow;
                media = user.ProfileImage;
            }

            //
            await MediaHelper.SaveMediaAsync(stream, contentType, media);

            //  Save changes made
            DB.SaveChanges();

            return OperationSuccess("Profile image updated successfully!");
        }

        [Authorize]
        [Route("me")]
        [HttpPut]
        public async Task UpdateMyself([FromBody]UpdateUserModel model)
        {
            var user = await DB.Users.Include(x => x.Companies).FirstOrDefaultAsync(x => x.Id == UserId);

            if (model.RemoveCompanyIds?.Count > 0)
            {
                foreach (var item in model.RemoveCompanyIds)
                {
                    var cmp = user.Companies.FirstOrDefault(x => x.Id == item);
                    if (cmp != null)
                        user.Companies.Remove(cmp);

                }
            }

            if (model.AddCompany?.Count > 0)
            {
                foreach (var company in Map<IEnumerable<CompanyInfo>>(model.AddCompany))
                    user.Companies.Add(company);

            }

            if (model.UpdateCompany?.Count > 0)
            {
                foreach (var cmp in model.UpdateCompany)
                {
                    var other = user.Companies.FirstOrDefault(x => x.Id == cmp.Id);
                    if (other != null)
                    {
                        ObjectUpdater.CopyPropertiesTo(cmp, other, ObjectUpdater.UpdateFlag.DeferUpdateOnNull, only: new string[]
                        {
                            nameof(CompanyInfo.Name),
                            nameof(CompanyInfo.Country),
                            nameof(CompanyInfo.Comment),
                            nameof(CompanyInfo.Email),
                            nameof(CompanyInfo.Phone),
                            nameof(CompanyInfo.Region),
                            nameof(CompanyInfo.Location),
                            nameof(CompanyInfo.LocationLat),
                            nameof(CompanyInfo.LocationLng),
                            nameof(CompanyInfo.EstablishmentDate),
                        });

                        //
                        foreach (var item in cmp.RemoveBranchIds)
                        {
                            var _item = other.Branches.FirstOrDefault(x => x.Id == item);
                            if (_item != null)
                                other.Branches.Remove(_item);
                        }

                        //
                        var branches = Map<IEnumerable<CompanyBranch>>(cmp.AddBranches);
                        foreach (var item in branches)
                            other.Branches.Add(item);
                    }


                }
            }

            await DB.SaveChangesAsync();
        }

        [Route("photo")]
        [Authorize]
        [HttpDelete]
        public async Task DeletePhoto()
        {
            var user = await DB.Users.Include(x => x.ProfileImage).FirstOrDefaultAsync(x => x.Id == UserId);
            if (user.ProfileImage != null)
            {
                await MediaHelper.RemoveMediaAsync(user.ProfileImage);
                user.ProfileImage = null;
                DB.SaveChanges();
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<HttpResponseMessage> Register([FromBody]RegisterUserModel model)
        {
            var validationErrors = new List<ValidationField>();
            var usernameExists = DB.Users.DeferredAny(x => x.Username.Equals(model.Username));
            var phoneExists = DB.Users.DeferredAny(x => x.Phone.Equals(model.Phone));
            var emailExists = model.Email != null ? DB.Users.DeferredAny(x => x.Email.Equals(model.Email)) : (null);

            if (await usernameExists.ExecuteAsync())
            {
                validationErrors.Add(new ValidationField()
                {
                    Message = "Username already exists!",
                    Property = nameof(RegisterUserModel.Username),
                    Value = model.Username
                });
            }

            if (await phoneExists.ExecuteAsync())
            {
                validationErrors.Add(new ValidationField()
                {
                    Message = "Phone number already used!",
                    Property = nameof(RegisterUserModel.Phone),
                    Value = model.Phone
                });
            }

            if (model.Email != null && await emailExists.ExecuteAsync())
            {
                validationErrors.Add(new ValidationField()
                {
                    Message = "Email already used!",
                    Property = nameof(RegisterUserModel.Email),
                    Value = model.Email
                });
            }

            if (validationErrors.Count > 0)
            {
                return ValidationError(validationErrors.ToArray());
            }

            switch (model.AccountType)
            {
                case UserAccountType.Administrator:
                    {
                        if (User == null)
                            return OperationFail("Authorization has been denied for this request!", HttpStatusCode.Forbidden);
                        var currentUser = await GetCurrentUserAsync();
                        if (currentUser.AccountType != UserAccountType.Administrator)
                            return OperationFail("You do not have enough rights to register an administrator!", HttpStatusCode.Forbidden);

                    }
                    break;
            }

            //  Get user model
            var user = Map<AppUser>(model);

            string salt;
            user.PasswordHash = PasswordHelpers.HashPassword(model.Password, out salt);
            user.PasswordSalt = salt;

            //
            DB.Users.Add(user);

            //  Save changes
            await DB.SaveChangesAsync();

            //
            return Data(Map<UserModel>(user), HttpStatusCode.Created);
        }

        [Route("beginactivate")]
        [HttpPost]
        public async Task<HttpResponseMessage> BeginActivation([FromBody]BeginActivateAccountModel model)
        {
            var user = await GetUserWithIdentity(model.Identity);
            if (user == null)
                return OperationFail("Invalid user identity", HttpStatusCode.Forbidden);

            //
            var activateAccount = user.Tokens.FirstOrDefault(x => x.Type == UserTokenType.ActivateAccount);

            //
            string dispatchToken = null;
            if (activateAccount != null)
            {
                dispatchToken = UserTokensHelper.UnProtectToken(activateAccount.Token, activateAccount.Salt);
            }
            else
            {
                //
                dispatchToken = UserTokensHelper.GenerateToken(6);

                string salt;
                string hashedToken = UserTokensHelper.ProtectToken(dispatchToken, out salt);
                user.Tokens.Add(new UserToken()
                {
                    Type = UserTokenType.ActivateAccount,
                    Salt = salt,
                    Token = hashedToken,
                });


                await DB.SaveChangesAsync();
            }

            HttpResponseMessage response = OperationSuccess("Activation code sent successfully!");

#if DEBUG
            response.Headers.Add("X-Dispatch-Token", dispatchToken);
#endif

            //  Send activation code to user
            switch (UserActivationMode)
            {
                case PrimaryActivationMode.Email:
                    {
                        await emailService.SendFileTemplate("ActivationCode.cshtml", new SendTemplateOptions()
                        {
                            Destinations = new string[] { } ,
                            Model = new
                            {
                                Token = dispatchToken
                            },
                            Subject = "Activate Account"
                        });
                    }
                    break;
                case PrimaryActivationMode.Phone:
                    {
                        await smsService.SendAsync(new SendSMSOptions()
                        {
                            Destinations = new string[] {  },
                            Message = $"Here's your activation code: {dispatchToken}",
                        });
                    }
                    break;
            }

            return response;
        }

        [Route("activate")]
        [HttpPost]
        public async Task<HttpResponseMessage> CompleteAccountActivationAsync([FromBody]CompleteAccountActivationModel model)
        {
            var user = await GetUserWithIdentity(model.Identity);
            if (user == null)
                return OperationFail("Invalid user identity", HttpStatusCode.Forbidden);

            //
            var token = user.Tokens.FirstOrDefault(x => x.Type == UserTokenType.ActivateAccount);
            if (token == null)
                return OperationFail("The operation was not expected to be called at this time");

            //
            if (UserTokensHelper.UnProtectToken(token.Token, token.Salt).Equals(model.Token))
            {
                //  set account activated
                user.IsActive = true;

                //
                DB.Entry(token).State = EntityState.Deleted;

                await DB.SaveChangesAsync();

                //
                switch (UserActivationMode)
                {
                    case PrimaryActivationMode.Email:
                        user.IsEmailConfirmed = true;
                        break;
                    case PrimaryActivationMode.Phone:
                        user.IsPhoneConfirmed = true;
                        break;
                }

                //
                if (user.AccountType == UserAccountType.Dealer)
                {
                    await emailService.SendFileTemplate(@"Dealers\ActivationComplete.cshtml", new SendTemplateOptions()
                    {
                        Destinations = new string[] { user.Email },
                        Subject = "Account Activated",
                        Model = new
                        {
                            Name = user.FullName
                        }
                    });
                }


                return OperationSuccess("Account activation complete!");
            }

            return OperationFail("Invalid token entered. Check and retry again!", HttpStatusCode.Forbidden);
        }

        private AuthenticationTicket GenerateUserTicket(AppUser user)
        {
            string accessToken = JwtHelper.GenerateToken(user.Id.ToString(), user.AccountType.ToString());

            string refreshToken = JwtHelper.GenerateToken(user.Id.ToString(), user.AccountType.ToString(),
                ConfigurationManager.AppSettings["JWT_REFRESH_AUDIENCE"],
                int.Parse(ConfigurationManager.AppSettings["JWT_REFRESH_TOKEN_DURATION_DAYS"]));

            var duration = TimeSpan.FromDays(long.Parse(ConfigurationManager.AppSettings["JWT_DURATION_DAYS"]));

            return new AuthenticationTicket()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Duration = (int)duration.TotalMilliseconds,
                IssuedAt = DateTime.UtcNow,
                UserAccountType = user.AccountType,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.Add(duration)
            };
        }

        [Route("login")]
        [HttpPost]
        public async Task<HttpResponseMessage> Login([FromBody]LoginModel model)
        {
            var user = await DB.Users.FirstOrDefaultAsync(x => (x.Username == model.Username || x.Email == model.Username) && (model.AccountType != null ? x.AccountType == model.AccountType : true));
            if (user != null)
            {
                if (!user.IsActive)
                {
                    var response = ApiResponse(new APIResponse()
                    {
                        Status = ResponseStatus.UserAccountActivationRequired,
                        Message = APIResources.STATUS_INACTIVE_USER_MSG
                    });

                    response.Headers.Add("X-Identity", GetUserIdentity(user));

                    return response;
                }

                if (PasswordHelpers.AreEqual(user.PasswordHash, user.PasswordSalt, model.Password))
                {
                    //  clear password reset token
                    var token = user.Tokens.FirstOrDefault(x => x.Type == UserTokenType.ResetPassword);

                    if (token != null)
                        DB.Entry(token).State = EntityState.Deleted;

                    //  update last login
                    user.LastLogin = DateTime.UtcNow;

                    //
                    await DB.SaveChangesAsync();

                    //
                    return Data(GenerateUserTicket(user));
                }

            }

            return OperationFail("You entered an invalid email or password. Please try again!");
        }

        [Route("refresh_token")]
        [HttpPost]
        public async Task<HttpResponseMessage> RefreshTokenAsync([FromBody]RefreshTokenModel model)
        {
            var principal = JwtHelper.ValidateToken(model.RefreshToken, ConfigurationManager.AppSettings["JWT_REFRESH_AUDIENCE"]);
            if (principal != null)
            {
                var user = await DB.Users.FindAsync(principal.FindFirst(ClaimTypes.NameIdentifier));
                if (user != null)
                {
                    return Data(GenerateUserTicket(user));
                }

            }

            return OperationFail("Invalid refresh token!", HttpStatusCode.Forbidden);
        }

        [Route("forgotpassword")]
        [HttpPost]
        public async Task<HttpResponseMessage> ForgotPassword([FromBody]ForgotPasswordModel model)
        {
            var user = await DB.Users.Where(x => x.IsActive)
                .Include(x => x.Tokens)
                .FirstOrDefaultAsync(x => x.Email == model.UserInput);

            if (user != null)
            {
                //
                string dispatchToken = null;
                var token = user.Tokens.FirstOrDefault(x => x.Type == UserTokenType.ResetPassword);
                if (token != null)
                {
                    dispatchToken = UserTokensHelper.UnProtectToken(token.Token, token.Salt);
                }
                else
                {
                    //
                    dispatchToken = UserTokensHelper.GenerateToken(6);

                    string salt;
                    string hashed = UserTokensHelper.ProtectToken(dispatchToken, out salt);
                    token = new UserToken()
                    {
                        Salt = salt,
                        Token = hashed,
                        Type = UserTokenType.ResetPassword,
                    };

                    //
                    user.Tokens.Add(token);

                    //
                    await DB.SaveChangesAsync();
                }

                //  Dispatch token
                await emailService.SendFileTemplate("ForgotPassword.cshtml", new SendTemplateOptions()
                {
                    Destinations = new string[] { user.Email },
                    Model = new
                    {
                        Name = user.FullName,
                        Token = token
                    }

                });

                //  send token to client
                return Data(new
                {
                    Identity = GetUserIdentity(user)
                });
            }

            return OperationFail("No such user account found!", HttpStatusCode.Forbidden);
        }

        [Route("resetpassword")]
        [HttpPost]
        public async Task<HttpResponseMessage> ResetPassword([FromBody]ResetPasswordModel model)
        {
            var user = await GetUserWithIdentity(model.Identity);
            if (user == null)
                return OperationFail("Invalid user identity", HttpStatusCode.Forbidden);

            var token = user.Tokens.FirstOrDefault(x => x.Type == UserTokenType.ResetPassword);
            if (token == null)
            {
                return OperationFail("Operation was not expected to be called at this time!", HttpStatusCode.Forbidden);
            }

            if (UserTokensHelper.UnProtectToken(token.Token, token.Salt).Equals(model.Token))
            {

                if (model.Verify)
                    return OperationSuccess("Verification successful");

                //
                DB.Entry(token).State = EntityState.Deleted;

                string salt;
                user.PasswordHash = PasswordHelpers.HashPassword(model.Password, out salt);
                user.PasswordSalt = salt;

                //
                await DB.SaveChangesAsync();

                return OperationSuccess("Password reset complete!");
            }
            else
            {
                return OperationFail("Invalid token entered. Please check and try again!");
            }
        }

        [Authorize]
        [Route("me")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetMySelfAsync()
        {
            var user = await DB.Users.Include(x => x.ProfileImage)
                .IncludeOptimized(x => x.Companies)
                .FirstOrDefaultAsync(x => x.Id == UserId);

            return Data(Map<UserModel>(user));
        }

    }
}