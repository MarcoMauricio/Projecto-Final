using System.Collections.Generic;
namespace Context.Models
{
    public class Document
    {
        public Document()
        {
            Entities = new List<Entity>();
        }
        public string Id { get; set; }

        public List<Entity> Entities { get; set; }
    }
}