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

        public async Task<List<BusRoute>> GetBusRoutes(int busServiceId)
        {
            var connection = await GetDatabaseConnectionAsync<BusRoute>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.Table<BusRoute>().Where(route => route.BusServiceId == busServiceId).ToListAsync());
        }

        public async Task<List<BusService>> GetBusServices()
        {
            var connection = await GetDatabaseConnectionAsync<BusService>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.Table<BusService>().ToListAsync()).ConfigureAwait(false);
        }

        public async Task<List<BusStation>> GetStationsForRoute(BusRoute route)
        {
            var connection = await GetDatabaseConnectionAsync<BusStation, BusRouteDetails>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<BusStation>("Select * From BusStation WHERE Id IN (SELECT BusStopId FROM BusRouteDetails WHERE BusRouteId = ?)", route.Id));
        }

        public async Task<List<BusRouteDetails>> GetStationsDetailsForRouteVariant(BusRoute route, int routeVariant)
        {
            var connection = await GetDatabaseConnectionAsync<BusRouteDetails>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.Table<BusRouteDetails>().Where(station => station.BusRouteId == route.Id && station.RouteVariant == routeVariant).ToListAsync()).ConfigureAwait(false);
        }

        public async Task<List<RouteBeginTime>> GetRouteBeginTimes(BusRoute route)
        {
            var connection = await GetDatabaseConnectionAsync<RouteBeginTime>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.Table<RouteBeginTime>().Where(time => time.RouteId == route.Id).ToListAsync()).ConfigureAwait(false);
        }

        public async Task<List<BusRouteDetails>> GetStationDetailsForRoute(BusRoute route, BusStation station)
        {
            var connection = await GetDatabaseConnectionAsync<BusRouteDetails>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<BusRouteDetails>("Select * From BusRouteDetails Where BusRouteId = ? And BusStopId = ?", route.Id, station.Id)).ConfigureAwait(false);
        }

        public async Task<List<StationTimeAdjustment>> GetTimeAdjustmentForRoute(int routeId)
        {
            var connection = await GetDatabaseConnectionAsync<StationTimeAdjustment>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<StationTimeAdjustment>("Select * From StationTimeAdjustment Where RouteId = ?", routeId));
        }

        public Task<List<BusStation>> GetBusRoutes(List<BusRouteDetails> stationsDetails)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(ScheduleData schedule)
        {
            await UpdateTableAsync(schedule.BusServices);
            await UpdateTableAsync(schedule.Routes);
            await UpdateTableAsync(schedule.BusStations);
            await UpdateTableAsync(schedule.RoutesBeginTimes);
            await UpdateTableAsync(schedule.RoutesDetails);
            await UpdateTableAsync(schedule.TimeAdjustments);
        }

        private async Task UpdateTableAsync<T>(IList<T> data)
        {
            try
            {
                var connection = await GetDatabaseConnectionAsync<T>().ConfigureAwait(false);
                await AttemptAndRetry(() => connection.DeleteAllAsync<T>()).ConfigureAwait(false);
                await AttemptAndRetry(() => connection.InsertAllAsync(data)).ConfigureAwait(false);
            }
            catch(SQLiteException exc)
            {
                var msg = exc.Message;
            }
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

        protected async ValueTask<SQLiteAsyncConnection> GetDatabaseConnectionAsync<T,U>()
        {
            if (!(_connection.TableMappings.Any(x => x.MappedType == typeof(T)) && _connection.TableMappings.Any(t => t.MappedType == typeof(U))))
            {
                await _connection.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                await _connection.CreateTablesAsync(CreateFlags.None, new Type[] { typeof(T), typeof(U) }).ConfigureAwait(false);
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
