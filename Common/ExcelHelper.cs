using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace WSY.Common
{
    /// <summary>
    /// 公共接口
    /// </summary>
    /// <returns></returns>
    public class ExcelHelper
    {
        /// <summary>
        /// 导出真正的电子表格
        /// </summary>
        /// <param name="ds">数据源</param>
        /// <param name="heads">表头设置列表</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="downLoad">是否从网络下载 true 下载，false 不下载</param>
        /// <param name="savePath">保存到本地路径(不含文件名称)，savePath为空不保存本地文件</param>
        /// <remarks>
        /// 创建：王春宝 2020-08-24 
        /// </remarks>
        public static dynamic ExporXSSFExcel(DataTable dt, List<head> heads, string fileName, bool downLoad = true, string savePath = "")
        {
            //创建 电子表格文件
            XSSFWorkbook book = new XSSFWorkbook();
            try
            {
                CreateSheet(book, dt, heads);

                //保存文件
                if (!string.IsNullOrEmpty(savePath))
                {
                    SaveExcel(book, fileName, savePath);
                }
                // 服务端下载文件
                if (downLoad)
                {
                    return DownXssfFile(book);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                book.Close();
            }
        }

        /// <summary>
        /// 创建 sheet
        /// </summary>
        /// <param name="book"></param>
        /// <param name="dt">数据</param>
        /// <param name="heads">表头数据</param>
        /// <remarks>
        /// 创建：王春宝 2020-10-24 
        /// </remarks>
        private static void CreateSheet(XSSFWorkbook book, DataTable dt, List<head> heads)
        {
            //创建该sheet页
            ISheet sheet = book.CreateSheet(dt.TableName);

            //创建 表格头部
            CreadHeader(book, ref sheet, dt, heads);

            //创建 表格数据
            CreadDataRows(book, ref sheet, dt, heads);
        }

        /// <summary>
        /// 创建表头，支持多行
        /// </summary>
        /// <param name="book"></param>
        /// <param name="sheet"></param>
        /// <param name="dt">数据</param>
        /// <param name="heads">表头数据</param>
        /// <remarks>
        /// 创建：王春宝 2020-10-24 
        /// </remarks>
        private static void CreadHeader(XSSFWorkbook book, ref ISheet sheet, DataTable dt, List<head> heads)
        {
            //创建 表格头部
            if (heads != null && heads.Count > 0)
            {
                //使用自定义表头（可以支持多行表头）
                IRow headRow = sheet.CreateRow(0);//创建空行
                headRow.Height = (short)(heads[0].Height * 20);  //设置行高 为25

                //遍历自定义列头
                int maxHeadRowNum = 0;//多行最大行号
                int newColIndex = 0;
                //记录当前列最多变成几列
                Dictionary<int, int[]> mgs = new Dictionary<int, int[]>();
                //
                for (int i = 0; i < heads.Count; i++)
                {
                    if (heads[i].Children.Count == 0)
                    {
                        #region 无子节点
                        ICell cell = headRow.CreateCell(newColIndex); //创建单元格 
                        cell.SetCellValue(heads[i].ColName);    //设置单元格内容

                        var style = GetCellStyle(book, heads[i]);
                        cell.CellStyle = style;
                        // 设置列宽
                        if (heads[i].Width > 0)
                        {
                            sheet.SetColumnWidth(cell.ColumnIndex, heads[i].Width * 256);
                        }
                        else
                        {
                            sheet.SetColumnWidth(cell.ColumnIndex, 13 * 256);
                        }
                        // 
                        mgs.Add(i, new int[] { newColIndex, 1 });
                        newColIndex += 1;
                        #endregion
                    }
                    else
                    {
                        #region 多个子节点
                        int rowIndex = 0;
                        int outRowIndex = 0;
                        int old_colIndex = newColIndex;
                        int new_colIndex = CreateHeadCell(headRow, newColIndex, rowIndex, out outRowIndex, heads[i]);    // 返回最大列数 
                        //
                        for (int j = old_colIndex; j < new_colIndex; j++)
                        {
                            if (headRow.GetCell(j) == null)
                            {
                                ICell _cell = headRow.CreateCell(j); //创建单元格   
                                _cell.SetCellValue(heads[i].ColName);  //设置单元格内容  
                                var style = GetCellStyle(book, heads[i]);
                                _cell.CellStyle = style;
                            }
                        }
                        mgs.Add(i, new int[] { old_colIndex, new_colIndex - old_colIndex });
                        // 
                        //合并单元格
                        //参数1：起始行 参数2：终止行 参数3：起始列 参数4：终止列  
                        CellRangeAddress region1 = new CellRangeAddress(headRow.RowNum, headRow.RowNum, (short)old_colIndex, (short)new_colIndex - 1);
                        headRow.Sheet.AddMergedRegion(region1);
                        //
                        newColIndex = new_colIndex;
                        //
                        if (outRowIndex > maxHeadRowNum)
                        {
                            maxHeadRowNum = outRowIndex;//更新多行最大行号
                        }
                        #endregion
                    }
                }
                var fullstyle = GetCellStyle(book, heads[0]);
                //合并 列
                #region 合并列
                if (maxHeadRowNum > 0)
                {
                    foreach (var mg in mgs)
                    {
                        var values = mg.Value;
                        int cIndex = values[0];
                        int cCount = values[1];
                        if (cCount == 1)
                        {
                            for (int j = headRow.RowNum; j <= maxHeadRowNum; j++)
                            {
                                ICell cell = sheet.GetRow(j).GetCell(cIndex);
                                if (cell == null)
                                {
                                    cell = sheet.GetRow(j).CreateCell(cIndex);
                                    cell.CellStyle = fullstyle;
                                }
                            }
                            CellRangeAddress region1 = new CellRangeAddress(headRow.RowNum, maxHeadRowNum, (short)cIndex, (short)cIndex);
                            headRow.Sheet.AddMergedRegion(region1);
                        }
                        else
                        {
                            for (int j = maxHeadRowNum; j >= headRow.RowNum; j--)
                            {
                                IRow row = sheet.GetRow(j);
                                ICell cell = row.GetCell(cIndex);
                                if (cell == null)
                                {
                                    for (int y = 0; y < cCount; y++)
                                    {
                                        cell = row.CreateCell(cIndex + y);
                                        cell.CellStyle = fullstyle;
                                        //向上行合并
                                        CellRangeAddress region1 = new CellRangeAddress(j - 1, maxHeadRowNum, (short)(cIndex + y), (short)(cIndex + y));
                                        headRow.Sheet.AddMergedRegion(region1);
                                    }
                                }
                                else
                                {
                                    for (int y = 0; y < cCount; y++)
                                    {
                                        cell = row.GetCell(cIndex + y);
                                        if (cell == null)
                                        {
                                            cell = row.CreateCell(cIndex + y);
                                            cell.CellStyle = fullstyle;
                                            //判断上一行是否空
                                            for (int x = j - 1; x >= headRow.RowNum; x--)
                                            {
                                                IRow preRow = sheet.GetRow(x);
                                                var precell = preRow.GetCell(cIndex + y);
                                                if (precell == null)
                                                {
                                                    var newcell = preRow.CreateCell(cIndex + y);
                                                    newcell.CellStyle = fullstyle;
                                                }
                                                else
                                                {
                                                    //向下行合并
                                                    CellRangeAddress region1 = new CellRangeAddress(x, maxHeadRowNum, (short)(cIndex + y), (short)(cIndex + y));
                                                    headRow.Sheet.AddMergedRegion(region1);
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                //使用数据源列名作表头（只支持单行表头）
                IRow headRow = sheet.CreateRow(0);//创建空行
                var style = GetCellStyle(book, null);
                //遍历列
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ICell cell = headRow.CreateCell(i);
                    cell.CellStyle = style;
                    if (!string.IsNullOrEmpty(dt.Columns[i].Caption))
                    {
                        cell.SetCellValue(dt.Columns[i].Caption);
                    }
                    else
                    {
                        cell.SetCellValue(dt.Columns[i].ColumnName);
                    }
                }
            }
        }

        /// <summary>
        /// 创建表头单元格，（支持递归调用)
        /// </summary>
        /// <param name="preHeadRow">上一行</param>
        /// <param name="startColIndex">开始列索引</param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="outRowIndex">输出最新行索引</param>
        /// <param name="headCfg">表头配置</param>
        /// <returns>返回最新的列索引</returns>
        /// <remarks>
        /// 创建：王春宝 2020-10-24 
        /// </remarks>
        private static int CreateHeadCell(IRow preHeadRow, int startColIndex, int rowIndex, out int outRowIndex, head headCfg)
        {
            int preRowIndex = rowIndex;
            rowIndex += 1;
            outRowIndex = rowIndex;
            var sheet = preHeadRow.Sheet;
            XSSFWorkbook book = (XSSFWorkbook)sheet.Workbook;
            var style = GetCellStyle(book, headCfg);
            //
            IRow curHeadRow = null;
            if (sheet.LastRowNum >= rowIndex)
            {
                curHeadRow = sheet.GetRow(rowIndex);
            }
            else
            {
                curHeadRow = sheet.CreateRow(rowIndex);//创建空行
                for (int i = 0; i < startColIndex; i++)
                {
                    ICell cell = curHeadRow.CreateCell(i); //创建单元格 
                    cell.CellStyle = style;

                    ICell mycell = preHeadRow.GetCell(i); //获取单元格 
                    if (mycell != null)
                        cell.SetCellValue(mycell.StringCellValue);//设置单元格内容 

                }
            }
            int newColIndex = startColIndex;
            for (int i = 0; i < headCfg.Children.Count; i++)
            {
                if (headCfg.Children[i].Children.Count > 0)
                {                    //
                    int _outRowIndex = 0;
                    int old_ColIndex = newColIndex;
                    //
                    int new_ColIndex = CreateHeadCell(curHeadRow, newColIndex, rowIndex, out _outRowIndex, headCfg.Children[i]);//递归调用 
                    // 
                    for (int j = old_ColIndex; j < new_ColIndex; j++)
                    {
                        if (curHeadRow.GetCell(j) == null)
                        {
                            ICell _cell = curHeadRow.CreateCell(j); //创建单元格  
                            _cell.SetCellValue(headCfg.Children[i].ColName);  //设置单元格内容  
                            _cell.CellStyle = style;
                        }
                    }
                    //合并单元格
                    //参数1：起始行 参数2：终止行 参数3：起始列 参数4：终止列  
                    CellRangeAddress region1 = new CellRangeAddress(curHeadRow.RowNum, curHeadRow.RowNum, (short)old_ColIndex, (short)(new_ColIndex - 1));
                    sheet.AddMergedRegion(region1);
                    //
                    if (_outRowIndex > outRowIndex)
                    {
                        outRowIndex = _outRowIndex;
                    }
                    newColIndex = new_ColIndex;
                }
                else
                {
                    ICell _cell = curHeadRow.CreateCell(newColIndex); //创建单元格 
                    _cell.SetCellValue(headCfg.Children[i].ColName);//设置单元格内容 
                    _cell.CellStyle = style;
                    // 设置列宽
                    if (headCfg.Width > 0)
                    {
                        sheet.SetColumnWidth(_cell.ColumnIndex, headCfg.Width * 256);
                    }
                    else
                    {
                        sheet.SetColumnWidth(_cell.ColumnIndex, 13 * 256);
                    }
                    //
                    newColIndex += 1;
                }
            }
            //
            return newColIndex;
        }

        /// <summary>
        /// 创建 表格数据
        /// </summary>
        /// <param name="book"></param>
        /// <param name="sheet"></param>
        /// <param name="dt">数据源</param>
        /// <param name="heads">表头配置</param>
        /// <remarks>
        /// 创建：王春宝 2020-10-24 
        /// </remarks>
        private static void CreadDataRows(XSSFWorkbook book, ref ISheet sheet, DataTable dt, List<head> heads)
        {
            // 
            if (dt == null || dt.Rows.Count == 0)
            {
                return;
            }
            if (heads != null && heads.Count > 0)
            {
                List<head> curhds = new List<head>();
                //遍历所有顶层节点
                for (int x = 0; x < heads.Count; x++)
                {
                    List<head> hds = GetAllLeafNode(heads[x]);//获取所有叶子子节点 
                    curhds.AddRange(hds);
                }
                //遍历所有数据行
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(sheet.LastRowNum + 1);//创建空行   
                    int colIndex = 0;
                    //遍历所有叶子子节点
                    for (var y = 0; y < curhds.Count; y++)
                    {
                        var colname = curhds[y].DataField;
                        if (dt.Columns.Contains(colname))
                        {//数据源列是否含有配置列
                            ICell cell = row.CreateCell(colIndex);
                            if (dt.Rows[i][colname] != DBNull.Value)
                            {
                                //所有数据类型统统 ToString()
                                cell.SetCellValue(dt.Rows[i][colname].ToString());
                            }
                            colIndex++;
                        }
                    }
                }
            }
            else
            {
                //遍历行 
                for (int x = 0; x < dt.Rows.Count; x++)
                {
                    IRow row = sheet.CreateRow(sheet.LastRowNum + 1);//创建行  
                    //遍历列 
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        var colname = dt.Columns[i].ColumnName;
                        ICell cell = row.CreateCell(i);
                        if (dt.Rows[x][i] != DBNull.Value)
                        {
                            cell.SetCellValue(dt.Rows[x][i].ToString());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 获取当前节点所有的叶子节点
        /// </summary>
        /// <param name="headcfg">配置</param> 
        /// <returns></returns>
        /// <remarks>
        /// 创建：王春宝 2020-10-28
        /// </remarks>
        private static List<head> GetAllLeafNode(head headcfg)
        {
            List<head> heads = new List<head>();
            if (headcfg != null)
            {
                if (headcfg.Children.Count > 0)
                {
                    for (int i = 0; i < headcfg.Children.Count; i++)
                    {
                        var hds = GetAllLeafNode(headcfg.Children[i]);//递归调用
                        if (hds.Count > 0)
                        {
                            heads.AddRange(hds);
                        }
                    }
                    return heads;
                }
                else
                {
                    heads.Add(headcfg);
                    return heads;
                }
            }
            else
            {
                return heads;
            }
        }

        /// <summary>
        /// 判断是否指定名称的叶子节点
        /// </summary>
        /// <param name="heads">配置列表</param>
        /// <param name="colname">目标列名称</param>
        /// <returns></returns>
        /// <remarks>
        /// 创建：王春宝 2020-10-26
        /// </remarks>
        private static bool HasTargetNode(List<head> heads, string colname)
        {
            if (heads != null)
            {
                var bhav = heads.Exists(w => w.Children.Count == 0 && w.DataField.ToLower() == colname.ToLower()); // 只查叶子节点,忽略大小写
                if (bhav)
                {
                    return bhav;
                }
                else
                {
                    for (int i = 0; i < heads.Count; i++)
                    {
                        bhav = HasTargetNode(heads[i].Children, colname);//递归调用
                        if (bhav)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 设置单元格样式
        /// 风格请自己按headCfg参数编写
        /// </summary>
        /// <param name="book"></param>
        /// <param name="headCfg">表头配置</param>
        /// <returns></returns>
        /// <remarks>
        /// 创建：王春宝 2020-10-26 
        /// </remarks>
        private static ICellStyle GetCellStyle(XSSFWorkbook book, head headCfg)
        {
            ICellStyle style0 = book.CreateCellStyle();
            // 2、行高
            // row.Height = 30 * 20;    //行高为30
            // excelRow.Height = 25 * 20;
            // 单元格 列宽:
            //if (headCfg.Width > 0) {
            //    cell.Row.Sheet.SetColumnWidth(cell.ColumnIndex, headCfg.Width * 256);
            //}
            //三、设置居中: 
            //cellStyle.setAlignment(HSSFCellStyle.ALIGN_CENTER); // 居中    
            style0.Alignment = HorizontalAlignment.Center;
            style0.VerticalAlignment = VerticalAlignment.Center;
            //四、设置字体: 
            IFont font = book.CreateFont();
            font.FontName = "黑体";//.SetFontName("黑体");
            font.FontHeightInPoints = (short)11.5;//设置字体大小    
            style0.SetFont(font);//选择需要用到的字体格式 

            //必须设置单元格背景色 FillForegroundColor 和 FillPattern 的值才能正确显示背景色
            style0.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LightCornflowerBlue.Index; //(short)1灰色  NPOI.HSSF.Util.HSSFColor.LightBlue.Index; 
            style0.FillPattern = FillPattern.SolidForeground; // CellStyle.SOLID_FOREGROUND

            //二、设置边框:
            //cellStyle.setBorderBottom(HSSFCellStyle.BORDER_THIN); //下边框    
            //cellStyle.setBorderLeft(HSSFCellStyle.BORDER_THIN);//左边框    
            //cellStyle.setBorderTop(HSSFCellStyle.BORDER_THIN);//上边框    
            //cellStyle.setBorderRight(HSSFCellStyle.BORDER_THIN);//右边框    

            style0.BorderBottom = BorderStyle.Medium;// CellStyle.SOLID_FOREGROUND
            style0.BorderRight = BorderStyle.Medium;

            return style0;
        }

        /// <summary>
        /// 保存电子表格
        /// </summary>
        /// <param name="book"></param>
        /// <param name="fileName">文件名称</param>
        /// <param name="savePath">保存物理路径（不含文件名）</param>
        /// <remarks>
        /// 创建：王春宝 2020-10-26 
        /// </remarks>
        private static void SaveExcel(XSSFWorkbook book, string fileName, string savePath = "")
        {
            string fullpath = fileName;
            if (!string.IsNullOrEmpty(savePath))
            {
                if (!savePath.EndsWith("\\"))
                    savePath += "\\";
                fullpath = savePath + fileName;
            }
            if (File.Exists(fullpath))
            {
                File.Delete(fullpath);
            }
            FileStream fsOut = new FileStream(fullpath, FileMode.Create);
            book.Write(fsOut);
            fsOut.Close();
            fsOut.Dispose();
        }

        /// <summary>
        /// 下载电子表格文件
        /// </summary>
        /// <param name="book"></param>
        /// <param name="saveFileName">文件名称</param>
        /// <returns></returns>
        /// <remarks>
        /// 创建：王春宝 2020-10-26 
        /// </remarks>
        private static NpoiMemoryStream DownXssfFile(XSSFWorkbook book)
        {
            var exportData = new NpoiMemoryStream();
            exportData.AllowClose = false;
            book.Write(exportData);
            exportData.Flush();
            exportData.Seek(0, SeekOrigin.Begin);
            exportData.AllowClose = true;
            return exportData;
        }
    }

    /// <summary>
    /// 内存字节流
    /// </summary>
    class NpoiMemoryStream : MemoryStream
    {
        /// <summary>
        /// 
        /// </summary>
        public bool AllowClose { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public override void Close()
        {
            if (AllowClose)
            {
                base.Close();
            }
        }
    }

    /// <summary>
    ///  电子表格的表头设置（支持多行表头设置）
    /// </summary>
    /// <remarks>
    /// 创建：王春宝 2020-08-27
    /// </remarks>
    public class head
    {
        /// <summary>
        /// 表头格式设置（支持多行表头）
        /// </summary>
        public head()
        {
            Children = new List<head>();
        }

        /// <summary>
        /// 重载构造函数
        /// </summary> 
        /// <param name="FieldName">数据源列名</param>
        /// <param name="FieldLable">电子表格列名</param>
        /// <param name="Height">行高</param>
        /// <param name="Width">列宽</param>
        public head(string FieldName, string FieldLable, int Width = 10, int Height = 13)
        {
            this.ColName = FieldName;
            this.DataField = FieldLable;
            this.Height = Height;
            this.Width = Width;
            this.IsBold = true;
            //
            Children = new List<head>();
        }
        /// <summary>
        /// 字段名称
        /// </summary>
        public string ColName { get; set; }
        /// <summary>
        /// 字段标签（表格行头标题）
        /// </summary>
        public string DataField { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 背景颜色
        /// </summary>
        public string BackColor { get; set; }
        /// <summary>
        /// 文本颜色
        /// </summary>
        public string FontColor { get; set; }
        /// <summary>
        /// 是否粗体显示
        /// </summary>
        public bool IsBold { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public List<head> Children { get; set; }
    }
}
