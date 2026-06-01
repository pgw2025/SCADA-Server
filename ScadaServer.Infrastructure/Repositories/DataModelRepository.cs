using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class DataModelRepository : SqlSugarRepository<DataModel>, IDataModelRepository { public DataModelRepository(ISqlSugarClient db) : base(db) { } } }
