using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class VariableTriggerRepository : SqlSugarRepository<VariableTrigger>, IVariableTriggerRepository { public VariableTriggerRepository(ISqlSugarClient db) : base(db) { } } }
