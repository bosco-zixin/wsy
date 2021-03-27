using System;
using System.Collections.Generic;
using System.Data;
using System.Collections;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using WSY.Common;

namespace WSY.SQLTools
{
    /// <summary>
    /// Copyright (C) 2004-2008 LiTianPing 
    /// 数据访问基础类(基于Oracle)
    /// 可以用户可以修改满足自己项目的需要。
    /// </summary>
    public class OracleHelper
    {
        private readonly string ConnectionString = string.Empty;
        public OracleHelper(string connectionString)
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                string ConStringEncrypt = ConfigurationManager.AppSettings["ConStringEncrypt"];
                if (ConStringEncrypt == "true")
                {
                    ConnectionString = DESEncrypt.DecryptDEC(ConnectionString, "jingqidianzijichengbu1==");
                }
                ConnectionString = connectionString;
            }
            else
            {
                throw new Exception("连接字符串不能为空");
            }
        }

        #region 公用方法
        public int GetMaxID(string FieldName, string TableName)
        {
            string strsql = "select max(" + FieldName + ")+1 from " + TableName;
            object obj = GetSingle(strsql);
            if (obj == null)
            {
                return 1;
            }
            else
            {
                return int.Parse(obj.ToString());
            }
        }

        public bool Exists(string strSql, params OracleParameter[] cmdParms)
        {
            object obj = GetSingle(strSql, "", cmdParms);
            int cmdresult;
            if ((Equals(obj, null)) || (Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        #region  执行简单SQL语句
        public bool Exists(string SQLString)
        {
            object obj = GetSingle(SQLString);
            int cmdresult;
            if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString)
        {

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                connection.Open();
                using (OracleTransaction trans = connection.BeginTransaction())
                {
                    using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                    {
                        try
                        {
                            int rows = cmd.ExecuteNonQuery();
                            trans.Commit();
                            return rows;
                        }
                        catch (OracleException E)
                        {
                            trans.Rollback();
                            connection.Close();
                            throw E;
                        }
                        finally
                        {
                            cmd.Dispose();
                            connection.Close();
                        }
                    }
                }
            }
        }


        public int ExecuteSql1(string SQLString, string connStr = "")
        {
            if (connStr == "")
            {
                connStr = ConnectionString;
            }

            using (OracleConnection connection = new OracleConnection(connStr))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (OracleException E)
                    {
                        connection.Close();
                        throw E;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>  
        public bool ExecuteSqlTran(ArrayList SQLStringList)
        {
            bool re = false;
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                connection.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = connection;
                OracleTransaction tx = connection.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    re = true;
                }
                catch (OracleException E)
                {
                    re = false;
                    tx.Rollback();
                    throw E;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
            return re;

        }

        /// <summary>
        /// 执行带一个存储过程参数的的SQL语句。
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="content">参数内容,比如一个字段是格式复杂的文章，有特殊符号，可以通过这个方式添加</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, string content)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleCommand cmd = new OracleCommand(SQLString, connection);
                OracleParameter myParameter = new OracleParameter("@content", OracleDbType.NVarchar2);
                myParameter.Value = content;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (OracleException E)
                {
                    throw E;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 向数据库里插入图像格式的字段(和上面情况类似的另一种实例)
        /// </summary>
        /// <param name="strSQL">SQL语句</param>
        /// <param name="fs">图像字节,数据库的字段类型为image的情况</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSqlInsertImg(string strSQL, byte[] fs)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleCommand cmd = new OracleCommand(strSQL, connection);
                OracleParameter myParameter = new OracleParameter("@fs", OracleDbType.LongRaw);
                myParameter.Value = fs;
                cmd.Parameters.Add(myParameter);
                try
                {
                    connection.Open();
                    int rows = cmd.ExecuteNonQuery();
                    return rows;
                }
                catch (OracleException E)
                {
                    throw E;
                }
                finally
                {
                    cmd.Dispose();
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString)
        {

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    try
                    {
                        connection.Open();
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (OracleException e)
                    {
                        connection.Close();
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public OracleDataReader ExecuteReader(string strSQL)
        {
            OracleConnection connection = new OracleConnection(ConnectionString);
            OracleCommand cmd = new OracleCommand(strSQL, connection);
            try
            {
                connection.Open();
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                return myReader;
            }
            catch (OracleException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                DataSet ds = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter command = new OracleDataAdapter(SQLString, connection);
                    command.Fill(ds, "ds");
                }
                catch (OracleException ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
                return ds;
            }
        }
        /// <summary>
        /// 批量执行sql语句
        /// </summary>
        /// <param name="dicsql"></param>
        /// <returns></returns>
        public DataSet Query(Dictionary<string, string> dicsql)
        {

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                DataSet data = new DataSet();
                try
                {
                    connection.Open();
                    OracleDataAdapter adapter = new OracleDataAdapter();
                    OracleTransaction myTrans = connection.BeginTransaction();

                    foreach (string hstkey in dicsql.Keys)
                    {
                        DataSet ds = new DataSet();
                        OracleCommand command = new OracleCommand(dicsql[hstkey].ToString(), connection);
                        adapter.SelectCommand = command;
                        adapter.SelectCommand.Transaction = myTrans;
                        if (hstkey.ToString() != "")
                            adapter.Fill(ds, hstkey.ToString());
                        else
                            adapter.Fill(ds);
                        data.Merge(ds);
                    }
                }
                catch (OracleException ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
                return data;
            }
        }

        #endregion

        #region 执行带参数的SQL语句

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        int rows = cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        return rows;
                    }
                    catch (OracleException E)
                    {
                        throw E;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OracleParameter[]）</param>
        public void ExecuteSqlTran(Hashtable SQLStringList)
        {
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OracleParameter[] cmdParms = (OracleParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            trans.Commit();
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 执行一条计算查询结果语句，返回查询结果（object）。
        /// </summary>
        /// <param name="SQLString">计算查询结果语句</param>
        /// <returns>查询结果（object）</returns>
        public object GetSingle(string SQLString, string connStr = "", params OracleParameter[] cmdParms)
        {
            if (connStr == "")
            {
                connStr = ConnectionString;
            }
            using (OracleConnection connection = new OracleConnection(connStr))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    try
                    {
                        PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                        object obj = cmd.ExecuteScalar();
                        cmd.Parameters.Clear();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (OracleException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行查询语句，返回OracleDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="strSQL">查询语句</param>
        /// <returns>OracleDataReader</returns>
        public OracleDataReader ExecuteReader(string SQLString, params OracleParameter[] cmdParms)
        {
            OracleConnection connection = new OracleConnection(ConnectionString);
            OracleCommand cmd = new OracleCommand();
            try
            {
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                OracleDataReader myReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                cmd.Parameters.Clear();
                return myReader;
            }
            catch (OracleException e)
            {
                throw e;
            }
        }

        /// <summary>
        /// 执行查询语句，返回DataSet
        /// </summary>
        /// <param name="SQLString">查询语句</param>
        /// <returns>DataSet</returns>
        public DataSet Query(string SQLString, params OracleParameter[] cmdParms)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleCommand cmd = new OracleCommand();
                PrepareCommand(cmd, connection, null, SQLString, cmdParms);
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();
                    try
                    {
                        da.Fill(ds, "ds");
                        cmd.Parameters.Clear();
                    }
                    catch (OracleException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        cmd.Dispose();
                        connection.Close();
                    }
                    return ds;
                }
            }
        }

        private void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (OracleParameter parm in cmdParms)
                    cmd.Parameters.Add(parm);
            }
        }
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms, CommandType cmdType)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = cmdType;
            if (cmdParms != null)
            {
                foreach (OracleParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }

        #endregion

        #region 存储过程操作

        /// <summary>
        /// 执行存储过程 返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleDataReader</returns>
        public OracleDataReader RunProcedureReader(string storedProcName, IDataParameter[] parameters)
        {
            OracleConnection connection = new OracleConnection(ConnectionString);
            OracleDataReader returnReader;
            connection.Open();
            OracleCommand command = BuildQueryCommand(connection, null, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            returnReader = command.ExecuteReader(CommandBehavior.CloseConnection);
            return returnReader;
        }

        /// <summary>
        /// 执行存储过程加事务
        /// </summary>
        ///  <param name="dicSqlList">SQL语句的哈希表（key为sql语句，value是该语句的SqlParameter[]）</param>
        ///  <param name="reVal">执行存储过程返回结果</param>
        /// <returns></returns>
        public int RunProcedure(Dictionary<string, OracleParameter[]> dicSqlList, out int reVal)
        {
            int affectRow = 0;
            reVal = 0;
            using (OracleConnection conn = new OracleConnection(ConnectionString))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (KeyValuePair<string, OracleParameter[]> myDE in dicSqlList)
                        {
                            CommandType cmdType = CommandType.StoredProcedure;
                            string cmdText = myDE.Key;
                            OracleParameter[] cmdParms = myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms, cmdType);
                            cmd.Parameters.Add(new OracleParameter("ReturnValue",
                                OracleDbType.Int32, 4, ParameterDirection.ReturnValue,
                                false, 0, 0, string.Empty, DataRowVersion.Default, null));
                            affectRow = cmd.ExecuteNonQuery();
                            reVal = (int)cmd.Parameters["ReturnValue"].Value;
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
            return affectRow;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数  
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public object RunProcedure(string storedProcName, OracleParameter[] parameters, out int rowsAffected)
        {
            object result = null;
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleCommand cmd = BuildQueryCommand(connection, null, storedProcName, parameters);
                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }


        /// <summary>
        /// 执行存储过程，返回影响的行数  
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public object RunProcedure(OracleTransaction trans, string storedProcName, OracleParameter[] parameters, out int rowsAffected)
        {
            object result = null;
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleCommand cmd = BuildQueryCommand(connection, trans, storedProcName, parameters);
                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }


        /// <summary>
        /// 执行存储过程，返回影响的行数  
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public object RunProcedureTrans(string storedProcName, OracleParameter[] parameters, out int rowsAffected)
        {
            object result = null;
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleTransaction trans = null;
                try
                {
                   
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    trans = connection.BeginTransaction();
                    OracleCommand cmd = BuildQueryCommand(connection, trans, storedProcName, parameters);
                    rowsAffected = cmd.ExecuteNonQuery();
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 执行存储过程，什么值也不返回 
        /// </summary>
        /// <param name="storedProcName"></param>
        /// <param name="parameters"></param>
        public void RunProcedure(string storedProcName, OracleParameter[] parameters)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleCommand cmd = BuildQueryCommand(connection, null, storedProcName, parameters);
                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }


        /// <summary>
        /// 执行存储过程 返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleDataReader</returns>
        public List<string> RunProcedureAddList(string storedProcName, IDataParameter[] parameters)
        {
            List<string> Listerror = new List<string>();
            OracleConnection connection = new OracleConnection(ConnectionString);
            connection.Open();
            OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteReader(CommandBehavior.CloseConnection);

            string errcode = command.Parameters["errcode"].Value.ToString();
            if (!string.IsNullOrEmpty(errcode))
            {
                Listerror.Add(errcode);
            }

            return Listerror;
        }

        /// <summary>
        /// 执行存储过程 返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleDataReader</returns>
        public  List<string> RunProcedureReade(string storedProcName, IDataParameter[] parameters)
        {
            List<string> Listerror = new List<string>();
            OracleConnection connection = new OracleConnection(ConnectionString);
            connection.Open();
            OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteReader(CommandBehavior.CloseConnection);

            string errcode = command.Parameters["errcode"].Value.ToString();
            //string errmsg = command.Parameters["errmsg"].Value.ToString();
            if (!string.IsNullOrEmpty(errcode))
            {
                Listerror.Add(errcode);
                //Listerror.Add(errmsg);
            }

            return Listerror;
        }

        /// <summary>
        /// 执行存储过程 返回SqlDataReader ( 注意：调用该方法后，一定要对SqlDataReader进行Close )
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleDataReader</returns>
        public List<string> RunProcedureSuccFlag(string storedProcName, IDataParameter[] parameters)
        {
            List<string> Listerror = new List<string>();
            OracleConnection connection = new OracleConnection(ConnectionString);
            connection.Open();
            OracleCommand command = BuildQueryCommand(connection, storedProcName, parameters);
            command.CommandType = CommandType.StoredProcedure;
            command.ExecuteReader(CommandBehavior.CloseConnection);

            string errcode = command.Parameters["SuccFlag"].Value.ToString();

            if (!string.IsNullOrEmpty(errcode))
            {
                Listerror.Add(errcode);
            }

            return Listerror;
        }

        /// <summary>
        /// 执行存储过程,返回数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedureGetDataSet(string storedProcName, OracleParameter[] parameters)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                OracleDataAdapter sqlDA = new OracleDataAdapter();
                sqlDA.SelectCommand = BuildQueryCommand(connection, null, storedProcName, parameters);
                sqlDA.Fill(dataSet, "dt");
                connection.Close();
                return dataSet;
            }
        }

        /// <summary>
        /// 执行存储过程,返回数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="connectionString"></param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedureGetDataSet(out OracleTransaction trans, string storedProcName, OracleParameter[] parameters)
        {
            OracleConnection connection = new OracleConnection(ConnectionString);
            DataSet dataSet = new DataSet();
            connection.Open();
            trans = connection.BeginTransaction();
            OracleDataAdapter sqlDA = new OracleDataAdapter();
            sqlDA.SelectCommand = BuildQueryCommand(connection, null, storedProcName, parameters);
            sqlDA.Fill(dataSet, "dt");
            return dataSet;
        }

        /// <summary>
        /// 执行存储过程,返回数据集
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>DataSet</returns>
        public DataSet RunProcedureGetDataSetTrans(string storedProcName, OracleParameter[] parameters)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                DataSet dataSet = new DataSet();
                connection.Open();
                using (OracleTransaction trans = connection.BeginTransaction())
                {
                    try
                    {
                        OracleDataAdapter sqlDA = new OracleDataAdapter();
                        sqlDA.SelectCommand = BuildQueryCommand(connection, null, storedProcName, parameters);
                        sqlDA.Fill(dataSet, "dt");
                 
                        trans.Rollback();
                        connection.Close();
                        //if (dataSet != null)
                        //{

                        //}
                        //else
                        //{
                        //    trans.Commit();
                        //}
                        return dataSet;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        throw ex;
                    }
                }
            }
        }


        /// <summary>
        /// 构建 OracleCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand</returns>
        private OracleCommand BuildQueryCommand(OracleConnection connection, OracleTransaction trans, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, connection);
            if (trans != null)
                command.Transaction = trans;
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        /// <summary>
        /// 执行存储过程，返回影响的行数  
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <param name="rowsAffected">影响的行数</param>
        /// <returns></returns>
        public int RunProcedure_rowsAffected(string storedProcName, IDataParameter[] parameters, out int rowsAffected)
        {
            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                int result;
                connection.Open();
                OracleCommand command = BuildIntCommand(connection, storedProcName, parameters);
                rowsAffected = command.ExecuteNonQuery();
                result = (int)command.Parameters["ReturnValue"].Value;
                //Connection.Close();
                return result;
            }
        }

        /// <summary>
        /// 创建 OracleCommand 对象实例(用来返回一个整数值) 
        /// </summary>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand 对象实例</returns>
        private OracleCommand BuildIntCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = BuildQueryCommand(connection, null, storedProcName, parameters);
            command.Parameters.Add(new OracleParameter("ReturnValue",
                         OracleDbType.Int32, 4, ParameterDirection.ReturnValue,
             false, 0, 0, string.Empty, DataRowVersion.Default, null));
            return command;
        }

        /// <summary>
        /// 构建 OracleCommand 对象(用来返回一个结果集，而不是一个整数值)
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="storedProcName">存储过程名</param>
        /// <param name="parameters">存储过程参数</param>
        /// <returns>OracleCommand</returns>
        private  OracleCommand BuildQueryCommand(OracleConnection connection, string storedProcName, IDataParameter[] parameters)
        {
            OracleCommand command = new OracleCommand(storedProcName, connection);
            command.CommandType = CommandType.StoredProcedure;
            foreach (OracleParameter parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
            return command;
        }

        #endregion

    }
}