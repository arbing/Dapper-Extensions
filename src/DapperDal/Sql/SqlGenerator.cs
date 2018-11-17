using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DapperDal.Mapper;
using DapperDal.Predicate;
using DapperDal.Utils;

namespace DapperDal.Sql
{
    /// <summary>
    /// SQL生成器接口
    /// </summary>
    public interface ISqlGenerator
    {
        /// <summary>
        /// 数据访问配置类实例
        /// </summary>
        IDalConfiguration Configuration { get; }

        /// <summary>
        /// 生成获取语句
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">查询条件谓词</param>
        /// <param name="sort">排序条件</param>
        /// <param name="parameters">查询参数</param>
        /// <param name="limit">前几条</param>
        /// <returns>生成获取语句</returns>
        string Select(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters, int limit = 0);

        /// <summary>
        /// 生成分页获取语句
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">查询条件谓词</param>
        /// <param name="sort">排序条件</param>
        /// <param name="page">页索引</param>
        /// <param name="resultsPerPage">每页条数</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>分页获取语句</returns>
        string SelectPaged(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDictionary<string, object> parameters);

        /// <summary>
        /// 生成区间获取语句
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">查询条件谓词</param>
        /// <param name="sort">排序条件</param>
        /// <param name="firstResult">起始行数</param>
        /// <param name="maxResults">最大条数</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>区间获取语句</returns>
        string SelectSet(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDictionary<string, object> parameters);

        /// <summary>
        /// 生成条数获取语句
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">查询条件谓词</param>
        /// <param name="parameters">查询参数</param>
        /// <returns>条数获取语句</returns>
        string Count(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters);

        /// <summary>
        /// 生成插入语句
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <returns>条数获取语句</returns>
        string Insert(IClassMapper classMap);

        /// <summary>
        /// 生成更新语句
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">更新条件谓词</param>
        /// <param name="parameters">更新参数</param>
        /// <param name="props">更新属性列表</param>
        /// <returns>更新语句</returns>
        string Update(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, IList<string> props = null);

        /// <summary>
        /// 生成删除语句
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <param name="predicate">删除条件谓词</param>
        /// <param name="parameters">删除参数</param>
        /// <returns>删除语句</returns>
        string Delete(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters);

        /// <summary>
        /// 生成包含主键条件的获取语句
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <returns>包含主键条件的获取语句</returns>
        string IdentitySql(IClassMapper classMap);

        /// <summary>
        /// 生成表名
        /// </summary>
        /// <param name="map">实体类型映射</param>
        /// <returns></returns>
        string GetTableName(IClassMapper map);

        /// <summary>
        /// 生成字段名
        /// </summary>
        /// <param name="map">实体类型映射</param>
        /// <param name="property">属性映射</param>
        /// <param name="includeAlias">是否包含别名</param>
        /// <returns>字段名</returns>
        string GetColumnName(IClassMapper map, IPropertyMap property, bool includeAlias);

        /// <summary>
        /// 生成字段名
        /// </summary>
        /// <param name="map">实体类型映射</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="includeAlias">是否包含别名</param>
        /// <returns>字段名</returns>
        string GetColumnName(IClassMapper map, string propertyName, bool includeAlias);

        /// <summary>
        /// 是否支持多语句执行
        /// </summary>
        /// <returns></returns>
        bool SupportsMultipleStatements();
    }

    /// <summary>
    /// SQL生成器类
    /// </summary>
    public class SqlGeneratorImpl : ISqlGenerator
    {
        /// <summary>
        /// 初始化SQL生成器类
        /// </summary>
        /// <param name="configuration">数据访问配置类实例</param>
        public SqlGeneratorImpl(IDalConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <inheritdoc />
        public IDalConfiguration Configuration { get; private set; }

        /// <inheritdoc />
        public virtual string Select(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, IDictionary<string, object> parameters, int limit = 0)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder sql = new StringBuilder(string.Format("SELECT {0} FROM {1}",
                BuildSelectColumns(classMap),
                GetTableName(classMap)));
            if (predicate != null)
            {
                sql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters));
            }

