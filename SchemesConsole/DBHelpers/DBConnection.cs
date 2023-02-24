using System.Configuration;

namespace CubiscanConsole.DBHelpers
{
    public class DBConnection : IDisposable
    {
        #region "Dispose Logic"
        private bool disposed = false;

        /// <summary>
        /// Constructor
        /// </summary>
        public DBConnection()
        {
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

        public string ReadSetting(string key)
        {
            string result = null;
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                result = appSettings[key] ?? "Not Found";

            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
            return result;

        }
    }
}
