using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SHJ
{
    //扩展方法
    public static class Extends
    {
        /// <summary>
        /// 获取节点属性值
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="name">属性名</param>
        /// <returns></returns>
        public static string GetNameItemValue(this XmlNode node,string name)
        {
            return node.Attributes.GetNamedItem(name).ToString();
        }

        /// <summary>
        /// 设置节点属性值
        /// </summary>
        /// <param name="node">节点</param>
        /// <param name="name">属性名</param>
        /// <param name="value">属性值</param>
        public static void WriteNameItemVAlue(this XmlNode node,string name,string value)
        {
            node.Attributes.GetNamedItem(name).Value = value;
        }
        
    }
}
