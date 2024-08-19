using System;
using System.Collections.Generic;
using System.Globalization;
using toom1s.TaskPlanner.Domain.Logic;
using toom1s.TaskPlanner.Domain.Models.Enums;
using toom1s.TaskPlanner.Domain.Models;

internal static class Program
{
    public static void Main(string[] args)
    {
        var workItems = new List<WorkItem>();

        while (true)
        {
            Console.WriteLine("Enter a WorkItem (or type 'done' to finish):");

            Console.Write("Title: ");
            string title = Console.ReadLine();
            if (title?.ToLower() == "done") break;

            Console.Write("DueDate (dd.MM.yyyy): ");
            DateTime dueDate;
            while (!DateTime.TryParseExact(Console.ReadLine(), "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dueDate))
            {
                Console.Write("Invalid date format. Enter again (dd.MM.yyyy): ");
            }

            Console.Write("Priority (None, Low, Medium, High, Urgent): ");
            Priority priority;
            while (!Enum.TryParse(Console.ReadLine(), true, out priority))
            {
                Console.Write("Invalid priority. Enter again (None, Low, Medium, High, Urgent): ");
            }

            Console.Write("Complexity (None, Minutes, Hours, Days, Weeks): ");
            Complexity complexity;
            while (!Enum.TryParse(Console.ReadLine(), true, out complexity))
            {
                Console.Write("Invalid complexity. Enter again (None, Minutes, Hours, Days, Weeks): ");
            }

            Console.Write("Description: ");
            string description = Console.ReadLine();

            workItems.Add(new WorkItem
            {
                Title = title,
                DueDate = dueDate,
                Priority = priority,
                Complexity = complexity,
                Description = description,
                CreationDate = DateTime.Now,
                IsCompleted = false
            });
        }

        var taskPlanner = new SimpleTaskPlanner();
        WorkItem[] sortedWorkItems = taskPlanner.CreatePlan(workItems.ToArray());

        Console.WriteLine("\nSorted WorkItems:");
        foreach (var item in sortedWorkItems)
        {
            Console.WriteLine(item);
        }
    }
}
