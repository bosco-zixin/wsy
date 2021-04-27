using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.WeChat
{
    /// <summary>
    /// 
    /// </summary>
    public class CardIdent
    {
        /// <summary>
        /// 厂家编号
        /// </summary>
        public string FacyID { get; set; }
        /// <summary>
        /// 系统号
        /// </summary>
        public string SysCode { get; set; }
        /// <summary>
        /// 卡号
        /// </summary>
        public string CardID { get; set; }
        /// <summary>
        /// 大区号
        /// </summary>
        public string AreaCode { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Card
    {
        /// <summary>
        /// 厂家编号
        /// </summary>
        public string FacyID { get; set; }

        /// <summary>
        /// 系统号
        /// </summary>
        public string SysCode { get; set; }

        /// <summary>
        /// 卡号
        /// </summary>
        public string CardID { get; set; }

        /// <summary>
        /// 区域号
        /// </summary>
        public string AreaCode { get; set; }

        /// <summary>
        /// 卡版本号
        /// </summary>
        public string CardVer { get; set; }

        /// <summary>
        /// 卡中表数量
        /// </summary>
        public string MeterCount { get; set; }

        /// <summary>
        /// 操作表列表
        /// </summary>
        public List<CardMeter> MeterList { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CardMeter
    {
        /// <summary>
        /// 卡表序号
        /// </summary>
        public string IndexNo { get; set; }

        /// <summary>
        /// 操作类型，1：加表；2：收费；3：补卡；4：换表；99：读卡
        /// </summary>
        public string OperType { get; set; }

        /// <summary>
        /// 表型
        /// </summary>
        public string MeterType { get; set; }

        /// <summary>
        /// 表号
        /// </summary>
        public string MeterCode { get; set; } = "";

        /// <summary>
        /// 参数版本
        /// </summary>
        public string ParamVer { get; set; } = "0";

        /// <summary>
        /// 表模式，0：数量；1：月阶梯；2：年阶梯
        /// </summary>
        public string ChargeMode { get; set; } = "0";

        /// <summary>
        /// 一级报警
        /// </summary>
        public string FirstAlert { get; set; } = "0";

        /// <summary>
        /// 二级报警
        /// </summary>
        public string SecdAlert { get; set; } = "0";

        /// <summary>
        /// 电表常数
        /// </summary>
        public string HFConst { get; set; } = "0";

        /// <summary>
        /// 允许负荷
        /// </summary>
        public string PowerLoad { get; set; } = "0";

        /// <summary>
        /// 允许阻性负载
        /// </summary>
        public string RestLoad { get; set; } = "0";

        /// <summary>
        /// 允许囤积量/金额
        /// </summary>
        public string Hoard { get; set; } = "0";

        /// <summary>
        /// 允许透支量/金额
        /// </summary>
        public string Overdraft { get; set; } = "0";

        /// <summary>
        /// 倍率
        /// </summary>
        public string Multiple { get; set; } = "1";

        /// <summary>
        /// 购买次数
        /// </summary>
        public string BuyTimes { get; set; } = "0";

        /// <summary>
        /// 本次购买量/金额
        /// </summary>
        public string ThisBuy { get; set; } = "0";

        /// <summary>
        /// 本次购买时间
        /// </summary>
        public string ThisBuyTime { get; set; } = "";

        /// <summary>
        /// 总购买量/金额
        /// </summary>
        public string BuyTotal { get; set; } = "0";

        /// <summary>
        /// 一阶数量
        /// </summary>
        public string StepNum1 { get; set; } = "0";

        /// <summary>
        /// 二阶数量
        /// </summary>
        public string StepNum2 { get; set; } = "0";

        /// <summary>
        /// 三阶数量
        /// </summary>
        public string StepNum3 { get; set; } = "0";

        /// <summary>
        /// 一阶价格
        /// </summary>
        public string StepPrice1 { get; set; } = "0";

        /// <summary>
        /// 二阶价格
        /// </summary>
        public string StepPrice2 { get; set; } = "0";


        /// <summary>
        /// 三阶价格
        /// </summary>
        public string StepPrice3 { get; set; } = "0";

        /// <summary>
        /// 四阶价格
        /// </summary>
        public string StepPrice4 { get; set; } = "0";

        /// <summary>
        /// 阶梯价格起始时间
        /// </summary>
        public string StepPriceStartTime { get; set; } = "";

        /// <summary>
        /// 供热起始时间
        /// </summary>
        public string HeatStartTime { get; set; } = "";

        /// <summary>
        /// 供热结束时间
        /// </summary>
        public string HeatEndTime { get; set; } = "";

        /// <summary>
        /// 累计使用量/金额
        /// </summary>
        public string UsedTotal { get; set; } = "0";

        /// <summary>
        /// 剩余量/金额
        /// </summary>
        public string Surplus { get; set; } = "0";

        /// <summary>
        /// 剩余量/金额
        /// </summary>
        public List<string> MonFreezeNums { get; set; }

        /// <summary>
        /// 插表时间
        /// </summary>
        public string InsertTime { get; set; } = "";

        /// <summary>
        /// 硬件状态，1：正常；2：异常
        /// </summary>
        public string HardStatus { get; set; } = "1";

        /// <summary>
        /// 运行状态，1：运行；2：停止
        /// </summary>
        public string RunStatus { get; set; } = "1";

        /// <summary>
        /// 读卡状态，1：成功；2：未插表；99：未读卡
        /// </summary>
        public string ReadStatus { get; set; } = "99";
    }

    /// <summary>
    /// 
    /// </summary>
    public class Charge
    {
        /// <summary>
        /// 
        /// </summary>
        public long NodeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public long OwnerID { get; set; }        
        /// <summary>
        /// 
        /// </summary>
        public int PayTypeID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FeeInfos { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal DueTotal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal FactTotal { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal ProcedureFee { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PaySerialNo { get; set; }
    }
}
