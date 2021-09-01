
# Bulk SQL Updater

Quickly update or insert data from a csv file into a SQL database.

### Run Locally

Clone the project

```bash
  git clone https://github.com/lorenza12/BulkSQLUpdater.git
```

- Once cloned, open the sln file and rebuild the project.
- BulkSQLUpdater.exe can now be ran from within the bin folder


### Bulk Update:
1. Enter Server, Database, User and/or password needed to connect to the desired table.
- Once a successfull connection has been made, select the desired table from the table dropdown.
2. Select the columns you wish to update that correspond to the data in your csv file.
- The firt selected column MUST be a unique identifier (ex: ID) to be able to link to the table properly
    - This column will not be used in the update stament, it is just needed in the where clause to link 1-1
- After the unique identifier column is selected, select all other columns wanting to updated
    - Thse MUST be in the same order as the csv file for an accurate update.
3. Once the desired columns are selected, browse for the csv file containing the data that you wish to update
4. Select the desired delimiter used in the file and if the file contains headers or not.
5. Run Update

### Bulk Insert:
1. Enter Server, Database, User and/or password needed to connect to the desired table.
- Once a successfull connection has been made, select the desired table from the table dropdown.
2. Select the columns you wish to insert into that correspond to the data in your csv file.
- The columns selected must be in the same order as the csv file data for accurate inserts.
3. Once the desired columns are selected, browse for the csv file containing the data that you wish to insert.
4. Select the desired delimiter used in the file and if the file contains headers or not.
5. Run Insert
## Screenshots

![Bulk Update](/Screenshots/BulkUpdate.PNG)

![Bulk Insert](/Screenshots/BulkInsert.PNG)
## Authors

- [Alik Lorenz](https://github.com/lorenza12)

  
