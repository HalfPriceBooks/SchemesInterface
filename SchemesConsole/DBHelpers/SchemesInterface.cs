using SchemesConsole.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using Dapper;
using System.Data.SqlClient;
using CubiscanConsole.DBHelpers;
using System.Configuration;
using System.Text;

namespace SchemesConsole.DBHelpers
{
    internal class SchemesInterface
    {
        #region "Dispose Logic"
        private bool disposed = false;
        public string stringConn;

        /// <summary>
        /// Constructor
        /// </summary>
        public SchemesInterface()
        {
            DBConnection dbc = new DBConnection();
            // Determine running mode 
            stringConn = dbc.ReadSetting("SCHEMES" + (dbc.ReadSetting("MODE")).ToUpper().ToString());
            Dispose(false);
        }

        /// <summary>
        /// Implements Dipose method
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Overrides Dispose method
        /// </summary>
        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!this.disposed)
            {
                disposed = true;
            }
        }

        #endregion

        public ObservableCollection<ItemsToInterface> GetItems()
        {
            IEnumerable<ItemsToInterface> output;
            ObservableCollection<ItemsToInterface> result = new ObservableCollection<ItemsToInterface>();
            using (IDbConnection Conn = new SqlConnection(stringConn))
            {
                output = Conn.Query<ItemsToInterface>("GetAllReadyShipments", commandType: CommandType.StoredProcedure);
            }

            foreach (var item in output)
            {
                result.Add(item);
            }
            return result;
        }


        public void CreateXMLFiles(ObservableCollection<ItemsToInterface> model)
        {
            IEnumerable<string> result;
            foreach (var item in model)
            {
                using (IDbConnection Conn = new SqlConnection(stringConn))
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@waveNo", item.Launch_Num);
                    param.Add("@item", item.Item);
                    result = Conn.Query<string>("CreateXML", param, commandType: CommandType.StoredProcedure);
                }
                var fullResult = string.Concat(result);
                CreateXMLFile(item.Item, fullResult);
            }

        }

        private void CreateXMLFile(string item, string fullResult)
        {
            string _file = "";
            string FilePathDestinationFile = item + DateTime.Now.ToString("yyyyMMddHHmmss") + ".shxml.xml";

            try
            {
                using (FileStream fs = File.Create(ConfigurationManager.AppSettings["SOURCE"] + Path.GetFileName(FilePathDestinationFile)))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(fullResult);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }
            catch (Exception ex)
            {
                //ErrorLogParams exparams = new ErrorLogParams();
                //exparams.ITEM = "File";
                //exparams.DESCRIPTION = ex.Message.ToString();
                //exparams.FILE_NAME = _file.ToString();
                //Update_ErrLog(exparams);
            }
        }

    }
}
