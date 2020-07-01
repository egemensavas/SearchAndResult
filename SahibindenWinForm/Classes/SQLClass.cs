using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using static SahibindenWinForm.Models.GeneralModel;

namespace SahibindenWinForm.Classes
{
    class SQLClass
    {
        #region Properties
        readonly string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Sahibinden"].ConnectionString;
        readonly SqlConnection Connection = new SqlConnection();
        #endregion

        #region Constructor
        public SQLClass()
        {
            Connection.ConnectionString = ConnectionString;
        }
        #endregion

        #region Helper Methods
        bool OpenConnection()
        {
            if (Connection.State != ConnectionState.Open)
                try
                {
                    Connection.Open();
                }
                catch (Exception)
                {
                    return false;
                }
            return true;
        }

        bool CloseConnection()
        {
            if (Connection.State != ConnectionState.Closed)
                try
                {
                    Connection.Close();
                }
                catch (Exception)
                {
                    return false;
                }
            return true;
        }

        public DataTable GetDataTable(string CommandString)
        {
            DataTable DataTable = new DataTable();
            if (OpenConnection())
            {
                SqlCommand Command = new SqlCommand(CommandString, Connection);
                try
                {
                    SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                    DataAdapter.Fill(DataTable);
                }
                catch { }
                CloseConnection();
            }
            return DataTable;
        }

        public string GetSingleCellDataSimple(string ColumnName, string TableName, string IMDBID)
        {
            string CommandString = "SELECT " + ColumnName + " FROM " + TableName + " (NOLOCK) WHERE IMDB_ID = '" + IMDBID + "'";
            string result = string.Empty;
            DataTable DataTable = GetDataTable(CommandString);
            if (DataTable.Rows.Count > 0)
                result = DataTable.Rows[0][0].ToString();
            return result;
        }

        public string GetSingleCellDataComplex(string CommandString)
        {
            string result = string.Empty;
            DataTable DataTable = GetDataTable(CommandString);
            if (DataTable.Rows.Count > 0)
                result = DataTable.Rows[0][0].ToString();
            return result;
        }

        int ManipulateData(string CommandString, List<SQLParameter> SQLParameters)
        {
            int result = 0;
            if (OpenConnection())
            {
                try
                {
                    int i = 0;
                    SqlCommand Command = new SqlCommand(CommandString, Connection);
                    foreach (var item in SQLParameters)
                    {
                        var value = item.SQLDBType == SqlDbType.Date ? Convert.ToDateTime(item.Value) :
                                    item.SQLDBType == SqlDbType.Int ? Convert.ToInt32(item.Value) :
                                    item.SQLDBType == SqlDbType.NVarChar ? Convert.ToString(item.Value) :
                                    item.Value;
                        i++;
                        SqlParameter Param = new SqlParameter("@Param" + i.ToString(), item.SQLDBType)
                        {
                            Value = value
                        };
                        Command.Parameters.Add(Param);
                    }
                    result = (int?)Command.ExecuteScalar() ?? 0;
                }
                catch (Exception e)
                {
                    e = e.InnerException;
                    return 0;
                }
                CloseConnection();
            }
            return result;
        }

        string GenerateInsertCommand(string TableName)
        {
            string result = "INSERT INTO " + TableName + " (";
            string CommandString = "SELECT * FROM " + TableName + " (NOLOCK) WHERE 1 = 0";
            DataTable x = GetDataTable(CommandString);
            foreach (DataColumn item in x.Columns)
            {
                if (item.ColumnName == "ID")
                    continue;
                result += item.ColumnName + ", ";
            }
            result = result.Substring(0, result.Length - 2);
            result += ") OUTPUT INSERTED.ID SELECT ";
            for (int i = 1; i < x.Columns.Count; i++)
                result += "@Param" + i.ToString() + ", ";
            result = result.Substring(0, result.Length - 2);
            return result;
        }

        string GenerateUpdateCommand(string TableName, string SetColumnName, string WhereColumnName)
        {
            string result = "UPDATE " + TableName + " SET " + SetColumnName + " = @Param1 WHERE " + WhereColumnName + " = @Param2";
            return result;
        }

        bool IsRecordExist(string TableName, List<ColumnValuePair> ColumnValuePairs, bool Add)
        {
            string CommandString = "SELECT 1 FROM " + TableName + " (NOLOCK) WHERE ";
            string ColumnName = string.Empty;
            string Value = string.Empty;
            foreach (var item in ColumnValuePairs)
            {
                ColumnName = item.ColumnName;
                Value = item.Value.ToString();
                CommandString += ColumnName + " = '" + Value + "' AND ";
            }
            CommandString = CommandString.Substring(0, CommandString.Length - 5);
            if (GetDataTable(CommandString).Rows.Count == 0)
            {
                if (Add && ColumnValuePairs.Count == 1)
                    InsertWithOneParameter(TableName, Value);
                return false;
            }
            return true;
        }

        int GetID(string TableName, List<ColumnValuePair> ColumnValuePairs, bool Add)
        {
            int result = 0;
            string temp = string.Empty;
            string CommandString = "SELECT ID FROM " + TableName + " (NOLOCK) WHERE ";
            string ColumnName = string.Empty;
            string Value = string.Empty;
            foreach (var item in ColumnValuePairs)
            {
                ColumnName = item.ColumnName;
                Value = item.Value.ToString();
                CommandString += ColumnName + " = '" + Value.Replace("'", "") + "' AND ";
            }
            CommandString = CommandString.Substring(0, CommandString.Length - 5);
            temp = GetSingleCellDataComplex(CommandString);
            if (string.IsNullOrEmpty(temp))
                result = 0;
            else
                result = Convert.ToInt32(temp);
            if (result == 0 && Add && ColumnValuePairs.Count == 1)
                result = InsertWithOneParameter(TableName, Value);
            return result;
        }

        int InsertWithOneParameter(string TableName, string Value)
        {
            int result = 0;
            string CommandString = GenerateInsertCommand(TableName);
            List<SQLParameter> SQLParameters = new List<SQLParameter>()
            {
                new SQLParameter() { SQLDBType = SqlDbType.NVarChar, Value = Value },
            };
            result = ManipulateData(CommandString, SQLParameters);
            return result;
        }

        public void BulkInsert(DataTable DataTable, string TableName)
        {
            if (OpenConnection())
            {
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(Connection))
                {
                    bulkCopy.DestinationTableName = TableName;
                    try
                    {
                        bulkCopy.WriteToServer(DataTable);
                    }
                    catch 
                    {

                    }
                    CloseConnection();
                }
            }
        }
        #endregion

        #region Use Methods

        #endregion
    }
}
