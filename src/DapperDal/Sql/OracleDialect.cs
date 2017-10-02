using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperDal.Sql
{
    /// <summary>
    /// Oracle方言类
    /// </summary>
    public class OracleDialect : SqlDialectBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public OracleDialect() { }

        /// <inheritdoc />
        public override string GetIdentitySql(string tableName)
        {
            throw new System.NotImplementedException("Oracle does not support get last inserted identity.");
        }

        /// <inheritdoc />
        public override bool SupportsMultipleStatements
        {
            get { return false; }
        }

        //from Simple.Data.Oracle implementation https://github.com/flq/Simple.Data.Oracle/blob/master/Simple.Data.Oracle/OraclePager.cs
        /// <inheritdoc />
        public override string GetPagingSql(string sql, int page, int resultsPerPage, IDictionary<string, object> parameters)
        {
            var toSkip = page * resultsPerPage;
            var topLimit = (page + 1) * resultsPerPage;

            var sb = new StringBuilder();
            sb.AppendLine("SELECT * FROM (");
            sb.AppendLine("SELECT \"_ss_dapper_1_\".*, ROWNUM RNUM FROM (");
            sb.Append(sql);
            sb.AppendLine(") \"_ss_dapper_1_\"");
            sb.AppendLine("WHERE ROWNUM <= :topLimit) \"_ss_dapper_2_\" ");
            sb.AppendLine("WHERE \"_ss_dapper_2_\".RNUM > :toSkip");

            parameters.Add(":topLimit", topLimit);
            parameters.Add(":toSkip", toSkip);

            return sb.ToString();
        }

        /// <inheritdoc />
        public override string GetSetSql(string sql, int firstResult, int maxResults, IDictionary<string, object> parameters)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT * FROM (");
            sb.AppendLine("SELECT \"_ss_dapper_1_\".*, ROWNUM RNUM FROM (");
            sb.Append(sql);
            sb.AppendLine(") \"_ss_dapper_1_\"");
            sb.AppendLine("WHERE ROWNUM <= :topLimit) \"_ss_dapper_2_\" ");
            sb.AppendLine("WHERE \"_ss_dapper_2_\".RNUM > :toSkip");

            parameters.Add(":topLimit", maxResults + firstResult);
            parameters.Add(":toSkip", firstResult);

            return sb.ToString();
        }

        /// <inheritdoc />
        public override string QuoteString(string value)
        {
            if (value != null && value[0] == '`')
            {
                return string.Format("{0}{1}{2}", OpenQuote, value.Substring(1, value.Length - 2), CloseQuote);
            }
            return value.ToUpper();
        }

        /// <inheritdoc />
        public override char ParameterPrefix
        {
            get { return ':'; }
        }

        /// <inheritdoc />
        public override char OpenQuote
        {
            get { return '"'; }
        }

        /// <inheritdoc />
        public override char CloseQuote
        {
            get { return '"'; }
        }

        /// <inheritdoc />
        public override string SelectLimit(string sql, int limit)
        {//rownum <= 1
            if (sql.Contains("ROWNUM <"))
            {
                return sql;
            }

            return sql.Insert(sql.Length,
                string.Format(" AND ROWNUM <= {0}", limit));
        }

        /// <inheritdoc />
        public override string SetNolock(string sql)
        {
                return sql;
        }
    }
}
