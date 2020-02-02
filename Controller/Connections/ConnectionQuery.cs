using System;
using System.Reflection;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace MicroORM {
    /// <summary>
    /// Esta classe tem a capacidade de gerar as string de operacoes de banco de dados
    /// O nome do objeto e suas propriedades sao acessados usando reflection
    /// </summary>
    internal static class ConnectionQuery {
        /// <summary>
        /// Cria a tabela por reflexão
        /// </summary>
        /// <param name="name">nome do campo</param>
        /// <param name="value">tipo do campo</param>
        /// <param name="optional">define se o campo e final ou não</param>
        /// <returns>Retorna os campos da tabela</returns>
        private static string CheckTable(string name, string value, string optional) {
            StringBuilder temp = new StringBuilder(null);
            switch (value.ToUpper()) {
                case "BOOLEAN": {
                        temp.Append($"{name} bool{optional}");
                    }
                    break;
                case "INT16": {
                        temp.Append($"{name} int{optional}");
                    }
                    break;
                case "INT32": {
                        temp.Append($"{name} int{optional}");
                    }
                    break;
                case "INT64": {
                        temp.Append($"{name} long{optional}");
                    }
                    break;
                case "STRING": {
                        temp.Append($"{name} varchar(128){optional}");
                    }
                    break;
                case "CHAR": {
                        temp.Append($"{name} varchar(1){optional}");
                    }
                    break;
                case "FLOAT": {
                        temp.Append($"{name} numeric(5, 2){optional}");
                    }
                    break;
                case "DOUBLE": {
                        temp.Append($"{name} numeric(10, 2){optional}");
                    }
                    break;
                case "DECIMAL": {
                        temp.Append($"{name} numeric(20, 4){optional}");
                    }
                    break;
                case "DATETIME": {
                        temp.Append($"{name} timestamp{optional}");
                    }
                    break;
            }
            return temp.ToString();
        }
        /// <summary>
        /// Completamenta a função de criação de campos
        /// </summary>
        /// <param name="fields">variavel que armazena os valores do campo</param>
        /// <param name="properties">os tipos de variaveis da tabela</param>
        /// <param name="i">o indice da variavel</param>
        /// <param name="values">o valor da variável</param>
        /// <param name="optional">define se o campos é final ou não</param>
        private static void CheckString(StringBuilder fields, PropertyInfo[] properties, int i, StringBuilder values, string optional) {
            fields.Append($"{properties[i].Name}{optional}");
            values.Append($"@{properties[i].Name}{optional}");
        }
        /// <summary>
        /// Query da função CREATE TABLE
        /// </summary>
        /// <param name="obj">objeto a ser verificado</param>
        /// <returns>Retornar um comando SQL</returns>
        internal static string QueryCreateTable(this Object obj) {
            string table = obj.GetType().Name;
            PropertyInfo[] properties = obj.GetType().GetProperties();
            StringBuilder create = new StringBuilder($"create table if not exists {table} (");
            for (int i = 0; i < properties.Length; i++) {
                var atr = CheckProp(properties[i]);
                if (atr != null) {
                    if (atr.Tipo.Equals(ConnectionEnum.FIELD)) {
                        create.Append((!properties[i].Name.ToUpper().Equals("ID")) ?
                            CheckTable(properties[i].Name,
                            properties[i].PropertyType.Name,
                            (i < properties.GetUpperBound(0) ? "," : ")")) :
                            $"{properties[i].Name} serial unique primary key not null, ");
                    }
                }
            }
            if (create[create.Length - 1] == ',')
                create[create.Length - 2] = ')';
            return create.ToString();
        }
        private static ORMAttribute CheckProp(PropertyInfo info) {
            var _var = info.GetCustomAttributes(typeof(ORMAttribute), false);
            if (_var != null)
                return (ORMAttribute)_var[0];
            else
                return null;
        }
        /// <summary>
        /// Query da funação IMSERT
        /// </summary>
        /// <param name="obj">objeto a ser verificado</param>
        /// <returns>Retornar um comando SQL</returns>
        internal static string QueryAdd(this Object obj) {
            string table = obj.GetType().Name;
            PropertyInfo[] properties = obj.GetType().GetProperties();
            StringBuilder fields = new StringBuilder("(");
            StringBuilder values = new StringBuilder("(");
            for (int i = 0; i < properties.Length; i++) {
                var atr = CheckProp(properties[i]);
                if (atr != null) {
                    if (atr.Tipo.Equals(ConnectionEnum.FIELD)) {
                        if (!properties[i].Name.ToUpper().Equals("ID"))
                            CheckString(fields, properties, i, values, ((i < properties.GetUpperBound(0) ? "," : ")")));
                    }
                }
            }
            if (fields[fields.Length - 1] == ',')
                fields[fields.Length - 1] = ')';
            if (values[values.Length - 1] == ',')
                values[values.Length - 1] = ')';
            return ($"INSERT INTO {table}{fields.ToString()} VALUES {values.ToString()}").ToString();
        }
        /// <summary>
        /// Query da função REMOVE
        /// </summary>
        /// <param name="obj">objeto a ser verificado</param>
        /// <returns>Retornar um comando SQL</returns>
        internal static string QueryRemove(this Object obj) {
            string table = obj.GetType().Name;
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return $"DELETE FROM {table} WHERE {properties[0].Name} = @{properties[0].Name}";
        }
        /// <summary>
        /// Query da função UPDATE
        /// </summary>
        /// <param name="obj">objeto a ser verificado</param>
        /// <returns>Retornar um comando SQL</returns>
        internal static string QueryUpdate(this Object obj) {
            string table = obj.GetType().Name;
            PropertyInfo[] properties = obj.GetType().GetProperties();
            StringBuilder fields = new StringBuilder("");
            for (int i = 0; i < properties.Length; i++) {
                var atr = CheckProp(properties[i]);
                if (atr != null) {
                    if (atr.Tipo.Equals(ConnectionEnum.FIELD)) {
                        if (!properties[i].Name.ToUpper().Equals("ID")) {
                            fields.Append((i < properties.GetUpperBound(0)) ? $"{properties[i].Name} = @{properties[i].Name}," : $"{properties[i].Name} = @{properties[i].Name}");
                        }
                    }
                }
            }
            if (fields[fields.Length - 1] == ',')
                fields[fields.Length - 1] = ' ';
            return $"UPDATE {table} SET {fields.ToString()} WHERE {properties[0].Name} = @{properties[0].Name}";
        }
        /// <summary>
        /// Query da função FIND BY ID
        /// </summary>
        /// <param name="obj">objeto a ser verificado</param>
        /// <returns>Retornar um comando SQL</returns>
        public static string QueryFindById(this Object obj) {
            string table = obj.GetType().Name;
            PropertyInfo[] properties = obj.GetType().GetProperties();
            return $"SELECT *FROM {table} WHERE {properties[0].Name} = @{properties[0].Name}";
        }
        /// <summary>
        /// Query da função SELECT *FROM
        /// </summary>
        /// <param name="obj">objeto a ser verificado</param>
        /// <returns>Retornar um comando SQL</returns>
        internal static string QueryFindAll(this Object obj) {
            string table = obj.GetType().Name;
            return $"SELECT *FROM {table}";
        }
    }
}
