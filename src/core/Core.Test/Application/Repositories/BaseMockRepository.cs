﻿using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NArchitecture.Core.Application.Rules;
using NArchitecture.Core.Localization.Resource.Yaml;
using NArchitecture.Core.Persistence.Repositories;
using NArchitecture.Core.Test.Application.FakeData;
using NArchitecture.Core.Test.Application.Helpers;

namespace NArchitecture.Core.Test.Application.Repositories;

public abstract class BaseMockRepository<TRepository, TEntity, TEntityId, TMappingProfile, TBusinessRules, TFakeData>
    where TEntity : Entity<TEntityId>, new()
    where TRepository : class, IAsyncRepository<TEntity, TEntityId>, IRepository<TEntity, TEntityId>
    where TMappingProfile : Profile, new()
    where TBusinessRules : BaseBusinessRules
    where TFakeData : BaseFakeData<TEntity, TEntityId>, new()
{
    public IMapper Mapper;
    public Mock<TRepository> MockRepository;
    public TBusinessRules BusinessRules;

    public BaseMockRepository(TFakeData fakeData)
    {
        var mapperConfig = new MapperConfiguration(
         c => c.AddProfile(new TMappingProfile()),
         new NullLoggerFactory() // LoggerFactory parametresi zorunlu
        );
        Mapper = mapperConfig.CreateMapper();

        MockRepository = MockRepositoryHelper.GetRepository<TRepository, TEntity, TEntityId>(fakeData.Data);
        BusinessRules =
            (TBusinessRules)
                Activator.CreateInstance(
                    type: typeof(TBusinessRules),
                    MockRepository.Object,
                    new ResourceLocalizationService(resources: []) { AcceptLocales = new[] { "en" } }
                )! ?? throw new InvalidOperationException($"Cannot create an instance of {typeof(TBusinessRules).FullName}.");
    }
}
