using System;
using System.Data;
using System.Data.Common;

namespace DapperDal.Providers
{
    /// <summary>
    /// 数据库驱动提供程序
    /// </summary>
	public interface IDbProvider
	{
        /// <summary>
        /// 创建数据库连接，返回数据库连接
        /// </summary>
        /// <returns></returns>
		IDbConnection CreateConnection();
	}
}
