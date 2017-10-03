using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using DapperDal.Utils;
using DapperDal.Attributes;

namespace DapperDal.Mapper
{
    /// <summary>
    /// 实体类型映射器接口
    /// </summary>
    public interface IClassMapper
    {
        /// <summary>
        /// 数据库架构名
        /// </summary>
        string SchemaName { get; }

        /// <summary>
        /// 数据库表名
        /// </summary>
        string TableName { get; }

        /// <summary>
        /// 实体属性信息列表
        /// </summary>
        IList<IPropertyMap> Properties { get; }

        /// <summary>
        /// 实体类型
        /// </summary>
        Type EntityType { get; }
    }

    /// <summary>
    /// 泛型实体映射器接口
    /// </summary>
    /// <typeparam name="T">实体类型</typeparam>
    public interface IClassMapper<T> : IClassMapper where T : class
    {
    }

    /// <summary>
    /// 默认泛型实体映射器类
    /// </summary>
    public class ClassMapper<T> : IClassMapper<T> where T : class
    {
        /// <inheritdoc />
        public string SchemaName { get; protected set; }

        /// <inheritdoc />
        public string TableName { get; protected set; }

        /// <inheritdoc />
        public IList<IPropertyMap> Properties { get; private set; }

        /// <inheritdoc />
        public Type EntityType
        {
            get { return typeof(T); }
        }

        /// <summary>
        /// 初始化默认泛型实体映射器
        /// </summary>
        public ClassMapper()
        {
            PropertyTypeKeyTypeMapping = new Dictionary<Type, KeyType>
                                             {
                                                 { typeof(byte), KeyType.Identity }, { typeof(byte?), KeyType.Identity },
                                                 { typeof(sbyte), KeyType.Identity }, { typeof(sbyte?), KeyType.Identity },
                                                 { typeof(short), KeyType.Identity }, { typeof(short?), KeyType.Identity },
                                                 { typeof(ushort), KeyType.Identity }, { typeof(ushort?), KeyType.Identity },
                                                 { typeof(int), KeyType.Identity }, { typeof(int?), KeyType.Identity },
                                                 { typeof(uint), KeyType.Identity}, { typeof(uint?), KeyType.Identity },
                                                 { typeof(long), KeyType.Identity }, { typeof(long?), KeyType.Identity },
                                                 { typeof(ulong), KeyType.Identity }, { typeof(ulong?), KeyType.Identity },
                                                 { typeof(BigInteger), KeyType.Identity }, { typeof(BigInteger?), KeyType.Identity },
                                                 { typeof(Guid), KeyType.Guid }, { typeof(Guid?), KeyType.Guid },
                                             };

            Properties = new List<IPropertyMap>();
            Table(typeof(T).Name);
        }

        /// <summary>
        /// 属性类型与键类型的对应关系字典
        /// </summary>
        protected Dictionary<Type, KeyType> PropertyTypeKeyTypeMapping { get; private set; }

        /// <summary>
        /// 设置数据库架构名
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        public virtual void Schema(string schemaName)
        {
            SchemaName = schemaName;
        }

        /// <summary>
        /// 设置数据库表名
        /// </summary>
        /// <param name="tableName">设置数据库表名</param>
        public virtual void Table(string tableName)
        {
            TableName = tableName;
        }

        /// <summary>
        /// 执行自动映射
        /// </summary>
        protected virtual void AutoMap()
        {
            AutoMap(null);
        }

        /// <summary>
        /// 执行自动映射
        /// </summary>
        /// <param name="canMap">指示实体属性是否要映射的方法提供</param>
        protected virtual void AutoMap(Func<Type, PropertyInfo, bool> canMap)
        {
            Type type = typeof(T);
            bool hasDefinedKey = Properties.Any(p => p.KeyType != KeyType.NotAKey);
            PropertyMap keyMap = null;
            foreach (var propertyInfo in type.GetProperties())
            {
                
                if (Properties.Any(p => p.Name.Equals(propertyInfo.Name, StringComparison.InvariantCultureIgnoreCase)))
                {
                    continue;
                }

                if ((canMap != null && !canMap(type, propertyInfo)))
                {
                    continue;
                }

                PropertyMap map = Map(propertyInfo);
                if (!hasDefinedKey)
                {
                    //--------------------jiftle 添加----- 解决主键不是id 或id结尾的情况----------------
                    var fieldAtrribute = (FieldAtrribute)Attribute.GetCustomAttribute(propertyInfo, typeof(FieldAtrribute));
                    if (fieldAtrribute != null && fieldAtrribute.IsPrimaryKey)
                    {
                        keyMap = map;
                    }

                    //字段名是 id 认为是主键
                    //if (string.Equals(map.PropertyInfo.Name, "id", StringComparison.InvariantCultureIgnoreCase))
                    if (keyMap == null && string.Equals(map.PropertyInfo.Name, "id", StringComparison.InvariantCultureIgnoreCase))
                    {
                        keyMap = map;
                    }

                    //字段名用id结尾 认为是主键
                    if (keyMap == null && map.PropertyInfo.Name.EndsWith("id", true, CultureInfo.InvariantCulture))
                    {
                        keyMap = map;
                    }
                }
            }

            if (keyMap != null)
            {
                keyMap.Key(PropertyTypeKeyTypeMapping.ContainsKey(keyMap.PropertyInfo.PropertyType)
                    ? PropertyTypeKeyTypeMapping[keyMap.PropertyInfo.PropertyType]
                    : KeyType.Assigned);
            }
        }

        /// <summary>
        /// 设置实体属性映射，支持链式调用
        /// </summary>
        /// <param name="expression">实体属性映射表达式</param>
        /// <returns>实体属性映射器</returns>
        protected PropertyMap Map(Expression<Func<T, object>> expression)
        {
            PropertyInfo propertyInfo = ReflectionHelper.GetProperty(expression) as PropertyInfo;
            return Map(propertyInfo);
        }

        /// <summary>
        /// 设置实体属性映射，支持链式调用
        /// </summary>
        /// <param name="propertyInfo">实体属性元数据访问器</param>
        /// <returns>实体属性映射器</returns>
        protected PropertyMap Map(PropertyInfo propertyInfo)
        {
            PropertyMap result = new PropertyMap(propertyInfo);
            this.GuardForDuplicatePropertyMap(result);
            Properties.Add(result);
            return result;
        }

        private void GuardForDuplicatePropertyMap(PropertyMap result)
        {
            if (Properties.Any(p => p.Name.Equals(result.Name)))
            {
                throw new ArgumentException(string.Format("Duplicate mapping for property {0} detected.",result.Name));
            }
        }
    }
}