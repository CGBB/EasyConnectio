using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MicroORM.SoftwareEnum;

namespace MicroORM {
    internal class ConnectionConfig {
        internal const string LOCALHOST = "localhost";
        public string GetConfig { get; }
        public ConnectionConfig(ConnectioBean bean) {
            switch(bean.Software) {
                case MYSQL: {
                        this.GetConfig = $"Server={bean.Server}; Database={bean.Database}; Uid={bean.User}; Pwd={bean.Password}";
                    } break;
                case POSTGRESQL: {
                        this.GetConfig = $"Server={bean.Server}; Port=5432; Database={bean.Database}; User Id={bean.User}; Password={bean.Password}";
                    } break;
            }
            
        }
    }

    internal class ConnectioBean {
        public SoftwareEnum Software { get; set; }
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public ConnectioBean(
            SoftwareEnum software,
            string server   = null,
            string user     = null,
            string password = null,
            string database = ConnectionConfig.LOCALHOST) {

            if (string.IsNullOrEmpty(server) ||
                string.IsNullOrEmpty(user) ||
                string.IsNullOrEmpty(password))
                throw new ArgumentNullException($"Only essential field [{nameof(server)}, {nameof(user)}, {nameof(password)}] is null or empty");
            this.Software = software;
            this.Server   = server;
            this.User     = user;
            this.Password = password;
            this.Database = database;
        }
    }
}
