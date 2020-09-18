using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Polly;

namespace BusSchedule.Providers
{
    public class SQLDataProvider : IDataProvider
    {
        private readonly SQLiteAsyncConnection _connection;

        public SQLDataProvider()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "schedule.db3");
            _connection = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
        }

        public Task<List<BusRoute>> GetBusRoutes(int busServiceId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<BusService>> GetBusServices()
        {
            var connection = await GetDatabaseConnectionAsync<BusService>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.Table<BusService>().ToListAsync()).ConfigureAwait(false);
        }

        public async Task UpdateAsync(ScheduleData schedule)
        {
            var connection = await GetDatabaseConnectionAsync<BusService>().ConfigureAwait(false);
            await AttemptAndRetry(() => connection.DeleteAllAsync<BusService>()).ConfigureAwait(false);
            await AttemptAndRetry(() => connection.InsertAllAsync(schedule.BusServices)).ConfigureAwait(false);
        }

        protected async ValueTask<SQLiteAsyncConnection> GetDatabaseConnectionAsync<T>()
        {
            if (!_connection.TableMappings.Any(x => x.MappedType == typeof(T)))
            {
                await _connection.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                await _connection.CreateTablesAsync(CreateFlags.None, typeof(T)).ConfigureAwait(false);
            }

            return _connection;
        }

        protected Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries = 12)
        {
            return Policy.Handle<SQLiteException>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(action);

            TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));
        }
    }
}
