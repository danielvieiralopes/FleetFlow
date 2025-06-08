using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FleetFlow.Domain.Entities
{
    /// <summary>
    /// Representa um usuário do sistema.
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; 
        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}
