using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UserEntity
    {
        [Key] public int Id { get; set; }
        public required string Name { get; set; }

        public DateTime LastOnline { get; set; }
    }
}
