using SqlSugar;
using ScadaServer.Domain.Entities;
using System.Reflection;
using Microsoft.AspNetCore.Identity;

namespace ScadaServer.Infrastructure.Persistence
{
    public class DatabaseInitializer
    {
        private readonly ISqlSugarClient _db;
        private readonly string _currentVersion = "1.0.0"; // 当前数据库版本

        public DatabaseInitializer(ISqlSugarClient db)
        {
            _db = db;
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public void Initialize()
        {
            // 1. 创建数据库表
            CreateTables();

            // 2. 初始化默认数据
            SeedData();

            // 3. 记录版本号
            SaveDbVersion();
        }

        /// <summary>
        /// 自动扫描所有实体并创建表
        /// </summary>
        private void CreateTables()
        {
            var entityTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t =>
                    t.IsClass &&
                    !t.IsAbstract &&
                    typeof(EntityBase).IsAssignableFrom(t))
                .ToArray();

            _db.CodeFirst.InitTables(entityTypes);
        }

        /// <summary>
        /// 初始化默认数据
        /// </summary>
        private void SeedData()
        {
            // 示例：初始化默认区域
            if (!_db.Queryable<Area>().Any())
            {
                _db.Insertable(new Area
                {
                    Name = "默认区域"

                }).ExecuteCommand();
            }

            // 示例：初始化默认管理员
            // 4. 初始化种子数据：管理员用户
            if (!_db.Queryable<SystemUser>().Any())
            {
                var passwordHasher = new PasswordHasher<SystemUser>();
                var hashedPassword = passwordHasher.HashPassword(new SystemUser(), "123456");

                _db.Insertable(new SystemUser
                {
                    Username = "admin",
                    PasswordHash = hashedPassword, // 存储哈希值
                    Role = "Admin",
                    Status = "Active"
                }).ExecuteCommand();
                Console.WriteLine(">>>> Default admin user (admin/123456) created with hashed password.");
            }
        }

        /// <summary>
        /// 保存当前数据库版本
        /// </summary>
        private void SaveDbVersion()
        {
            var exists = _db.Queryable<DbVersion>()
                .Any(v => v.Version == _currentVersion);

            if (!exists)
            {
                _db.Insertable(new DbVersion
                {
                    Version = _currentVersion,
                    AppliedAt = DateTime.Now
                }).ExecuteCommand();
            }
        }
    }
}