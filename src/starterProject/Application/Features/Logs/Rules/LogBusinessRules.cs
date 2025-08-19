using Application.Features.Logs.Constants;
using Application.Services.Repositories;
using NArchitectureTemplate.Core.Application.Rules;
using NArchitectureTemplate.Core.CrossCuttingConcerns.Exception.Types;
using NArchitectureTemplate.Core.Localization.Abstraction;
using Domain.Entities;

namespace Application.Features.Logs.Rules;

public class LogBusinessRules : BaseBusinessRules
{
    private readonly ILogRepository _logRepository;
    private readonly ILocalizationService _localizationService;

    public LogBusinessRules(ILogRepository logRepository, ILocalizationService localizationService)
    {
        _logRepository = logRepository;
        _localizationService = localizationService;
    }

    private async Task throwBusinessException(string messageKey)
    {
        string message = await _localizationService.GetLocalizedAsync(messageKey, LogsBusinessMessages.SectionName);
        throw new BusinessException(message);
    }

    public async Task LogShouldExistWhenSelected(Log? log)
    {
        if (log == null)
            await throwBusinessException(LogsBusinessMessages.LogNotExists);
    }

    public async Task LogIdShouldExistWhenSelected(Guid id, CancellationToken cancellationToken)
    {
        Log? log = await _logRepository.GetAsync(
            predicate: l => l.Id == id,
            enableTracking: false,
            cancellationToken: cancellationToken
        );
        await LogShouldExistWhenSelected(log);
    }
}