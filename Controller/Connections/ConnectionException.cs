using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MicroORM {
    internal static class ConnectionException<T> {
        /// <summary>
        /// Manipula excecoes para operacoes de retorno
        /// </summary>
        /// <returns>retorna o resultado da <see cref="Func{TResult}"/></returns>
        /// <param name="funtion">Funtion.</param>
        internal static T AsyncCheck(Func<T> funtion) {
            var _task = new Task<T>(() => {
                try {
                    return funtion.Invoke();
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                    return default(T);
                }
            });
            _task.RunSynchronously();
            return _task.Result;
        }
        /// <summary>
        /// Manipula exececoes para operacoes de retorno <see cref="List{T}"/>
        /// </summary>
        /// <returns>retorna o resultado da <see cref="Func{TResult}"/></returns>
        /// <param name="funtion">Funtion.</param>
        internal static List<T> AsyncCheck(Func<List<T>> funtion) {
            var _task = new Task<List<T>>(() => {
                try {
                    return funtion.Invoke();
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                    return null;
                }
            });
            _task.RunSynchronously();
            return _task.Result;
        }

    }
}
