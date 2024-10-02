using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using toom1s.TaskPlanner.Domain.Logic;
using toom1s.TaskPlanner.Domain.Models;
using toom1s.TaskPlanner.DataAccess.Abstractions;
using AbstractionsRepo = toom1s.TaskPlanner.DataAccess.Abstractions;

// Фейковий репозиторій для тестування
public class FakeWorkItemsRepository : AbstractionsRepo.IWorkItemsRepository
{
    private List<WorkItem> _workItems;

    public FakeWorkItemsRepository(List<WorkItem> initialItems = null)
    {
        _workItems = initialItems ?? new List<WorkItem>();
    }

    public Guid Add(WorkItem workItem)
    {
        workItem.Id = Guid.NewGuid();
        _workItems.Add(workItem);
        return workItem.Id;
    }

    public WorkItem Get(Guid id)
    {
        return _workItems.FirstOrDefault(item => item.Id == id);
    }

    public IEnumerable<WorkItem> GetAll()
    {
        return _workItems;
    }

    public bool Remove(Guid id)
    {
        var item = _workItems.FirstOrDefault(x => x.Id == id);
        if (item != null)
        {
            _workItems.Remove(item);
            return true;
        }
        return false;
    }

    public void SaveChanges()
    {
        // This would normally persist changes, but here it's not needed.
    }

    public bool Update(WorkItem workItem)
    {
        throw new NotImplementedException();
    }

    WorkItem[] AbstractionsRepo.IWorkItemsRepository.GetAll()
    {
        throw new NotImplementedException();
    }
}

// Тести для класу SimpleTaskPlanner без використання Moq
public class SimpleTaskPlannerTests
{
    // Тест на перевірку коректного сортування задач
    [Fact]
    public void CreatePlan_ShouldReturnSortedTasksByDueDate()
    {
        // Arrange
        var tasks = new List<WorkItem>
        {
            new WorkItem { Title = "Task 1", DueDate = new DateTime(2024, 10, 10), IsCompleted = false },
            new WorkItem { Title = "Task 2", DueDate = new DateTime(2024, 10, 05), IsCompleted = false },
            new WorkItem { Title = "Task 3", DueDate = new DateTime(2024, 10, 08), IsCompleted = false },
        };

        var fakeRepository = new FakeWorkItemsRepository(tasks);
        var planner = new SimpleTaskPlanner(fakeRepository);

        // Act: викликаємо метод CreatePlan
        var result = planner.CreatePlan();

        // Assert: перевіряємо, що задачі відсортовані за DueDate
        var expectedOrder = tasks.OrderBy(t => t.DueDate).ToList();
        Assert.Equal(expectedOrder, result);
    }

    // Тест на перевірку того, що план не містить завершених задач
    [Fact]
    public void CreatePlan_ShouldExcludeCompletedTasks()
    {
        // Arrange
        var tasks = new List<WorkItem>
        {
            new WorkItem { Title = "Task 1", IsCompleted = false },
            new WorkItem { Title = "Task 2", IsCompleted = true },  // Завершене завдання
            new WorkItem { Title = "Task 3", IsCompleted = false }
        };

        var fakeRepository = new FakeWorkItemsRepository(tasks);
        var planner = new SimpleTaskPlanner(fakeRepository);

        // Act
        var result = planner.CreatePlan();

        // Assert: переконайтеся, що завершені завдання не включені
        Assert.All(result, task => Assert.False(task.IsCompleted));
    }

    // Тест на перевірку, що всі релевантні (незавершені) задачі включені в план
    [Fact]
    public void CreatePlan_ShouldIncludeAllRelevantTasks()
    {
        // Arrange
        var tasks = new List<WorkItem>
        {
            new WorkItem { Title = "Task 1", IsCompleted = false },
            new WorkItem { Title = "Task 2", IsCompleted = true },
            new WorkItem { Title = "Task 3", IsCompleted = false }
        };

        var fakeRepository = new FakeWorkItemsRepository(tasks);
        var planner = new SimpleTaskPlanner(fakeRepository);

        // Act
        var result = planner.CreatePlan().ToList();

        // Assert: перевіряємо, що всі незавершені завдання включені в план
        var expectedTasks = tasks.Where(t => !t.IsCompleted).ToList();
        Assert.Equal(expectedTasks.Count, result.Count);  // Перевіряємо кількість завдань
    }

    // Тест на перевірку, що якщо всі задачі завершені, план порожній
    [Fact]
    public void CreatePlan_ShouldReturnEmptyIfAllTasksAreCompleted()
    {
        // Arrange
        var tasks = new List<WorkItem>
        {
            new WorkItem { Title = "Task 1", IsCompleted = true },
            new WorkItem { Title = "Task 2", IsCompleted = true }
        };

        var fakeRepository = new FakeWorkItemsRepository(tasks);
        var planner = new SimpleTaskPlanner(fakeRepository);

        // Act
        var result = planner.CreatePlan();

        // Assert: переконайтеся, що план порожній, якщо всі задачі завершені
        Assert.Empty(result);
    }
}