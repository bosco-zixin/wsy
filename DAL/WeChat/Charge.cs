using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WSY.SQLTools;

namespace DAL.WeChat
{
    public class Charge
    {
        SQLHelper sqlHelper = null;
        string connStr = string.Empty;
        public Charge(string _connStr)
        {
            connStr = _connStr;
            sqlHelper = new SQLHelper(connStr);
        }

        /// <summary>
        /// 收费
        /// </summary>
        /// <param name="iNodeID">节点编号</param>
        /// <param name="iOwnerID">业主编号</param>
        /// <param name="iPayType">支付类型</param>
        /// <param name="iPayWay">支付方式</param>
        /// <param name="sFeeInfos">费用编号串</param>
        /// <param name="fDueTotal">本次应收</param>
        /// <param name="fFactTotal">本次实收</param>
        /// <param name="sRemark">备注</param>
        /// <param name="iOperID">操作员编号</param>
        public int ChargeFee(Model.WeChat.Charge chargeDto, ref string sChrgID)
        {
            int result;
            SqlParameter[] parameter = new SqlParameter[]
            {
                  new SqlParameter("@P_NodeID", SqlDbType.BigInt, 8),
                  new SqlParameter("@P_OwnerID", SqlDbType.BigInt, 8),
                  new SqlParameter("@P_PayTypeID", SqlDbType.Int, 4),
                  new SqlParameter("@P_PayWay", SqlDbType.SmallInt, 2),
                  new SqlParameter("@P_FeeInfos", SqlDbType.VarChar, -1),
                  new SqlParameter("@P_DueTotal", SqlDbType.Decimal, 9),
                  new SqlParameter("@P_FactTotal", SqlDbType.Decimal, 9),
                  new SqlParameter("@P_ProcedureFee", SqlDbType.Decimal, 9),
                  new SqlParameter("@P_PayAccountNo", SqlDbType.VarChar, 50),
                  new SqlParameter("@P_PaySerialNo", SqlDbType.VarChar, 50),
                  new SqlParameter("@P_TermID", SqlDbType.VarChar, 50),
                  new SqlParameter("@P_Remark", SqlDbType.VarChar, 200),
                  new SqlParameter("@P_OperID", SqlDbType.SmallInt, 2),
                  new SqlParameter("@P_WriteCardStatus", SqlDbType.SmallInt, 2),
                  new SqlParameter("@P_IsEmergency", SqlDbType.SmallInt, 2),
                  new SqlParameter("@P_ChrgID", SqlDbType.VarChar, 200)
                };
            parameter[0].Value = chargeDto.NodeID;
            parameter[1].Value = chargeDto.OwnerID;
            parameter[2].Value = chargeDto.PayTypeID;
            parameter[3].Value = 1;
            parameter[4].Value = chargeDto.FeeInfos;
            parameter[5].Value = chargeDto.DueTotal;
            parameter[6].Value = chargeDto.FactTotal;
            parameter[7].Value = chargeDto.ProcedureFee;
            parameter[8].Value = DBNull.Value;
            if (!string.IsNullOrEmpty(chargeDto.PaySerialNo))
            {
                parameter[9].Value = chargeDto.PaySerialNo;
            }
            else
            {
                parameter[9].Value = DBNull.Value;
            }
            parameter[10].Value = DBNull.Value;
            parameter[11].Value = "";
            // parameter[12].Value = iOperID;
            parameter[13].Value = 1;
            parameter[14].Value = 0;
            parameter[15].Direction = ParameterDirection.Output;
            result = sqlHelper.RunProcedure("DBO.P_Charge", parameter);
            if (result == 1)
            {
                sChrgID = parameter[15].Value.ToString();
            }
            return result;
        }


