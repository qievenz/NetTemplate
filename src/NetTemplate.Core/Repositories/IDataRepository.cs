using NetTemplate.Core.Entities;

namespace NetTemplate.Core.Repositories
{
    public interface IDataRepository
    {
        Task<IEnumerable<Data>> GetAll();
        Task<Data?> GetById(int id);
        Task Add(Data data);
        Task Delete(int id);
        Task Update(Data data);
    }
}
