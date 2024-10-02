using System;
using System.Collections.Generic;
using System.Globalization;
using toom1s.TaskPlanner.Domain.Logic;
using toom1s.TaskPlanner.Domain.Models.Enums;
using toom1s.TaskPlanner.Domain.Models;
using toom1s.TaskPlanner.DataAccess;
using toom1s.TaskPlanner.DataAccess.Abstractions;

internal static class Program
{
    private static FileWorkItemsRepository repository = new FileWorkItemsRepository();

    static void Main(string[] args)
    {
        Console.WriteLine("Welcome to Task Planner!");

        bool running = true;

        while (running)
        {
            Console.WriteLine("\nPlease choose an action:");
            Console.WriteLine("[A]dd work item");
            Console.WriteLine("[B]uild a plan (list all work items)");
            Console.WriteLine("[M]ark work item as completed");
            Console.WriteLine("[R]emove a work item");
            Console.WriteLine("[Q]uit the app");
            Console.Write("Your choice: ");
            string choice = Console.ReadLine()?.ToUpper();

            switch (choice)
            {
                case "A":
                    AddWorkItem();
                    break;
                case "B":
                    BuildPlan();
                    break;
                case "M":
                    MarkAsCompleted();
                    break;
                case "R":
                    RemoveWorkItem();
                    break;
                case "Q":
                    running = false;
                    Console.WriteLine("Exiting the app...");
                    repository.SaveChanges();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    // Метод для додавання нового завдання
    // Метод для додавання нового завдання
    static void AddWorkItem()
    {
        Console.WriteLine("\nAdding a new work item:");

        Console.Write("Title: ");
        string title = Console.ReadLine();

        Console.Write("Description: ");
        string description = Console.ReadLine();

        Priority priority;
        while (true)
        {
            Console.Write("Priority (None, Low, Medium, High, Urgent): ");
            string priorityInput = Console.ReadLine();
            if (Enum.TryParse(priorityInput, true, out priority) && Enum.IsDefined(typeof(Priority), priority))
            {
                break;
            }
            Console.WriteLine("Invalid priority value. Please enter None, Low, Medium, High, or Urgent.");
        }

        Complexity complexity;
        while (true)
        {
            Console.Write("Complexity (None, Minutes, Hours, Days, Weeks): ");
            string complexityInput = Console.ReadLine();
            if (Enum.TryParse(complexityInput, true, out complexity) && Enum.IsDefined(typeof(Complexity), complexity))
            {
                break;
            }
            Console.WriteLine("Invalid complexity value. Please enter None, Minutes, Hours, Days, or Weeks.");
        }

        DateTime dueDate;
        while (true)
        {
            Console.Write("Due date (yyyy-mm-dd): ");
            if (DateTime.TryParse(Console.ReadLine(), out dueDate))
            {
                break;
            }
            Console.WriteLine("Invalid date format. Please enter the date in the format yyyy-mm-dd.");
        }

        WorkItem workItem = new WorkItem
        {
            Title = title,
            Description = description,
            Priority = priority,
            Complexity = complexity,
            DueDate = dueDate,
            IsCompleted = false
        };

        Guid newId = repository.Add(workItem);
        Console.WriteLine($"Work item added with ID: {newId}");
        repository.SaveChanges();
    }


    // Метод для відображення всіх завдань
    static void BuildPlan()
    {
        Console.WriteLine("\nWork items:");
        var workItems = repository.GetAll().ToList();
        if (workItems.Count == 0)
        {
            Console.WriteLine("No work items found.");
        }
        else
        {
            foreach (var item in workItems)
            {
                Console.WriteLine($"{item.Id}: {item}");
            }
        }
    }

    // Метод для відзначення завдання як виконаного
    static void MarkAsCompleted()
    {
        Console.Write("\nEnter the ID of the work item to mark as completed: ");
        Guid id = Guid.Parse(Console.ReadLine());

        var workItem = repository.Get(id);
        if (workItem != null)
        {
            workItem.IsCompleted = true;
            Console.WriteLine($"Work item {workItem.Title} marked as completed.");
            repository.SaveChanges();
        }
        else
        {
            Console.WriteLine("Work item not found.");
        }
    }

    // Метод для видалення завдання
    static void RemoveWorkItem()
    {
        Console.Write("\nEnter the ID of the work item to remove: ");
        Guid id = Guid.Parse(Console.ReadLine());

        bool removed = repository.Remove(id);
        if (removed)
        {
            Console.WriteLine("Work item removed successfully.");
            repository.SaveChanges();
        }
        else
        {
            Console.WriteLine("Work item not found.");
        }
    }
}
