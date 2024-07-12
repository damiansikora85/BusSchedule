import sqlite3

def create_legend_by_destination(db,file):
    legend = [
        ["1","0","Bolszewo Strażacka 02",",1,0,A,kurs skrócony do: Bolszewo Strażacka 02"],
        ["1","1","Gościcino Robakowska 02",",1,1,R,kurs do: Gościcino Robakowska"],
        ["1","1","Góra Szkolna 02",",1,1,G,kurs do: Góra Szkolna (do przystanku Bolszewo Prusa 01 po trasie)"],
        ["1","1","Wejherowo Budowlanych 02",",1,1,Z,kurs skrócony do: Wejherowo Budowlanych 02 (do przystanku Wejherowo Tartaczna 01 po trasie)"],
        ["3","0","Wejherowo Cmentarz 02",",3,0,C,kurs do: Wejherowo Cmentarz"],
        ["4","0","Wejherowo Sobieskiego - GS 02",",4,0,A,kurs skrócony do: Wejherowo Sobieskiego - GS 02"],
        ["4","0","Wejherowo Budowlanych 02",",4,0,B,kurs skrócony do: Wejherowo Budowlanych 02"],
        ["4","1","Orle Szkoła 01",",4,1,A,kurs do: Orle Szkoła"],
        ["4","1","Bolszewo Słoneczna 02",",4,1,B,kurs do: Bolszewo Słoneczna"],
        ["5","0","Wejherowo Broniewskiego - Dworzec PKP 02",",5,0,B,kurs do: Broniewskiego - Dworzec PKP. Dalej jako linia 1 do: Os. Fenikowskiego"],
        ["5","1","Orle Szkoła 01",",5,1,A,kurs do: Orle Szkoła"],
        ["6","0","Wejherowo Karnowskiego 02",",6,0,A,kurs do: Karnowskiego. Nie kursuje przez: Szpital"],
        ["6","1","Wejherowo Os. Fenikowskiego 01",",6,1,A,kurs do: Os. Fenikowskiego przez: Szpital"],
        ["7","0","Wejherowo Dworzec PKP 01",",7,0,A,kurs do: Wejherowo Dworzec PKP"],
        ["7","1","Gościcino Szkoła 01",",7,1,A,kurs do: Gościcino Szkoła bez zajazdu na: Gościcino PKP"],
        ["7","1","Bolszewo Słoneczna 02",",7,1,B,kurs do: Bolszewo Słoneczna bez zajazdu na: Gościcino PKP"],
        ["7","1","Gościcino Robakowska 02",",7,1,C,kurs do: Gościcino Robakowska bez zajazdu na: Gościcino PKP"],
        ["8","0","Wejherowo Kochanowskiego SKM Nanice 01",",8,0,K,kurs skrócony do: Wejherowo Kochanowskiego SKM Nanice 01"],
        ["9","0","Reda Ciechocino 02",",9,0,Z,kurs skrócony do: Reda Ciechocino 02"],
        ["10","0","Bolszewo Zamostna 04",",10,0,Z,kurs skrócony do: Bolszewo Zamostna 04"],
        ["10","0","Wejherowo Dworzec PKP 04",",10,0,D,kurs do: Wejherowo Dworzec PKP (do przystanku Urząd Pracy po trasie)"],
        ["10","1","Gościcino Równa 04",",10,1,A,kurs skrócony do: Gościcino Równa 04"],
        ["10","1","Kębłowo Wiejska 02",",10,1,B,kurs do: Kębłowo Wiejska 02 (zatoka przy ul. Wejherowskiej)"],
        ["12","0","Wejherowo Broniewskiego - Dworzec PKP 01",",11,0,B,kurs skrócony do: Wejherowo Broniewskiego - Dworzec PKP 01"],
        ["14","0","Bolszewo Słowackiego 01",",14,0,A,kurs do: Bolszewo Słowackiego"],
        ["18","1","Rumia C.H. \"Port Rumia\" 01",",18,1,R,kurs do: C.H. Port Rumia"],
        ["18","1","Reda Ciechocino 02",",18,1,Z,kurs skrócony do: Reda Ciechocino 02"]
    ]

    #create dictionary [shape_id] = trip_headsign for each route
    #for each item in dictionary:
    #  check if value is in legend
    #  and update trip_description info

    cursor = db.cursor()

    for data in legend:
        #print (data)
        cursor.execute("SELECT shape_id, trip_headsign FROM trips WHERE route_id = (?) AND direction_id = (?) AND (service_id = 10 OR service_id = 14 OR service_id = 16 OR service_id = 17  OR service_id = 24 OR service_id = 27 OR service_id = 28)", (data[0], data[1]))
        db.commit()
        all_rows = cursor.fetchall()
        shapes_dict = {}
        for row in all_rows:
            shapes_dict[row[0]] = row[1]
            #if data[0] == "4":
                #print('{0} - {1}'.format(row[1], row[0]))

        final_dict = {}
        for key,value in shapes_dict.items():
            if value == data[2]:
                final_dict[value] = key
                file.write('{0}{1}\n'.format(key,data[3]))

def create_legend_by_stop(db,file):
    legend = [
        ["3", "1", "53", ",3,1,D,kurs z podjazdem do przystanków przy ul. Drzewiarza w Gościcinie"],
        ["8", "0", "199",",8,0,C,kurs z podjazdem do Ciechocina"],
        ["8", "1", "199",",8,1,C,kurs z podjazdem do Ciechocina"],
        ["11", "0", "318",",11,0,A,kurs przez: Szpital"]
        ["11", "0", "520",",11,0,B,kurs przez: Szpital i Karnowskiego"],
        ["11", "1", "318",",11,1,A,kurs przez: Szpital"]
        ["11", "1", "520",",11,1,B,kurs przez: Karnowskiego"],
        ["12", "0", "107",",12,0,C,kurs przez os. Kaszubskie (nie kursuje przez: Pruszkowskiego)"],
        ["12", "1", "106",",12,1,A,kurs przez os. Kaszubskie"],
    ]

    cursor = db.cursor()

    for data in legend:
        cursor.execute("SELECT shape_id FROM trips WHERE route_id = (?) AND direction_id = (?) AND trip_id IN (SELECT trip_id FROM stop_times WHERE stop_id=(?))", (data[0], data[1], data[2]))
        db.commit()
        all_rows = cursor.fetchall()
        #print(all_rows)
        shapes = []
        for row in all_rows:
            if row[0] not in shapes:
                shapes.append(row[0])

        for shape in shapes:
            file.write('{0}{1}\n'.format(shape,data[3]))
    


db = sqlite3.connect('sqlite_temp.db')
file = open("trip_description.csv", "w", encoding="UTF-8")

create_legend_by_destination(db,file)
create_legend_by_stop(db,file)

db.close()
file.close()