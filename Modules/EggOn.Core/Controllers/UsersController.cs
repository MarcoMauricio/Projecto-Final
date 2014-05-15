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
            List<User> users = (List<User>)this.Database.All<User>();

            if (this.CurrentUser.HasRole("Administrator"))
            {
                return Mapper.Map<List<User>, List<UserDto>>(users);
            }

            //throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, new {
            //    Error = "You need to be administrator to see this resource."
            //}));

            List<UserDto> filteredUsers = new List<UserDto>();

            foreach (User user in users)
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
            User user = new User();

            user.Id = GuidComb.NewGuid();

            var AllowUsersToRegister = ConfigurationManager.AppSettings["AllowUsersToRegister"];
            if (String.IsNullOrEmpty(AllowUsersToRegister) || AllowUsersToRegister.ToLower().Trim() != "true" ||
                this.CurrentUser != null && !this.CurrentUser.HasRole("Administrator"))
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

            if (this.Database.Exists<User>("Email = @0", user.Email))
            {
                throw BadRequest("An user with that email already exists.");
            }

            if (data.Password == null)
            {
                throw BadRequest("The field \"Password\" is required.");
            }
            user.Password = data.Password;

            // TODO: Configure default language.
            user.InterfaceLanguageId = this.Database.FirstOrDefault<Language>("WHERE Code = @0", "en").Id;
            if (data.InterfaceLanguageId.HasValue)
            {
                if (this.Database.Exists<Language>(data.InterfaceLanguageId.Value))
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


            using (var tr = this.Database.GetTransaction())
            {
                this.Database.Insert(user);

                if (this.CurrentUser != null && this.CurrentUser.HasRole("Administrator"))
                {
                    foreach (var role in data.Roles)
                    {
                        this.Database.Insert("EggOn.CoreUsersRoles", null, false, new { UserId = user.Id, RoleId = role.Id }); 
                    }
                }

                tr.Complete();
            }

            user = this.Database.SingleOrDefault<User>("WHERE Email = @0", data.Email);

            var UserEmailValidationRequired = ConfigurationManager.AppSettings["UserEmailValidationRequired"];
            var SMTPEmail = ConfigurationManager.AppSettings["SMTPEmail"];

            if (String.IsNullOrEmpty(UserEmailValidationRequired) || UserEmailValidationRequired.ToLower().Trim() != "true" ||
                String.IsNullOrEmpty(ConfigurationManager.AppSettings["SMTPEmail"]) ||
                this.CurrentUser != null && this.CurrentUser.HasRole("Administrator"))
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

            this.Database.Save(user);

            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["SMTPServerHost"], Convert.ToInt32(ConfigurationManager.AppSettings["SMTPServerPort"]));

            if (ConfigurationManager.AppSettings["SMTPUsername"] != "")
            {
                System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTPUsername"], ConfigurationManager.AppSettings["SMTPPassword"]);
                client.UseDefaultCredentials = false;
                client.Credentials = myCredential;
            }

            string url = ConfigurationManager.AppSettings["WebDashboardUrl"] + "/?key=" + user.HashKey;

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
            if (!this.CurrentUser.HasRole("Administrator") && this.CurrentUser.Id != userId)
            {
               throw Forbidden("You need to be administrator to see this resource.");
            }

            User user = this.Database.SingleOrDefault<User>(userId);

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
            if (!this.CurrentUser.HasRole("Administrator") && this.CurrentUser.Id != userId)
            {
                throw Forbidden("You need to be administrator to see this resource.");
            }

            User user = this.Database.SingleOrDefault<User>(userId);

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
                if (this.Database.Exists<Language>(data.InterfaceLanguageId.Value))
                {
                    user.InterfaceLanguageId = data.InterfaceLanguageId.Value;
                }
                else
                {
                    throw BadRequest("Language with that id not found.");
                }
            }

            using (var tr = this.Database.GetTransaction())
            {
                this.Database.Update(user);

                if (this.CurrentUser.HasRole("Administrator"))
                {
                    this.Database.Delete("EggOn.CoreUsersRoles", "UserId", null, user.Id);
                    foreach (var role in data.Roles)
                    {
                        this.Database.Insert("EggOn.CoreUsersRoles", null, false, new { UserId = user.Id, RoleId = role.Id });
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
            if (!this.CurrentUser.HasRole("Administrator") && this.CurrentUser.Id != userId)
            {
                throw Forbidden("You need to be administrator to see this resource.");
            }

            User user = this.Database.SingleOrDefault<User>(userId);

            if (user == null)
            {
                throw NotFound("User not Found.");
            }

            using (var tr = this.Database.GetTransaction())
            {
                this.Database.Delete("EggOn.CoreUsersRoles", "UserId", null, user.Id);
                this.Database.Delete(user);

                tr.Complete();
            }

            return Mapper.Map<UserDto>(user);
        }


        [Route("users/current"), HttpGet]
        public UserDto FetchCurrentUser()
        {
            return Mapper.Map<UserDto>(this.CurrentUser);
        }


        // ATTENTION: NOT REST!
        [Route("users/validate"), HttpPost, AllowAnonymous]
        public HttpResponseMessage ValidateUser([FromUri] string key)
        {
            User user = this.Database.SingleOrDefault<User>("WHERE HashKey = @0", key);

            if (user == null)
            {
                throw NotFound("User not Found.");
            }

            user.Validated = true;
            user.HashKey = null;

            this.Database.Update(user, new string[] { "Validated", "HashKey" });

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
            User user = this.Database.SingleOrDefault<User>("WHERE Email = @0", email.Trim());

            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    MessageType = "Information",
                    MessageContents = "Email sent."
                });
            }

            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["SMTPServerHost"], Convert.ToInt32(ConfigurationManager.AppSettings["SMTPServerPort"]));

            if (ConfigurationManager.AppSettings["SMTPUsername"] != "")
            {
                System.Net.NetworkCredential myCredential = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["SMTPUsername"], ConfigurationManager.AppSettings["SMTPPassword"]);
                client.UseDefaultCredentials = false;
                client.Credentials = myCredential;
            }


            if (ConfigurationManager.AppSettings["SMTPEmail"] != "")
            {
                user.HashKey = Guid.NewGuid().ToString("N").ToUpper();

                this.Database.Update(user, new string[] { "HashKey" });

                string url = "http://www." + ConfigurationManager.AppSettings["Domain"] + "/recover?key=" + user.HashKey;

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
            User user = this.Database.SingleOrDefault<User>("WHERE HashKey = @0", key);

            if (user == null)
            {
                throw NotFound("User not Found.");
            }

            user.Password = password;
            user.HashKey = null;

            this.Database.Update(user, new string[] { "Password", "HashKey" });

            return Request.CreateResponse(HttpStatusCode.OK, new ServiceMessage()
            {
                Type = MessageType.Information,
                Message = "Password changed."
            });
        }
    }
}
