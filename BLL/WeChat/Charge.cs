using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.WeChat
{
    public class Charge
    {
        private DAL.WeChat.Charge dal = null;
        private string connStr = string.Empty;
        public Charge(string _connStr)
        {
            connStr = _connStr;
            dal = new DAL.WeChat.Charge(connStr);
        }

        public bool PayCharge(Model.WeChat.Charge chargeDto, out string errMsg,out Model.WeChat.Card card)
        {
            int ret = 0;
            string chrgID = "";
            errMsg = string.Empty;
            card = new Model.WeChat.Card();
            try
            {
                ret = dal.ChargeFee(chargeDto, ref chrgID);
                if (ret == 1)
                {
                    Model.WeChat.CardIdent mdlCardIdent = dal.GetChargeCardInfo(chrgID);
                    if (!string.IsNullOrEmpty(mdlCardIdent.CardID)) //判断本次收费有无卡表
                    {
                        card = dal.GetCardStr(2, chargeDto.NodeID, mdlCardIdent, "", 0, chrgID);
                    }
                    else
                    {
                        errMsg = "收费成功！";
                    }
                }
                else
                {
                    if (ret == -1 || ret == -2)
                    {
                        errMsg = "欠费金额发生变化，收费失败！";
                    }
                    else if (ret == -3)
                    {
                        errMsg = "基本账户金额发生变化，收费失败！";
                    }
                    else if (ret == -4)
                    {
                        errMsg = "项目账户金额发生变化，收费失败！";
                    }
                    else
                    {
                        errMsg = "其它原因导致收费失败！(" + ret.ToString() + ")";
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = "收费异常！" + ex.Message;
            }

            if (ret != 1) //收费失败，存余额
            {
                ret = dal.ChargeBalance(chargeDto, ref chrgID);
                if (ret == 1)
                {
                    errMsg = "收费失败后充入余额成功！";
                }
                else
                {
                    errMsg = "收费失败后充入余额失败！";
                }
            }
            return ret == 1;
        }
    }
}
