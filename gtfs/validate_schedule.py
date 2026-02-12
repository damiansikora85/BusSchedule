import sqlite3

db = sqlite3.connect('sqlite.db')
cursor = db.cursor()
cursor.execute("SELECT route_id FROM routes")
db.commit()
all_routes = cursor.fetchall()
is_valid = True
for route in all_routes:
    #print(route)
    cursor.execute("SELECT stop_id FROM Route_Stop WHERE route_id = ? AND direction_id = ?", (route[0], 1))
    db.commit()
    stopIds = cursor.fetchall()
    for stopId in stopIds:
        #print(stopId)
        cursor.execute("SELECT EXISTS (SELECT 1 FROM Stops WHERE stop_id = ?)", (stopId[0],))
        if cursor.fetchone()[0] == 0:
            is_valid = False
            print("Brak "+ str(stopId[0]) + " dla linii " + str(route[0]))


if( is_valid ):
    print("Schedule is valid")

db.close()