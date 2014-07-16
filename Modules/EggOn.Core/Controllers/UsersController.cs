using AutoMapper;
using FlowOptions.EggOn.Base.Models;
using FlowOptions.EggOn.Base.ViewModels;
using FlowOptions.EggOn.ModuleCore;
using FlowOptions.EggOn.ModuleCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web.Http;

namespace FlowOptions.EggOn.Base.Controllers
{
    public class UsersController : EggOnApiController
    {
        // GET /api/users
        [Route("users"), HttpGet]
        public List<UserDto> GetAllUsers()
        {
            var users = (List<User>)Database.All<User>();

            if (CurrentUser.HasRole("Administrator"))
            {
                return Mapper.Map<List<User>, List<UserDto>>(users);
            }

            //throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, new {
            //    Error = "You need to be administrator to see this resource."
            //}));

            var filteredUsers = new List<UserDto>();

            foreach (var user in users)
            {
                filteredUsers.Add(new UserDto()
                {
                    Id = user.Id,
                    Name = user.Name
                });
            }

            return filteredUsers;
        }

        // POST /api/users
        [Route("users"), HttpPost, AllowAnonymous]
        public HttpResponseMessage CreateUser([FromBody] UserDto data)
        {
            var user = new User();

            user.Id = GuidComb.NewGuid();

            var AllowUsersToRegister = ConfigurationManager.AppSettings["AllowUsersToRegister"];
            if (String.IsNullOrEmpty(AllowUsersToRegister) || AllowUsersToRegister.ToLower().Trim() != "true" ||
                CurrentUser != null && !CurrentUser.HasRole("Administrator"))
            {
                throw new HttpResponseException(Request.CreateErrorResponse(HttpStatusCode.NotImplemented, "User registration is disabled."));
            }

            if (data.Name == null)
            {
                throw BadRequest("The field \"Name\" is required.");
            }
            user.Name = data.Name.Trim();

            if (data.Email == null)
            {
                throw BadRequest("The field \"Email\" is required.");
            }
            user.Email = data.Email.ToLower().Trim();

            if (Database.Exists<User>("Email = @0", user.Email))
            {
                throw BadRequest("An user with that email already exists.");
            }

            if (data.Password == null)
            {
                throw BadRequest("The field \"Password\" is required.");
            }
            user.Password = data.Password;

            // TODO: Configure default language.
            user.InterfaceLanguageId = Database.FirstOrDefault<Language>("WHERE Code = @0", "en").Id;
            if (data.InterfaceLanguageId.HasValue)
            {
                if (Database.Exists<Language>(data.InterfaceLanguageId.Value))
                {
                    user.InterfaceLanguageId = data.InterfaceLanguageId.Value;
                } 
                else 
                {
                    throw BadRequest("Language with that id not found.");
                }
            }

            /*
            if (data.CompanyId.HasValue && this.Database.Exists<Company>(data.CompanyId.Value))
            {
                user.CompanyId = data.CompanyId.Value;
            }
            else
            {
                throw BadRequest("Company with that id not found.");
            }
            */

            user.LastAction = DateTime.Now;

            user.Validated = true;


            using (var tr = Database.GetTransaction())
            {
                Database.Insert(user);

                if (CurrentUser != null && CurrentUser.HasRole("Administrator"))
                {
                    foreach (var role in data.Roles)
                    {
                        Database.Insert("EggOn.CoreUsersRoles", null, false, new { UserId = user.Id, RoleId = role.Id }); 
                    }
                }

                tr.Complete();
            }

            user = Database.SingleOrDefault<User>("WHERE Email = @0", data.Email);

            var UserEmailValidationRequired = ConfigurationManager.AppSettings["UserEmailValidationRequired"];
            var SMTPEmail = ConfigurationManager.AppSettings["SMTPEmail"];

            if (String.IsNullOrEmpty(UserEmailValidationRequired) || UserEmailValidationRequired.ToLower().Trim() != "true" ||
                String.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPEmail"]) ||
                CurrentUser != null && CurrentUser.HasRole("Administrator"))
            {
                return Request.CreateResponse<UserDto>(HttpStatusCode.Created, Mapper.Map<UserDto>(user));
            }
            else
            {
                return SendValidationEmail(user);
            }
        }

