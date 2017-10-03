using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DapperDal
{
    public partial class DalBase<TEntity, TPrimaryKey> where TEntity : class
    {
        /// <summary>
        /// 插入指定实体
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>实体主键</returns>
        public virtual TPrimaryKey Insert(TEntity entity)
        {
            using (var connection = OpenConnection())
            {
                return Configuration.DalImplementor.Insert(
                    connection: connection,
                    entity: entity,
                    transaction: null,
                    commandTimeout: null);
            }
        }

        /// <summary>
        /// 批量插入指定实体集合
        /// </summary>
        /// <param name="entities">实体集合</param>
        public virtual void Insert(IEnumerable<TEntity> entities)
        {
            using (var connection = OpenConnection())
            {
                Configuration.DalImplementor.Insert(
                    connection: connection,
                    entities: entities,
                    transaction: null,
                    commandTimeout: null);
            }
        }
    }
}
