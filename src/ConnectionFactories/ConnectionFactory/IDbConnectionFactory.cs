using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Repository.ConnectionFactory
{
    public interface IDbConnectionFactory
    {
        Task<IDbConnection> CreateDbConnectionAsync();
    }
}
