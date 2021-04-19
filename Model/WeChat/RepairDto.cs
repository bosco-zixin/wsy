using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WeChat
{
    /// <summary>
    /// 添加保修实体
    /// </summary>
    public class RepairAddDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string AreaID  { get;set; }
        /// <summary>
        /// 
        /// </summary>
        public string NodeID { get;set; }
        /// <summary>
        /// 
        /// </summary>
        public string Address { get;set; }
        /// <summary>
        /// 
        /// </summary>
        public string ContactPerson { get;set; }
        /// <summary>
        /// 
        /// </summary>
        public string ContactTel { get;set; }
        /// <summary>
        /// 
        /// </summary>
        public string ServiceNote { get;set; }
        /// <summary>
        /// 
        /// </summary>
        public string Picture1 { get;set; }
        /// <summary>
        /// 
        /// </summary>
        public string Picture2 { get;set; }
        /// <summary>
        /// 
        /// </summary>
        public string Picture3 { get;set; }     
        /// <summary>
        /// 是否反馈 0不1电话反馈
        /// </summary>
        public byte ISFeedback { get; set; }
            
    }

    /// <summary>
    /// 报修列表请求实体
    /// </summary>
    public class RepairListDto
    {
        /// <summary>
        /// 
        /// </summary>
        public string ServiceSer { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateTimeS { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CreateTimeE { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ContactPerson { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ContactTel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ServiceNote { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Picture3 { get; set; }
        /// <summary>
        /// 是否反馈 0不1电话反馈
        /// </summary>
        public byte ISFeedback { get; set; }

    }
}
