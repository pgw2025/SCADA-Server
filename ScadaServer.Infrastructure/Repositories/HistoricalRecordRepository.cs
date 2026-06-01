using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class HistoricalRecordRepository : SqlSugarRepository<HistoricalRecord>, IHistoricalRecordRepository { public HistoricalRecordRepository(ISqlSugarClient db) : base(db) { } } }
