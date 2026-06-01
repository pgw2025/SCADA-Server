using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class ModelVariableRepository : SqlSugarRepository<ModelVariable>, IModelVariableRepository { public ModelVariableRepository(ISqlSugarClient db) : base(db) { } } }
