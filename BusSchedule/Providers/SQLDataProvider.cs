﻿using BusSchedule.Core.Model;
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

        public SQLDataProvider(string databasePath)
        {
            _connection = new SQLiteAsyncConnection(databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache);
        }

        public async Task<List<Routes>> GetRoutes()
        {
            var connection = await GetDatabaseConnectionAsync<Routes>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.Table<Routes>().ToListAsync()).ConfigureAwait(false);
        }

        public async Task<List<Stops>> GetStopsForRoute(Routes route, int direction)
        {
            var connection = await GetDatabaseConnectionAsync<Stops, Route_Stop>().ConfigureAwait(false);
            var routeStops = (await AttemptAndRetry(() => connection.QueryAsync<Route_Stop>("Select * From Route_Stop Where route_id = ? And direction_id = ? Order by stop_sequence", route.Route_Id, direction))).Select(rs => rs.Stop_Id).ToList();
            var temp = await AttemptAndRetry(() => connection.Table<Stops>().Where(stop => routeStops.Contains(stop.Stop_Id)).ToListAsync());
            return temp.OrderBy(stop => routeStops.IndexOf(stop.Stop_Id)).ToList();
            //return await AttemptAndRetry(() => connection.Table<Stops>().Where(stop => routeStops.Contains(stop.Stop_Id)).OrderBy(stop => routeStops.IndexOf(stop.Stop_Id)).ToListAsync());
            //return await AttemptAndRetry(() => connection.QueryAsync<Stops>("Select * From Stops Where stop_id In (Select stop_id From Route_Stop Where route_id = ? And direction_id = ? Order by stop_sequence)", route.Route_Id, direction));
            //return stops.OrderBy(stop => routeStops.IndexOf(stop.Stop_Id)).ToList();
        }

        public async Task<Destination> GetRouteDestinations(Routes route)
        {
            var connection = await GetDatabaseConnectionAsync<Destination>().ConfigureAwait(false);
            var all = await AttemptAndRetry(() => connection.Table<Destination>().ToListAsync());
            return await AttemptAndRetry(() => connection.Table<Destination>().Where(dest => dest.Route_Id == route.Route_Id).FirstAsync());
        }

        public async Task<IEnumerable<Trips>> GetTripsForRoute(Routes route, int direction)
        {
            var connection = await GetDatabaseConnectionAsync<Trips>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<Trips>("Select * From Trips Where route_id = ? And direction_id = ?", route.Route_Id, direction));
        }

        public async Task<IEnumerable<Stop_Times>> GetStopTimesForTrip(Trips trip)
        {
            var connection = await GetDatabaseConnectionAsync<Stop_Times>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<Stop_Times>("Select * From Stop_Times Where trip_id = ?", trip.Trip_Id));
        }

        public async Task<Stops> GetStopById(string stopId)
        {
            var connection = await GetDatabaseConnectionAsync<Stops>().ConfigureAwait(false);
            var stops = await AttemptAndRetry(() => connection.QueryAsync<Stops>("Select * From Stops Where stop_id = ?", stopId));
            return stops.FirstOrDefault();
        }

        public async Task<IEnumerable<Trips>> GetTripsForRoute(Routes route, int direction, string serviceId)
        {
            var connection = await GetDatabaseConnectionAsync<Trips>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<Trips>("Select * From Trips Where route_id = ? And direction_id = ? And service_id = ?", route.Route_Id, direction, serviceId));
        }

        public async Task<IEnumerable<Stop_Times>> GetStopTimesForTrip(string tripId, string stopId)
        {
            var connection = await GetDatabaseConnectionAsync<Stop_Times>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<Stop_Times>("Select * From Stop_Times Where trip_id = ? And stop_id = ?", tripId, stopId));
        }

        public async Task<IEnumerable<Calendar>> GetCalendar()
        {
            var connection = await GetDatabaseConnectionAsync<Calendar>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.Table<Calendar>().ToListAsync());
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
