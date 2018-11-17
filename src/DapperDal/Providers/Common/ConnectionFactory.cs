using System.Data;
using System.Data.Common;

namespace DapperDal.Providers.Common
{
    /// <summary>
    /// 数据库连接工厂
    /// </summary>
	internal class ConnectionFactory
	{
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
		public static IDbConnection CreateConnection(string providerName, string connectionString)
		{
			var factory = DbProviderFactories.GetFactory(providerName);

			var connection = factory.CreateConnection();
			connection.ConnectionString = connectionString;
			return connection;
		}
	}
}
