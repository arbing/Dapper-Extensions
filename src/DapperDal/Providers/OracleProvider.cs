using System;
using System.Data;
using System.Data.Common;

namespace DapperDal.Providers
{
    /// <summary>
    /// Oracle数据库提供程序
    /// </summary>
	public class OracleProvider : IDbProvider
	{
		private static readonly Lazy<DbProviderFactory> _dbProviderFactory = new Lazy<DbProviderFactory>(CreateDbProviderFactory, true);

		private static DbProviderFactory CreateDbProviderFactory()
		{
			return DbProviderFactories.GetFactory(ProviderName);
		}

        /// <summary>
        /// 创建数据库连接，返回数据库连接
        /// </summary>
        /// <returns></returns>
		public IDbConnection CreateConnection()
		{
			return _dbProviderFactory.Value.CreateConnection();
		}

        /// <summary>
        /// 数据库提供程序名字
        /// </summary>
		public static string ProviderName
		{ 
			get
			{
                //return "System.Data.OracleClient";//依赖于oracle官方驱动，需要另外安装oracle客户端
                //return "Oracle.DataAccess.Client";//Oracle数据库，官方非托管驱动，限制比较多
                return "Oracle.ManagedDataAccess.Client";//Oracle官方托管驱动,10g以下版本不支持，无任何依赖
            } 
		}
	}
}
