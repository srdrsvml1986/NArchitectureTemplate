using NArchitecture.Core.Application.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.UserSessions.Commands.CheckSuspiciousSessions;

// CheckSuspiciousSessionsResponse
public class CheckSuspiciousSessionsResponse : IResponse
{
    public Guid UserId { get; set; }
    public string Message { get; set; }
}
