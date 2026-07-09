using TaskManager.Domain.Enums;

namespace TaskManager.Application.DTOs;

public record AddMemberRequest(Guid UserId, MemberRole Role);