        public int ChargeBalance(Model.WeChat.Charge chargeDto, ref string sChrgID)
        {
            int result;
            SqlParameter[] parameter = new SqlParameter[] {
                        new SqlParameter("@P_NodeID", SqlDbType.BigInt, 8),
                        new SqlParameter("@P_OwnerID", SqlDbType.BigInt, 8),
                        new SqlParameter("@P_PayTypeID", SqlDbType.Int, 4),
                        new SqlParameter("@P_PayWay", SqlDbType.SmallInt, 2),
                        new SqlParameter("@P_ChrgInfos", SqlDbType.VarChar, -1),
                        new SqlParameter("@P_ProcedureFee", SqlDbType.Decimal, 9),
                        new SqlParameter("@P_PayAccountNo", SqlDbType.VarChar, 50),
                        new SqlParameter("@P_PaySerialNo", SqlDbType.VarChar, 50),
                        new SqlParameter("@P_TermID", SqlDbType.VarChar, 50),
                        new SqlParameter("@P_Remark", SqlDbType.VarChar, 200),
                        new SqlParameter("@P_OperID", SqlDbType.SmallInt, 2),
                        new SqlParameter("@P_ChrgID", SqlDbType.VarChar, 30)
                      };
           
            parameter[0].Value = chargeDto.NodeID;
            parameter[1].Value = chargeDto.OwnerID;
            parameter[2].Value = chargeDto.PayTypeID;
            parameter[3].Value = 1;
            parameter[4].Value = "0" + (chargeDto.FactTotal + chargeDto.ProcedureFee).ToString() + "交费失败充入";
            parameter[5].Value = chargeDto.ProcedureFee;
            parameter[6].Value = DBNull.Value;
            if (!string.IsNullOrEmpty(chargeDto.PaySerialNo))
            { 
                parameter[7].Value = chargeDto.PaySerialNo;
            }
            else
            { 
                parameter[7].Value = DBNull.Value;
            }
            parameter[8].Value = DBNull.Value;
            parameter[9].Value = "";
           // parameter[10].Value = iOperID;
            parameter[11].Direction = ParameterDirection.Output;
            result = sqlHelper.RunProcedure("DBO.P_ChargeBalance", parameter);
            if (result == 1)
            {
                sChrgID = parameter[11].Value.ToString(); 
            }
            return result;
        }

        public Model.WeChat.CardIdent GetChargeCardInfo(string sChrgID)
        {
            Model.WeChat.CardIdent mdlCardIdent = new Model.WeChat.CardIdent();
            sChrgID = sChrgID.Replace(",", "','");
            string sql = string.Format(@"SELECT DISTINCT A.FacyID, A.SysCode, A.CardID
                                         FROM V_Meter A
                                           INNER JOIN MeterDosage B ON A.MetID = B.MetID
                                           INNER JOIN ChargeFee C ON B.DosageID = C.DosageID
                                         WHERE A.ChrgMode = 2 AND A.GetWay IN (2, 4) AND C.ChrgID IN ('{0}')", sChrgID);
            DataSet ds = sqlHelper.Query(sql);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                mdlCardIdent.FacyID = ds.Tables[0].Rows[0]["FacyID"].ToString();
                mdlCardIdent.SysCode = ds.Tables[0].Rows[0]["SysCode"].ToString();
                mdlCardIdent.CardID = ds.Tables[0].Rows[0]["CardID"].ToString();
            }
            return mdlCardIdent;
        }

