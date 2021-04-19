using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    /// <summary>
    /// 错误定义实体类
    /// </summary>
    public class ResInfo
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public ResInfo()
        {
            ResCode = 1;
            ResMsg = "成功";
        }

        /// <summary>
        /// 响应代码1:成功-1失败 0:没有满足条件记录
        /// </summary>
        public int ResCode { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string ResMsg { get; set; }
        /// <summary>
        /// 响应内容
        /// </summary>
        public object ResData { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ResDataDto
    {
        /// <summary>
        /// 总行数
        /// </summary>
        public int Records { get; set; }
        /// <summary>
        /// 列表数据
        /// </summary>
        public object Rows { get; set; }

    }
}
