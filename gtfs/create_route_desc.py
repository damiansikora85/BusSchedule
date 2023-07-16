import sqlite3

def create_legend_by_destination(db,file):
    legend = [
        ["1","0","Bolszewo Strażacka 02",",1,0,A,kurs skrócony do: Bolszewo Strażacka 02"],
        ["1","1","Gościcino Robakowska 02",",1,1,R,kurs do: Gościcino Robakowska"],
        ["1","1","Góra Szkolna 02",",1,1,G,kurs do: Góra Szkolna (do przystanku Bolszewo Prusa 01 po trasie)"],
        ["1","1","Wejherowo Budowlanych 02",",1,1,Z,kurs skrócony do: Wejherowo Budowlanych 02 (do przystanku Wejherowo Tartaczna 01 po trasie)"],
        ["4","1","Wejherowo Urząd Pracy 01 n/ż",",4,1,Z,kurs skrócony do: Wejherowo Urząd Pracy 01 n/ż"],
        ["4","1","Orle Szkoła 01",",4,1,S,kurs do: Orle Szkoła w dni wolne od nauki do: Orle Łąkowa"],
        ["5","0","Wejherowo Obrońców Helu 01",",5,0,A,kurs skrócony do: Wejherowo Obrońców Helu 01"],
        ["5","1","Wejherowo Urząd Pracy 01 n/ż",",5,1,Z,kurs skrócony do: Wejherowo Urząd Pracy 01 n/ż"],
        ["5","1","Orle Szkoła 01",",5,1,S,kurs wydłużony do: Orle Szkoła w dni nauki szkolnej"],
        ["6","1","Wejherowo Szpital 02",",6,1,B,kurs tylko do Szpitala"],
        ["7","0","Wejherowo Sobieskiego - GS 02",",7,0,S,kurs skrócony do: Wejherowo Sobieskiego - GS 02"],
        ["7","0","Wejherowo Dworzec PKP 01",",7,0,D,kurs do: Wejherowo Dworzec PKP"],
        ["7","1","Wejherowo Sucharskiego 01",",7,1,Z,kurs do: Wejherowo Sucharskiego 01 (do przystanku Ogródki Działkowe 01 po trasie)"],
        ["7","1","Gościcino Robakowska 02",",7,1,R,kurs do: Gościcino Robakowska (do przystanku Gościcino PKP – Fabryczna 02 po trasie)"],
        ["7","1","Wejherowo Dworzec PKP 02",",7,1,D,kurs do: Wejherowo Dworzec PKP 02 (do przystanku Sobieskiego - Sąd 01 po trasie)"],
        ["8","0","Wejherowo Kochanowskiego SKM Nanice 01",",8,0,K,kurs skrócony do: Wejherowo Kochanowskiego SKM Nanice 01"],
        ["9","0","Reda Ciechocino 02",",9,0,Z,kurs skrócony do: Reda Ciechocino 02"],
        ["10","0","Bolszewo Zamostna 04",",10,0,Z,kurs skrócony do: Bolszewo Zamostna 04"],
        ["10","0","Wejherowo Dworzec PKP 04",",10,0,D,kurs do: Wejherowo Dworzec PKP (do przystanku Urząd Pracy po trasie)"],
        ["10","1","Gościcino Równa 04",",10,1,A,kurs skrócony do: Gościcino Równa 04"],
        ["11","0","Wejherowo Broniewskiego - Dworzec PKP 01",",11,0,B,kurs do: Wejherowo Broniewskiego – Dworzec PKP"],
        ["11","0","Wejherowo Dworzec PKP 03",",11,0,A,kurs do: Wejherowo Dworzec PKP w dni wolne od nauki do: Wejherowo Broniewskiego – Dworzec PKP"],
        ["13","0","Wejherowo Broniewskiego - Dworzec PKP 01",",13,0,A,kurs do: Wejherowo Broniewskiego – Dworzec PKP"],
        ["13","0","Wejherowo Dworzec PKP 03",",13,0,A,kurs do: Wejherowo Broniewskiego – Dworzec PKP"],
        ["18","1","Rumia C.H. \"Port Rumia\" 01",",18,1,R,kurs do: C.H. Port Rumia"]
    ]

    #create dictionary [shape_id] = trip_headsign for each route
    #for each item in dictionary:
    #  check if value is in legend
    #  and update trip_description info

    cursor = db.cursor()

    for data in legend:
        #print (data)
        cursor.execute("SELECT shape_id, trip_headsign FROM trips WHERE route_id = (?) AND direction_id = (?) AND (service_id = 2 OR service_id = 24 OR service_id = 27  OR service_id = 28)", (data[0], data[1]))
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
        ["6", "0", "318",",6,0,S,kurs z podjazdem do Szpitala"],
        ["6", "1", "318",",6,1,A,kurs z podjazdem do Szpitala"],
        ["8", "0", "199",",8,0,C,kurs z podjazdem do Ciechocina"],
        ["8", "1", "199",",8,0,C,kurs z podjazdem do Ciechocina"],
        ["11", "1", "115",",11,1,K,kurs przez: Rybacką Prusa Pomorską Kochanowskiego"]
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