        /// <summary>
        /// 获取读写卡数据包
        /// </summary>
        /// <param name="iType">操作类型，1：加表；2：收费；3：补卡；4：换表；5：刷量；6：退费；99：读卡</param>
        /// <param name="iNodeID">节点编号</param>
        /// <param name="iFacyID">厂家编号</param>
        /// <param name="sSysCode">系统号</param>
        /// <param name="sCardID">卡号</param>
        /// <param name="sInserteds">是否已插表，补卡用</param>
        /// <param name="iMetID">表号</param>
        /// <param name="sFundID">收费回单号</param>
        /// <returns></returns>
        public Model.WeChat.Card GetCardStr(int iType, long iNodeID, Model.WeChat.CardIdent mdlCardIdent, string sInserteds, long iMetID, string sChrgID)
        {
            string stepPriceStartTime = "";
            decimal stepNum1 = 0M, stepNum2 = 0M, stepNum3 = 0M, stepPrice1 = 0M, stepPrice2 = 0M, stepPrice3 = 0M, stepPrice4 = 0M;
            Model.WeChat.Card mdlCard = new Model.WeChat.Card();
            string where = string.Format(@" WHERE A.GetWay IN (2, 4) AND A.Status = 2 AND A.NodeID = {0} AND A.FacyID = {1} AND A.SysCode = '{2}' AND A.CardID = '{3}' AND C.IsPrimary = 1 AND D.Status = 1",
                iNodeID, mdlCardIdent.FacyID, mdlCardIdent.SysCode, mdlCardIdent.CardID);
            int index = 0;
            string[] aryInserted = sInserteds.Split(',');
            if (iType == 1) //加表
            {
                where += " AND A.CardStatus = 1";
            }
            else
            {
                where += " AND A.CardStatus = 2";
                if (iType == 2 || iType == 6) //收费、退费
                {
                    where += @" AND A.ChrgMode = 2 AND A.MetID IN (
                            SELECT A.MetID FROM ChargeFee A  WHERE A.ChrgID IN ('" + sChrgID.Replace(",", "','") + "'))";
                }
                else if (iType == 4) //换表
                {
                    where += string.Format(" AND A.MetID = {0}", iMetID);
                }
                else if (iType == 5) //刷量
                {
                    where += " AND A.NotWriteTotal > 0";
                }
            }
            if (iType == 2)
                where += " AND A.MetClassID <> 260"; //贝林抄表式水表不写卡
            string sql = string.Format(@"SELECT A.*, ISNULL(B.MeterCount, 0) AS MeterCount, D.PriceType
                                         FROM V_Meter A
                                           LEFT JOIN (SELECT NodeID, FacyID, SysCode, CardID, COUNT(1) AS MeterCount
                                                      FROM V_Meter
                                                      WHERE GetWay IN (2, 4) AND Status = 2 AND CardStatus = 2
                                                      GROUP BY NodeID, FacyID, SysCode, CardID
                                           ) B ON A.NodeID = B.NodeID AND A.FacyID = B.FacyID AND A.SysCode = B.SysCode AND A.CardID = B.CardID
                                           INNER JOIN ChargeItemMeter C ON A.MetID = C.MetID
                                           INNER JOIN V_ChargeItem D ON C.ChrgItemID = D.ChrgItemID{0}
                                         ORDER BY A.IndexNo", where);
            DataSet ds = sqlHelper.Query(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                mdlCard.FacyID = ds.Tables[0].Rows[0]["FacyID"].ToString();
                mdlCard.SysCode = ds.Tables[0].Rows[0]["SysCode"].ToString();
                mdlCard.CardID = ds.Tables[0].Rows[0]["CardID"].ToString();
                mdlCard.AreaCode = ds.Tables[0].Rows[0]["AreaCode"].ToString();
                mdlCard.CardVer = ds.Tables[0].Rows[0]["CardVer"].ToString();
                mdlCard.MeterCount = ds.Tables[0].Rows[0]["MeterCount"].ToString();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Model.WeChat.CardMeter mdlCardMeter = new Model.WeChat.CardMeter();
                    mdlCardMeter.IndexNo = dr["IndexNo"].ToString();
                    mdlCardMeter.MeterType = dr["MetClassID"].ToString();
                    mdlCardMeter.MeterCode = dr["MetCode"].ToString();
                    mdlCardMeter.ParamVer = dr["ParamVer"].ToString();
                    mdlCardMeter.ChargeMode = "0";
                    if (dr["BuyMode"].ToString() == "2") //金额模式
                    {
                        if (dr["PriceType"].ToString() == "2")
                        {
                            mdlCardMeter.ChargeMode = "1"; //月阶梯
                        }
                        else
                        {
                            mdlCardMeter.ChargeMode = "2"; //年阶梯
                        }
                    }
                    SetStepParam(int.Parse(dr["WriteTimes"].ToString()) <= 1 ? 1 : 2, long.Parse(ds.Tables[0].Rows[0]["MetID"].ToString()),
                        ref stepPriceStartTime, ref stepNum1, ref stepNum2, ref stepNum3, ref stepPrice1, ref stepPrice2, ref stepPrice3, ref stepPrice4);
                    mdlCardMeter.StepNum1 = stepNum1.ToString();
                    mdlCardMeter.StepNum2 = stepNum2.ToString();
                    mdlCardMeter.StepNum3 = stepNum3.ToString();
                    if (mdlCardMeter.ChargeMode == "0" && Convert.ToInt16(dr["NumMode"]) == 1) //量模式，并且是充量
                    {
                        mdlCardMeter.StepPrice1 = "1";
                        mdlCardMeter.StepPrice2 = "1";
                        mdlCardMeter.StepPrice3 = "1";
                        mdlCardMeter.StepPrice4 = "1";
                    }
                    else
                    {
                        mdlCardMeter.StepPrice1 = stepPrice1.ToString();
                        mdlCardMeter.StepPrice2 = stepPrice2.ToString();
                        mdlCardMeter.StepPrice3 = stepPrice3.ToString();
                        mdlCardMeter.StepPrice4 = stepPrice4.ToString();
                    }
                    mdlCardMeter.StepPriceStartTime = stepPriceStartTime;
                    if (mdlCardMeter.ChargeMode == "0" && Convert.ToInt16(dr["NumMode"]) == 2) //量模式，并且是充金额
                    {
                        if (int.Parse(dr["WriteTimes"].ToString()) > 1)
                        //此处只取现价
                        {
                            SetStepParam(1, long.Parse(ds.Tables[0].Rows[0]["MetID"].ToString()),
                               ref stepPriceStartTime, ref stepNum1, ref stepNum2, ref stepNum3, ref stepPrice1, ref stepPrice2, ref stepPrice3, ref stepPrice4);
                        }
                        if (dr["FirstAlert"].ToString() != "")
                        {
                            mdlCardMeter.FirstAlert = (Convert.ToDecimal(dr["FirstAlert"]) * stepPrice1).ToString("0.00");
                        }
                        if (dr["SecdAlert"].ToString() != "")
                        {
                            mdlCardMeter.SecdAlert = (Convert.ToDecimal(dr["SecdAlert"]) * stepPrice1).ToString("0.00");
                        }
                        if (dr["Hoard"].ToString() != "")
                        {
                            mdlCardMeter.Hoard = (Convert.ToDecimal(dr["Hoard"]) * stepPrice1).ToString("0.00");
                        }
                        if (dr["Overdraft"].ToString() != "")
                        {
                            mdlCardMeter.SecdAlert = (Convert.ToDecimal(dr["Overdraft"]) * stepPrice1).ToString("0.00");
                        }
                    }
                    else
                    {
                        if (dr["FirstAlert"].ToString() != "")
                        {
                            mdlCardMeter.FirstAlert = dr["FirstAlert"].ToString();
                        }
                        if (dr["SecdAlert"].ToString() != "")
                        {
                            mdlCardMeter.SecdAlert = dr["SecdAlert"].ToString();
                        }
                        if (dr["Hoard"].ToString() != "")
                        {
                            mdlCardMeter.Hoard = dr["Hoard"].ToString();
                        }
                        if (dr["Overdraft"].ToString() != "")
                        {
                            mdlCardMeter.Overdraft = dr["Overdraft"].ToString();
                        }
                    }
                    if (dr["HFConst"].ToString() != "")
                    {
                        mdlCardMeter.HFConst = dr["HFConst"].ToString();
                    }
                    if (dr["PowerLoad"].ToString() != "")
                    {
                        mdlCardMeter.PowerLoad = dr["PowerLoad"].ToString();
                    }
                    if (dr["RestLoad"].ToString() != "")
                    {
                        mdlCardMeter.RestLoad = dr["RestLoad"].ToString();
                    }
                    mdlCardMeter.Multiple = dr["Multiple"].ToString();
                    if (iType != 5) //正常业务
                    {
                        if (iType != 6) //不是退费
                        {
                            mdlCardMeter.OperType = iType.ToString();
                        }
                        else
                        {
                            mdlCardMeter.OperType = "3";
                        }
                        mdlCardMeter.BuyTimes = dr["WriteTimes"].ToString();
                        mdlCardMeter.ThisBuy = dr["LastWrite"].ToString();
                        mdlCardMeter.BuyTotal = dr["WriteTotal"].ToString();
                    }
                    else //刷量
                    {
                        mdlCardMeter.OperType = "2";
                        mdlCardMeter.BuyTimes = (int.Parse(dr["WriteTimes"].ToString()) + 1).ToString();
                        if (mdlCardMeter.BuyTimes == "1") //第一次购买，下补量
                        {
                            if (mdlCardMeter.ChargeMode == "0" && Convert.ToInt16(dr["NumMode"]) == 2) //量模式，并且是充金额
                            {
                                mdlCardMeter.ThisBuy = ((decimal.Parse(dr["NotWriteTotal"].ToString()) + decimal.Parse(dr["SubNum"].ToString())) * stepPrice1).ToString("0.00");
                                mdlCardMeter.BuyTotal = (decimal.Parse(dr["WriteTotal"].ToString()) + decimal.Parse(((decimal.Parse(dr["NotWriteTotal"].ToString()) + decimal.Parse(dr["SubNum"].ToString())) * stepPrice1).ToString("0.00"))).ToString();
                            }
                            else
                            {
                                mdlCardMeter.ThisBuy = (decimal.Parse(dr["NotWriteTotal"].ToString()) + decimal.Parse(dr["SubNum"].ToString())).ToString();
                                mdlCardMeter.BuyTotal = (decimal.Parse(dr["WriteTotal"].ToString()) + decimal.Parse(dr["NotWriteTotal"].ToString()) + decimal.Parse(dr["SubNum"].ToString())).ToString();
                            }
                        }
                        else
                        {
                            if (mdlCardMeter.ChargeMode == "0" && Convert.ToInt16(dr["NumMode"]) == 2) //量模式，并且是充金额
                            {
                                mdlCardMeter.ThisBuy = (decimal.Parse(dr["NotWriteTotal"].ToString()) * stepPrice1).ToString("0.00");
                                mdlCardMeter.BuyTotal = (decimal.Parse(dr["WriteTotal"].ToString()) + decimal.Parse((decimal.Parse(dr["NotWriteTotal"].ToString()) * stepPrice1).ToString("0.00"))).ToString();
                            }
                            else
                            {
                                mdlCardMeter.ThisBuy = dr["NotWriteTotal"].ToString();
                                mdlCardMeter.BuyTotal = (decimal.Parse(dr["WriteTotal"].ToString()) + decimal.Parse(dr["NotWriteTotal"].ToString())).ToString();
                            }
                        }
                    }
                    if (dr["LastBuyTime"].ToString() != "")
                    {
                        mdlCardMeter.ThisBuyTime = DateTime.Parse(dr["LastBuyTime"].ToString()).ToString("yyyyMMddHHmm");
                    }
                    mdlCardMeter.HardStatus = "1";
                    mdlCardMeter.RunStatus = "1";
                    if (iType == 99)
                    {
                        mdlCardMeter.ReadStatus = "99";
                    }
                    else if (iType == 3)
                    {
                        if (aryInserted[index] == "0")
                        {
                            mdlCardMeter.ReadStatus = "2"; //复用，补卡时传入是否已插表选项
                        }
                        else
                        {
                            mdlCardMeter.ReadStatus = "1"; //复用，补卡时传入是否已插表选项
                            mdlCardMeter.ThisBuy = "0";
                        }
                        ++index;
                    }
                    else
                    {
                        mdlCardMeter.ReadStatus = "1"; 
                    }
                    mdlCard.MeterList.Add(mdlCardMeter);
                }
            }
            return mdlCard;
        }

