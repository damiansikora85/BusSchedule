﻿using BusSchedule.Core.Model;
using BusSchedule.Core.Utils;
using Polly;
using SQLite;
using System.Globalization;
using Point = BusSchedule.Core.Model.Point;

namespace BusSchedule.Providers
{
    public class SQLDataProvider : IDataProvider
    {
        private Lazy<SQLiteAsyncConnection> _connection;
        private string _databasePath;

        public SQLDataProvider()
        {
            _connection = new Lazy<SQLiteAsyncConnection>(() => new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache));
        }

        public SQLDataProvider(string databasePath)
        {
            _databasePath = databasePath;
            _connection = new Lazy<SQLiteAsyncConnection>(() => new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache));
        }

        public void SetDatabasePath(string databasePath)
        {
            _databasePath = databasePath;
            _connection = new Lazy<SQLiteAsyncConnection>(() => new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache));
        }

        public async Task<List<Routes>> GetRoutes()
        {
            var connection = await GetDatabaseConnectionAsync<Routes>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.Table<Routes>().ToListAsync()).ConfigureAwait(false);
        }

        public async Task<Routes> GetRoute(string routeId)
        {
            var connection = await GetDatabaseConnectionAsync<Routes>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.Table<Routes>().Where(route => route.Route_Id == routeId).FirstOrDefaultAsync());
        }

        public async Task<List<Stops>> GetStopsForRoute(Routes route, int direction)
        {
            var connection = await GetDatabaseConnectionAsync<Stops, Route_Stop>().ConfigureAwait(false);
            var routeStops = (await AttemptAndRetry(() => connection.QueryAsync<Route_Stop>("Select * From Route_Stop Where route_id = ? And direction_id = ? Order by stop_sequence", route.Route_Id, direction))).Select(rs => rs.Stop_Id).ToList();
            //var allStops = await AttemptAndRetry(() => connection.QueryAsync<Stops>("Select * From Stops"));
            var temp = await AttemptAndRetry(() => connection.Table<Stops>().Where(stop => routeStops.Contains(stop.Stop_Id)).ToListAsync());
            return temp.OrderBy(stop => routeStops.IndexOf(stop.Stop_Id)).ToList();
        }

        public async Task<List<Stops>> GetStopsForRoute(Routes route)
        {
            var connection = await GetDatabaseConnectionAsync<Stops, Route_Stop>().ConfigureAwait(false);
            var routeStops = (await AttemptAndRetry(() => connection.QueryAsync<Route_Stop>("Select * From Route_Stop Where route_id = ? Order by stop_sequence", route.Route_Id))).Select(rs => rs.Stop_Id).ToList();
            //var allStops = await AttemptAndRetry(() => connection.QueryAsync<Stops>("Select * From Stops"));
            var temp = await AttemptAndRetry(() => connection.Table<Stops>().Where(stop => routeStops.Contains(stop.Stop_Id)).ToListAsync());
            return temp.OrderBy(stop => routeStops.IndexOf(stop.Stop_Id)).ToList();
        }

        public async Task Test()
        {
            var connection = await GetDatabaseConnectionAsync<Stops>().ConfigureAwait(false);
            var allStops = await AttemptAndRetry(() => connection.Table<Stops>().ToListAsync());
            return;
        }

        public async Task<Destination> GetRouteDestinations(Routes route)
        {
            var connection = await GetDatabaseConnectionAsync<Destination>().ConfigureAwait(false);
            //var all = await AttemptAndRetry(() => connection.Table<Destination>().ToListAsync());
            return await AttemptAndRetry(() => connection.Table<Destination>().Where(dest => dest.Route_Id == route.Route_Id).FirstAsync());
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
            return stops.Any() ? stops.FirstOrDefault() : null;
        }

        public async Task<IEnumerable<Trips>> GetTripsForRoute(string routeId, string serviceId)
        {
            var connection = await GetDatabaseConnectionAsync<Trips>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<Trips>("Select * From Trips Where route_id = ? And service_id = ?", routeId, serviceId));
        }

        public async Task<IEnumerable<Trips>> GetTripsForRoute(string routeId, int direction)
        {
            var connection = await GetDatabaseConnectionAsync<Trips>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<Trips>("Select * From Trips Where route_id = ? And direction_id = ?", routeId, direction));
        }

        private async Task<IEnumerable<Trips>> GetTripsForRoute(string routeId)
        {
            var connection = await GetDatabaseConnectionAsync<Trips>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<Trips>("Select * From Trips Where route_id = ?", routeId));
        }

        public async Task<IEnumerable<Trips>> GetTripsForRoute(string routeId, int direction, string serviceId)
        {
            var connection = await GetDatabaseConnectionAsync<Trips>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<Trips>("Select * From Trips Where route_id = ? And direction_id = ? And service_id = ?", routeId, direction, serviceId));
        }

        public async Task<IEnumerable<Stop_Times>> GetStopTimesForTrip(string tripId, string stopId)
        {
            var connection = await GetDatabaseConnectionAsync<Stop_Times>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.QueryAsync<Stop_Times>("Select * From Stop_Times Where trip_id = ? And stop_id = ?", tripId, stopId));
        }

        public async Task<IEnumerable<Core.Model.Calendar>> GetCalendar()
        {
            var connection = await GetDatabaseConnectionAsync<Core.Model.Calendar>().ConfigureAwait(false);
            return await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().ToListAsync());
        }

        public async Task<IEnumerable<Trip_Description>> GetRouteLegend(string route_Id, int? direction)
        {
            var connection = await GetDatabaseConnectionAsync<Trip_Description>().ConfigureAwait(false);
            var query = direction.HasValue ? "Select * From trip_description Where route_id = ? And direction_id = ?" : "Select * From trip_description Where route_id = ?";
            return await AttemptAndRetry(() => connection.QueryAsync<Trip_Description>(query, route_Id, direction ?? 0));
        }

        public async Task<IEnumerable<Trip_Description>> GetRouteDescriptionForTrips(IEnumerable<Trips> tripsForRoute)
        {
            var connection = await GetDatabaseConnectionAsync<Trip_Description>().ConfigureAwait(false);
            var shapesIds = tripsForRoute.Select(trip => trip.Shape_Id);
            return await AttemptAndRetry(() => connection.Table<Trip_Description>().Where(desc => shapesIds.Contains(desc.Shape_Id)).ToListAsync());
        }

        public async Task<(DateTime startDate, DateTime endDate)> GetFeedStartEndDates()
        {
            var connection = await GetDatabaseConnectionAsync<Feed_Info>().ConfigureAwait(false);
            var feedInfo = await AttemptAndRetry(() => connection.Table<Feed_Info>().FirstAsync());
            return (DateTime.ParseExact(feedInfo.Feed_Start_Date, "yyyyMMdd", CultureInfo.InvariantCulture), DateTime.ParseExact(feedInfo.Feed_End_Date, "yyyyMMdd", CultureInfo.InvariantCulture));
        }

        public async Task<string> GetWorkdaysServiceId()
        {
            var connection = await GetDatabaseConnectionAsync<Core.Model.Calendar>().ConfigureAwait(false);
            var infos = await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().Where(cal => cal.Saturday == "0" && cal.Sunday == "0").ToListAsync());
            if(infos.Count == 1)
            {
                return infos.First().Service_Id;
            }
            else
            {
                int maxWorkingDays = 0;
                string workingDayServiceId = string.Empty;
                foreach(var info in infos)
                {
                    int currentWorkingDaysCount = info.GetWorkingDaysCount();
                    if (currentWorkingDaysCount > maxWorkingDays)
                    {
                        maxWorkingDays = currentWorkingDaysCount;
                        workingDayServiceId = info.Service_Id;
                    }
                }
                return workingDayServiceId;
            }
        }

        public async Task<string> GetSaturdayServiceId()
        {
            var connection = await GetDatabaseConnectionAsync<Core.Model.Calendar>().ConfigureAwait(false);
            var info = await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().Where(cal => cal.Saturday == "1").FirstOrDefaultAsync());
            return info.Service_Id;
        }

        public async Task<string> GetSundayServiceId()
        {
            var connection = await GetDatabaseConnectionAsync<Core.Model.Calendar>().ConfigureAwait(false);
            var info = await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().Where(cal => cal.Sunday == "1").FirstOrDefaultAsync());
            return info.Service_Id;
        }

        public async Task<string> GetTodayServiceId()
        {
            var connection = await GetDatabaseConnectionAsync<Calendar_Dates>().ConfigureAwait(false);
            var todayDate = DateTime.Today;
            var today = todayDate.ToString("yyyymmdd");
            var dates = await AttemptAndRetry(() => connection.Table<Calendar_Dates>().Where(cal => cal.Date == today).ToListAsync());
            if(dates.Count == 2)
            {
                return dates.First(date => date.Exception_Type == Calendar_Dates.ServiceAdded).Service_Id;
            }
            else
            {
                if(todayDate.DayOfWeek == DayOfWeek.Saturday)
                {
                    return await GetSaturdayServiceId();
                }
                else if(todayDate.DayOfWeek == DayOfWeek.Sunday || HolidaysHelper.IsTodayHoliday())
                {
                    return await GetSundayServiceId();
                }
                else
                {
                    return await GetWorkdaysServiceId();
                }
            }
        }

        public async Task<string> GetServiceIdByDate(DateTime dateTime)
        {
            var connection = await GetDatabaseConnectionAsync<Calendar_Dates>().ConfigureAwait(false);
            var formattedDate = dateTime.ToString("yyyyMMdd");
            var dates = await AttemptAndRetry(() => connection.Table<Calendar_Dates>().Where(cal => cal.Date == formattedDate).ToListAsync());
            if (dates.Count == 2)
            {
                return dates.First(date => date.Exception_Type == Calendar_Dates.ServiceAdded).Service_Id;
            }
            return string.Empty;
        }

        public async Task<string> GetServiceIdByWeekDay(DayOfWeek dayOfWeek)
        {
            var connection = await GetDatabaseConnectionAsync<Core.Model.Calendar>().ConfigureAwait(false);
            var info = new Core.Model.Calendar();
            switch (dayOfWeek)
            {
                case DayOfWeek.Monday:
                    info = await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().Where(cal => cal.Monday == "1").FirstOrDefaultAsync());
                    break;
                case DayOfWeek.Tuesday:
                    info = await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().Where(cal => cal.Tuesday == "1").FirstOrDefaultAsync());
                    break;
                case DayOfWeek.Wednesday:
                    info = await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().Where(cal => cal.Wednesday == "1").FirstOrDefaultAsync());
                    break;
                case DayOfWeek.Thursday:
                    info = await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().Where(cal => cal.Thursday == "1").FirstOrDefaultAsync());
                    break;
                case DayOfWeek.Friday:
                    info = await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().Where(cal => cal.Friday == "1").FirstOrDefaultAsync());
                    break;
                case DayOfWeek.Saturday:
                    info = await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().Where(cal => cal.Saturday == "1").FirstOrDefaultAsync());
                    break;
                case DayOfWeek.Sunday:
                    info = await AttemptAndRetry(() => connection.Table<Core.Model.Calendar>().Where(cal => cal.Sunday == "1").FirstOrDefaultAsync());
                    break; 
            }

            return info.Service_Id;
        }

        public async Task SaveNews(IList<News> news)
        {
            var connection = await GetDatabaseConnectionAsync<News>().ConfigureAwait(false);
            await connection.DeleteAllAsync<News>();
            await connection.InsertAllAsync(news);
        }

        public async Task<IList<News>> GetNews(bool showOnly)
        {
            var connection = await GetDatabaseConnectionAsync<News>().ConfigureAwait(false);
            var news = await AttemptAndRetry(() => connection.Table<News>().ToListAsync());
            if(showOnly)
            {
                news = news.Where(item => item.Show == "true").ToList();
            }
            return news;
        }

        public async Task<IList<Trace>> GetRouteTrace(string routeId, int? direction)
        {
            var routes = direction.HasValue ? await GetTripsForRoute(routeId, direction.Value) : await GetTripsForRoute(routeId);
            var shapes = routes.Select(r => r.Shape_Id).Distinct();

            var connection = await GetDatabaseConnectionAsync<Shapes>().ConfigureAwait(false);
            var traces = new List<Trace>();
            foreach (var shapeId in shapes)
            {
                var shape_points = await AttemptAndRetry(() => connection.Table<Shapes>().Where(shape => shape.Shape_Id == shapeId).ToListAsync());
                traces.Add(CreateTraceFromPoints(shape_points));
            }

            return traces;
        }

        private Trace CreateTraceFromPoints(List<Shapes> shape_points)
        {
            var trace = new Trace();
            foreach(var point in shape_points)
            {
                trace.Points.Add(new Point(double.Parse(point.Shape_Pt_Lat, CultureInfo.InvariantCulture), double.Parse(point.Shape_Pt_Lon, CultureInfo.InvariantCulture)));
            }
            return trace;
        }

        protected async ValueTask<SQLiteAsyncConnection> GetDatabaseConnectionAsync<T>()
        {
            if (!_connection.Value.TableMappings.Any(x => x.MappedType == typeof(T)))
            {
                await _connection.Value.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                await _connection.Value.CreateTablesAsync(CreateFlags.None, typeof(T)).ConfigureAwait(false);
            }

            return _connection.Value;
        }

        protected async ValueTask<SQLiteAsyncConnection> GetDatabaseConnectionAsync<T,U>()
        {
            if (!(_connection.Value.TableMappings.Any(x => x.MappedType == typeof(T)) && _connection.Value.TableMappings.Any(t => t.MappedType == typeof(U))))
            {
                //await _connection.Value.EnableWriteAheadLoggingAsync().ConfigureAwait(false);
                await _connection.Value.CreateTablesAsync(CreateFlags.None, new Type[] { typeof(T), typeof(U) }).ConfigureAwait(false);
            }

            return _connection.Value;
        }

        protected Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries = 12)
        {
            return Policy.Handle<SQLiteException>().WaitAndRetryAsync(numRetries, pollyRetryAttempt).ExecuteAsync(action);

            TimeSpan pollyRetryAttempt(int attemptNumber) => TimeSpan.FromMilliseconds(Math.Pow(2, attemptNumber));
        }
    }
}
