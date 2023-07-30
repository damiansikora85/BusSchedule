CREATE TABLE "agency" (
	"agency_id"	INTEGER,
	"agency_name"	TEXT,
	"agency_url"	TEXT,
	"agency_timezone"	TEXT,
	"agency_lang"	TEXT,
	"agency_phone"	REAL,
	"agency_fare_url"	TEXT
);
.separator ,
.import agency.txt agency
DELETE FROM agency
WHERE agency_id = 'agency_id';

CREATE TABLE "calendar" (
	"service_id"	INTEGER,
	"monday"	INTEGER,
	"tuesday"	INTEGER,
	"wednesday"	INTEGER,
	"thursday"	INTEGER,
	"friday"	INTEGER,
	"saturday"	INTEGER,
	"sunday"	INTEGER,
	"start_date"	INTEGER,
	"end_date"	INTEGER
);
.separator ,
.import calendar.txt calendar
DELETE FROM calendar
WHERE service_id = 'service_id';

CREATE TABLE "calendar_dates" (
	"service_id"	INTEGER,
	"date"	INTEGER,
	"exception_type"	INTEGER
);
.separator ,
.import calendar_dates.txt calendar_dates
DELETE FROM calendar_dates
WHERE service_id = 'service_id';

CREATE TABLE "destination" (
	"route_id"	INTEGER,
	"outbound"	TEXT,
	"inbound"	TEXT
);
.separator ,
.import destination.txt destination
DELETE FROM destination
WHERE route_id = 'route_id';

CREATE TABLE "feed_info" (
	"feed_publisher_name"	TEXT,
	"feed_publisher_url"	TEXT,
	"feed_lang"	TEXT,
	"feed_start_date"	INTEGER,
	"feed_end_date"	INTEGER
);
.separator ,
.import feed_info.txt feed_info
DELETE FROM feed_info
WHERE feed_publisher_name = 'feed_publisher_name';

CREATE TABLE "route_stop" (
	"route_id"	INTEGER,
	"direction_id"	INTEGER,
	"stop_sequence"	INTEGER,
	"stop_id"	INTEGER
);
.separator ,
.import route_stop.txt route_stop
DELETE FROM route_stop
WHERE route_id = 'route_id';

CREATE TABLE "routes" (
	"route_id"	INTEGER,
	"agency_id"	INTEGER,
	"route_short_name"	INTEGER,
	"route_long_name"	TEXT,
	"route_desc"	TEXT,
	"route_type"	INTEGER,
	"route_color"	TEXT,
	"route_text_color"	TEXT
);
.separator ,
.import routes.txt routes
DELETE FROM routes
WHERE route_id = 'route_id';

CREATE TABLE "shapes" (
	"shape_id"	INTEGER,
	"shape_pt_lat"	REAL,
	"shape_pt_lon"	REAL,
	"shape_pt_sequence"	INTEGER
);
.separator ,
.import shapes.txt shapes
DELETE FROM shapes
WHERE shape_id = 'shape_id';

CREATE TABLE "stop_times" (
	"trip_id"	TEXT,
	"arrival_time"	TEXT,
	"departure_time"	TEXT,
	"stop_id"	INTEGER,
	"stop_sequence"	INTEGER,
	"pickup_type"	INTEGER,
	"drop_off_type"	INTEGER
);
.separator ,
.import stop_times.txt stop_times
DELETE FROM stop_times
WHERE trip_id = 'trip_id';

CREATE TABLE "stops" (
	"stop_id"	INTEGER,
	"stop_code"	TEXT,
	"stop_name"	TEXT,
	"stop_lat"	REAL,
	"stop_lon"	REAL
);
.separator ,
.import stops.txt stops
DELETE FROM stops
WHERE stop_id = 'stop_id';

CREATE TABLE "trip_description" (
	"shape_id"	INTEGER,
	"route_id"	INTEGER,
	"direction_id"	INTEGER,
	"shortDescription"	TEXT,
	"longDescription"	TEXT
);
.separator ,
.import trip_description.csv trip_description
DELETE FROM trip_description
WHERE shape_id = 'shape_id';

CREATE TABLE "trips" (
	"route_id"	INTEGER,
	"service_id"	INTEGER,
	"trip_id"	TEXT,
	"trip_headsign"	TEXT,
	"direction_id"	INTEGER,
	"shape_id"	INTEGER
);
.separator ,
.import trips.txt trips
DELETE FROM trips
WHERE route_id = 'route_id';

CREATE TABLE "news" (
	"title" TEXT,
	"show" TEXT,
	"message" TEXT
)