        private HttpResponseMessage SendValidationEmail(User user)
        {
            user.Validated = false;
            user.HashKey = Guid.NewGuid().ToString("N").ToUpper();

            Database.Save(user);

            var mail = new MailMessage();
            var client = new SmtpClient(ConfigurationManager.AppSettings["SMTPServerHost"], Convert.ToInt32(ConfigurationManager.AppSettings["SMTPServerPort"]));

            if (ConfigurationManager.AppSettings["SMTPUsername"] != "")
            {
                var myCredential = new NetworkCredential(ConfigurationManager.AppSettings["SMTPUsername"], ConfigurationManager.AppSettings["SMTPPassword"]);
                client.UseDefaultCredentials = false;
                client.Credentials = myCredential;
            }

            var url = ConfigurationManager.AppSettings["WebDashboardUrl"] + "/?key=" + user.HashKey;

            mail.From = new MailAddress(ConfigurationManager.AppSettings["SMTPEmail"]);
            mail.To.Add(user.Email);
            mail.Subject = "User Validation";
            mail.Body = "To validate your EggOn user please click <a href='" + url + "'>here</a>.";
            mail.IsBodyHtml = true;
            client.Send(mail);

            return Request.CreateResponse<ServiceMessage>(HttpStatusCode.Accepted, new ServiceMessage()
            {
                Type = MessageType.Information,
                Message = "The user was created, email validation is required."
            });
        }

        // GET /api/users/<id>
        [Route("users/{userId:guid}"), HttpGet]
        public UserDto GetUser(Guid userId)
        {
            if (!CurrentUser.HasRole("Administrator") && CurrentUser.Id != userId)
            {
               throw Forbidden("You need to be administrator to see this resource.");
            }

            var user = Database.SingleOrDefault<User>(userId);

            if (user == null)
            {
                throw NotFound("User not Found.");
            }

            return Mapper.Map<UserDto>(user);
        }

        // PUT /api/users/<id>
        [Route("users/{userId:guid}"), HttpPut]
        public UserDto UpdateUser(Guid userId, [FromBody] UserDto data)
        {
            if (!CurrentUser.HasRole("Administrator") && CurrentUser.Id != userId)
            {
                throw Forbidden("You need to be administrator to see this resource.");
            }

            var user = Database.SingleOrDefault<User>(userId);

            if (user == null)
            {
                throw NotFound("User not Found.");
            }

            if (data.Name != null)
                user.Name = data.Name;

            // TODO: Verify Email change.
            if (data.Email != null)
                user.Email = data.Email;

            if (data.Password != null)
                user.Password = data.Password;

            if (data.InterfaceLanguageId.HasValue)
            {
                if (Database.Exists<Language>(data.InterfaceLanguageId.Value))
                {
                    user.InterfaceLanguageId = data.InterfaceLanguageId.Value;
                }
                else
                {
                    throw BadRequest("Language with that id not found.");
                }
            }

            using (var tr = Database.GetTransaction())
            {
                Database.Update(user);

                if (CurrentUser.HasRole("Administrator"))
                {
                    Database.Delete("EggOn.CoreUsersRoles", "UserId", null, user.Id);
                    foreach (var role in data.Roles)
                    {
                        Database.Insert("EggOn.CoreUsersRoles", null, false, new { UserId = user.Id, RoleId = role.Id });
                    }
                }

                tr.Complete();
            }

            

            return Mapper.Map<UserDto>(user);
        }

