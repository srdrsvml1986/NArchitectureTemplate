using Application.Features.Users.Constants;
using Application.Features.Users.Rules;
using Application.Services.Repositories;
using AutoMapper;
using Domain.Entities;
using MediatR;
using NArchitectureTemplate.Core.Application.Pipelines.Authorization;
using NArchitectureTemplate.Core.Application.Responses;
using static Application.Features.Users.Constants.UsersOperationClaims;

namespace Application.Features.Users.Queries.GetUserSessionsByUserId;

public class GetUserSessionsByUserIdQuery : IRequest<GetUserSessionsByUserIdResponse>, ISecuredRequest
{
    public Guid UserId { get; set; }

    public string[] Roles => [Read];

    public class GetUserSessionsByUserIdQueryHandler : IRequestHandler<GetUserSessionsByUserIdQuery, GetUserSessionsByUserIdResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IMapper _mapper;
        private readonly UserBusinessRules _userBusinessRules;

        public GetUserSessionsByUserIdQueryHandler(
            IUserRepository userRepository,
            IMapper mapper,
            UserBusinessRules userBusinessRules,
            IUserSessionRepository userSessionRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _userBusinessRules = userBusinessRules;
            _userSessionRepository = userSessionRepository;
        }

        public async Task<GetUserSessionsByUserIdResponse> Handle(GetUserSessionsByUserIdQuery request, CancellationToken cancellationToken)
        {
            await _userBusinessRules.UserIdShouldBeExistsWhenSelected(request.UserId);

            var userSessions = _userSessionRepository.Query().Where(g => g.UserId==request.UserId);

            return new GetUserSessionsByUserIdResponse { UserSessions = userSessions };
        }
    }
}

public class GetUserSessionsByUserIdResponse : IResponse
{
    public IQueryable<UserSession>? UserSessions { get; set; }
}
