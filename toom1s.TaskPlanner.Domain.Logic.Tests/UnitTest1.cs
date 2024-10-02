using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using toom1s.TaskPlanner.Domain.Logic;
using toom1s.TaskPlanner.Domain.Models;
using toom1s.TaskPlanner.DataAccess.Abstractions;
using AbstractionsRepo = toom1s.TaskPlanner.DataAccess.Abstractions;

// �������� ���������� ��� ����������
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

// ����� ��� ����� SimpleTaskPlanner ��� ������������ Moq
public class SimpleTaskPlannerTests
{
    // ���� �� �������� ���������� ���������� �����
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

        // Act: ��������� ����� CreatePlan
        var result = planner.CreatePlan();

        // Assert: ����������, �� ������ ���������� �� DueDate
        var expectedOrder = tasks.OrderBy(t => t.DueDate).ToList();
        Assert.Equal(expectedOrder, result);
    }

    // ���� �� �������� ����, �� ���� �� ������ ���������� �����
    [Fact]
    public void CreatePlan_ShouldExcludeCompletedTasks()
    {
        // Arrange
        var tasks = new List<WorkItem>
        {
            new WorkItem { Title = "Task 1", IsCompleted = false },
            new WorkItem { Title = "Task 2", IsCompleted = true },  // ��������� ��������
            new WorkItem { Title = "Task 3", IsCompleted = false }
        };

        var fakeRepository = new FakeWorkItemsRepository(tasks);
        var planner = new SimpleTaskPlanner(fakeRepository);

        // Act
        var result = planner.CreatePlan();

        // Assert: �������������, �� �������� �������� �� �������
        Assert.All(result, task => Assert.False(task.IsCompleted));
    }

    // ���� �� ��������, �� �� ��������� (����������) ������ ������� � ����
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

        // Assert: ����������, �� �� ���������� �������� ������� � ����
        var expectedTasks = tasks.Where(t => !t.IsCompleted).ToList();
        Assert.Equal(expectedTasks.Count, result.Count);  // ���������� ������� �������
    }

    // ���� �� ��������, �� ���� �� ������ ��������, ���� �������
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

        // Assert: �������������, �� ���� �������, ���� �� ������ ��������
        Assert.Empty(result);
    }
}