        // DELETE /api/users/<id>
        [Route("users/{userId:guid}"), HttpDelete]
        public UserDto DeleteUser(Guid userId)
        {
            if (!CurrentUser.HasRole("Administrator") && CurrentUser.Id != userId)
            {
                throw Forbidden("You need to be administrator to see this resource.");
            }

            var user = Database.SingleOrDefault<User>(userId);

            if (user == null)
            {
                throw NotFound("User not Found.");
            }

            using (var tr = Database.GetTransaction())
            {
                Database.Delete("EggOn.CoreUsersRoles", "UserId", null, user.Id);
                Database.Delete(user);

                tr.Complete();
            }

            return Mapper.Map<UserDto>(user);
        }


        [Route("users/current"), HttpGet]
        public UserDto FetchCurrentUser()
        {
            return Mapper.Map<UserDto>(CurrentUser);
        }


        // ATTENTION: NOT REST!
        [Route("users/validate"), HttpPost, AllowAnonymous]
        public HttpResponseMessage ValidateUser([FromUri] string key)
        {
            var user = Database.SingleOrDefault<User>("WHERE HashKey = @0", key);

            if (user == null)
            {
                throw NotFound("User not Found.");
            }

            user.Validated = true;
            user.HashKey = null;

            Database.Update(user, new string[] { "Validated", "HashKey" });

            return Request.CreateResponse(HttpStatusCode.OK, new ServiceMessage()
            {
                Type = MessageType.Information,
                Message = "User was validated successfully."
            });
        }

        // ATTENTION: NOT REST!
        [Route("users/recover1"), HttpPost, AllowAnonymous]
        public HttpResponseMessage RecoverPasswordStepOne([FromUri] string email)
        {
            var user = Database.SingleOrDefault<User>("WHERE Email = @0", email.Trim());

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MessageType = "Information",
                    MessageContents = "Email sent."
                });
            }

            var mail = new MailMessage();
            var client = new SmtpClient(ConfigurationManager.AppSettings["SMTPServerHost"], Convert.ToInt32(ConfigurationManager.AppSettings["SMTPServerPort"]));

            if (ConfigurationManager.AppSettings["SMTPUsername"] != "")
            {
                var myCredential = new NetworkCredential(ConfigurationManager.AppSettings["SMTPUsername"], ConfigurationManager.AppSettings["SMTPPassword"]);
                client.UseDefaultCredentials = false;
                client.Credentials = myCredential;
            }


            if (ConfigurationManager.AppSettings["SMTPEmail"] != "")
            {
                user.HashKey = Guid.NewGuid().ToString("N").ToUpper();

                Database.Update(user, new string[] { "HashKey" });

                var url = "http://www." + ConfigurationManager.AppSettings["Domain"] + "/recover?key=" + user.HashKey;

                mail.From = new MailAddress(ConfigurationManager.AppSettings["SMTPEmail"]);
                mail.To.Add(user.Email);
                mail.Subject = "User Password Recovery";
                mail.Body = "To recover your EggOn user password, please click <a href='" + url + "'>here</a>.";
                mail.IsBodyHtml = true;
                client.Send(mail);
            }

            return Request.CreateResponse(HttpStatusCode.OK, new ServiceMessage()
            {
                Type = MessageType.Information,
                Message = "Email sent."
            });
        }

        // ATTENTION: NOT REST!
        [Route("users/recover2"), HttpPost, AllowAnonymous]
        public HttpResponseMessage RecoverPasswordStepTwo([FromUri] string key, [FromUri] string password)
        {
            var user = Database.SingleOrDefault<User>("WHERE HashKey = @0", key);

            if (user == null)
            {
                throw NotFound("User not Found.");
            }

            user.Password = password;
            user.HashKey = null;

            Database.Update(user, new string[] { "Password", "HashKey" });

            return Request.CreateResponse(HttpStatusCode.OK, new ServiceMessage()
            {
                Type = MessageType.Information,
                Message = "Password changed."
            });
        }
    }
}
