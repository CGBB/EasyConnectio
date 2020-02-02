using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace MicroORM {
    /// <summary>
    /// Classe de ponte de connexão
    /// </summary>
    /// <typeparam name="T">Classe de connexão</typeparam>
    internal class ConnectionBase<T> {
        /// <summary>
        /// Criar uma connexão com o banco de dados
        /// </summary>
        /// <param name="config">Parâmetros de connexão</param>
        /// <returns>Retorna uma instância do database connection</returns>
        internal DbConnection OpenDB(string config) {
            var database = typeof(T).GetConstructor(new Type[] { typeof(string) });
            return (DbConnection)database?.Invoke(new object[] { config });
        } 
    }
}
