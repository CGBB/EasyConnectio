using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySQLMicroORM {
    class DynamicEventArgs: EventArgs {
        public dynamic Obj { get; set; }
    }
}