        /// <summary>
        /// 设置卡表阶梯参数
        /// </summary>
        /// <param name="iType"></param>
        /// <param name="iMetID"></param>
        /// <param name="StepPriceStartTime"></param>
        /// <param name="iStepNum1"></param>
        /// <param name="iStepNum2"></param>
        /// <param name="iStepNum3"></param>
        /// <param name="fStepPrice1"></param>
        /// <param name="fStepPrice2"></param>
        /// <param name="fStepPrice3"></param>
        /// <param name="fStepPrice4"></param>
        private void SetStepParam(int iType, long iMetID, ref string StepPriceStartTime, ref decimal iStepNum1, ref decimal iStepNum2, ref decimal iStepNum3,
    ref decimal fStepPrice1, ref decimal fStepPrice2, ref decimal fStepPrice3, ref decimal fStepPrice4)
        {
            string where = "";
            if (iType == 1) //开户
                where = string.Format(" WHERE A.MetID = {0} AND CONVERT(VARCHAR(10), GETDATE(), 120) BETWEEN D.StartDate AND ISNULL(D.EndDate, CONVERT(VARCHAR(10), GETDATE(), 120))", iMetID);
            else  //其它
                where = string.Format(" WHERE A.MetID = {0} AND D.EndDate IS NULL", iMetID);

            DataSet ds = sqlHelper.Query(string.Format(@"SELECT D.StartDate, ISNULL(D.MaxNum, 99999999) AS MaxNum, D.Price
                                                           FROM MeterHouse A
                                                             INNER JOIN RoomHouse B ON A.NodeID = B.NodeID
                                                             INNER JOIN ChargeItemMeter C ON A.MetID = C.MetID
                                                             INNER JOIN ChargePrice D ON B.ChrgStdID = D.ChrgStdID AND C.ChrgItemID = D.ChrgItemID{0}
                                                           ORDER BY D.ChrgPriceID", where));
            for (int i = 0; i < 4; i++)
            {
                switch (i)
                {
                    case 0:
                        StepPriceStartTime = ds.Tables[0].Rows[0]["StartDate"].ToString().Replace("-", "") + "0000"; //yyyyMMddHHmm
                        if (ds.Tables[0].Rows.Count > 1)
                            iStepNum1 = decimal.Parse(ds.Tables[0].Rows[0]["MaxNum"].ToString());
                        fStepPrice1 = decimal.Parse(ds.Tables[0].Rows[0]["Price"].ToString());
                        break;
                    case 1:
                        if (ds.Tables[0].Rows.Count > 2)
                            iStepNum2 = decimal.Parse(ds.Tables[0].Rows[1]["MaxNum"].ToString());
                        else
                            iStepNum2 = iStepNum1;
                        if (ds.Tables[0].Rows.Count >= 2)
                            fStepPrice2 = decimal.Parse(ds.Tables[0].Rows[1]["Price"].ToString());
                        else
                            fStepPrice2 = fStepPrice1;
                        break;
                    case 2:
                        if (ds.Tables[0].Rows.Count > 3)
                            iStepNum3 = decimal.Parse(ds.Tables[0].Rows[2]["MaxNum"].ToString());
                        else
                            iStepNum3 = iStepNum2;
                        if (ds.Tables[0].Rows.Count >= 3)
                            fStepPrice3 = decimal.Parse(ds.Tables[0].Rows[2]["Price"].ToString());
                        else
                            fStepPrice3 = fStepPrice2;
                        break;
                    case 3:
                        if (ds.Tables[0].Rows.Count >= 4)
                            fStepPrice4 = decimal.Parse(ds.Tables[0].Rows[ds.Tables[0].Rows.Count - 1]["Price"].ToString());
                        else
                            fStepPrice4 = fStepPrice3;
                        break;
                }
            }
        }
    }
}
