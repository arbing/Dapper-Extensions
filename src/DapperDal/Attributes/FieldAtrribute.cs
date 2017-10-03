using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Reflection;

namespace DapperDal.Attributes
{
    /// <summary>
    /// 字段属性，注解
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class FieldAtrribute:Attribute
    {
        /// <summary>
        /// 是主键 ？
        /// </summary>
        public bool IsPrimaryKey { get; set; }


        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }

        

        /// <summary>
        /// 描述
        /// </summary>
        public string Describtion { get; set; }
    }
}
