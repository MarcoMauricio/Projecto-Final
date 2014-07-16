using AutoMapper;
using FlowOptions.EggOn.Base.Models;
using FlowOptions.EggOn.Base.ViewModels;
using FlowOptions.EggOn.ModuleCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FlowOptions.EggOn.Base.Controllers
{
    public class RolesController : EggOnApiController
    {
        // GET /api/roles
        [Route("roles"), HttpGet]
        public List<RoleDto> GetAllUsers()
        {
            if (!CurrentUser.HasRole("Administrator"))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, new {
                    Error = "You need to be administrator to see this resource."
                }));
            }

            var roles = Database.Fetch<Role>("");

            return Mapper.Map<List<Role>, List<RoleDto>>(roles);
        }

        // POST /api/roles
        [Route("roles"), HttpPost]
        public HttpResponseMessage CreateRole([FromBody] RoleDto data)
        {
            if (!CurrentUser.HasRole("Administrator"))
            {
                throw new HttpResponseException(Request.CreateResponse(HttpStatusCode.Forbidden, new
                {
                    Error = "You need to be administrator to create this resource."
                }));
            }

            var role = new Role();

            role.Id = GuidComb.NewGuid();

            // TODO: Role name check.

            role.Name = data.Name;

            Database.Insert(role);

            return Request.CreateResponse<RoleDto>(HttpStatusCode.Created, Mapper.Map<RoleDto>(role));
        }

        // GET /api/roles/<id>
        [Route("roles/{roleId:guid}"), HttpGet]
        public RoleDto GetRole(Guid roleId)
        {
            var role = Database.SingleOrDefault<Role>(roleId);

            if (role == null)
            {
                throw NotFound("Role not Found.");
            }

            return Mapper.Map<RoleDto>(role);
        }

        // PUT /api/users/<id>
        [Route("roles/{roleId:guid}"), HttpPut]
        public RoleDto UpdateRole(Guid roleId, [FromBody] RoleDto data)
        {
            if (!CurrentUser.HasRole("Administrator"))
            {
                throw Forbidden("You need to be administrator to update this resource.");
            }

            var role = Database.SingleOrDefault<Role>(roleId);

            if (role == null)
            {
                throw NotFound("Role not Found.");
            }

            if (data.Name != null)
                role.Name = data.Name;

            Database.Update(role, new string[] { "Name" });

            return Mapper.Map<RoleDto>(role);
        }

        // DELETE /api/roles/<id>
        [Route("roles/{roleId:guid}"), HttpDelete]
        public RoleDto DeleteRole(Guid roleId)
        {
            if (!CurrentUser.HasRole("Administrator"))
            {
                throw Forbidden("You need to be administrator to delete this resource.");
            }

            var role = Database.SingleOrDefault<Role>(roleId);

            if (role == null)
            {
                throw NotFound("Role not Found.");
            }

            Database.Delete(role);

            return Mapper.Map<RoleDto>(role);
        }


        [Route("roles/current"), HttpGet]
        public List<RoleDto> FetchCurrentRoles()
        {
            return Mapper.Map<List<RoleDto>>(CurrentUser.Roles);
        }
    }
}
