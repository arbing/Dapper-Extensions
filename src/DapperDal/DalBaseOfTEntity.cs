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
    /// ʵ�����ݷ��ʲ����
    /// </summary>
    /// <typeparam name="TEntity">ʵ������</typeparam>
    public class DalBase<TEntity> : DalBase<TEntity, int> where TEntity : class
    {
        /// <summary>
        /// Ĭ�ϳ�ʼ�� DAL ��ʵ��
        /// </summary>
        public DalBase() : this("Default")
        {
        }

        /// <summary>
        /// �����ýڵ�����ʼ�� DAL ��ʵ��
        /// </summary>
        /// <param name="connectionName">DB�����ַ������ýڵ���</param>
        /// <exception cref="ArgumentNullException">����Ϊ��</exception>
        /// <exception cref="ConfigurationErrorsException">�Ҳ������ýڵ�</exception>
        public DalBase(string connectionName) : base(connectionName)
        {
        }
    }

    /// <summary>
    /// ʵ�����ݷ��ʲ����
    /// </summary>
    /// <typeparam name="TEntity">ʵ������</typeparam>
    /// <typeparam name="TPrimaryKey">ʵ��ID������������</typeparam>
    public partial class DalBase<TEntity, TPrimaryKey> where TEntity : class
    {
        static DalBase()
        {
            SetDefaultConfiguration();
        }

        /// <summary>
        /// Ĭ�ϳ�ʼ�� DAL ��ʵ��
        /// </summary>
        public DalBase() : this("Default")
        {

        }

        /// <summary>
        /// �����ýڵ�����ʼ�� DAL ��ʵ��
        /// </summary>
        /// <param name="connNameOrConnStr">DB�����ַ������ýڵ���</param>
        /// <exception cref="ArgumentNullException">����Ϊ��</exception>
        /// <exception cref="ConfigurationErrorsException">�Ҳ������ýڵ�</exception>
        public DalBase(string connNameOrConnStr)
        {
            Configuration = DalConfiguration.Default;
            
            // ��ʼ��������
            SetDefaultOptions();

            //ConnectionString = ResolveConnectionString(connNameOrConnStr);

            ConnectionStringSettings connectionStringSettings = ResolveConnectionStringFromConfig("DapperDalConnStr");

            DbType = connectionStringSettings.ProviderName.IndexOf("Oracle") > 0 ?"Oracle":"";
            ConnectionString = connectionStringSettings.ConnectionString;
        }

        /// <summary>
        /// ������
        /// </summary>
        public IDalConfiguration Configuration { get; private set; }

        /// <summary>
        /// ������
        /// </summary>
        public DalOptions Options { get; private set; }

        /// <summary>
        /// DB�����ַ���
        /// </summary>
        public string ConnectionString { get; private set; }

        /// <summary>
        /// ���ݿ����� Oracle, SqlServer
        /// </summary>
        public string DbType { get; set; }

        /// <summary>
        /// ��DB����
        /// </summary>
        /// <returns>DB����</returns>
        protected virtual IDbConnection OpenConnection()
        {
            return OpenConnection(DbType, ConnectionString);
            //return OpenConnection(ConnectionString);
        }

        /// <summary>
        /// �������ݿ��ṩ��������
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

            //------------------------����Ӧ�����������ݿ�����-------------------
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
        /// ��DB����
        /// </summary>
        /// <param name="connNameOrConnStr">DB �����ַ������ýڵ���</param>
        /// <returns>DB����</returns>
        protected virtual IDbConnection OpenConnection(string connNameOrConnStr)
        {
            var connectionString = ResolveConnectionString(connNameOrConnStr);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString");
            }

            //------------------------����Ӧ�����������ݿ�����-------------------
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
        /// ��ʼ��������
        /// </summary>
        private static void SetDefaultConfiguration()
        {
            if (DalConfiguration.Default != null)
            {
                DalConfiguration.Default.DefaultMapper = typeof(AutoEntityMapper<>);
                DalConfiguration.Default.Nolock = true;
                DalConfiguration.Default.Buffered = true;//Ĭ����true,Ҳ�������û���,��Ϊ��ѯ����ʱ�����£����Խ���

                //------------------�Զ��� jiftle -----------------------
                //DalConfiguration.Default.Buffered = false;
                DalConfiguration.Default.Dialect = new OracleDialect();
            }
        }

        /// <summary>
        /// ��ʼ��������
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
        /// ��ȡ DB ���Ӵ�
        /// </summary>
        /// <param name="connNameOrConnStr">DB �����ַ������ýڵ���</param>
        /// <returns>DB ���Ӵ�</returns>
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
        /// �������ļ��л�ȡ���ݿ������ַ���
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