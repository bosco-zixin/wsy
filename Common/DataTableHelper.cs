using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace WSY.Common
{
    public static class DataTableHelper
    {
        /// <summary>
        /// 判断数据列是否为空
        /// </summary>
        /// <param name="curr"></param>
        /// <param name="dFiled"></param>
        /// <returns></returns>
        public static bool IsNull(DataRow curr, string dFiled)
        {
            bool bRet = curr[dFiled] == null || curr[dFiled] == DBNull.Value;
            return bRet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtResult"></param>
        /// <returns></returns>
        public static List<string> GetDateTimeColName(ref DataTable dtResult)
        {
            List<string> lsCol = new List<string>();
            foreach (DataColumn col in dtResult.Columns)
            {
                if (col.DataType == typeof(System.DateTime)
                    && !lsCol.Contains(col.ColumnName))
                {
                    lsCol.Add(col.ColumnName);
                }
                //修改列类型
                col.DataType = typeof(string);
            }
            return lsCol;
        }

        /// <summary>
        /// DataRow转换为DataTable
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="strWhere">筛选的条件</param>
        /// <returns></returns>
        public static DataTable DataRowToDataTable(DataTable dt, string strWhere)
        {
            DataTable dtNew = dt.Clone();//复制数据源的表结构
            if (dt.Rows.Count <= 0)
            {
                return dtNew; //当数据为空时返回
            }

            DataRow[] dr = dt.Select(strWhere);  //strWhere条件筛选出需要的数据！
            for (int i = 0; i < dr.Length; i++)
            {
                dtNew.Rows.Add(dr[i].ItemArray);  // 将DataRow添加到DataTable中
            }
            return dtNew;
        }
        /// <summary>
        /// 将DataTable按照参数排序
        /// </summary>
        /// <param name="tmpDt">待排序DataTable</param>
        /// <param name="orderFileds">排序字段数组</param>
        /// <returns>按照排序参数排序后的DataTable</returns>
        public static DataTable LinqSortDataTable(DataTable tmpDt, string[] orderFileds)
        {
            DataTable dtsort = tmpDt.Clone();
            if (orderFileds.Length > 0)
            {
                string orderExpression = orderFileds.Where(s => !string.IsNullOrEmpty(s)).Aggregate(string.Empty, (current, s) => current + string.Format("{0},", s));
                orderExpression = orderExpression.Substring(0, orderExpression.Length - 1);
                tmpDt.DefaultView.Sort = " " + orderExpression + " asc ";
                dtsort = tmpDt.DefaultView.ToTable();
            }
            return dtsort;
        }


        /// <summary>
        /// 行转列
        /// </summary>
        /// <param name="dataTable">待行转列的源数据</param>
        /// <param name="groupingColumn">待行转列的字段</param>
        /// <param name="nameColumn">名称列</param>
        /// <param name="key">唯一标识</param>
        /// <param name="dataColumn">数据列</param>
        /// <returns></returns>
        public static DataTable RowsToCol(DataTable dataTable, string groupingColumn, string[] nameColumn, string key, string[] dataColumn)
        {
            //取出不重复的需要转换的列的数据
            DataTable distinct_GroupingColumn = dataTable.DefaultView.ToTable(true, groupingColumn);
            //取出不重复的名称列
            DataTable dictinct_NameColumn = dataTable.DefaultView.ToTable(true, nameColumn);
            //构建新表
            DataTable table = new DataTable();
            #region 构建新表的列                        
            //将名称列添加进新表
            DataColumn newColumn = null;
            foreach (string colName in nameColumn)
            {
                newColumn = new DataColumn();
                newColumn.ColumnName = colName;
                //添加列   所以要.Columns
                table.Columns.Add(newColumn);
            }
            //添加需要转换的列            
            foreach (DataRow dr in distinct_GroupingColumn.Rows)
            {
                foreach (string s in dataColumn)
                {
                    newColumn = new DataColumn();
                    newColumn.ColumnName = s + "_" + dr[groupingColumn].ToString();
                    table.Columns.Add(newColumn);
                }
            }
            #endregion
            #region 向新表中添加数据
            DataRow newRow = null;
            DataRow[] dnewRow = null;
            foreach (DataRow item in dictinct_NameColumn.Rows)
            {
                try
                {
                    //添加一个新行
                    newRow = table.NewRow();
                    //为此新行添加第一个行数据
                    newRow.ItemArray = item.ItemArray;
                    //为此新行添加列数据
                    foreach (DataRow r in distinct_GroupingColumn.Rows)
                    {
                        if (!string.IsNullOrEmpty(item[key].ToString()))
                        {
                            dnewRow = dataTable.Select(key + " ='" + item[key].ToString() + "' and " + groupingColumn + " ='" + r[groupingColumn].ToString() + "'");
                            if (dnewRow.Length != 0)
                            {
                                //将数据与新建表进行连合
                                foreach (string s in dataColumn)
                                {
                                    newRow[s + "_" + r[groupingColumn].ToString()] = dnewRow[0][s];
                                }
                            }
                        }
                    }
                    table.Rows.Add(newRow);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            #endregion

            return table;
        }
    }
}
