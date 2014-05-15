using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace FlowOptions.EggOn.Base.Models
{
    [TableName("[EggOn].[CoreUsers]"), PrimaryKey("Id", autoIncrement = false)]
    public class User : IPrincipal
    {
        [DefaultValue("newsequentialid()")]
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Email { get; set; }
        
        [Column(Name="Password")]
        public string HashedPassword { get; set; }
        
        [Ignore]
        public string Password
        {
            get
            {
                return null;
            }

            set
            {
                if (value != null)
                    this.HashedPassword = BCrypt.Net.BCrypt.HashPassword(value);
                else
                    this.HashedPassword = null;
            }
        }

        public bool Validated { get; set; }

        // Used as a key for user validation and password recovery.
        [AllowNull]
        public string HashKey { get; set; }

        [AllowNull]
        public DateTime LastAction { get; set; }

        [Constraint(typeof(Language), "Id")]
        public Guid InterfaceLanguageId { get; set; }

        [Ignore]
        public Language InterfaceLanguage { 
            get {
                using (var database = new EggOnDatabase())
                {
                    return database.SingleOrDefault<Language>(this.InterfaceLanguageId);
                }
            }
        }

        [Ignore]
        public List<Role> Roles
        {
            get
            {
                using (var database = new EggOnDatabase())
                {
                    var sql = Sql.Builder
                        .Select("CoreRoles.*")
                        .From("EggOn.CoreUsersRoles").InnerJoin("EggOn.CoreRoles").On("CoreUsersRoles.RoleId = CoreRoles.Id")
                        .Where("UserId = @0", this.Id);
                    return database.Fetch<Role>(sql);
                }
            }
        }

        public bool CheckPassword(string password)
        {
            if (this.HashedPassword == null)
                return false;

            return BCrypt.Net.BCrypt.Verify(password, this.HashedPassword);
        }

        public bool HasRole(string role)
        {
            using (var database = new EggOnDatabase())
            {
                var sql = Sql.Builder
                    .Select("COUNT(*)")
                    .From("EggOn.CoreUsersRoles").InnerJoin("EggOn.CoreRoles").On("CoreUsersRoles.RoleId = CoreRoles.Id")
                    .Where("UserId = @0 AND (Name = @1 OR Name = @2)", this.Id, role, "Administrator");
                return database.ExecuteScalar<int>(sql) != 0;
            }
        }

        #region IPrincipal Properties

        [Ignore]
        public IIdentity Identity
        {
            get { return null; }
        }

        public bool IsInRole(string role)
        {
            return HasRole(role);
        }

        #endregion
    }
}