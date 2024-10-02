using System.Text;
using System.Threading.Tasks;
using toom1s.TaskPlanner.Domain.Models;
using toom1s.TaskPlanner.Domain.Logic;
namespace toom1s.TaskPlanner.DataAccess.Abstractions
{
    public interface IWorkItemsRepository
    {
        Guid Add(WorkItem workItem);


        WorkItem Get(Guid id);


        WorkItem[] GetAll();

        bool Update(WorkItem workItem);

        bool Remove(Guid id);

        void SaveChanges();
    }
}
