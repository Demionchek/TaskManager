using MediatR;
using TaskManager.Application.Common;
using TaskManager.Application.DTOs;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Projects.Commands;

public record CreateProjectCommand(Guid UserId, string Name, string? Description) :  IRequest<ProjectResponse>;

public class CreateProjectCommandHandler(IAppDbContext db) : IRequestHandler<CreateProjectCommand, ProjectResponse>
{
    public async Task<ProjectResponse> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        Project project = new Project {
            Id = Guid.NewGuid(),
            OwnerId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            Name = request.Name,
            Description = request.Description
        };
        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);
        return new ProjectResponse(project);
    }
}