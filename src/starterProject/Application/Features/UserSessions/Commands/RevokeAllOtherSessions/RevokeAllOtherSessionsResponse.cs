using NArchitectureTemplate.Core.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserSessions.Commands.RevokeAllOtherSessions;
public class RevokeAllOtherSessionsResponse : IResponse
{
    public Guid UserId { get; set; }
    public int RevokedSessionCount { get; set; }
    public string Message { get; set; }
}
