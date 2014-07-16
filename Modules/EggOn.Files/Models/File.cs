using FlowOptions.EggOn.Base;
using FlowOptions.EggOn.Base.Models;
using FlowOptions.EggOn.DataHost;
using FlowOptions.EggOn.ModuleCore;
using System;
using System.Collections.Generic;

namespace FlowOptions.EggOn.Files.Models
{
    [TableName("[EggOn].[FilesFiles]"), PrimaryKey("Id", autoIncrement = false)]
    public class File
    {
        [DefaultValue("newsequentialid()")]
        public Guid Id { get; set; }

        public string Name { get; set; }


        public int Size { get; set; }

        // Contains mimetype.
        [AllowNull]
        public string ContentType { get; set; }


        #region For files in repositories.

        [AllowNull, Constraint(typeof(Repository))]
        public Guid? RepositoryId { get; set; }

        [AllowNull, Constraint(typeof(File))]
        public Guid? ParentFileId { get; set; }

        public FileTypes Type { get; set; }

        [Ignore]
        public File Parent 
        {
            get 
            {
                if (ParentFileId == null)
                    return null;

                using (var database = new EggOnDatabase())
                {
                    return database.SingleOrDefault<File>(ParentFileId);
                }
            }
        }

        [Ignore]
        public List<File> Children
        {
            get
            {
                using (var database = new EggOnDatabase())
                {
                    return database.Fetch<File>("WHERE ParentFileId = @0 AND RepositoryId = @1", Id, RepositoryId);
                }
            }
        }

        #endregion

        #region For database embedded files.

        [AllowNull]
        public byte[] Contents { get; set; }

        #endregion


        // Details about the file:

        [Constraint(typeof(User))]
        public Guid UserId { get; set; }

        public DateTime UploadDate { get; set; }
    }

    public enum FileTypes
    {
        File = 1,
        Folder = 2,
        Link = 3
    }
}