using Microsoft.EntityFrameworkCore;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories.Interfaces;

namespace TaskHub.Data.Repositories;

public class TaskItemRepository
    : ITaskItemRepository
{
    private readonly AppDbContext _context;

    public TaskItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskItem>> GetAll()
    {
        return await _context.TaskItems
            .Include(ti =>ti.Author)
            .Include(ti => ti.AssignedUser)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TaskItem?> GetById(Guid id)
    {
        return await _context.TaskItems
            .Include(ti => ti.Author)
            .Include(ti => ti.AssignedUser)
            .FirstOrDefaultAsync(ti => ti.Id == id);
    }

    public async Task<IEnumerable<TaskItem>> GetByStatusAndPriority(
        TaskItemStatus? taskStatus, TaskItemPriority? taskPriority)
    {
        IQueryable<TaskItem> query = _context.TaskItems
            .AsNoTracking();

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

    public async Task Add(TaskItem entity)
    {
        await _context.TaskItems.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Update(TaskItem entity)
    {
        var trackedEntity = _context.TaskItems.Local
            .FirstOrDefault(ti => ti.Id == entity.Id);

        if (trackedEntity != null)
        {
            _context.Entry(trackedEntity).State = EntityState.Detached;
        }

        _context.TaskItems.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(Guid id)
    {
        TaskItem? taskItem = await _context.TaskItems.FindAsync(id);

        if (taskItem != null)
        {
            _context.TaskItems.Remove(taskItem);
            await _context.SaveChangesAsync();
        }
    }
}
