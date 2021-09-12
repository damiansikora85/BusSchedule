sqlite3.exe sqlite_temp.db < create.sql
python.exe create_route_desc.py
del sqlite_temp.db
sqlite3.exe sqlite.db < create.sql