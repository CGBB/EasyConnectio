using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORM{
    /// <summary>
    /// Define os tipos de banco de dados
    /// </summary>
    public enum SoftwareEnum {
        /// <summary>
        /// Banco de dados MySQL
        /// </summary>
        MYSQL = 0,
        /// <summary>
        /// Banco de dados PostgreSQL
        /// </summary>
        POSTGRESQL = 1
    }
}
