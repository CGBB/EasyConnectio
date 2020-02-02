using System;
namespace MicroORM {
    /// <summary>
    /// Determina o tipo de campos do objeto de conexao
    /// </summary>
    public enum ConnectionEnum {
        /// <summary>
        /// Campos de atributo
        /// </summary>
        FIELD = 0,
        /// <summary>
        /// Campos de composição
        /// </summary>
        ONE_ALL = 1
    }
}
