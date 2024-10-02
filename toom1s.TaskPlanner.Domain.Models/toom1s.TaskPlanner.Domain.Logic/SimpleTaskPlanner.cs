using System;
using System.Collections.Generic;
using System.Linq;
using toom1s.TaskPlanner.Domain.Models;

namespace toom1s.TaskPlanner.Domain.Logic
{
    public class SimpleTaskPlanner
    {
        public WorkItem[] CreatePlan(WorkItem[] items)
        {
            var itemsAsList = items.ToList();
            itemsAsList.Sort(CompareWorkItems);
            return itemsAsList.ToArray();
            
        }
        private static int CompareWorkItems(WorkItem firstItem,WorkItem secondItem)
        {
            int priorityComparison = secondItem.Priority.CompareTo(firstItem.Priority);
            if (priorityComparison != 0)
            {
                return priorityComparison;
            }
            int dueDateComparison = firstItem.DueDate.CompareTo(secondItem.DueDate);
            if (dueDateComparison != 0)
            {
                return dueDateComparison;
            }
            return string.Compare(firstItem.Title, secondItem.Title, StringComparison.Ordinal);
        }
    }
}
