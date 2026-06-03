using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskHub.Data;
using TaskHub.Data.Entities;
using TaskHub.Data.Repositories;

namespace TaskHub.Tests
{
    public class TaskRepositoryTest
    {
        private AppDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddingTaskItem_ShouldIncreaseCount()
        {
            // Arrange
            var context = GetDbContext();
            var repository = new TaskItemRepository(context);
            var taskItem = new TaskItem(
                "New task", 
                "Task description", 
                TaskItemPriority.Middle, 
                DateTime.UtcNow.AddDays(14),
                Guid.NewGuid());

            // Act
            await repository.Add(taskItem);

            // Assert
            var allTasks = await repository.GetAll();
            Assert.Single(allTasks);
            Assert.Equal(taskItem.Id, allTasks.First().Id);
        }
    }
}
