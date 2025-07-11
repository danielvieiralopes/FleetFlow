﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FleetFlow.Domain.Entities;

namespace FleetFlow.Application.Interfaces
{
    /// <summary>
    /// Define o contrato para o repositório de usuários.
    /// </summary>
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task AddAsync(User user);
    }
}
