using Microsoft.EntityFrameworkCore;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Data.Repositories;

public class TaskItemRepository(AppDbContext context) 
    : ITaskItemRepository
{
    public async Task<IReadOnlyList<TaskItem>> GetAll()
    {
        return await context.TaskItems
            .AsNoTracking()
            .Include(ti =>ti.Author)
            .Include(ti => ti.AssignedUser)
            .ToListAsync();
    }

    public async Task<TaskItem?> GetById(Guid id)
    {
        return await context.TaskItems
            .AsNoTracking()
            .Include(ti => ti.Author)
            .Include(ti => ti.AssignedUser)
            .FirstOrDefaultAsync(ti => ti.Id == id);
    }

    public async Task<IReadOnlyList<TaskItem>> GetFilteredTasks(Guid teamId,
        TaskItemStatus? taskStatus, TaskItemPriority? taskPriority)
    {
        IQueryable<TaskItem> query = context.TaskItems
            .AsNoTracking()
            .Where(ti => ti.TeamId == teamId)
            .Include(ti => ti.Author)
            .Include(ti => ti.AssignedUser);

        if (taskStatus.HasValue)
        {
            query = query.Where(ti => ti.Status == taskStatus);
        }

        if (taskPriority.HasValue)
        {
            query = query.Where(ti => ti.Priority == taskPriority);
        }

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<TaskItem>> GetForTeam(Guid teamId)
    {
        return await context.TaskItems
            .AsNoTracking()
            .Where(ti => ti.TeamId == teamId)
            .Include(ti => ti.Author)
            .Include(ti => ti.AssignedUser)
            .ToListAsync();
    }

    public async Task Add(TaskItem entity)
    {
        await context.TaskItems.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task Update(TaskItem entity)
    {
        var trackedEntity = context.TaskItems.Local
            .FirstOrDefault(ti => ti.Id == entity.Id);

        if (trackedEntity != null)
        {
            context.Entry(trackedEntity).State = EntityState.Detached;
        }

        context.TaskItems.Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        TaskItem? taskItem = await context.TaskItems.FindAsync(id);

        if (taskItem != null)
        {
            context.TaskItems.Remove(taskItem);
            await context.SaveChangesAsync();
        }
    }
}
