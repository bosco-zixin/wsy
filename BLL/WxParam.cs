using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    //存放Api请求地址
    private static string urlWxAPI = ConfigParam.WxAPI;

    //用于存放终端客户号
    private static string WxOrgCode = ConfigParam.WxOrgCode;

    //用于存放终端appkey
    private static string WxAppKey = ConfigParam.mchidKey;//用mchidKey用作每个水司的唯一密匙

    //时间戳
    private static string timestamp = GetTimeStamp(DateTime.Now);

    //Md5转化过得appkey参数值
    private static string AppKey;

    #region 调用旌旗三方业务WebAPI==================================================================================
    /// <summary>
    /// 检查此是否关注过此微信公众号
    /// </summary>
    /// <param name="WXID"></param>
    /// <returns></returns>
    public DataTable GetMCHInfo(string WXID)
    {
        JObject resObjRoom = null;
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&MchID=" + ConfigParam.mchid);

            string resultJson = WebGet(urlWxAPI + "api/Service/GetMCHInfo", JsonStr.ToString());
            log.Debug("api/Service/GetMCHInfo End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                DataTbRoom = JsonConvert.DeserializeObject<DataTable>(resultsRoom);
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }

    #region 根据微信号查询
    /// <summary>
    /// 根据微信号查看参数
    /// </summary>
    /// <param name="Opid"></param>
    /// <returns></returns>
    public bool GetParams(string Opid)
    {
        ILog log = LogManager.GetLogger(this.GetType());
        log.Debug("Opid:" + Opid + ";WebType:" + ConfigParam.WebType);
        try
        {
            DataTable DataTb = CheckWeChatCode(Opid);
            if (DataTb != null && DataTb.Rows.Count > 0)
            {
                try
                {
                    muid = DataTb.Rows[0]["MUID"].ToString();
                    log.Debug("muid:" + muid);

                    if (DataTb != null && DataTb.Rows.Count > 0)
                    {
                        AddressID = DataTb.Rows[0]["NodeID"].ToString();
                        LiveID = DataTb.Rows[0]["LiveID"].ToString();
                        AreaCode = DataTb.Rows[0]["AREACODE"].ToString();
                        UserName = DataTb.Rows[0]["USERNAME"].ToString();
                        CustID = DataTb.Rows[0]["CUSTID"].ToString();
                        ISPayUnit = DataTb.Rows[0]["ISPayUnit"].ToString();
                        ISWeChat = DataTb.Rows[0]["ISWeChat"].ToString();
                    }
                    else
                    {
                        AddressID = "";
                        LiveID = "";
                        AreaCode = "";
                        UserName = "";
                        CustID = "";
                    }
                    log.Debug("addressid:" + AddressID + ";liveid:" + LiveID + ";areacode:" + AreaCode + ";username:" + UserName + ";custid:" + CustID);
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    DataTb.Dispose();
                    GC.Collect();
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            log.Debug("try:NO");
            return false;
        }
    }
    #endregion

    #region 用户
    /// <summary>
    /// 检查此是否关注过此微信公众号
    /// </summary>
    /// <param name="WXID"></param>
    /// <returns></returns>
    public DataTable CheckWeChatCode(string WXID)
    {
        JObject resObjRoom = null;
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&wxid=" + WXID + "&MchID=" + ConfigParam.mchid);
            string resultJson = WebGet(urlWxAPI + "api/Service/CheckWeChatCode", JsonStr.ToString());
            log.Debug("api/Service/CheckWeChatCode End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                DataTbRoom = JsonConvert.DeserializeObject<DataTable>(resultsRoom);
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }

    #endregion

    #region 房间相关

    /// <summary>
    /// 添加绑定房间
    /// </summary>
    public bool Add_Room(MWeChat_MyRoomInfo model)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            //创建POST基础参数
            StringBuilder POSTsb = new StringBuilder();
            POSTsb.Append("{");
            POSTsb.Append("\"orgcode\":\"" + WxOrgCode + "\",");
            POSTsb.Append("\"timestamp\":\"" + timestamp + "\",");
            POSTsb.Append("\"appkey\":\"" + AppKey + "\",");
            //加入基本参数
            POSTsb.Append("\"CUSTID\":\"" + model.CUSTID + "\",");
            POSTsb.Append("\"NODEID\":\"" + model.NODEID + "\",");
            POSTsb.Append("\"MUID\":\"" + model.MUID + "\",");
            POSTsb.Append("\"ADDRESS\":\"" + model.ADDRESS + "\",");
            POSTsb.Append("\"LIVEID\":\"" + model.LIVEID + "\",");
            POSTsb.Append("\"RMALIAS\":\"" + model.RMALIAS + "\",");
            POSTsb.Append("\"USERNAME\":\"" + model.USERNAME + "\",");
            POSTsb.Append("\"AREACODE\":\"" + model.AREACODE + "\",");
            POSTsb.Append("\"MchID\":\"" + ConfigParam.mchid + "\"");
            POSTsb.Append("}");

            string resultJson = WebPost(urlWxAPI + "api/Service/BindRoom", POSTsb.ToString());

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 修改房间简称
    /// </summary>
    /// <param name="muid"></param>
    /// <returns></returns>
    public bool Rename_Room(string Custid, string Nodeid, string Muid, string Rmalias)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&custid=" + Custid + "&Rmalias=" + Rmalias + "&MchID=" + ConfigParam.mchid);

            string resultJson = WebGet(urlWxAPI + "api/Service/UpdRmaliasRoom", JsonStr.ToString());

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 删除房间
    /// </summary>
    public bool Del_Room(string CustID, string NodeID, string MuID)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&custid=" + CustID + "&nodeid=" + NodeID + "&muid=" + MuID + "&MchID=" + ConfigParam.mchid);

            string resultJson = WebGet(urlWxAPI + "api/Service/DeleteRoom", JsonStr.ToString());

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// 查询房间列表
    /// </summary>
    /// <param name="muid"></param>
    /// <returns></returns>
    public DataTable GetMyroomByMuID(string muid)
    {
        JObject resObjRoom = null;
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&muid=" + muid + "&MchID=" + ConfigParam.mchid);

            string resultJson = WebGet(urlWxAPI + "api/Service/GetMyroomByMuID", JsonStr.ToString());
            log.Debug("api/Service/GetMyroomByMuID End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                DataTbRoom = JsonConvert.DeserializeObject<DataTable>(resultsRoom);
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }

    #endregion

    #region 用户与房信息===========================================
    /// <summary>
    /// 用户与房信息
    /// </summary>
    /// <param name="muid"></param>
    /// <returns></returns>
    public DataTable GetMyUserRoomBind(string value, string AreaCode, string aCode)
    {
        JObject resObjRoom = null;
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&value=" + value + "&pwd=" + "&AreaCode=" + aCode + "&MchID=" + ConfigParam.mchid);
            log.Debug("JsonStr:" + JsonStr.ToString());

            log.Debug("api/Service/GetUserAndHouInfo Begin:" + JsonStr.ToString());
            string resultJson = WebGet(urlWxAPI + "api/Service/GetUserAndHouInfo", JsonStr.ToString());
            log.Debug("api/Service/GetUserAndHouInfo End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                DataTbRoom = JsonConvert.DeserializeObject<DataTable>(resultsRoom);
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }
    #endregion

    #region 获取用户基本信息（姓名，地址，账户余额）===========================================================
    /// <summary>
    /// 获取用户基本信息
    /// </summary>
    /// <param name="muid"></param>
    /// <returns></returns>
    public DataTable GetAccountBalance(string liveID)
    {
        JObject resObjRoom = null;
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();
        //JsonStr.Append("{");
        //JsonStr.Append("\"access_token\": \"wx9fdb8ble7ce3c68f\",");
        //JsonStr.Append("\"liveID\": \"" + liveID + "\"");
        //JsonStr.Append("}");
        //string RetString1 = JsonStr.ToString();
        try
        {
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&liveID=" + liveID + "&MchID=" + ConfigParam.mchid);

            string resultJson = WebGet(urlWxAPI + "api/Service/GetAccountBalance", JsonStr.ToString());
            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                DataTbRoom = JsonConvert.DeserializeObject<DataTable>(resultsRoom);
            }
            else
            {
                DataTbRoom = null;
            }
            return DataTbRoom;

        }
        catch
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }
    #endregion

    #region 获取用户表具信息（预付费、后付费表具，用水性质，收费项目，时间用量，数量，金额）============

    /// <summary>
    /// 请求可购买的表具信息 预付费（金额、数量）和后付费
    /// 获取用户表具信息（预付费表具，上次购买量，购买时间，剩余量）
    /// 获取用户表具信息（预付费表具，用水性质，收费项目，时间用量，金额）
    /// </summary>
    /// <param name="liveid"></param>
    /// <returns></returns>
    public DataSet GetPrepaymeterInfo(string liveid)
    {
        JObject resObjRoom = null;
        DataSet DataTbRoom = new DataSet();
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&liveid=" + liveid + "&MchID=" + ConfigParam.mchid);
            string resultJson = WebGet(urlWxAPI + "api/Service/GetPrepaymeterInfo", JsonStr.ToString());

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                var ds = JsonConvert.DeserializeObject<DataSet>(resultsRoom);

                DataTbRoom = ds;// JsonConvert.DeserializeObject<DataTable>(resultsRoom.Replace("null", " "));
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }

    /// <summary>
    /// 获取用户代缴信息（收费月份，用户数量，总用量，应收金额）
    /// </summary>
    /// <param name="liveid"></param>
    /// <returns></returns>
    public DataSet GetPrepaymeterInfoByTJ(string liveid)
    {
        JObject resObjRoom = null;
        DataSet DataTbRoom = new DataSet();
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&liveid=" + liveid + "&MchID=" + ConfigParam.mchid);
            string resultJson = WebGet(urlWxAPI + "api/Service/GetPrepaymeterInfoByTJ", JsonStr.ToString());

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                var ds = JsonConvert.DeserializeObject<DataSet>(resultsRoom);

                DataTbRoom = ds;// JsonConvert.DeserializeObject<DataTable>(resultsRoom.Replace("null", " "));
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }
    #endregion

    #region 获取用户表具信息(只获取表具不附带其它字段)============
    /// <summary>
    /// 获取用户表具信息
    /// </summary>
    /// <param name="liveid"></param>
    /// <returns></returns>
    public DataTable GetMetersByLiveid(string liveid)
    {
        JObject resObjRoom = null;
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&liveid=" + liveid + "&MchID=" + ConfigParam.mchid);

            string resultJson = WebGet(urlWxAPI + "api/Service/GetMetersByLiveid", JsonStr.ToString());

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                DataTbRoom = JsonConvert.DeserializeObject<DataTable>(resultsRoom);
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }

    #endregion

    #region 获取月用量============
    /// <summary>
    /// 获取月用量
    /// </summary>
    /// <param name="metid"></param>
    /// <param name="sm"></param>
    /// <param name="em"></param>
    /// <returns></returns>
    public DataTable GetMonQty(string metid, string sm, string em)
    {
        JObject resObjRoom = null;
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&metid=" + metid + "&sm=" + sm + "&em=" + em + "&MchID=" + ConfigParam.mchid);

            string resultJson = WebGet(urlWxAPI + "api/Service/GetMonQty", JsonStr.ToString());

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                DataTbRoom = JsonConvert.DeserializeObject<DataTable>(resultsRoom);
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }

    #endregion

    #region 获取预付费用户输量计算收费明细信息（预付费输量）============

    /// <summary>
    /// 获取预付费用户输量计算收费明细信息（预付费输量）
    /// </summary>
    /// <param name="liveid"></param>
    /// <returns></returns>
    public string GetCalAmountAndList(string MetID, string ChrgNum, string OpCode)
    {
        string resultCode = "";
        JObject resObjRoom = null;
        StringBuilder JsonStr = new StringBuilder();
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&MetID=" + MetID + "&ChrgNum=" + ChrgNum + "&OpCode=" + OpCode + "&MchID=" + ConfigParam.mchid);

            log.Debug("api/Service/GetCalAmountAndList Begin:" + JsonStr.ToString());
            string resultJson = WebGet(urlWxAPI + "api/Service/GetCalAmountAndList", JsonStr.ToString());
            log.Debug("api/Service/GetCalAmountAndList End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                resultCode = SerializeObject(resObjRoom["results"]);
            }

            return resultCode;
        }
        catch
        {
            return "";
        }
        finally
        {
            JsonStr.Clear();
        }
    }
    #endregion

    #region 获取用户缴费信息============

    /// <summary>
    /// 缴费项目
    /// 缴费明细
    /// </summary>
    /// <param name="liveid">用户主键</param>
    /// <param name="nodeid"></param>
    /// <param name="sm">开始日期</param>
    /// <param name="em">结束日期</param>
    /// <param name="n">根据不同的值 获取不同的实体</param>
    /// <returns></returns>
    public DataTable GetPaymentQuery(string liveid, string nodeid, string sm, string em, int n)
    {
        JObject resObjRoom = null;
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&id=" + liveid + "&nodeid=" + nodeid + "&sm=" + sm + "&em=" + em + "&detail=" + n + "&MchID=" + ConfigParam.mchid);
            string resultJson = WebGet(urlWxAPI + "api/Service/GetPaymentQuery", JsonStr.ToString());
            log.Debug("api/Service/GetPaymentQuery:" + JsonStr.ToString());
            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson.Replace("null", "''"));
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                if (n == 0)
                {
                    JObject tt = (JObject)JsonConvert.DeserializeObject(resultsRoom);
                    string PQTable = SerializeObject(tt["PQTable"]);
                    DataTbRoom = JsonConvert.DeserializeObject<DataTable>(PQTable);
                }
                else
                {
                    JObject tt = (JObject)JsonConvert.DeserializeObject(resultsRoom);
                    string PQDetailTable = SerializeObject(tt["PQDetailTable"]);
                    DataTbRoom = JsonConvert.DeserializeObject<DataTable>(PQDetailTable);
                }
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch (Exception ex)
        {
            log.Debug("api/Service/GetPaymentQuery:" + ex.Message);
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }

    public DataTable GetAlreadyInvoice(string chaser)
    {
        JObject resObj = null;
        DataTable DataTb = new DataTable();
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&chaser=" + chaser + "&MchID=" + ConfigParam.mchid);
            string resultJson = WebGet(urlWxAPI + "api/Service/GetAlreadyInvoice", JsonStr.ToString());

            resObj = (JObject)JsonConvert.DeserializeObject(resultJson.Replace("null", "''"));
            if (resObj["errmsg"].ToString() == "success")
            {
                string results = SerializeObject(resObj["results"]);
                if (!string.IsNullOrEmpty(results))
                {
                    DataTb = JsonConvert.DeserializeObject<DataTable>(results);
                }
            }
            else
            {
                DataTb = null;
            }

            return DataTb;
        }
        catch (Exception ex)
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }
    #endregion

    #region 获取用量告警信息============

    /// <summary>
    /// 获取用量告警信息
    /// </summary>
    /// <param name="timetype">提醒时间跨度</param>
    /// <param name="tempid">模板ID</param>
    /// <param name="maxnum"></param>
    /// <param name="minnum"></param>
    /// <returns></returns>
    public string GetDosageMonitoringAlarm(string timetype, string tempid, string maxnum, string minnum)
    {
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&timetype=" + timetype + "&tempid=" + tempid + "&maxnum=" + maxnum + "&minnum=" + minnum + "&MchID=" + ConfigParam.mchid);
            string resultJson = WebGet(urlWxAPI + "api/Service/getDosageMonitoringAlarm", JsonStr.ToString());

            //resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            //if (resObjRoom["errmsg"].ToString() == "success")
            //{
            //    string resultsRoom = SerializeObject(resObjRoom["results"].ToString().Replace("[", "").Replace("]", "").Replace("\r\n", "").Replace(" ", ""));
            //    var dt = JSONToObject<DataTable>(resultsRoom);

            //    DataTbRoom = dt;// JsonConvert.DeserializeObject<DataTable>(resultsRoom.Replace("null", " "));
            //}
            //else
            //{
            //    DataTbRoom = null;
            //}

            return resultJson;
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    /// <summary>
    /// 获取欠费提醒信息
    /// </summary>
    /// <param name="timetype">提醒时间跨度</param>
    /// <param name="tempid">模板ID</param>
    /// <returns></returns>
    public string GetFeeinformationList(string timetype, string tempid)
    {
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&timetype=" + timetype + "&tempid=" + tempid + "&MchID=" + ConfigParam.mchid);
            return WebGet(urlWxAPI + "api/Service/ArrearagePushInfo", JsonStr.ToString());
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    /// <summary>
    /// 获取预付费低额提醒信息
    /// </summary>
    /// <param name="timetype">提醒时间跨度</param>
    /// <param name="tempid">模板ID</param>
    /// <param name="num">低额值例如 低于10元的用户</param>
    /// <returns></returns>
    public string GetPrepaidMonitoringAlarm(string timetype, string tempid, string num)
    {
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&timetype=" + timetype + "&tempid=" + tempid + "&num=" + num + "&MchID=" + ConfigParam.mchid);
            return WebGet(urlWxAPI + "api/Service/PrepaidPushInfo", JsonStr.ToString());
        }
        catch (Exception ex)
        {
            return "";
        }
    }

    /// <summary>
    /// 添加推送日志
    /// </summary>
    /// <param name="areaid"></param>
    /// <returns></returns>
    public bool GetPushAdd(string orgid, string nodeid, string liveid, string metid, string openid, string tempid, string typevalue, string maxnum, string value, string time, string remark, int type)
    {
        JObject resObjRoom = null;
        bool IsResultCode = false;
        StringBuilder JsonStr = new StringBuilder();
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&orgid=" + orgid + "&nodeid=" + nodeid + "&liveid=" + liveid + "&metid=" + metid + "&openid=" + openid + "&tempid=" + tempid + "&typevalue=" + typevalue + "&maxnum=" + maxnum + "&value=" + value + "&time=" + time + "&remark=" + remark + "&type=" + type + "&MchID=" + ConfigParam.mchid);
            string resultJson = WebGet(urlWxAPI + "api/Service/getPushAdd", JsonStr.ToString());

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }
    #endregion


    /// <summary>
    /// 获取支付方式
    /// </summary>
    /// <param name="custid"></param>
    /// <param name="areacode"></param>
    /// <returns></returns>
    public DataTable GetPaylist(string custid, string areacode)
    {
        JObject resObjRoom = null;
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&custid=" + custid + "&areacode=" + areacode + "&MchID=" + ConfigParam.mchid);

            string resultJson = WebGet(urlWxAPI + "api/Service/GetPaylist", JsonStr.ToString());

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                DataTbRoom = JsonConvert.DeserializeObject<DataTable>(resultsRoom.Replace("null", " "));
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }

    /// <summary>
    /// 保存订单
    /// </summary>
    /// <param name="mchid">商户微信编号</param>
    /// <param name="productid">商品编号</param>
    /// <param name="productbody">商品描述</param>
    /// <param name="mchorderid">商户订单号</param>
    /// <param name="mchdetail">商品详情</param>
    /// <param name="payorderid">支付订单号</param>
    /// <param name="payfeetotal">订单金额</param>
    /// <param name="paystatus">支付状态（0：预支付； 1：待支付；2：支付成功；3：支付失败；4：取消支付;5：退款成功；6退款失败；7交易结束，不可退款)</param>
    /// <param name="paytype">支付类型 1 手机支付  2 系统支付   3 三方设备扫码支付 </param>
    /// <param name="paysource">支付来源（PayType = 1 传入OPENID｜PayType = 2 传入操作员编号｜PayType = 3 传入设备标识，以区分那台设备支付的）</param>
    /// <param name="trade_type">交易类型 //JSAPI，NATIVE，APP</param>
    /// <param name="memo">附加数据</param>
    /// <param name="muid">Opid</param>
    /// <param name="FeeAmount">应收金额</param>
    /// <returns></returns>
    public bool Sav_PayOrder(string mchid, string productid, string productbody, string mchorderid, string mchdetail, string payorderid, string payfeetotal, string paystatus, string paytype, string paysource, string trade_type, string memo, string muid, string FeeAmount)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            //创建POST基础参数
            StringBuilder POSTsb = new StringBuilder();
            POSTsb.Append("{");
            POSTsb.Append("\"orgcode\":\"" + WxOrgCode + "\",");
            POSTsb.Append("\"timestamp\":\"" + timestamp + "\",");
            POSTsb.Append("\"appkey\":\"" + AppKey + "\",");
            //加入基本参数
            POSTsb.Append("\"mchid\":\"" + mchid + "\",");
            POSTsb.Append("\"productid\":\"" + productid + "\",");
            POSTsb.Append("\"productbody\":\"" + productbody + "\",");
            POSTsb.Append("\"mchorderid\":\"" + mchorderid + "\",");
            POSTsb.Append("\"mchdetail\":\"" + mchdetail + "\",");
            POSTsb.Append("\"payorderid\":\"" + payorderid + "\",");
            POSTsb.Append("\"payfeetotal\":\"" + payfeetotal + "\",");
            POSTsb.Append("\"paystatus\":\"" + paystatus + "\",");
            POSTsb.Append("\"paytype\":\"" + paytype + "\",");
            POSTsb.Append("\"paysource\":\"" + paysource + "\",");
            POSTsb.Append("\"trade_type\":\"" + trade_type + "\",");
            POSTsb.Append("\"memo\":\"" + memo + "\",");
            POSTsb.Append("\"muid\":\"" + muid + "\",");
            POSTsb.Append("\"FeeAmount\":\"" + FeeAmount + "\"");
            POSTsb.Append("}");

            log.Debug("api/Service/PayOrder Begin:" + POSTsb.ToString());
            string resultJson = WebPost(urlWxAPI + "api/Service/PayOrder", POSTsb.ToString());
            log.Debug("api/Service/PayOrder End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 缴费
    /// </summary>        
    /// <param name="LiveID">用户编号</param>
    /// <param name="NodeID">房屋编号</param>
    /// <param name="PayID">支付方式编号,微信4</param>
    /// <param name="CSign">收费方式 1：交费； 2：预收； -3：预收退费（插入负数）</param>
    /// <param name="OpCode">操作员</param>
    /// <param name="DueAmount">应收金额，用来判断费用是否一致</param>
    /// <param name="GetAmount">实收金额</param>
    /// <param name="Memo">备注</param>
    /// <returns></returns>
    public bool PayCharge(string LiveID, string NodeID, string PayID, string CSign, string OpCode, string DueAmount, string GetAmount, string Memo, out int reVal, out string reMsg)
    {
        string resultCode = "";
        JObject resObjRoom = null;
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            //创建POST基础参数
            StringBuilder POSTsb = new StringBuilder();
            POSTsb.Append("{");
            POSTsb.Append("\"orgcode\":\"" + WxOrgCode + "\",");
            POSTsb.Append("\"timestamp\":\"" + timestamp + "\",");
            POSTsb.Append("\"appkey\":\"" + AppKey + "\",");
            //加入基本参数
            POSTsb.Append("\"LiveID\":\"" + LiveID + "\",");
            POSTsb.Append("\"NodeID\":\"" + NodeID + "\",");
            POSTsb.Append("\"PayID\":\"" + PayID + "\",");
            POSTsb.Append("\"CSign\":\"" + CSign + "\",");
            POSTsb.Append("\"OpCode\":\"" + OpCode + "\",");
            POSTsb.Append("\"DueAmount\":\"" + DueAmount + "\",");
            POSTsb.Append("\"GetAmount\":\"" + GetAmount + "\",");
            POSTsb.Append("\"Memo\":\"" + Memo + "\",");
            POSTsb.Append("\"MchID\":\"" + ConfigParam.mchid + "\"");
            POSTsb.Append("}");

            log.Debug("api/Service/PayCharge Begin:" + POSTsb.ToString());
            string resultJson = WebPost(urlWxAPI + "api/Service/PayCharge", POSTsb.ToString());
            log.Debug("api/Service/PayCharge End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                reVal = 1;
                reMsg = resObjRoom["errmsg"].ToString();
                log.Debug("api/Service/PayCharge/reVal:" + reVal + "|reMsg:" + reMsg + "|Memo:" + Memo);
                return true;
            }
            else
            {
                reVal = -1;
                reMsg = resObjRoom["errmsg"].ToString();
                log.Debug("api/Service/PayCharge/reVal:" + reVal + "|reMsg:" + reMsg + "|Memo:" + Memo);
                return false;
            }
        }
        catch (Exception ex)
        {
            reVal = -1;
            reMsg = ex.Message;
            return false;
        }
    }

    /// <summary>
    /// 订单日志
    /// </summary> 
    /// <param name="orderserial">订单序号</param>
    /// <param name="detailtype">业务类型   1：请求二维码信息；2：三方回调；3：查询订单；4：退款；</param>
    /// <param name="reqtime">请求时间</param>
    /// <param name="reqcontent">请求内容</param>
    /// <param name="restime">响应时间</param>
    /// <param name="rescontent">响应内容</param>
    /// <param name="ordermoney">订单金额</param>
    /// <param name="paytype">支付类型  1 微信公众号支付  2 系统支付   3 三方设备扫码支付 4 手机APP</param>
    /// <param name="paysource">支付来源  /// 如果是由程序发起的退款，操作员和支付下单时的操作员可能不一致。顾需要单独存储支付来源  PayType = 1 传入OPENID PayType = 2 传入操作员编号  PayType = 3 传入设备标识，以区分那台设备支付的 PayType = 4 传入操作员编号</param>
    /// <returns></returns>
    public bool PayOrderDetail(string orderserial, string detailtype, string reqtime, string reqcontent, string restime, string rescontent, string ordermoney, string paytype, string paysource)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            //创建POST基础参数
            StringBuilder POSTsb = new StringBuilder();
            POSTsb.Append("{");
            POSTsb.Append("\"orgcode\":\"" + WxOrgCode + "\",");
            POSTsb.Append("\"timestamp\":\"" + timestamp + "\",");
            POSTsb.Append("\"appkey\":\"" + AppKey + "\",");
            //加入基本参数
            POSTsb.Append("\"orderserial\":\"" + orderserial + "\",");
            POSTsb.Append("\"detailtype\":\"" + detailtype + "\",");
            POSTsb.Append("\"reqtime\":\"" + reqtime + "\",");
            POSTsb.Append("\"reqcontent\":\"" + reqcontent + "\",");
            POSTsb.Append("\"restime\":\"" + restime + "\",");
            POSTsb.Append("\"rescontent\":\"" + rescontent + "\",");
            POSTsb.Append("\"ordermoney\":\"" + ordermoney + "\",");
            POSTsb.Append("\"paysource\":\"" + paysource + "\",");
            POSTsb.Append("\"paytype\":\"" + paytype + "\",");
            POSTsb.Append("\"MchID\":\"" + ConfigParam.mchid + "\"");
            POSTsb.Append("}");

            log.Debug("api/Service/PayOrderDetail Begin:" + POSTsb.ToString());
            string resultJson = WebPost(urlWxAPI + "api/Service/PayOrderDetail", POSTsb.ToString());
            log.Debug("api/Service/PayOrderDetail End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 订单与缴费关系表添加
    /// </summary> 
    /// <param name="MCHOrderID">商户订单号</param>
    /// <param name="PayFeeTotal">订单金额</param>
    /// <param name="PayType">支付类型</param>
    /// <param name="PayOrderID">支付订单号</param>
    /// <param name="TodayDate">下单时间</param>
    /// <param name="ChargeStatus">支付状态  -2：未收到缴费返回； -1：支付失败； 0:未支付；1：已支付成功；2:缴费成功 </param>
    /// <param name="ChargeSer">收费单号</param>
    /// <param name="ChargeTime">缴费时间</param>
    /// <returns></returns>
    public bool AddPayOrderChange(string MCHOrderID, string PayFeeTotal, string PayType, string PayOrderID, string TodayDate, string ChargeStatus, string ChargeSer, string ChargeTime)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            //创建POST基础参数
            StringBuilder POSTsb = new StringBuilder();
            POSTsb.Append("{");
            POSTsb.Append("\"orgcode\":\"" + WxOrgCode + "\",");
            POSTsb.Append("\"timestamp\":\"" + timestamp + "\",");
            POSTsb.Append("\"appkey\":\"" + AppKey + "\",");
            //加入基本参数
            POSTsb.Append("\"MCHOrderID\":\"" + MCHOrderID + "\",");
            POSTsb.Append("\"PayFeeTotal\":\"" + PayFeeTotal + "\",");
            POSTsb.Append("\"PayType\":\"" + PayType + "\",");
            POSTsb.Append("\"PayOrderID\":\"" + PayOrderID + "\",");
            POSTsb.Append("\"TodayDate\":\"" + TodayDate + "\",");
            POSTsb.Append("\"ChargeStatus\":\"" + ChargeStatus + "\",");
            POSTsb.Append("\"ChargeSer\":\"" + ChargeSer + "\",");
            POSTsb.Append("\"ChargeTime\":\"" + ChargeTime + "\",");
            POSTsb.Append("\"MchID\":\"" + ConfigParam.mchid + "\"");
            POSTsb.Append("}");

            string resultJson = WebPost(urlWxAPI + "api/Service/AddPayOrderChange", POSTsb.ToString());
            log.Debug("AddPayOrderChange:" + resultJson + "|" + POSTsb.ToString());
            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 订单与缴费关系表更新
    /// </summary> 
    /// <param name="MCHOrderID">商户订单号</param>
    /// <param name="PayOrderID">支付订单号</param>
    /// <param name="paystatus">支付状态  -2：未收到缴费返回； -1：支付失败； 0:未支付；1：已支付成功；2:缴费成功 </param>
    /// <param name="ChargeSer">收费单号</param>
    /// <returns></returns>
    public bool UpdPayOrderChange(string MCHOrderID, string PayOrderID, string ChargeStatus, string ChargeSer)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            //创建POST基础参数
            StringBuilder POSTsb = new StringBuilder();
            POSTsb.Append("{");
            POSTsb.Append("\"orgcode\":\"" + WxOrgCode + "\",");
            POSTsb.Append("\"timestamp\":\"" + timestamp + "\",");
            POSTsb.Append("\"appkey\":\"" + AppKey + "\",");
            //加入基本参数
            POSTsb.Append("\"MCHOrderID\":\"" + MCHOrderID + "\",");
            POSTsb.Append("\"PayOrderID\":\"" + PayOrderID + "\",");
            POSTsb.Append("\"ChargeStatus\":\"" + ChargeStatus + "\",");
            POSTsb.Append("\"ChargeSer\":\"" + ChargeSer + "\",");
            POSTsb.Append("\"MchID\":\"" + ConfigParam.mchid + "\"");
            POSTsb.Append("}");

            string resultJson = WebPost(urlWxAPI + "api/Service/UpdPayOrderChange", POSTsb.ToString());
            log.Debug("UpdPayOrderChange:" + resultJson);
            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 缴费后更新状态（PAYORDER，PayOrderCharge，PAYORDERDETAIL）
    /// </summary> 
    /// <param name="MCHOrderID">商户订单号</param>
    /// <param name="PayOrderID">WX支付订单号</param>
    /// <param name="ChargeStatus">支付状态  -2：未收到缴费返回； -1：支付失败； 0:未支付；1：已支付成功；2:缴费成功 </param>
    /// <param name="ChargeSer">收费单号</param>
    /// <param name="reqcontent">收费单号</param>
    /// <param name="paystatus">收费单号</param>
    /// <param name="resultString">收费单号</param>
    /// <returns></returns>
    public bool UpdPayOrderChangeDetail(string MCHOrderID, string PayOrderID, string ChargeStatus, string ChargeSer, string reqcontent, string paystatus, string rescontent, string orderAmount)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            //创建POST基础参数
            StringBuilder POSTsb = new StringBuilder();
            POSTsb.Append("{");
            POSTsb.Append("\"orgcode\":\"" + WxOrgCode + "\",");
            POSTsb.Append("\"timestamp\":\"" + timestamp + "\",");
            POSTsb.Append("\"appkey\":\"" + AppKey + "\",");
            //加入基本参数
            POSTsb.Append("\"MCHOrderID\":\"" + MCHOrderID + "\",");
            POSTsb.Append("\"PayOrderID\":\"" + PayOrderID + "\",");
            POSTsb.Append("\"ChargeStatus\":\"" + ChargeStatus + "\",");
            POSTsb.Append("\"ChargeSer\":\"" + ChargeSer + "\",");
            POSTsb.Append("\"reqcontent\":\"" + reqcontent + "\",");
            POSTsb.Append("\"paystatus\":\"" + paystatus + "\",");
            POSTsb.Append("\"orderAmount\":\"" + orderAmount + "\",");
            POSTsb.Append("\"rescontent\":\"" + rescontent + "\",");
            POSTsb.Append("\"MchID\":\"" + ConfigParam.mchid + "\"");
            POSTsb.Append("}");

            string resultJson = WebPost(urlWxAPI + "api/Service/UpdPayOrderChangeDetail", POSTsb.ToString());
            log.Debug("UpdPayOrderChange:" + resultJson);
            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 设置订单状态
    /// </summary> 
    /// <param name="payorderid">支付订单号</param>
    /// <param name="paystatus">支付状态（0：预支付； 1：待支付；2：支付成功；3：支付失败；4：取消支付;5：退款成功；6退款失败；7交易结束，不可退款)</param>
    /// <returns></returns>
    public bool SetOrderStatus(string payorderid, string paystatus)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            //创建POST基础参数
            StringBuilder POSTsb = new StringBuilder();
            POSTsb.Append("{");
            POSTsb.Append("\"orgcode\":\"" + WxOrgCode + "\",");
            POSTsb.Append("\"timestamp\":\"" + timestamp + "\",");
            POSTsb.Append("\"appkey\":\"" + AppKey + "\",");
            //加入基本参数
            POSTsb.Append("\"payorderid\":\"" + payorderid + "\",");
            POSTsb.Append("\"paystatus\":\"" + paystatus + "\",");
            POSTsb.Append("\"MchID\":\"" + ConfigParam.mchid + "\"");
            POSTsb.Append("}");

            string resultJson = WebPost(urlWxAPI + "api/Service/SetOrderStatus", POSTsb.ToString());
            log.Debug("SetOrderStatus:" + resultJson);
            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 设置订单状态
    /// </summary> 
    /// <param name="NodeID">支付订单号</param>
    /// <returns></returns>
    public bool IsWeChatStatus(string NodeID)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            //创建POST基础参数
            StringBuilder POSTsb = new StringBuilder();
            POSTsb.Append("{");
            POSTsb.Append("\"orgcode\":\"" + WxOrgCode + "\",");
            POSTsb.Append("\"timestamp\":\"" + timestamp + "\",");
            POSTsb.Append("\"appkey\":\"" + AppKey + "\",");
            //加入基本参数
            POSTsb.Append("\"nodeid\":\"" + NodeID + "\",");
            POSTsb.Append("\"MchID\":\"" + ConfigParam.mchid + "\"");
            POSTsb.Append("}");

            string resultJson = WebPost(urlWxAPI + "api/Service/IsWeChatStatus", POSTsb.ToString());
            log.Debug("IsWeChatStatus:" + resultJson);
            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 根据liveid查询 feeser
    /// </summary> 
    /// <param name="liveid">liveid</param>
    /// <returns></returns>
    public string GetFeeSers(string liveid)
    {
        string resultCode = "";
        JObject resObjRoom = null;
        StringBuilder JsonStr = new StringBuilder();
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&liveid=" + liveid + "&MchID=" + ConfigParam.mchid);

            log.Debug("api/Service/GetFeeSers Begin:" + JsonStr.ToString());
            string resultJson = WebGet(urlWxAPI + "api/Service/GetFeeSers", JsonStr.ToString());
            log.Debug("api/Service/GetFeeSers End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errmsg"].ToString() == "success")
            {
                resultCode = SerializeObject(resObjRoom["results"]);

                if (resultCode == "\"\"")
                {
                    resultCode = "";
                }
            }

            return resultCode;
        }
        catch
        {
            return "";
        }
        finally
        {
            JsonStr.Clear();
        }
    }

    /// <summary>
    /// 获取到对账单添加到主库对账表中
    /// </summary>
    /// <param name="resultString">对账字符串</param>
    /// <returns></returns>
    public string CheckFeeAmount(string liveid, string feeAmt)
    {
        string resultCode = "";
        JObject resObjRoom = null;
        StringBuilder JsonStr = new StringBuilder();
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&liveid=" + liveid + "&feeAmt=" + feeAmt + "&MchID=" + ConfigParam.mchid);

            log.Debug("api/Service/CheckFeeAmount Begin:" + JsonStr.ToString());
            string resultJson = WebGet(urlWxAPI + "api/Service/CheckFeeAmount", JsonStr.ToString());
            log.Debug("api/Service/CheckFeeAmount End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                resultCode = "";
            }
            else
            {
                resultCode = "异常";
            }

            return resultCode;
        }
        catch
        {
            return "异常";
        }
        finally
        {
            JsonStr.Clear();
        }
    }
    /// <summary>
    /// 根据订单号查询订单应收金额与实收金额
    /// </summary> 
    /// <param name="payorderid">支付订单号</param>
    /// <returns></returns>
    public DataTable GetOrderAmount(string payorderid)
    {
        JObject resObjRoom = null;
        DataTable DataTbRoom = new DataTable();
        StringBuilder JsonStr = new StringBuilder();
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&payorderid=" + payorderid + "&MchID=" + ConfigParam.mchid);

            log.Debug("api/Service/GetOrderAmount Begin:" + JsonStr.ToString());
            string resultJson = WebGet(urlWxAPI + "api/Service/GetOrderAmount", JsonStr.ToString());
            log.Debug("api/Service/GetOrderAmount End:" + resultJson);

            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                string resultsRoom = SerializeObject(resObjRoom["results"]);
                DataTbRoom = JsonConvert.DeserializeObject<DataTable>(resultsRoom);
            }
            else
            {
                DataTbRoom = null;
            }

            return DataTbRoom;
        }
        catch
        {
            return null;
        }
        finally
        {
            JsonStr.Clear();
        }
    }

    /// <summary>
    /// 获取到对账单添加到主库对账表中
    /// </summary>
    /// <param name="resultString">对账字符串</param>
    /// <returns></returns>
    public bool AddBill(string resultString)
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        StringBuilder JsonStr = new StringBuilder();
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            //创建POST基础参数
            StringBuilder POSTsb = new StringBuilder();
            POSTsb.Append("{");
            POSTsb.Append("\"orgcode\":\"" + WxOrgCode + "\",");
            POSTsb.Append("\"timestamp\":\"" + timestamp + "\",");
            POSTsb.Append("\"appkey\":\"" + AppKey + "\",");
            //加入基本参数
            POSTsb.Append("\"result\":\"" + resultString + "\",");
            POSTsb.Append("\"MchID\":\"" + ConfigParam.mchid + "\"");
            POSTsb.Append("}");

            string resultJson = WebPost(urlWxAPI + "api/Service/AddBill", POSTsb.ToString());
            log.Debug("AddBill:" + resultJson);
            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 获取未对账的订单，自动执行缴费
    /// </summary>
    /// <param name="orgcode">机构ID</param>
    /// <returns></returns>
    public bool GetBillingCharge()
    {
        bool IsResultCode = false;
        JObject resObjRoom = null;
        StringBuilder JsonStr = new StringBuilder();

        try
        {
            //签名
            AppKey = GetMd5Key(WxOrgCode, WxAppKey, timestamp);
            JsonStr.Append("orgcode=" + WxOrgCode + "&timestamp=" + timestamp + "&appkey=" + AppKey + "&MchID=" + ConfigParam.mchid);

            string resultJson = WebGet(urlWxAPI + "api/Service/GetBillingCharge", JsonStr.ToString());
            // log.Debug("GetBillingCharge:" + resultJson);
            resObjRoom = (JObject)JsonConvert.DeserializeObject(resultJson);
            if (resObjRoom["errcode"].ToString() == "0")
            {
                IsResultCode = true;
            }

            return IsResultCode;
        }
        catch
        {
            return false;
        }
    }
    #endregion



    #region post/写日志方法=====================
    /// <summary>
    /// get方法
    /// </summary>
    /// <param name="p_url"></param>
    /// <param name="p_data"></param>
    /// <returns></returns>
    public string WebGet(string path, string p_data)
    {
        ILog log = LogManager.GetLogger(this.GetType());
        try
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            //数据较大的参数
            string datastr = "?" + p_data;
            //参数转流
            byte[] bytearray = Encoding.UTF8.GetBytes(datastr);
            //采取POST方式必须加的header，如果改为GET方式的话就去掉这句话即可
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded");//长度
            client.Headers.Add("ContentLength", bytearray.Length.ToString());
            client.Headers.Add("access_token", "Wx_Flag_Api_Filter_AllowAnonymous");
            //释放
            client.Dispose();
            //处理返回数据（一般用json）
            log.Debug("WebGet:" + path + datastr);
            return client.DownloadString(path + datastr);
        }
        catch (Exception ex)
        {
            log.Debug("WebGet:" + ex.Message);
            return string.Empty;
        }
    }

    /// <summary>
    /// post方法
    /// </summary>
    /// <param name="p_url"></param>
    /// <param name="p_data"></param>
    /// <returns></returns>
    public string WebPost(string p_url, string p_data)
    {
        try
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            //NameValueCollection list = new NameValueCollection();
            //list.Add("id", "11111");
            //list.Add("name", "lee");
            byte[] bytearray = Encoding.UTF8.GetBytes(p_data);
            client.Headers.Add(HttpRequestHeader.Accept, "json");
            client.Headers.Add("Content-Type", "application/json");
            client.Headers.Add("ContentLength", bytearray.Length.ToString());
            client.Headers.Add("access_token", "Wx_Flag_Api_Filter_AllowAnonymous");
            byte[] Ret_Data = client.UploadData(p_url, "POST", bytearray);
            return Encoding.GetEncoding("UTF-8").GetString(Ret_Data);
        }
        catch (Exception ex)
        {
            return string.Empty;
        }
    }

    public static string HttpPost(string url, string param)
    {
        string strURL = url;
        System.Net.HttpWebRequest request;
        request = (System.Net.HttpWebRequest)WebRequest.Create(strURL);
        request.Method = "POST";
        request.Headers.Add("access_token", "Wx_Flag_Api_Filter_AllowAnonymous");
        request.ContentType = "application/json;charset=UTF-8";
        string paraUrlCoded = param;
        byte[] payload;
        payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
        request.ContentLength = payload.Length;
        Stream writer = request.GetRequestStream();
        writer.Write(payload, 0, payload.Length);
        writer.Close();
        System.Net.HttpWebResponse response;
        response = (System.Net.HttpWebResponse)request.GetResponse();
        System.IO.Stream s;
        s = response.GetResponseStream();
        string StrDate = "";
        string strValue = "";
        StreamReader Reader = new StreamReader(s, Encoding.UTF8);
        while ((StrDate = Reader.ReadLine()) != null)
        {
            strValue += StrDate + "\r\n";
        }
        return strValue;
    }
    #endregion

    /// <summary> 
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public static string GetTimeStamp(System.DateTime time)
    {
        long ts = ConvertDateTimeToInt(time);
        return ts.ToString();
    }
    /// <summary>  
    /// 将c# DateTime时间格式转换为Unix时间戳格式  
    /// </summary>  
    /// <param name="time">时间</param>  
    /// <returns>long</returns>  
    public static long ConvertDateTimeToInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
        long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
        return t;
    }

    private static string GetMd5Key(string code, string key, string timestamp)
    {
        var hash = System.Security.Cryptography.MD5.Create();
        var signstr = code + key + timestamp;
        var bytes = Encoding.UTF8.GetBytes(signstr);

        var MD5str = hash.ComputeHash(bytes);

        StringBuilder sb = new StringBuilder();
        foreach (var c in MD5str)
        {
            sb.Append(c.ToString("X2"));
        }

        return sb.ToString();
    }

    /// 将一个对象序列化成一个JSON字符串
    public static string SerializeObject(object obj)
    {
        Newtonsoft.Json.Converters.IsoDateTimeConverter timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
        //这里使用自定义日期格式，如果不使用的话，默认是ISO8601格式
        timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
        string serialStr = JsonConvert.SerializeObject(obj, Formatting.None, timeConverter);
        return serialStr;
    }

    /// <summary>
    /// 根据日期和随机码生成订单号
    /// </summary>
    /// <returns></returns>
    public static string GetOrderNumber()
    {
        string num = DateTime.Now.ToString("yyyyMMddHHmmss");//yyyyMMddHHmmssms
        return num + Number(3, true).ToString();
    }
    /// <summary>
    /// 生成随机数字
    /// </summary>
    /// <param name="Length">生成长度</param>
    /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
    /// <returns></returns>
    public static string Number(int Length, bool Sleep)
    {
        if (Sleep)
            System.Threading.Thread.Sleep(3);
        string result = "";
        System.Random random = new Random();
        for (int i = 0; i < Length; i++)
        {
            result += random.Next(10).ToString();
        }
        return result;
    }


    public string SendTempletMessge(string OpenID)
    {
        string strReturn = string.Empty;
        try
        {
            #region 获取access_token
            string apiurl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid=你的appid&secret=你的secret";
            WebRequest request = WebRequest.Create(@apiurl);
            request.Method = "POST";
            WebResponse response = request.GetResponse();
            Stream stream = response.GetResponseStream();
            Encoding encode = Encoding.UTF8;
            StreamReader reader = new StreamReader(stream, encode);
            string detail = reader.ReadToEnd();
            var jd = JsonConvert.DeserializeObject<WXApi>(detail);
            string token = (String)jd.access_token;
            #endregion
            #region 组装信息推送，并返回结果（其它模版消息于此类似）
            string url = "https://api.weixin.qq.com/cgi-bin/message/template/send?access_token=" + token;
            string temp = "{\"touser\": \"" + OpenID + "\"," +
                   "\"template_id\": \"f3kRRjJeyLDf4tndtg-OJeRvgEdgjjDxCy4T9kuwM70\", " +
                   "\"topcolor\": \"#FF0000\", " +
                   "\"data\": " +
                   "{\"first\": {\"value\": \"您好，您有一条缴费通知信息\"}," +
                   "\"keyword1\": { \"value\": \"单位名称\"}," +
                   "\"keyword2\": { \"value\": \"日期\"}," +
                   "\"keyword3\": { \"value\": \"金额\"}," +
                   "\"keyword4\": { \"value\": \"业务员\"}," +
                   "\"remark\": {\"value\": \"\" }}}";
            #endregion
            //核心代码
            GetResponseData(temp, @url);
            strReturn = "推送成功";
        }
        catch (Exception ex)
        {
            strReturn = ex.Message;
        }
        return strReturn;
    }
    /// <summary>
    /// 返回JSon数据
    /// </summary>
    /// <param name="JSONData">要处理的JSON数据</param>
    /// <param name="Url">要提交的URL</param>
    /// <returns>返回的JSON处理字符串</returns>
    public string GetResponseData(string JSONData, string Url)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(JSONData);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        request.Method = "POST";
        request.ContentLength = bytes.Length;
        request.ContentType = "json";
        Stream reqstream = request.GetRequestStream();
        reqstream.Write(bytes, 0, bytes.Length);
        //声明一个HttpWebRequest请求
        request.Timeout = 90000;
        //设置连接超时时间
        request.Headers.Set("Pragma", "no-cache");
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream streamReceive = response.GetResponseStream();
        Encoding encoding = Encoding.UTF8;
        StreamReader streamReader = new StreamReader(streamReceive, encoding);
        string strResult = streamReader.ReadToEnd();
        streamReceive.Dispose();
        streamReader.Dispose();
        return strResult;
    }


    #region 属性部分
    private string muid;
    /// <summary>
    /// 用户号
    /// </summary>
    public string MuID
    {
        get
        {
            return muid;
        }
        set
        {
            muid = value;
        }
    }
    private string addressid;
    /// <summary>
    /// 用户房间地址
    /// </summary>
    public string AddressID
    {
        get
        {
            return addressid;
        }
        set
        {
            addressid = value;
        }
    }

    /// <summary>
    /// 用户房间数量
    /// </summary>
    private string addressidcount;
    /// <summary>
    /// 用户房间地址
    /// </summary>
    public string AddressIDCount
    {
        get
        {
            return addressidcount;
        }
        set
        {
            addressidcount = value;
        }
    }
    private string liveid;
    /// <summary>
    /// 用户房间地址
    /// </summary>
    public string LiveID
    {
        get
        {
            return liveid;
        }
        set
        {
            liveid = value;
        }
    }

    private string custid;
    /// <summary>
    /// 用户房间区域
    /// </summary>
    public string CustID
    {
        get
        {
            return custid;
        }
        set
        {
            custid = value;
        }
    }
    private string username;
    /// <summary>
    /// 用户名称
    /// </summary>
    public string UserName
    {
        get
        {
            return username;
        }
        set
        {
            username = value;
        }
    }



    private string areacode;
    /// <summary>
    /// 小区编号
    /// </summary>
    public string AreaCode
    {
        get
        {
            return areacode;
        }
        set
        {
            areacode = value;
        }
    }

    private string ispayUnit;
    /// <summary>
    /// 是否代缴
    /// </summary>
    public string ISPayUnit
    {
        get
        {
            return ispayUnit;
        }
        set
        {
            ispayUnit = value;
        }
    }

    private string iswechat;
    /// <summary>
    /// 是否允许微信缴费
    /// </summary>
    public string ISWeChat
    {
        get
        {
            return iswechat;
        }
        set
        {
            iswechat = value;
        }
    }

    private string identcode;
    /// <summary>
    /// 商用系统：用户识别码
    /// </summary>
    public string IdentCode
    {
        get
        {
            return identcode;
        }
        set
        {
            identcode = value;
        }
    }

    private string addressname;
    /// <summary>
    /// 商用系统：用户房间地址
    /// </summary>
    public string AddressName
    {
        get
        {
            return addressname;
        }
        set
        {
            addressname = value;
        }
    }
    private string estateid;
    /// <summary>
    /// 当前小区ID
    /// </summary>
    public string EstateID
    {
        get
        {
            return estateid;
        }
        set
        {
            estateid = value;
        }
    }

    private string estatename;
    /// <summary>
    /// 当前小区名称
    /// </summary>
    public string EstateName
    {
        get
        {
            return estatename;
        }
        set
        {
            estatename = value;
        }
    }

    private string isuploadphoto;
    /// <summary>
    /// 当前小区名称
    /// </summary>
    public string IsUploadPhoto
    {
        get
        {
            return isuploadphoto;
        }
        set
        {
            isuploadphoto = value;
        }
    }


    private string shopid;
    /// <summary>
    /// 商用系统：用户识别码
    /// </summary>
    public string ShopID
    {
        get
        {
            return shopid;
        }
        set
        {
            shopid = value;
        }
    }

    /// <summary>
    /// 三方接口地址
    /// </summary>
    private string ifurl;
    public string IFUrl
    {
        get
        {
            return ifurl;
        }
        set
        {
            ifurl = value;
        }
    }

    /// <summary>
    /// 第三方接口服务参数编号
    /// </summary>
    private string thspid;
    public string ThspID
    {
        get
        {
            return thspid;
        }
        set
        {
            thspid = value;
        }
    }




    #endregion

    #region 报修相关

    /// <summary>
    /// 报修ID
    /// </summary>
    private string repairid;
    public string RepairID
    {
        get
        {
            return repairid;
        }
        set
        {
            repairid = value;
        }
    }

    /// <summary>
    /// 报修时间
    /// </summary>
    private string createdt;
    public string CreateDT
    {
        get
        {
            return createdt;
        }
        set
        {
            createdt = value;
        }
    }
    /// <summary>
    /// 报修状态
    /// </summary>
    private string status;
    public string Status
    {
        get
        {
            return status;
        }
        set
        {
            status = value;
        }
    }
    /// <summary>
    /// 报修状态名称
    /// </summary>
    private string statusname;
    public string StatusName
    {
        get
        {
            return statusname;
        }
        set
        {
            statusname = value;
        }
    }

    /// <summary>
    /// 报修标题
    /// </summary>
    private string title;
    public string Title
    {
        get
        {
            return title;
        }
        set
        {
            title = value;
        }
    }

    /// <summary>
    /// 报修地址
    /// </summary>
    private string address;
    public string Address
    {
        get
        {
            return address;
        }
        set
        {
            address = value;
        }
    }

    /// <summary>
    /// 问题描述
    /// </summary>
    private string description;
    public string Description
    {
        get
        {
            return description;
        }
        set
        {
            description = value;
        }
    }
    /// <summary>
    /// 报修照片数据流
    /// </summary>
    private string imagesrepair;
    public string Imagesrepair
    {
        get
        {
            return imagesrepair;
        }
        set
        {
            imagesrepair = value;
        }
    }
    /// <summary>
    /// 报修照片张数
    /// </summary>
    private int imagesrepaircout;
    public int Imagesrepaircout
    {
        get
        {
            return imagesrepaircout;
        }
        set
        {
            imagesrepaircout = value;
        }
    }
    /// <summary>
    /// 报修照片1
    /// </summary>
    private string imagesrepair1;
    public string Imagesrepair1
    {
        get
        {
            return imagesrepair1;
        }
        set
        {
            imagesrepair1 = value;
        }
    }
    /// <summary>
    /// 报修照片2
    /// </summary>
    private string imagesrepair2;
    public string Imagesrepair2
    {
        get
        {
            return imagesrepair2;
        }
        set
        {
            imagesrepair2 = value;
        }
    }
    /// <summary>
    /// 报修照片3
    /// </summary>
    private string imagesrepair3;
    public string Imagesrepair3
    {
        get
        {
            return imagesrepair3;
        }
        set
        {
            imagesrepair3 = value;
        }
    }
    /// <summary>
    /// 联系人
    /// </summary>
    private string person;
    public string Person
    {
        get
        {
            return person;
        }
        set
        {
            person = value;
        }
    }
    /// <summary>
    /// 联系电话
    /// </summary>
    private string tel;
    public string Tel
    {
        get
        {
            return tel;
        }
        set
        {
            tel = value;
        }
    }

    /// <summary>
    /// 小区联系电话
    /// </summary>
    private string chargetel;
    public string ChargeTel
    {
        get
        {
            return chargetel;
        }
        set
        {
            chargetel = value;
        }
    }

    /// <summary>
    /// 满意度名称
    /// </summary>
    private string satitypename;
    public string satiTypeName
    {
        get
        {
            return satitypename;
        }
        set
        {
            satitypename = value;
        }
    }
    /// <summary>
    /// 评价
    /// </summary>
    private string evaluate;
    public string Evaluate
    {
        get
        {
            return evaluate;
        }
        set
        {
            evaluate = value;
        }
    }
    /// <summary>
    /// 评价等级
    /// </summary>
    private string satitype;
    public string SatiType
    {
        get
        {
            return satitype;
        }
        set
        {
            satitype = value;
        }
    }

    /// <summary>
    /// 处理进度
    /// </summary>
    private DataTable content;
    public DataTable Content
    {
        get
        {
            return content;
        }
        set
        {
            content = value;
        }
    }
    /// <summary>
    /// 评价等级
    /// </summary>
    private string cityname;
    public string CityName
    {
        get
        {
            return cityname;
        }
        set
        {
            cityname = value;
        }
    }
    /// <summary>
    /// 评价等级
    /// </summary>
    private string cityid;
    public string CityID
    {
        get
        {
            return cityid;
        }
        set
        {
            cityid = value;
        }
    }
    /// <summary>
    /// 处理进度
    /// </summary>
    private DataTable estatelist;
    public DataTable EstateList
    {
        get
        {
            return estatelist;
        }
        set
        {
            estatelist = value;
        }
    }
    #endregion

    #region 留言相关

    /// <summary>
    /// 留言内容
    /// </summary>
    private string lycontent;
    public string LyContent
    {
        get
        {
            return lycontent;
        }
        set
        {
            lycontent = value;
        }
    }
    /// <summary>
    /// 留言时间
    /// </summary>
    private string lydttime;
    public string LyDTTime
    {
        get
        {
            return lydttime;
        }
        set
        {
            lydttime = value;
        }
    }
    /// <summary>
    /// 回复内容
    /// </summary>
    private string lyanswcontent;
    public string LyAnswContent
    {
        get
        {
            return lyanswcontent;
        }
        set
        {
            lyanswcontent = value;
        }
    }
    /// <summary>
    /// 回复时间
    /// </summary>
    private string lyanswtime;
    public string LyAnswTime
    {
        get
        {
            return lyanswtime;
        }
        set
        {
            lyanswtime = value;
        }
    }
    #endregion

    #region 新闻公告
    /// <summary>
    ///公告TypeID
    /// </summary>
    private string noticetypeid;
    public string NoticeTypeID
    {
        get
        {
            return noticetypeid;
        }
        set
        {
            noticetypeid = value;
        }
    }
    /// <summary>
    /// <summary>
    ///公告ID
    /// </summary>
    private string noticeid;
    public string NoticeID
    {
        get
        {
            return noticeid;
        }
        set
        {
            noticeid = value;
        }
    }
    /// <summary>
    ///公告title
    /// </summary>
    private string noticetitle;
    public string NoticeTitle
    {
        get
        {
            return noticetitle;
        }
        set
        {
            noticetitle = value;
        }
    }
    /// <summary>
    ///公告内容
    /// </summary>
    private string noticecontent;
    public string NoticeContent
    {
        get
        {
            return noticecontent;
        }
        set
        {
            noticecontent = value;
        }
    }

    /// <summary>
    /// 公告时间
    /// </summary>
    private string noticetime;
    public string NoticeTime
    {
        get
        {
            return noticetime;
        }
        set
        {
            noticetime = value;
        }
    }

    /// <summary>
    /// index页面周边商业ICO
    /// </summary>
    private string ico;
    public string Ico
    {
        get
        {
            return ico;
        }
        set
        {
            ico = value;
        }
    }
    #endregion

    public class WXApi
    {
        public string access_token { set; get; }
    }
}
