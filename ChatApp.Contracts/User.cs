using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ChatApp.Contracts
{
    public record User
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public DateTime LastOnline { get; set; } = DateTime.Now;

        [JsonIgnore]
        public IPEndPoint? EndPoint { get; set; }

        public static User FromDomain(UserEntity userEntity) => new()
        {
            Id = userEntity.Id,
            Name = userEntity.Name,
            LastOnline = userEntity.LastOnline,
        };
    }
}
