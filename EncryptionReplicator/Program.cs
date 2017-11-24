using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionReplicator
{
    class Program
    {
        public static string _connString = string.Empty;
        private static string _logFilename = string.Empty;
        private static string _columnNames = string.Empty;
        private static string _tableName = string.Empty;

        static Program()
        {
            _connString = ConfigurationManager.AppSettings["DBCon"];
            _logFilename = ConfigurationManager.AppSettings["debugFile"];
            _columnNames = ConfigurationManager.AppSettings["ColumnNameToEncrypt"];
            _tableName = ConfigurationManager.AppSettings["TableName"];
        }

        static void Main(string[] args)
        {
            //var columnnametoencrypt = "colSecondOwnerFirstName";
            LogString("Process Started to encrypt the columns !!!!");

            if (_columnNames != null && _columnNames.Count() > 0)
            {
                var columnCollections = _columnNames.Split(',');
                foreach (var column in columnCollections)
                {
                    var columnFrom = column.Split('|')[0];
                    var columnToEncrypt = column.Split('|')[1];

                    LogString("Process Started to encrypt the column " + columnFrom + " !!!!");
                    Console.WriteLine("Process Started to encrypt the column " + columnFrom + " !!!!");
                    var result = GetEncryptionRecords(_tableName, columnFrom,columnToEncrypt, null);

                    foreach (var obj in result)
                    {
                        try
                        {
                            LogString("Update Started for ID : " + obj.ID);
                            Console.WriteLine("Update Started for ID : " + obj.ID);
                            UpdateEncryptedRecords(obj.ID, columnToEncrypt, obj.ColumnEncryptedValue, _tableName);
                            LogString("Updated ID : " + obj.ID + " Successfully");
                            Console.WriteLine("Updated ID : " + obj.ID + " Successfully");
                            //if (obj.ID == 25)
                            //    break;
                        }
                        catch (Exception ex)
                        {
                            LogString("Issue while updating ID : " + obj.ID + "\n" + ex.Message);
                            Console.WriteLine("Issue while updating ID : " + obj.ID + "\n" + ex.Message);
                        }
                    }
                }
            }
            
            LogString("Process End !!!!");
            LogString("------------------------------------------------");
            Console.WriteLine("Process End !!!!");
            Console.WriteLine("------------------------------------------------");
        }

        private static void LogString(string entry)
        {
            var s = DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToLongTimeString() + ": " + entry + "\r\n";

            var directoryName = Path.GetDirectoryName(_logFilename);
            if (directoryName != null && !Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            if (_logFilename != "")
            {
                File.AppendAllText(_logFilename, s);

            }
        }

        public static EncryptionRecord PopulateReader(IDataReader reader)
        {
            var model = new EncryptionRecord();

            if (reader[0] != DBNull.Value)
            {
                model.ID = Convert.ToInt32(reader[0]);
            }
            if (reader[1] != DBNull.Value)
            {
                model.ColumnValue = Convert.ToString(reader[1]);
                model.ColumnEncryptedValue = CVREncryptionEngine.CipherUtility.Encrypt(Convert.ToString(reader[1]));
            }
            //if (reader[2] != DBNull.Value)
            {
                
            }
            return model;
        }

        public static bool UpdateEncryptedRecords(int id, string columntoencrypt, string encryptedval, string tablename)
        {
            try
            {
                using (var conn = new SqlConnection(_connString))
                {
                    var com = new SqlCommand
                    {
                        CommandText = "updTableForEncryption",
                        CommandType = CommandType.StoredProcedure
                    };

                    IDataParameter p = new SqlParameter("@id", SqlDbType.BigInt) { Value = id };
                    com.Parameters.Add(p);

                    p = new SqlParameter("@ColumnName", SqlDbType.VarChar) { Value = columntoencrypt };
                    com.Parameters.Add(p);

                    p = new SqlParameter("@EncryptedVal", SqlDbType.VarChar) { Value = encryptedval };
                    com.Parameters.Add(p);

                    p = new SqlParameter("@TableName", SqlDbType.VarChar) { Value = tablename };
                    com.Parameters.Add(p);

                    conn.Open();
                    com.Connection = conn;
                    com.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public static List<EncryptionRecord> GetEncryptionRecords(string tablename,string columnfrom,string columntoencrypt,int? id)
        {
            //var conString = ConfigurationManager.AppSettings["DBCon"];
            var returnValue = new List<EncryptionRecord>();

            try
            {
                LogString(id == null ? "Started reading records " : "Started reading record for ID : " + id.Value.ToString());
                using (var conn = new SqlConnection(_connString))
                {
                    var com = new SqlCommand
                    {
                        CommandText = "getInformationForEncryption",
                        CommandType = CommandType.StoredProcedure
                    };

                    IDataParameter p = new SqlParameter("@ColumnName", SqlDbType.VarChar) { Value = columnfrom.ToUpper() };
                    com.Parameters.Add(p);

                    p = new SqlParameter("@ColumnNameToEncrypt", SqlDbType.VarChar) { Value = columntoencrypt.ToUpper() };
                    com.Parameters.Add(p);

                    p = new SqlParameter("@TableName", SqlDbType.VarChar) { Value = tablename.ToUpper() };
                    com.Parameters.Add(p);

                    if (id != null)
                    {
                        p = new SqlParameter("@id", SqlDbType.BigInt) { Value = id };
                        com.Parameters.Add(p);
                    }

                    conn.Open();
                    com.Connection = conn;
                    using (IDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            returnValue.Add(PopulateReader(reader));
                        }

                        reader.Close();
                    }
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
            }
            return returnValue;
        }
    }
}
