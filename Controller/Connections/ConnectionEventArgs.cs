using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroORM {
    internal class ConnectionEventArgs<T>: EventArgs{
        [Required]
        public int Id { get; set; }
        [Required]
        public string Tabela { get; set; }
        [Required]
        public string Descricao { get; set; }
        [Required]
        public DateTime Horario { get; set; }

        public ConnectionEventArgs(string _description) {
            this.Tabela = Activator.CreateInstance<T>().GetType().Name;
            this.Descricao = _description;
            this.Horario = DateTime.Now;
        }

        public ConnectionEventArgs() { }
    }
}
