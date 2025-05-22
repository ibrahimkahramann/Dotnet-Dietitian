using MediatR;
using System;

namespace Dotnet_Dietitian.Application.Features.CQRS.Commands.AppUserCommands
{
    public class UpdatePasswordCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }
} 