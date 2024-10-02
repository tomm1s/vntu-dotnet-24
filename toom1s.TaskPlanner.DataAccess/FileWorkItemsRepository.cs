using System.Text;
using System.Threading.Tasks;
using toom1s.TaskPlanner.Domain.Models;
using toom1s.TaskPlanner.Domain.Logic;
using toom1s.TaskPlanner.DataAccess.Abstractions;
using Newtonsoft.Json;
using AbstractionsRepo = toom1s.TaskPlanner.DataAccess.Abstractions;

namespace toom1s.TaskPlanner.DataAccess
{
    public class FileWorkItemsRepository :AbstractionsRepo.IWorkItemsRepository
    {
        private const string FileName = "work-items.json";
        private readonly Dictionary<Guid, WorkItem> _workItems;

        public FileWorkItemsRepository()
        {
            _workItems = new Dictionary<Guid, WorkItem>();
            if (File.Exists(FileName))
            {
                var fileContent = File.ReadAllText(FileName);
                if (!string.IsNullOrWhiteSpace(fileContent))
                {
                    var workItemsArray = JsonConvert.DeserializeObject<WorkItem[]>(fileContent);
                    _workItems = workItemsArray.ToDictionary(w => w.Id, w => w);

                    _workItems = new Dictionary<Guid, WorkItem>();
                    foreach (var workItem in workItemsArray)
                    {
                        _workItems[workItem.Id] = workItem;
                    }
                }
                else
                {
                    _workItems = new Dictionary<Guid, WorkItem>();
                }
            }
            else
                {
                    _workItems = new Dictionary<Guid, WorkItem>();
                }
        }

        public Guid Add(WorkItem workItem)
        {
            var newWorkItem = workItem.Clone();
            newWorkItem.Id = Guid.NewGuid();
            _workItems[newWorkItem.Id] = newWorkItem;
            return newWorkItem.Id;
        }

        public WorkItem Get(Guid id)
        {
            _workItems.TryGetValue(id, out var workItem);
            return workItem;
        }

        public WorkItem[] GetAll()
        {
            return _workItems.Values.ToArray();
        }

        public bool Remove(Guid id)
        {
            return _workItems.Remove(id);
        }

        public void SaveChanges()
        {
            var workItemsArray = _workItems.Values.ToList();
            string json = JsonConvert.SerializeObject(workItemsArray, Formatting.Indented);
            File.WriteAllText(FileName, json);
        }

        public bool Update(WorkItem workItem)
        {
            if (_workItems.ContainsKey(workItem.Id))
            {
                _workItems[workItem.Id] = workItem;
                return true;
            }
            return false;
        }

    }
}
