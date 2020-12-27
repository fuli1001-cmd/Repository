//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Repository.EFCore.Extensions
//{
//    public static class ServiceCollectionExtensions
//    {
//        public static void AddSqlDataAccessServices<T>(this IServiceCollection services,
//            string connectionString, string migrationsAssembly)
//            where T : DbContext
//        {
//            services.AddDbContext<T>(options =>
//            {
//                options.UseSqlServer(connectionString,
//                    sqlServerOptionsAction: sqlOptions =>
//                    {
//                        sqlOptions.MigrationsAssembly(migrationsAssembly);
//                        //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
//                        sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
//                    });
//            });
//        }

//        //public static void AddPSqlDataAccessServices<T>(this IServiceCollection services,
//        //    string connectionString, string migrationsAssembly)
//        //    where T : DbContext
//        //{
//        //    services.AddDbContext<T>(options =>
//        //    {
//        //        options.UseNpgsql(connectionString,
//        //            npgsqlOptionsAction: sqlOptions =>
//        //            {
//        //                sqlOptions.MigrationsAssembly(migrationsAssembly);
//        //                //Configuring Connection Resiliency: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency 
//        //                sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
//        //            });
//        //    });
//        //}
//    }
//}
