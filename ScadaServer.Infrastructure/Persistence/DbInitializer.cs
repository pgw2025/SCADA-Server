using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SqlSugar;
using ScadaServer.Domain.Entities;
using System.Reflection;
using Microsoft.AspNetCore.Identity; // Added for PasswordHasher

namespace ScadaServer.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static void InitDatabase(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();

            try
            {
                // 1. 尝试创建数据库（如果不存在）
                db.DbMaintenance.CreateDatabase();

                // 2. 获取 Domain 程序集中所有标记了 [SugarTable] 的类
                var entityTypes = typeof(Device).Assembly
                    .GetTypes()
                    .Where(t => t.GetCustomAttribute<SugarTable>() != null)
                    .ToArray();

                // 3. 执行 CodeFirst 同步结构
                // InitTables 会根据实体类自动创建表、增加缺少的字段
                db.CodeFirst.InitTables(entityTypes);

                // 4. 初始化种子数据：管理员用户
                if (!db.Queryable<SystemUser>().Any())
                {
                    var passwordHasher = new PasswordHasher<SystemUser>();
                    var hashedPassword = passwordHasher.HashPassword(new SystemUser(), "123456");

                    db.Insertable(new SystemUser
                    {
                        Username = "admin",
                        PasswordHash = hashedPassword, // 存储哈希值
                        Role = "Admin",
                        Status = "Active"
                    }).ExecuteCommand();
                    Console.WriteLine(">>>> Default admin user (admin/123456) created with hashed password.");
                }

                Console.WriteLine(">>>> Database & Tables synchronized successfully via CodeFirst.");
            }
            catch (Exception ex)
            {

                Console.WriteLine($">>>> Database initialization failed: {ex.Message}");
                throw; // 重新抛出异常以便调用者处理
            }
        }
    }
}
