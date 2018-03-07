using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB;

namespace RedDevTeamNames.Models
{
    public class Note : IComparable
    {
        [BsonId]
        public string Id { get; set; }
        public string Subject { get; set; }
        public string Details { get; set; }
        public int Priority { get; set; }

        public int CompareTo(object obj)
        {
            Note sortThis = obj as Note;
            
            if (sortThis != null)
            {
                return this.Priority.CompareTo(sortThis.Priority);
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }

}
