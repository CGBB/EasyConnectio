using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORM {
    /// <summary>
    /// Interface padrão de acesso para acesso a classe de conexão
    /// </summary>
    /// <typeparam name="T">Tipo de classe que esta implementando a interface</typeparam>
    public interface IORM <T> {
        /// <summary>
        /// Pega a instancia atual do elemento
        /// </summary>
        /// <returns>Retorna o elemento anônimo</returns>
        T GetThis();
    }
}
