using ScadaServer.Application.Interfaces;
using SqlSugar; using ScadaServer.Domain.Entities; using ScadaServer.Infrastructure.Persistence;  namespace ScadaServer.Infrastructure.Repositories {     public class MqttServerRepository : SqlSugarRepository<MqttServer>, IMqttServerRepository { public MqttServerRepository(ISqlSugarClient db) : base(db) { } } }
