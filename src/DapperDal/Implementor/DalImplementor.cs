using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dapper;
using DapperDal.Mapper;
using DapperDal.Predicate;
using DapperDal.Sql;
using DapperDal.Utils;
using NLog;

namespace DapperDal.Implementor
{
    /// <summary>
    /// 数据访问器实现接口
    /// </summary>
    public interface IDalImplementor
    {
        /// <summary>
        /// SQL生成器
        /// </summary>
        ISqlGenerator SqlGenerator { get; }

        /// <summary>
        /// 根据实体ID（主键）获取实体
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="id">实体ID（主键）</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>返回实体</returns>
        T Get<T>(IDbConnection connection, dynamic id, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 批量插入指定实体集合
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="entities">实体集合</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        void Insert<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 插入指定实体
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="entity">实体</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>返回插入实体ID（主键）</returns>
        dynamic Insert<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 更新指定实体
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="entity">实体</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>返回更新是否成功的结果</returns>
        bool Update<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 更新指定实体指定属性
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="entity">实体</param>
        /// <param name="props">要更新的属性名列表</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>返回更新是否成功的结果</returns>
        bool Update<T>(IDbConnection connection, T entity, IList<string> props, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 更新指定实体指定属性
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="entity">实体</param>
        /// <param name="props">要更新的属性名列表，以匿名对象提供</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>返回更新是否成功的结果</returns>
        bool Update<T>(IDbConnection connection, T entity, object props, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 根据指定指定主键ID更新实体指定属性
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="keyAndProps">更新实体，包含主键ID、更新属性及值</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>返回更新是否成功的结果</returns>
        bool Update<T>(IDbConnection connection, object keyAndProps, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 根据指定更新条件更新实体指定属性
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="props">要更新的属性名及值，以匿名对象提供</param>
        /// <param name="predicate">更新条件，使用谓词或匿名对象</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>返回更新是否成功的结果</returns>
        bool Update<T>(IDbConnection connection, object props, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 删除指定实体
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="entity">实体</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>返回删除是否成功的结果</returns>
        bool Delete<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="predicate">删除条件</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>返回删除是否成功的结果</returns>
        bool Delete<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 根据查询条件和排序条件获取实体集合
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <param name="buffered">实体集合返回前是否要缓冲（ToList）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体集合</returns>
        IEnumerable<T> GetList<T>(IDbConnection connection, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;

        /// <summary>
        /// 根据查询条件和排序条件获取实体集合的前N条
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="limit">前几条</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <param name="buffered">实体集合返回前是否要缓冲（ToList）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体集合</returns>
        IEnumerable<T> GetTop<T>(IDbConnection connection, int limit, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;

        /// <summary>
        /// 根据查询条件和排序条件获取实体分页集合
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="page">页索引，从0起始</param>
        /// <param name="resultsPerPage">每页条数</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <param name="buffered">实体集合返回前是否要缓冲（ToList）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体集合</returns>
        IEnumerable<T> GetPage<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;

        /// <summary>
        /// 根据查询条件和排序条件获取实体区间集合
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <param name="buffered">实体集合返回前是否要缓冲（ToList）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体集合</returns>
        IEnumerable<T> GetSet<T>(IDbConnection connection, object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class;

        /// <summary>
        /// 根据条件获取实体条数
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体条数</returns>
        int Count<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class;

        /// <summary>
        /// 根据多个条件组获取多个指定实体集合
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <returns>多实体集合读取器</returns>
        IMultipleResultReader GetMultiple(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout);

        /// <summary>
        /// 根据实体主键ID获取谓词组
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="id">实体主键ID</param>
        /// <returns>谓词组</returns>
        IPredicate GetIdPredicate(IClassMapper classMap, object id);

        /// <summary>
        /// 获取实体主键ID条件谓词
        /// </summary>
        /// <param name="id">实体主键ID</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>谓词</returns>
        IPredicate GetIdPredicate<T>(object id) where T : class;
    }

    /// <summary>
    /// 数据访问器默认实现类
    /// </summary>
    public class DalImplementor : IDalImplementor
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 初始化数据访问器
        /// </summary>
        /// <param name="sqlGenerator">SQL生成器</param>
        public DalImplementor(ISqlGenerator sqlGenerator)
        {
            SqlGenerator = sqlGenerator;
        }

        /// <inheritdoc />
        public ISqlGenerator SqlGenerator { get; private set; }

        /// <inheritdoc />
        public void Insert<T>(IDbConnection connection, IEnumerable<T> entities, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            PropertyInfo[] properties = null;
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            var notKeyProperties = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey).ToArray();
            var triggerIdentityColumn = classMap.Properties.SingleOrDefault(p => p.KeyType == KeyType.TriggerIdentity);

            var parameters = new List<DynamicParameters>();
            if (triggerIdentityColumn != null)
            {
                properties = typeof(T).GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.Name != triggerIdentityColumn.PropertyInfo.Name).ToArray();
            }

            foreach (var e in entities)
            {
                foreach (var column in notKeyProperties)
                {
                    if (column.KeyType == KeyType.Guid && (Guid)column.PropertyInfo.GetValue(e, null) == Guid.Empty)
                    {
                        Guid comb = SqlGenerator.Configuration.GetNextGuid();
                        column.PropertyInfo.SetValue(e, comb, null);
                    }
                }

                if (triggerIdentityColumn != null)
                {
                    var dynamicParameters = new DynamicParameters();
                    foreach (var prop in properties)
                    {
                        dynamicParameters.Add(prop.Name, prop.GetValue(e, null));
                    }

                    // defaultValue need for identify type of parameter
                    var defaultValue = typeof(T).GetProperty(triggerIdentityColumn.PropertyInfo.Name).GetValue(e, null);
                    dynamicParameters.Add("IdOutParam", direction: ParameterDirection.Output, value: defaultValue);

                    parameters.Add(dynamicParameters);
                }
            }

            string sql = SqlGenerator.Insert(classMap);

            if (triggerIdentityColumn == null)
            {
                logger.Debug("DapperDal SQL: " + sql);
                connection.Execute(sql, entities, transaction, commandTimeout, CommandType.Text);
            }
            else
            {
                logger.Debug("DapperDal SQL: " + sql);
                connection.Execute(sql, parameters, transaction, commandTimeout, CommandType.Text);
            }
        }

        /// <inheritdoc />
        public dynamic Insert<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            List<IPropertyMap> nonIdentityKeyProperties = classMap.Properties.Where(p => p.KeyType == KeyType.Guid || p.KeyType == KeyType.Assigned).ToList();
            var identityColumn = classMap.Properties.SingleOrDefault(p => p.KeyType == KeyType.Identity);
            var triggerIdentityColumn = classMap.Properties.SingleOrDefault(p => p.KeyType == KeyType.TriggerIdentity);
            foreach (var column in nonIdentityKeyProperties)
            {
                if (column.KeyType == KeyType.Guid && (Guid)column.PropertyInfo.GetValue(entity, null) == Guid.Empty)
                {
                    Guid comb = SqlGenerator.Configuration.GetNextGuid();
                    column.PropertyInfo.SetValue(entity, comb, null);
                }
            }

            IDictionary<string, object> keyValues = new ExpandoObject();
            string sql = SqlGenerator.Insert(classMap);
            if (identityColumn != null)
            {
                IEnumerable<long> result;
                if (SqlGenerator.SupportsMultipleStatements())
                {
                    sql += SqlGenerator.Configuration.Dialect.BatchSeperator + SqlGenerator.IdentitySql(classMap);

                    logger.Debug("DapperDal SQL: " + sql);
                    result = connection.Query<long>(sql, entity, transaction, false, commandTimeout, CommandType.Text);
                }
                else
                {
                    logger.Debug("DapperDal SQL: " + sql);
                    connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text);
                    sql = SqlGenerator.IdentitySql(classMap);
                    result = connection.Query<long>(sql, entity, transaction, false, commandTimeout, CommandType.Text);
                }

                long identityValue = result.First();
                int identityInt = Convert.ToInt32(identityValue);
                keyValues.Add(identityColumn.Name, identityInt);
                identityColumn.PropertyInfo.SetValue(entity, identityInt, null);
            }
            else if (triggerIdentityColumn != null)
            {
                var dynamicParameters = new DynamicParameters();
                foreach (var prop in entity.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public)
                    .Where(p => p.Name != triggerIdentityColumn.PropertyInfo.Name))
                {
                    dynamicParameters.Add(prop.Name, prop.GetValue(entity, null));
                }

                // defaultValue need for identify type of parameter
                var defaultValue = entity.GetType().GetProperty(triggerIdentityColumn.PropertyInfo.Name).GetValue(entity, null);
                dynamicParameters.Add("IdOutParam", direction: ParameterDirection.Output, value: defaultValue);

                logger.Debug("DapperDal SQL: " + sql);
                connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);

                var value = dynamicParameters.Get<object>(SqlGenerator.Configuration.Dialect.ParameterPrefix + "IdOutParam");
                keyValues.Add(triggerIdentityColumn.Name, value);
                triggerIdentityColumn.PropertyInfo.SetValue(entity, value, null);
            }
            else
            {
                logger.Debug("DapperDal SQL: " + sql);
                connection.Execute(sql, entity, transaction, commandTimeout, CommandType.Text);
            }

            foreach (var column in nonIdentityKeyProperties)
            {
                keyValues.Add(column.Name, column.PropertyInfo.GetValue(entity, null));
            }

            if (keyValues.Count == 1)
            {
                return keyValues.First().Value;
            }

            return keyValues;
        }

        /// <inheritdoc />
        public bool Update<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetKeyPredicate<T>(classMap, entity);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Update(classMap, predicate, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();

            var columns = classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.Assigned));
            foreach (var property in ReflectionHelper.GetObjectValues(entity).Where(property => columns.Any(c => c.Name == property.Key)))
            {
                dynamicParameters.Add(property.Key, property.Value);
            }

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            logger.Debug("DapperDal SQL: " + sql);
            return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        /// <inheritdoc />
        public bool Update<T>(IDbConnection connection, T entity, IList<string> props, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetKeyPredicate<T>(classMap, entity);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Update(classMap, predicate, parameters, props);
            DynamicParameters dynamicParameters = new DynamicParameters();

            var propKeys = props;
            var columns = classMap.Properties.Where(
                p => (propKeys == null || propKeys.Count == 0 || propKeys.Contains(p.Name, StringComparer.OrdinalIgnoreCase)) &&
                     !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.Assigned));
            foreach (var property in ReflectionHelper.GetObjectValues(entity).Where(property => columns.Any(c => c.Name.Equals(property.Key, StringComparison.OrdinalIgnoreCase))))
            {
                dynamicParameters.Add(property.Key, property.Value);
            }

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            logger.Debug("DapperDal SQL: " + sql);
            return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        /// <inheritdoc />
        public bool Update<T>(IDbConnection connection, T entity, object props, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetKeyPredicate<T>(classMap, entity);

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            var propValues = ReflectionHelper.GetObjectValues(props);
            var propKeys = propValues.Keys.ToList();
            string sql = SqlGenerator.Update(classMap, predicate, parameters, propKeys);
            DynamicParameters dynamicParameters = new DynamicParameters();

            var columns = classMap.Properties.Where(
                p => (propKeys.Count == 0 || propKeys.Contains(p.Name, StringComparer.OrdinalIgnoreCase)) &&
                     !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.Assigned));
            foreach (var property in propValues.Where(property => columns.Any(c => c.Name.Equals(property.Key, StringComparison.OrdinalIgnoreCase))))
            {
                dynamicParameters.Add(property.Key, property.Value);
            }

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            logger.Debug("DapperDal SQL: " + sql);
            return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        /// <inheritdoc />
        public bool Update<T>(IDbConnection connection, object keyAndProps, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetKeyPredicate<T>(classMap, keyAndProps);

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            var propValues = ReflectionHelper.GetObjectValues(keyAndProps);
            var propKeys = propValues.Keys.ToList();
            string sql = SqlGenerator.Update(classMap, predicate, parameters, propKeys);
            var columns = classMap.Properties.Where(
                p => (propKeys.Count == 0 || propKeys.Contains(p.Name, StringComparer.OrdinalIgnoreCase)) &&
                     !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.Assigned));

            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var property in propValues.Where(property => columns.Any(c => c.Name.Equals(property.Key, StringComparison.OrdinalIgnoreCase))))
            {
                dynamicParameters.Add(property.Key, property.Value);
            }

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            logger.Debug("DapperDal SQL: " + sql);
            return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        /// <inheritdoc />
        public bool Update<T>(IDbConnection connection, object props, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            var propValues = ReflectionHelper.GetObjectValues(props);
            var propKeys = propValues.Keys.ToList();
            string sql = SqlGenerator.Update(classMap, wherePredicate, parameters, propKeys);

            var columns = classMap.Properties.Where(
                p => (propKeys.Count == 0 || propKeys.Contains(p.Name, StringComparer.OrdinalIgnoreCase)) &&
                     !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.Assigned));
           // var columns = classMap.Properties.Where(
           //p => (propKeys.Count == 0 || propKeys.Contains(p.Name, StringComparer.OrdinalIgnoreCase)) &&
           //     !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity));
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var property in propValues.Where(property => columns.Any(c => c.Name.Equals(property.Key, StringComparison.OrdinalIgnoreCase))))
            {
                dynamicParameters.Add(property.Key, property.Value);
            }

            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            logger.Debug("DapperDal SQL: " + sql);
            return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        /// <inheritdoc />
        public bool Delete<T>(IDbConnection connection, T entity, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetKeyPredicate<T>(classMap, entity);
            return Delete<T>(connection, classMap, predicate, transaction, commandTimeout);
        }

        /// <inheritdoc />
        public bool Delete<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return Delete<T>(connection, classMap, wherePredicate, transaction, commandTimeout);
        }

        /// <inheritdoc />
        public T Get<T>(IDbConnection connection, dynamic id, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetIdPredicate(classMap, id);
            T result = GetList<T>(connection, classMap, predicate, null, transaction, commandTimeout, true).SingleOrDefault();
            return result;
        }

        /// <inheritdoc />
        public IEnumerable<T> GetList<T>(IDbConnection connection, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return GetList<T>(connection, classMap, wherePredicate, sort, transaction, commandTimeout, buffered);
        }

        /// <inheritdoc />
        public IEnumerable<T> GetTop<T>(IDbConnection connection, int limit, object predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return GetList<T>(connection, classMap, wherePredicate, sort, limit, transaction, commandTimeout, buffered);
        }

        /// <inheritdoc />
        public IEnumerable<T> GetPage<T>(IDbConnection connection, object predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return GetPage<T>(connection, classMap, wherePredicate, sort, page, resultsPerPage, transaction, commandTimeout, buffered);
        }

        /// <inheritdoc />
        public IEnumerable<T> GetSet<T>(IDbConnection connection, object predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            return GetSet<T>(connection, classMap, wherePredicate, sort, firstResult, maxResults, transaction, commandTimeout, buffered);
        }

        /// <inheritdoc />
        public int Count<T>(IDbConnection connection, object predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate wherePredicate = GetPredicate(classMap, predicate);
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Count(classMap, wherePredicate, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            logger.Debug("DapperDal SQL: " + sql);
            return (int)connection.Query(sql, dynamicParameters, transaction, false, commandTimeout, CommandType.Text).Single().Total;
        }

        /// <inheritdoc />
        public IMultipleResultReader GetMultiple(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            if (SqlGenerator.SupportsMultipleStatements())
            {
                return GetMultipleByBatch(connection, predicate, transaction, commandTimeout);
            }

            return GetMultipleBySequence(connection, predicate, transaction, commandTimeout);
        }

        /// <inheritdoc />
        protected IEnumerable<T> GetList<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            return GetList<T>(connection, classMap, predicate, sort, 0, transaction, commandTimeout, buffered);
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体集合
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="limit">前几条</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <param name="buffered">实体集合返回前是否要缓冲（ToList）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体集合</returns>
        protected IEnumerable<T> GetList<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int limit, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Select(classMap, predicate, sort, parameters, limit: limit);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            logger.Debug("DapperDal SQL: " + sql);

            //sql = "Select * from (SELECT ABS_TERMINFOUPLOAD.TERMNO, ABS_TERMINFOUPLOAD.UPLOADTIME, ABS_TERMINFOUPLOAD.OPDT, ABS_TERMINFOUPLOAD.STATUS, ABS_TERMINFOUPLOAD.VERINFO, ABS_TERMINFOUPLOAD.DESCRIPTION FROM ABS_TERMINFOUPLOAD WHERE (ABS_TERMINFOUPLOAD.TERMNO = '10000002')   ORDER BY ABS_TERMINFOUPLOAD.OPDT DESC) where ROWNUM <= 1 ";
            return connection.Query<T>(sql, dynamicParameters, transaction, buffered, commandTimeout, CommandType.Text);

            //IEnumerable<T> list = connection.Query<T>(sql, dynamicParameters, transaction, false, commandTimeout, CommandType.Text);
            //return list;
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体分页集合
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="page">页索引，从0起始</param>
        /// <param name="resultsPerPage">每页条数</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <param name="buffered">实体集合返回前是否要缓冲（ToList）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体集合</returns>
        protected IEnumerable<T> GetPage<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SelectPaged(classMap, predicate, sort, page, resultsPerPage, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            logger.Debug("DapperDal SQL: " + sql);
            return connection.Query<T>(sql, dynamicParameters, transaction, buffered, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据查询条件和排序条件获取实体区间集合
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">查询条件</param>
        /// <param name="sort">排序条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <param name="buffered">实体集合返回前是否要缓冲（ToList）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>实体集合</returns>
        protected IEnumerable<T> GetSet<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDbTransaction transaction, int? commandTimeout, bool buffered) where T : class
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.SelectSet(classMap, predicate, sort, firstResult, maxResults, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            logger.Debug("DapperDal SQL: " + sql);
            return connection.Query<T>(sql, dynamicParameters, transaction, buffered, commandTimeout, CommandType.Text);
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">删除条件</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>返回删除是否成功的结果</returns>
        protected bool Delete<T>(IDbConnection connection, IClassMapper classMap, IPredicate predicate, IDbTransaction transaction, int? commandTimeout) where T : class
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            string sql = SqlGenerator.Delete(classMap, predicate, parameters);
            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            logger.Debug("DapperDal SQL: " + sql);
            return connection.Execute(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text) > 0;
        }

        /// <summary>
        /// 根据实体对象获取谓词组
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">谓词表达式组，或实体对象</param>
        /// <returns>谓词组</returns>
        protected IPredicate GetPredicate(IClassMapper classMap, object predicate)
        {
            IPredicate wherePredicate = predicate as IPredicate;
            if (wherePredicate == null && predicate != null)
            {
                wherePredicate = GetEntityPredicate(classMap, predicate);
            }

            return wherePredicate;
        }

        /// <inheritdoc />
        public IPredicate GetIdPredicate(IClassMapper classMap, object id)
        {
            bool isSimpleType = ReflectionHelper.IsSimpleType(id.GetType());
            var keys = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey);
            IDictionary<string, object> paramValues = null;
            IList<IPredicate> predicates = new List<IPredicate>();
            if (!isSimpleType)
            {
                paramValues = ReflectionHelper.GetObjectValues(id);
            }

            foreach (var key in keys)
            {
                object value = id;
                if (!isSimpleType)
                {
                    value = paramValues[key.Name];
                }

                Type predicateType = typeof(FieldPredicate<>).MakeGenericType(classMap.EntityType);

                IFieldPredicate fieldPredicate = Activator.CreateInstance(predicateType) as IFieldPredicate;
                fieldPredicate.Not = false;
                fieldPredicate.Operator = Operator.Eq;
                fieldPredicate.PropertyName = key.Name;
                fieldPredicate.Value = value;
                predicates.Add(fieldPredicate);
            }

            return predicates.Count == 1
                       ? predicates[0]
                       : new PredicateGroup
                       {
                           Operator = GroupOperator.And,
                           Predicates = predicates
                       };
        }

        /// <inheritdoc />
        public IPredicate GetIdPredicate<T>(object id) where T : class
        {
            IClassMapper classMap = SqlGenerator.Configuration.GetMap<T>();
            IPredicate predicate = GetIdPredicate(classMap, id);
            return predicate;
        }

        /// <summary>
        /// 根据实体获取谓词组
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="entity">实体</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>谓词组</returns>
        /// <exception cref="ArgumentException">实体无主键</exception>
        protected IPredicate GetKeyPredicate<T>(IClassMapper classMap, T entity) where T : class
        {
            var whereFields = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey).ToArray();
            if (!whereFields.Any())
            {
                throw new ArgumentException("At least one Key column must be defined.");
            }

            IList<IPredicate> predicates = (from field in whereFields
                                            select new FieldPredicate<T>
                                            {
                                                Not = false,
                                                Operator = Operator.Eq,
                                                PropertyName = field.Name,
                                                Value = field.PropertyInfo.GetValue(entity, null)
                                            }).Cast<IPredicate>().ToList();

            return predicates.Count == 1
                       ? predicates[0]
                       : new PredicateGroup
                       {
                           Operator = GroupOperator.And,
                           Predicates = predicates
                       };
        }

        /// <summary>
        /// 根据实体获取谓词组
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="entity">实体</param>
        /// <typeparam name="T">实体类型</typeparam>
        /// <returns>谓词组</returns>
        /// <exception cref="ArgumentException">实体无主键</exception>
        protected IPredicate GetKeyPredicate<T>(IClassMapper classMap, object entity) where T : class
        {
            var whereFields = classMap.Properties.Where(p => p.KeyType != KeyType.NotAKey).ToArray();
            if (!whereFields.Any())
            {
                throw new ArgumentException("At least one Key column must be defined.");
            }
            Type t = entity.GetType();

            IList<IPredicate> predicates = (from field in whereFields
                                            select new FieldPredicate<T>
                                            {
                                                Not = false,
                                                Operator = Operator.Eq,
                                                PropertyName = field.Name,
                                                Value = t.GetProperty(field.Name).GetValue(entity, null)
                                            }).Cast<IPredicate>().ToList();

            return predicates.Count == 1
                       ? predicates[0]
                       : new PredicateGroup
                       {
                           Operator = GroupOperator.And,
                           Predicates = predicates
                       };
        }

        /// <summary>
        /// 根据实体获取谓词组
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="entity">实体</param>
        /// <returns>谓词组</returns>
        protected IPredicate GetEntityPredicate(IClassMapper classMap, object entity)
        {
            Type predicateType = typeof(FieldPredicate<>).MakeGenericType(classMap.EntityType);
            IList<IPredicate> predicates = new List<IPredicate>();
            foreach (var kvp in ReflectionHelper.GetObjectValues(entity))
            {
                IFieldPredicate fieldPredicate = Activator.CreateInstance(predicateType) as IFieldPredicate;
                fieldPredicate.Not = false;
                fieldPredicate.Operator = Operator.Eq;
                fieldPredicate.PropertyName = kvp.Key;
                fieldPredicate.Value = kvp.Value;
                predicates.Add(fieldPredicate);
            }

            return predicates.Count == 1
                       ? predicates[0]
                       : new PredicateGroup
                       {
                           Operator = GroupOperator.And,
                           Predicates = predicates
                       };
        }

        /// <summary>
        /// 根据多个条件组批量获取多个指定实体集合
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="predicate">多条件谓词组</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <returns>多实体集合读取器</returns>
        protected GridReaderResultReader GetMultipleByBatch(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            StringBuilder sql = new StringBuilder();
            foreach (var item in predicate.Items)
            {
                IClassMapper classMap = SqlGenerator.Configuration.GetMap(item.Type);
                IPredicate itemPredicate = item.Value as IPredicate;
                if (itemPredicate == null && item.Value != null)
                {
                    itemPredicate = GetPredicate(classMap, item.Value);
                }

                sql.AppendLine(SqlGenerator.Select(classMap, itemPredicate, item.Sort, parameters) + SqlGenerator.Configuration.Dialect.BatchSeperator);
            }

            DynamicParameters dynamicParameters = new DynamicParameters();
            foreach (var parameter in parameters)
            {
                dynamicParameters.Add(parameter.Key, parameter.Value);
            }

            SqlMapper.GridReader grid = connection.QueryMultiple(sql.ToString(), dynamicParameters, transaction, commandTimeout, CommandType.Text);
            return new GridReaderResultReader(grid);
        }

        /// <summary>
        /// 根据多个条件组顺次获取多个指定实体集合
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="predicate">多条件谓词组</param>
        /// <param name="transaction">数据库事务</param>
        /// <param name="commandTimeout">数据库命令超时时间（单位秒）</param>
        /// <returns>多实体集合读取器</returns>
        protected SequenceReaderResultReader GetMultipleBySequence(IDbConnection connection, GetMultiplePredicate predicate, IDbTransaction transaction, int? commandTimeout)
        {
            IList<SqlMapper.GridReader> items = new List<SqlMapper.GridReader>();
            foreach (var item in predicate.Items)
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>();
                IClassMapper classMap = SqlGenerator.Configuration.GetMap(item.Type);
                IPredicate itemPredicate = item.Value as IPredicate;
                if (itemPredicate == null && item.Value != null)
                {
                    itemPredicate = GetPredicate(classMap, item.Value);
                }

                string sql = SqlGenerator.Select(classMap, itemPredicate, item.Sort, parameters);
                DynamicParameters dynamicParameters = new DynamicParameters();
                foreach (var parameter in parameters)
                {
                    dynamicParameters.Add(parameter.Key, parameter.Value);
                }

                SqlMapper.GridReader queryResult = connection.QueryMultiple(sql, dynamicParameters, transaction, commandTimeout, CommandType.Text);
                items.Add(queryResult);
            }

            return new SequenceReaderResultReader(items);
        }
    }
}
