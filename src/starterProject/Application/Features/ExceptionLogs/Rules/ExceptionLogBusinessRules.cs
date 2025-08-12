using Application.Features.ExceptionLogs.Constants;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.ExceptionLogs.Rules;

public class ExceptionLogBusinessRules : BaseBusinessRules
{
    private readonly IExceptionLogRepository _exceptionLogRepository;
    private readonly ILocalizationService _localizationService;

    public ExceptionLogBusinessRules(IExceptionLogRepository exceptionLogRepository, ILocalizationService localizationService)
    {
        _exceptionLogRepository = exceptionLogRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, ExceptionLogsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task ExceptionLogShouldExistWhenSelected(ExceptionLog? exceptionLog)
    {
        if (exceptionLog == null)
            await throwBusinessException(ExceptionLogsBusinessMessages.ExceptionLogNotExists);
    }

    public async Task ExceptionLogIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        ExceptionLog? exceptionLog = await _exceptionLogRepository.GetAsync(
            predicate: el => el.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await ExceptionLogShouldExistWhenSelected(exceptionLog);
    }
}