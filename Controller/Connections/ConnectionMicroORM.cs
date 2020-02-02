using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using Dapper;
using System.IO;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace MicroORM {
    /// <summary>
    /// Classe de connexão dinamica para banco de dados MySQL
    /// </summary>
    /// <typeparam name="T">Representa a classe que vai ser manipulada pelo banco de dados</typeparam>
    public class ConnectionMicroORM <T>{
        /// <summary>
        /// Variavel de conexão padrão
        /// </summary>
        private string _connectionString { get; }
        /// <summary>
        /// Valor personalizado de insercao
        /// </summary>
        /// <value>Recebe uma query</value>
        public string AddString      { get; set; }
        /// <summary>
        /// Valor personalizado de remocao 
        /// </summary>
        /// <value>Recebe uma query</value>
        public string RemoveString   { get; set; }
        /// <summary>
        /// Valor personalizado de atualizacao
        /// </summary>
        /// <value>Recebe uma query</value>
        public string UpdateString   { get; set; }
        /// <summary>
        /// Valor personalizado de busca por identificacao
        /// </summary>
        /// <value>Recebe uma query</value>
        public string FindByIdString { get; set; }
        /// <summary>
        /// Valor personalizado de busca completa
        /// </summary>
        /// <value>Recebe uma query</value>
        public string FindAllString  { get; set; }
        /// <summary>
        /// Valor para uso no campo de execucao personalizada
        /// </summary>
        /// <value>Recebe uma query</value>
        public string CustomString   { get; set; }
        private SoftwareEnum _software { get; }
        /// <summary>
        /// Cosntrutor de connexao, recebe os parametros de acesso ao banco de dados
        /// </summary>
        /// <param name="user">o nome de usúario do banco de dados</param>
        /// <param name="password">a senha do de acesso ao banco de dados</param>
        /// <param name="database">o banco que se deseja acessar</param>
        /// <param name="server">o endereco do servidor padrão, é definido automaticamente como localhost</param>
        /// <param name="software">o tipo de conexão [<see cref="SoftwareEnum.MYSQL"/>, <see cref="SoftwareEnum.POSTGRESQL"/>)</param>
        public ConnectionMicroORM(
            SoftwareEnum software,
            string user,
            string password,
            string database,
            string server = ConnectionConfig.LOCALHOST) {
            _connectionString = new ConnectionConfig(
                new ConnectioBean(
                    software, 
                    server  : server, 
                    database: database, 
                    user    : user, 
                    password: password)).GetConfig;
            DataBaseLog    = null;
            AddString      = null;
            RemoveString   = null;
            UpdateString   = null;
            FindByIdString = null;
            FindAllString  = null;
            CustomString   = null;
            _software = software;
            if(!typeof(T).Equals(typeof(object)))
                CheckTable();
        }
        private DbConnection OpenDB() {
            DbConnection _database = null;
                switch (_software) {
                    case SoftwareEnum.MYSQL: {
                    _database = new MySqlConnection(_connectionString);
                }
                break;
                    case SoftwareEnum.POSTGRESQL: {
                    _database = new NpgsqlConnection(_connectionString);
                }
                break;
            }
            return _database;
        }
        /// <summary>
        /// Ativa ou desativa o monitoramento de logs de eventos do banco de dados
        /// </summary>
        /// <param name="active">defina como true para ativar e false para desativar</param>
        public void InitializeLogs(bool active) {
            if (active && DataBaseLog == null)
                InitializeLogs();
            else if(!active) {
                DataBaseLog = null;
            }
        }
        /// <summary>
        /// Verifica se ja existe um tabela do tipo referenciado no banco de dadps
        /// </summary>
        /// <returns>The table.</returns>
        private int CheckTable() {
            return ConnectionException<int>.AsyncCheck(() => {
                using (var connection = OpenDB()) {
                    connection.Open();
                    try {
                        return connection.Execute($"SELECT *FROM {Activator.CreateInstance<T>().GetType().Name}");
                    } catch (Exception) {
                        var instance = Activator.CreateInstance<T>();
                        return connection.Execute(instance.QueryCreateTable());
                    }
                }
            });
        }
        /// <summary>
        /// Adiciona um elemento do tipo <see cref="IORM{T}"/> no banco de dados
        /// </summary>
        /// <param name="item">objeto de classe que implementa e interface <see cref="IORM{T}"/></param>
        /// <returns>retorna um valor numérico indicando o resultado da operação</returns>
        public int? Add(IORM<T> item) {
            return ConnectionException<int>.AsyncCheck(() => {
                using (var connection = OpenDB()) {
                    connection.Open();
                    DataBaseLog?.Invoke(this, new ConnectionEventArgs<T>($"inserção do registro: {item.GetThis()}"));
                    if (AddString == null)
                        AddString = item.GetThis().QueryAdd();
                    return connection.Execute(AddString, item);
                }
            });
        }
        /// <summary>
        /// Seleciona todos elementos do tipo <see cref="IORM{T}"/> no banco de dados
        /// </summary>
        /// <returns>retorna um lista de elementos <see cref="IORM{T}"/></returns>
        public List<T> FindAll() {
            return ConnectionException<T>.AsyncCheck(() => {
                if (!(Activator.CreateInstance<T>() is IORM<T>)) throw new ArgumentException("class not implement interface IBean");
                using (var connection = OpenDB()) {
                    connection.Open();
                    DataBaseLog?.Invoke(this, new ConnectionEventArgs<T>("listagem de dados"));
                    if (FindAllString == null)
                        FindAllString = Activator.CreateInstance<T>().QueryFindAll();
                    return ReflectionConvertList(connection.Query(FindAllString));
                }
            });
        }
        private List<T> ReflectionConvertList(IEnumerable<dynamic> query) {
            var types = Enumerable.Repeat(typeof(object), typeof(T).GetProperties().Length).ToArray();
            var list = new List<T>();
            query.ToList<dynamic>().ForEach(i => list.Add(ReflectionConvert(i)));
            return list;
        }
        /// <summary>
        /// Pesquisa um elemento do tipo <see cref="IORM{T}"/> no banco de dados com base em seu Id
        /// </summary>
        /// <param name="Id">número de indentificação</param>
        /// <returns>retorna um tipo <see cref="IORM{T}"/></returns>
        public T FindById(int Id) {
            return ConnectionException<T>.AsyncCheck((Func<T>)(() => {
                if (!(Activator.CreateInstance<T>() is IORM<T>)) throw new ArgumentException("class not implement interface IBean");
                using (var connection = OpenDB()) {
                    connection.Open();
                    DataBaseLog?.Invoke(this, new ConnectionEventArgs<T>($"registro pesquisado: {Id}"));
                    if (FindByIdString == null)
                        FindByIdString = Activator.CreateInstance<T>().QueryFindById();
                    return ReflectionConvert(connection.Query(FindByIdString, new { id = Id }).FirstOrDefault());
                }
            }));
        }
        private T ReflectionConvert(dynamic query) {
            Task<T> task = new Task<T>(() => {
                var types = Enumerable.Repeat(typeof(object), typeof(T).GetProperties().Length).ToArray();
                var result = ((IDictionary<string, object>)query).Values.ToList();
                var tipo = typeof(T);
                var construtor = tipo.GetConstructor(types);
                return (T)construtor.Invoke(result.ToArray());
            });
            task.RunSynchronously();
            return task.Result;
        }
        /// <summary>
        /// Remove um elemento do tipo <see cref="IORM{T}"/> do banco de dados
        /// </summary>
        /// <param name="Id">número de identificação</param>
        /// <returns>retorna um valor numérico indicando o resultado da operação</returns>
        public int Remove(int Id) {
            if (!(Activator.CreateInstance<T>() is IORM<T>)) throw new ArgumentException("class not implement interface IBean");
            return ConnectionException<int>.AsyncCheck(() => {
                using (var connection = OpenDB()) {
                    connection.Open();
                    DataBaseLog?.Invoke(this, new ConnectionEventArgs<T>($"remoção do registro: {Id}"));
                    if (RemoveString == null)
                        RemoveString = Activator.CreateInstance<T>().QueryRemove();
                    return connection.Execute(RemoveString, new { id = Id });
                }
            });
        }
        /// <summary>
        /// Atualiza os dados to tipo <see cref="IORM{T}"/> que e passado no parâmetro
        /// </summary>
        /// <param name="item">objeto de classe que implementa e interface <see cref="IORM{T}"/></param>
        /// <returns>retorna um valor numérico indicando o resultado da operação</returns>
        public int Update(IORM<T> item) {
            return ConnectionException<int>.AsyncCheck(() => {
                using (var connection = OpenDB()) {
                    connection.Open();
                    DataBaseLog?.Invoke(this, new ConnectionEventArgs<T>($"atualização de dados: {item.GetThis()}"));
                    if (UpdateString == null)
                        UpdateString = item.GetThis().QueryUpdate();
                    return connection.Execute(UpdateString, item);
                }
            });
        }
        /// <summary>
        /// Realiza operacoes customizadas no banco de dados
        /// </summary>
        /// <returns>Retorna o objeto resultante da operacao</returns>
        /// <param name="obj">Dados de objetos para serem verificados no banco de daods</param>
        public object Custom(object obj = null) {
            try {
                using (var connection = OpenDB()) {
                    connection.Open();
                    DataBaseLog?.Invoke(this, new ConnectionEventArgs<T>($"execucao avancada"));
                    if (CustomString != null && obj != null)
                        return connection.Query<object>(CustomString, obj);
                    else if (CustomString != null)
                        return connection.Query<object>(CustomString);
                    throw new ArgumentNullException($"referencia nula fornecida {nameof(obj)} ou {nameof(CustomString)}");
                }
            } catch (Exception e) {
                DataBaseLog?.Invoke(this, new ConnectionEventArgs<T>($"{e.Message}"));
            }
            return null;
        }
        /// <summary>
        /// Exevcuta um comando sql apartir de um arquivo
        /// </summary>
        /// <param name="url">Diretorio do arquivo</param>
        public void OfTheFile(string url) {
            try {
                using (Stream stream = File.Open($"{url}", FileMode.Open))
                using (StreamReader reader = new StreamReader(stream)) {
                    using (var connection = OpenDB()) {
                        connection.Open();
                        connection.Execute(reader.ReadToEnd());
                    }
                }
            } catch(IOException e) {
                DataBaseLog?.Invoke(this, new ConnectionEventArgs<T>($"{e.Message}"));
            } catch(Exception e) {
                DataBaseLog?.Invoke(this, new ConnectionEventArgs<T>($"{e.Message}"));
            }
        }
        /// <summary>
        /// Define o tipo de evento que sera registro na operações do banco de dados
        /// </summary>
        public event EventHandler DataBaseLog;
        /// <summary>
        /// Padrão para inserção em manipuladores de eventos que utilizam o prefixo On
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnDataBaseLog(EventArgs args) {
            var handler = DataBaseLog;
            handler?.Invoke(this, args);
        }

        private delegate void DataBaseLogEventHandler(Object obj, EventArgs args);
        /// <summary>
        /// Implementa a verificacao de eventos
        /// </summary>
        private void InitializeLogs() {
            this.DataBaseLog += (sender, args) => {
                ConnectionException<int>.AsyncCheck(() => {
                    using (var connection = OpenDB()) {
                        connection.Open();
                        string sql = "create table if not exists logs (" +
                                        "Id serial unique primary key not null," +
                                        "Tabela varchar(128) not null," +
                                        "Descricao varchar(128) not null," +
                                        "Horario timestamp not null)";
                        connection.Execute(sql);
                        return connection.Execute("insert into logs(Tabela, Descricao, Horario) values (@Tabela, @Descricao, @Horario)", args);
                    }
                });
            };
        }
    }
}
