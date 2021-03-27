using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace WSY.Common
{
    public class ModelConvertHelper
    {
        /// <summary>
        /// datatable转化成list
        /// </summary>
        public static List<T> ConvertToModel<T>(DataTable dt)
        {
            // 定义集合  
            List<T> ts = new List<T>();
            // 获得此模型的类型  
            Type type = typeof(T);
            string tempName = "";
            ILog log = LogManager.GetLogger("ModelConvertHelper");
            try
            {
                foreach (DataRow dr in dt.Rows)
                {
                    T model = (T)Activator.CreateInstance(typeof(T));
                    // 获得此模型的公共属性  
                    PropertyInfo[] propertys = model.GetType().GetProperties();
                    foreach (PropertyInfo pi in propertys)
                    {
                        tempName = pi.Name;
                        // 检查DataTable是否包含此列  
                        if (dt.Columns.Contains(tempName))
                        {
                            // 判断此属性是否有Setter  
                            if (!pi.CanWrite)
                            {
                                continue;
                            }
                            object value = dr[tempName];
                            if (value != DBNull.Value && value != null && value.ToString() != "")
                            {
                                object t = null;
                                if (!pi.PropertyType.IsGenericType)
                                {
                                    t = Convert.ChangeType(value, pi.PropertyType);
                                }
                                else
                                {
                                    if (pi.PropertyType.Name == typeof(Nullable<>).Name)
                                    {
                                        t = Convert.ChangeType(value, Nullable.GetUnderlyingType(pi.PropertyType));
                                    }
                                }
                                pi.SetValue(model, t);
                            }
                            else
                            {
                                if (!pi.PropertyType.IsValueType)
                                {
                                    pi.SetValue(model, "");
                                }
                            }
                        }
                    }
                    ts.Add(model);
                }
            }
            catch (Exception ex)
            {
                log.Error(tempName, ex);
                throw ex;
            }

            return ts;
        }

        /// <summary>
        /// datarow 转化为model
        /// </summary>
        public static T DataRowToModel<T>(DataRow row)
        {
            if (row == null)
            {
                return default(T);
            }
            string tempName = "";
            T model = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] propertys = model.GetType().GetProperties();
            ILog log = LogManager.GetLogger("ModelConvertHelper");
            try
            {
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;
                    // 检查DataTable是否包含此列  
                    if (row.Table.Columns.Contains(tempName))
                    {
                        // 判断此属性是否有Setter  
                        if (!pi.CanWrite)
                        {
                            continue;
                        }
                        object value = row[tempName];
                        if (value != DBNull.Value && value != null && value.ToString() != "")
                        {
                            object t = null;
                            if (!pi.PropertyType.IsGenericType)
                            {
                                t = Convert.ChangeType(value, pi.PropertyType);
                            }
                            else
                            {
                                if (pi.PropertyType.Name == typeof(Nullable<>).Name)
                                {
                                    t = Convert.ChangeType(value, Nullable.GetUnderlyingType(pi.PropertyType));
                                }
                            }
                            pi.SetValue(model, t);
                        }
                        else
                        {
                            if (!pi.PropertyType.IsValueType)
                            {
                                pi.SetValue(model, "");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(tempName, ex);
                throw ex;
            }
            return model;
        }
    }
}