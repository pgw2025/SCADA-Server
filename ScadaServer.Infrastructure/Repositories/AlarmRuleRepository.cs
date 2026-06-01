using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class AlarmRuleRepository : SqlSugarRepository<AlarmRule>, IAlarmRuleRepository { public AlarmRuleRepository(ISqlSugarClient db) : base(db) { } } }
