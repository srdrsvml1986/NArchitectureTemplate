﻿using Domain.Entities;
using NArchitectureTemplate.Core.Persistence.Repositories;

namespace Application.Services.Repositories;

public interface IEmailAuthenticatorRepository
    : IAsyncRepository<EmailAuthenticator, Guid>,
        IRepository<EmailAuthenticator, Guid> { }
