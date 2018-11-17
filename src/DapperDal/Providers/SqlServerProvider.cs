using System;
using System.Data;
using System.Data.Common;
using System.Text;
//using MT.Data.Providers.Common;
//using MT.Data.Providers.Common.Builders;

namespace DapperDal.Providers
{
    /// <summary>
    /// SQL Server
    /// </summary>
	public class SqlServerProvider : IDbProvider
	{
		private static readonly Lazy<DbProviderFactory> _dbProviderFactory = new Lazy<DbProviderFactory>(CreateDbProviderFactory, true);

		private static DbProviderFactory CreateDbProviderFactory()
		{
			return DbProviderFactories.GetFactory(ProviderName);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public IDbConnection CreateConnection()
		{
			return _dbProviderFactory.Value.CreateConnection();
		}

        /// <summary>
        /// 
        /// </summary>
		public static string ProviderName
		{ 
			get
			{
				return "System.Data.SqlClient";
			} 
		}

	}
}
