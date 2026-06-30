using TaskManager.API.Domain.Enums;

namespace TaskManager.API.Application.DTOs;

public record AddMemberRequest(Guid UserId, MemberRole Role);