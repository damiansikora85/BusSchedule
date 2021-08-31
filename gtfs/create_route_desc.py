import sqlite3

db = sqlite3.connect('sqlite.db')
file = open("test.csv", "w", encoding="UTF-8")
legend = [
    ["1","0","Bolszewo Łąkowa 02",",1,0,Z,kurs skrócony do: Bolszewo Łąkowa 02"],
    ["1","1","Gościcino Robakowska 02",",1,1,R,kurs do: Gościcino Robakowska"],
    ["1","1","Góra Szkolna 02",",1,1,G,kurs do: Góra Szkolna (do przystanku Bolszewo Prusa 01 po trasie)"],
    ["1","1","Wejherowo Budowlanych 02",",1,1,Z,kurs skrócony do: Wejherowo Budowlanych 02 (do przystanku Wejherowo Tartaczna 01 po trasie)"],
    ["3","0","Wejherowo os. Sikorskiego 01",",3,0,W,kurs do: Wejherowo Odrębna 04 dalej do: Wejherowo os. Sikorskiego 01 przez: Sędzickiego Sikorskiego"],\
    ["3","0","Bolszewo Zamostna 02",",3,0,Z,kurs skrócony do: Bolszewo Zamostna 02"],
    ["4","1","Wejherowo Urząd Pracy 01 n/ż",",4,1,Z,kurs skrócony do: Wejherowo Urząd Pracy 01 n/ż"],
    ["4","1","Orle Szkoła 01",",4,1,S,kurs do: Orle Szkoła w dni wolne od nauki do: Orle Łąkowa"],
    ["5","0","Wejherowo Chopina 02",",5,0,C,kurs skrócony do: Wejherowo Chopina 02"],
    ["5","1","Wejherowo Urząd Pracy 01 n/ż",",5,1,Z,kurs skrócony do: Wejherowo Urząd Pracy 01 n/ż"],
    ["5","1","Orle Szkoła 01",",5,1,S,kurs wydłużony do: Orle Szkoła w dni nauki szkolnej"],
    ["7","0","Wejherowo Sobieskiego - GS 02",",7,0,S,kurs skrócony do: Wejherowo Sobieskiego - GS 02"],
    ["7","0","Wejherowo Dworzec PKP 02",",7,0,D,kurs do: Wejherowo Dworzec PKP 02 (do przystanku Filharmonia Kaszubska 02 po trasie)"],
    ["7","1","Wejherowo Sucharskiego 01",",7,1,Z,kurs do: Wejherowo Sucharskiego 01 (do przystanku Ogródki Działkowe 01 po trasie)"],
    ["7","1","Gościcino Robakowska 02",",7,1,R,kurs do: Gościcino Robakowska"],
    ["7","1","Wejherowo Dworzec PKP 02",",7,1,D,kurs do: Wejherowo Dworzec PKP 02 (do przystanku Sobieskiego - Sąd 01 po trasie)"],
    ["8","0","Wejherowo Kochanowskiego SKM Nanice 01",",8,0,K,kurs skrócony do: Wejherowo Kochanowskiego SKM Nanice 01"],
    ["8","1","Reda Aquapark 01",",8,1,A,kurs do: Reda Aquapark"],
    ["9","0","Reda Ciechocino 02",",9,0,Z,kurs skrócony do: Reda Ciechocino 02"],
    ["10","0","Bolszewo Zamostna 04",",10,0,Z,kurs skrócony do: Bolszewo Zamostna 04"],
    ["10","1","Kębłowo Wiejska 02",",10,1,A,kurs do: Kębłowo Wiejska 02 (przystanek przy DK 6)"],
    ["10","1","Gościcino Równa 04",",10,1,B,kurs skrócony do: Gościcino Równa 04"],
    ["11","0","Wejherowo Kusocińskiego 02",",11,0,Z,kurs skrócony do: Wejherowo Kusocińskiego 02"],
    ["11","0","Wejherowo Prusa - Szkoła 02",",11,0,P,kurs skrócony do: Wejherowo Prusa - Szkoła 02"],
    ["11","1","Wejherowo Pruszkowskiego 01",",11,1,P,kurs skrócony do: Wejherowo Pruszkowskiego 01"],
    ["11","1","Wejherowo Kochanowskiego SKM Nanice 01",",11,1,Z,kurs skrócony do: Wejherowo Kochanowskiego SKM Nanice 01"],
    ["18","0","Reda Ciechocino 02",",18,0,Z,kurs skrócony do: Reda Ciechocino 02"]
]

cursor = db.cursor()

for data in legend:
    print (data)
    cursor.execute("SELECT shape_id, trip_headsign FROM trips WHERE route_id = (?) AND direction_id = (?) AND (service_id = 4 OR service_id = 24 OR service_id = 7)", (data[0], data[1]))
    db.commit()
    all_rows = cursor.fetchall()
    shapes_dict = {}
    for row in all_rows:
        shapes_dict[row[0]] = row[1]
    #print('{0} - {1}'.format(row[1], row[0]))

    final_dict = {}
    for key,value in shapes_dict.items():
        if value == data[2]:
            final_dict[value] = key
            file.write('{0}{1}\n'.format(key,data[3]))
    
db.close()
file.close()

# select shapeid from trips where route_id = (?) direction_id = (?) trip_id = (select trid_id from stop_times where stopid = (?))