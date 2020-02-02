using System;
namespace MicroORM {
    /// <summary>
    /// Classe de attributo
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ORMAttribute: Attribute {
        /// <summary>
        /// Define o tipo de atributo <see cref="ConnectionEnum.FIELD"/> ou <see cref="ConnectionEnum.ONE_ALL"/>
        /// </summary>
        public ConnectionEnum Tipo { get; set; }
        /// <summary>
        /// Construtor, recebe um enum que determina o tipo de operacao no campo da classe
        /// </summary>
        /// <param name="tipo">Tipo.</param>
        public ORMAttribute(ConnectionEnum tipo) {
            Tipo = tipo;
        }
    }
}
