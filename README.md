#datebase creating 
1. There is a database folder that has sql script to create database, table and data (dummy)
2. Use SQL server or MSSQLS

#to change
1. after the database create change connection string to your local server (windows authentication). Exmple below:
     "ConnectionStrings": {
    "Booking_Db": "Server=(local);Database=BookingDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }

