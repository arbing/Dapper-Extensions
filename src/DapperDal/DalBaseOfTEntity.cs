using System;
using System.Configuration;
using System.Data;

using System.Data.SqlClient;
//using System.Data.OracleClient;
using Oracle.ManagedDataAccess.Client;

using DapperDal.Mapper;
using DapperDal.Sql;
using DapperDal.Providers;

namespace DapperDal
{
    /// <summary>
    /// 实体数据访问层基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public class DalBase<TEntity> : DalBase<TEntity, int> where TEntity : class
    {
        /// <summary>
        /// 默认初始化 DAL 新实例
        /// </summary>
        public DalBase() : this("Default")
        {
        }

        /// <summary>
        /// 用配置节点名初始化 DAL 新实例
        /// </summary>
        /// <param name="connectionName">DB连接字符串配置节点名</param>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="ConfigurationErrorsException">找不到配置节点</exception>
        public DalBase(string connectionName) : base(connectionName)
        {
        }
    }

    /// <summary>
    /// 实体数据访问层基类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    /// <typeparam name="TPrimaryKey">实体ID（主键）类型</typeparam>
    public partial class DalBase<TEntity, TPrimaryKey> where TEntity : class
    {
        static DalBase()
        {
            SetDefaultConfiguration();
        }

        /// <summary>
        /// 默认初始化 DAL 新实例
        /// </summary>
        public DalBase() : this("Default")
        {

        }

        /// <summary>
        /// 用配置节点名初始化 DAL 新实例
        /// </summary>
        /// <param name="connNameOrConnStr">DB连接字符串配置节点名</param>
        /// <exception cref="ArgumentNullException">参数为空</exception>
        /// <exception cref="ConfigurationErrorsException">找不到配置节点</exception>
        public DalBase(string connNameOrConnStr)
        {
            Configuration = DalConfiguration.Default;
            
            // 初始化配置项
            SetDefaultOptions();

            //ConnectionString = ResolveConnectionString(connNameOrConnStr);

            ConnectionStringSettings connectionStringSettings = ResolveConnectionStringFromConfig("DapperDalConnStr");

            DbType = connectionStringSettings.ProviderName.IndexOf("Oracle") > 0 ?"Oracle":"";
            ConnectionString = connectionStringSettings.ConnectionString;
        }

        /// <summary>
        /// 配置项
        /// </summary>
        public IDalConfiguration Configuration { get; private set; }

        /// <summary>
        /// 配置项
        /// </summary>
        public DalOptions Options { get; private set; }

        /// <summary>
        /// DB连接字符串
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// 数据库类型 Oracle, SqlServer
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// 打开DB连接
        /// </summary>
        /// <returns>DB连接</returns>
        protected virtual IDbConnection OpenConnection()
        {
            return OpenConnection(DbType, ConnectionString);
            //return OpenConnection(ConnectionString);
        }

        /// <summary>
        /// 创建数据库提供程序驱动
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static IDbProvider GetProvider(string type)
        {
            switch (type)
            {
                case "Oracle":
                    return new OracleProvider();

                case "SqlServer":
                default:
                    return new SqlServerProvider();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="connNameOrConnStr"></param>
        /// <returns></returns>
        protected virtual IDbConnection OpenConnection(string dbType,string connNameOrConnStr)
        {
            //var connectionString = ResolveConnectionString(connNameOrConnStr);
            //if (string.IsNullOrEmpty(connectionString))
            //{
            //    throw new ArgumentNullException("connectionString");
            //}

            //------------------------这里应该区分下数据库类型-------------------
            IDbProvider dbProvider = GetProvider(dbType);
            var connection = dbProvider.CreateConnection();

            try
            {
                connection.ConnectionString = connNameOrConnStr;
            }
            catch(Exception)
            {
                throw new ConfigurationErrorsException(string.Format("Failed to create a connection using the connection string '{0}'", connNameOrConnStr));
            }
           
            connection.Open();

            return connection;
        }

        /// <summary>
        /// 打开DB连接
        /// </summary>
        /// <param name="connNameOrConnStr">DB 连接字符串配置节点名</param>
        /// <returns>DB连接</returns>
        protected virtual IDbConnection OpenConnection(string connNameOrConnStr)
        {
            var connectionString = ResolveConnectionString(connNameOrConnStr);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            //------------------------这里应该区分下数据库类型-------------------
            //var connection = new SqlConnection(connectionString);
            //var connection = new OracleConnection(connectionString);
            var connection = new OracleConnection(connectionString);
            if (connection == null)
                throw new ConfigurationErrorsException(
                    string.Format("Failed to create a connection using the connection string '{0}'.", connectionString));

            connection.Open();

            return connection;
        }

        /// <summary>
        /// 初始化配置项
        /// </summary>
        private static void SetDefaultConfiguration()
        {
            if (DalConfiguration.Default != null)
            {
                DalConfiguration.Default.DefaultMapper = typeof(AutoEntityMapper<>);
                DalConfiguration.Default.Nolock = true;
                DalConfiguration.Default.Buffered = true;//默认是true,也就是启用缓存,因为查询数据时不更新，所以禁用

                //------------------自定义 jiftle -----------------------
                //DalConfiguration.Default.Buffered = false;
                DalConfiguration.Default.Dialect = new OracleDialect();
            }
        }

        /// <summary>
        /// 初始化配置项
        /// </summary>
        private void SetDefaultOptions()
        {
            if (Options == null)
            {
                Options = new DalOptions();
            }

            Options.SoftDeletePropsFactory = () => new { IsActive = 0, UpdateTime = DateTime.Now };
            Options.SoftActivePropsFactory = () => new { IsActive = 1, UpdateTime = DateTime.Now };
        }

        /// <summary>
        /// 获取 DB 连接串
        /// </summary>
        /// <param name="connNameOrConnStr">DB 连接字符串配置节点名</param>
        /// <returns>DB 连接串</returns>
        private string ResolveConnectionString(string connNameOrConnStr)
        {
            if (string.IsNullOrEmpty(connNameOrConnStr))
            {
                throw new ArgumentNullException("connNameOrConnStr");
            }

            if (connNameOrConnStr.Contains("=") || connNameOrConnStr.Contains(";"))
            {
                return connNameOrConnStr;
            }
            else
            {
                var conStr = ConfigurationManager.ConnectionStrings[connNameOrConnStr];
                if (conStr == null)
                {
                    throw new ConfigurationErrorsException(
                        string.Format("Failed to find connection string named '{0}' in app/web.config.", connNameOrConnStr));
                }

                return conStr.ConnectionString;
            }
        }

        /// <summary>
        /// 从配置文件中获取数据库连接字符串
        /// </summary>
        /// <returns></returns>
        private ConnectionStringSettings ResolveConnectionStringFromConfig(string ConnName)
        {
            var conStr = ConfigurationManager.ConnectionStrings[ConnName];
            if (conStr == null)
            {
                throw new ConfigurationErrorsException(
                    string.Format("Failed to find connection string named '{0}' in app/web.config.", ConnName));
            }

            return conStr;
        }

    }
}