            if (sort != null && sort.Any())
            {
                sql.Append(" ORDER BY ")
                    .Append(sort.Select(s => GetColumnName(classMap, s.PropertyName, false) + (s.Ascending ? " ASC" : " DESC")).AppendStrings());
            }

            var outSql = sql.ToString();
            if (limit > 0)
            {
                outSql = Configuration.Dialect.SelectLimit(outSql, limit);
            }

            if (Configuration.Nolock)
            {
                outSql = Configuration.Dialect.SetNolock(outSql);
            }

            if (Configuration.OutputSql != null)
            {
                Configuration.OutputSql(outSql);
            }

            return outSql;
        }

        /// <inheritdoc />
        public virtual string SelectPaged(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            if (sort == null || !sort.Any())
            {
                throw new ArgumentNullException("Sort", "Sort cannot be null or empty.");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder innerSql = new StringBuilder(string.Format("SELECT {0} FROM {1}",
                BuildSelectColumns(classMap),
                GetTableName(classMap)));
            if (predicate != null)
            {
                innerSql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters));
            }

            string orderBy = sort.Select(s => GetColumnName(classMap, s.PropertyName, false) + (s.Ascending ? " ASC" : " DESC")).AppendStrings();
            innerSql.Append(" ORDER BY " + orderBy);

            var sql = innerSql.ToString();

            if (Configuration.Nolock)
            {
                sql = Configuration.Dialect.SetNolock(sql);
            }

            string outSql = Configuration.Dialect.GetPagingSql(sql, page, resultsPerPage, parameters);

            if (Configuration.OutputSql != null)
            {
                Configuration.OutputSql(outSql);
            }

            return outSql;
        }

        /// <inheritdoc />
        public virtual string SelectSet(IClassMapper classMap, IPredicate predicate, IList<ISort> sort, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            if (sort == null || !sort.Any())
            {
                throw new ArgumentNullException("Sort", "Sort cannot be null or empty.");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder innerSql = new StringBuilder(string.Format("SELECT {0} FROM {1}",
                BuildSelectColumns(classMap),
                GetTableName(classMap)));
            if (predicate != null)
            {
                innerSql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters));
            }

            string orderBy = sort.Select(s => GetColumnName(classMap, s.PropertyName, false) + (s.Ascending ? " ASC" : " DESC")).AppendStrings();
            innerSql.Append(" ORDER BY " + orderBy);

            var sql = innerSql.ToString();

            if (Configuration.Nolock)
            {
                sql = Configuration.Dialect.SetNolock(sql);
            }

            string outSql = Configuration.Dialect.GetSetSql(sql, firstResult, maxResults, parameters);

            if (Configuration.OutputSql != null)
            {
                Configuration.OutputSql(outSql);
            }

            return outSql;
        }


        /// <inheritdoc />
        public virtual string Count(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder sql = new StringBuilder(string.Format("SELECT COUNT(*) AS {0}Total{1} FROM {2}",
                                Configuration.Dialect.OpenQuote,
                                Configuration.Dialect.CloseQuote,
                                GetTableName(classMap)));
            if (predicate != null)
            {
                sql.Append(" WHERE ")
                    .Append(predicate.GetSql(this, parameters));
            }

            var outSql = sql.ToString();
            if (Configuration.Nolock)
            {
                outSql = Configuration.Dialect.SetNolock(outSql);
            }

            if (Configuration.OutputSql != null)
            {
                Configuration.OutputSql(outSql);
            }

            return outSql;
        }

        /// <inheritdoc />
        public virtual string Insert(IClassMapper classMap)
        {
            var columns = classMap.Properties.Where(p => !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.TriggerIdentity));
            if (!columns.Any())
            {
                throw new ArgumentException("No columns were mapped.");
            }

            var columnNames = columns.Select(p => GetColumnName(classMap, p, false));
            var parameters = columns.Select(p => Configuration.Dialect.ParameterPrefix + p.Name);

            string sql = string.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                                       GetTableName(classMap),
                                       columnNames.AppendStrings(),
                                       parameters.AppendStrings());

            var triggerIdentityColumn = classMap.Properties.Where(p => p.KeyType == KeyType.TriggerIdentity).ToList();

            if (triggerIdentityColumn.Count > 0)
            {
                if (triggerIdentityColumn.Count > 1)
                    throw new ArgumentException("TriggerIdentity generator cannot be used with multi-column keys");

                sql += string.Format(" RETURNING {0} INTO {1}IdOutParam", triggerIdentityColumn.Select(p => GetColumnName(classMap, p, false)).First(), Configuration.Dialect.ParameterPrefix);
            }

            if (Configuration.OutputSql != null)
            {
                Configuration.OutputSql(sql);
            }

            return sql;
        }

        /// <inheritdoc />
        public virtual string Update(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters, IList<string> props = null)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("Predicate");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            var columns = classMap.Properties.Where(
                p => (props == null || props.Count == 0 || props.Contains(p.Name, StringComparer.OrdinalIgnoreCase)) &&
                     !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity || p.KeyType == KeyType.Assigned));
            //var columns = classMap.Properties.Where(
            // p => (props == null || props.Count == 0 || props.Contains(p.Name, StringComparer.OrdinalIgnoreCase)) &&
            //      !(p.Ignored || p.IsReadOnly || p.KeyType == KeyType.Identity));
            if (!columns.Any())
            {
                throw new ArgumentException("No columns were mapped.");
            }
            //DapperDal.Mapper.KeyType
            var setSql =
                columns.Select(
                    p =>
                    string.Format(
                        "{0} = {1}{2}", GetColumnName(classMap, p, false), Configuration.Dialect.ParameterPrefix, p.Name));
            string strSetSql = setSql.AppendStrings();

            var sql = string.Format("UPDATE {0} SET {1} WHERE {2}",
                GetTableName(classMap),
               strSetSql,
                predicate.GetSql(this, parameters));

            if (Configuration.OutputSql != null)
            {
                Configuration.OutputSql(sql);
            }

            return sql;
        }

        /// <inheritdoc />
        public virtual string Delete(IClassMapper classMap, IPredicate predicate, IDictionary<string, object> parameters)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException("Predicate");
            }

            if (parameters == null)
            {
                throw new ArgumentNullException("Parameters");
            }

            StringBuilder sql = new StringBuilder(string.Format("DELETE FROM {0}", GetTableName(classMap)));
            sql.Append(" WHERE ").Append(predicate.GetSql(this, parameters));

            var outSql = sql.ToString();

            if (Configuration.OutputSql != null)
            {
                Configuration.OutputSql(outSql);
            }

            return outSql;
        }

        /// <inheritdoc />
        public virtual string IdentitySql(IClassMapper classMap)
        {
            return Configuration.Dialect.GetIdentitySql(GetTableName(classMap));
        }

        /// <inheritdoc />
        public virtual string GetTableName(IClassMapper map)
        {
            return Configuration.Dialect.GetTableName(map.SchemaName, map.TableName, null);
        }

        /// <inheritdoc />
        public virtual string GetColumnName(IClassMapper map, IPropertyMap property, bool includeAlias)
        {
            string alias = null;
            if (property.ColumnName != property.Name && includeAlias)
            {
                alias = property.Name;
            }

            return Configuration.Dialect.GetColumnName(GetTableName(map), property.ColumnName, alias);
        }

        /// <inheritdoc />
        public virtual string GetColumnName(IClassMapper map, string propertyName, bool includeAlias)
        {
            IPropertyMap propertyMap = map.Properties.SingleOrDefault(p => p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
            if (propertyMap == null)
            {
                throw new ArgumentException(string.Format("Could not find '{0}' in Mapping.", propertyName));
            }

            return GetColumnName(map, propertyMap, includeAlias);
        }

        /// <inheritdoc />
        public virtual bool SupportsMultipleStatements()
        {
            return Configuration.Dialect.SupportsMultipleStatements;
        }

        /// <summary>
        /// 生成所有字段组合语句
        /// </summary>
        /// <param name="classMap">实体类型映射</param>
        /// <returns>字段组合语句</returns>
        public virtual string BuildSelectColumns(IClassMapper classMap)
        {
            var columns = classMap.Properties
                .Where(p => !p.Ignored)
                .Select(p => GetColumnName(classMap, p, true));
            return columns.AppendStrings();
        }
    }
}