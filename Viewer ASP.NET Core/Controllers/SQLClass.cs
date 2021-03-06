﻿using System;
using System.Data;
using System.Data.SqlClient;

namespace Viewer_ASP.NET_Core.Controllers
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

        public DataTable GetDataTable(string CommandString, out string Error)
        {
            Error = "";
            DataTable DataTable = new DataTable();
            if (OpenConnection())
            {
                SqlCommand Command = new SqlCommand(CommandString, Connection);
                try
                {
                    SqlDataAdapter DataAdapter = new SqlDataAdapter(Command);
                    DataAdapter.Fill(DataTable);
                }
                catch (Exception e) { Error = e.Message; }
                CloseConnection();
            }
            return DataTable;
        }

        public string GetSingleCellDataSimple(string ColumnName, string TableName, string IMDBID)
        {
            string CommandString = "SELECT " + ColumnName + " FROM " + TableName + " (NOLOCK) WHERE IMDB_ID = '" + IMDBID + "'";
            string result = string.Empty;
            DataTable DataTable = GetDataTable(CommandString, out string Error);
            if (DataTable.Rows.Count > 0)
                result = DataTable.Rows[0][0].ToString();
            return result == string.Empty ? Error : result;
        }

        public string GetSingleCellDataComplex(string CommandString)
        {
            string result = string.Empty;
            DataTable DataTable = GetDataTable(CommandString, out string Error);
            if (DataTable.Rows.Count > 0)
                result = DataTable.Rows[0][0].ToString();
            return result == string.Empty ? Error : result;
        }

        public void BulkInsert(DataTable DataTable, string TableName)
        {
            if (OpenConnection())
            {
                using SqlBulkCopy bulkCopy = new SqlBulkCopy(Connection)
                {
                    DestinationTableName = TableName
                };
                try
                {
                    bulkCopy.WriteToServer(DataTable);
                }
                catch (Exception)
                {

                }
                CloseConnection();
            }
        }
        #endregion

        #region Use Methods

        #endregion
    }
}
