# create the dataset for use in the cleanfood app

import pandas as pd
import re
import sqlite3
import sqlalchemy

df = pd.read_csv(r'Food_Service_Establishment__Last_Inspection_20250106.csv')

locations = df['Location1'].values
lats = []
longs = []
for item in locations:
    item = item.strip('()').split(',')
    lat = float(item[0])
    long = float(item[1])
    lats.append(lat)
    longs.append(long)

df['LATITUDE'] = lats
df['LONGITUDE'] = longs
df.drop(['Location1'], inplace=True, axis=1)
print(df.info())

# drop unused columns
df.drop(df.columns[[7,8,10,13,14,15,16,17,18,19,20,21,22,24]], axis=1, inplace=True)
print(df.info())

cols = ['NAME', 'ADDRESS', 'LAST_INSPECTED', 'VIOLATIONS',
       'CRITICAL_VIOLATIONS', 'CRITICAL_NOT_CORRECTED',   
       'NON_CRITICAL_VIOLATIONS', 'COUNTY', 'CITY',
       'ZIP_CODE', 'INSPECTION_COMMENTS', 'LATITUDE', 'LONGITUDE']
df.columns = cols
print(df.columns)

# title case columns
for col in df.columns:
    df.rename(columns={col: col.title()}, inplace=True)

# clean and drop zip codes
drops = df[df['Zip_Code'].isnull() == True]
idx = drops.index
df.drop(idx, axis=0, inplace=True)
df['Zip_Code'] = [str(item).strip(' ') for item in df['Zip_Code'].values]
df['Zip_Code'] = df['Zip_Code'].str.replace(';' , ' ')

# cleans a row where zipcode set to 'NY' 
zips = df['Zip_Code'][df['Zip_Code'] == 'NY']  
print("################# ", zips)
df.drop([5364], axis=0, inplace=True)
zips = df['Zip_Code'].values
for i in range(0,len(zips)):
    if(len(zips[i]) > 5):
        zips[i] = zips[i][0:5]

# clean 'Name' column values
df['Name'] = df['Name'].str.title()
df['Name'] = df['Name'].str.replace("'S", "'s")
df['Name'] = df['Name'].str.replace(';' , ' ')

names = df['Name'].values
newnames = []
for item in names:
    newnames.append(re.sub(r'[^a-zA-Z\s]', '', item))
    
    # TODO:
    # adhoc stuff, for example, taco bellpizza hut combos...

df['Name'] = newnames
df['Name'] = df['Name'].str.replace('  ' , ' ')
df['Name'] = df['Name'].str.rstrip(' ')
df['Name'] = df['Name'].str.lstrip(' ')

# clean other string column values
df['Address'] = df['Address'].str.title()
df['Address'] = df['Address'].str.replace(';' , ' ')
df['County'] = df['County'].str.title()
df['County'] = df['County'].str.replace(';' , ' ')
df['City'] = df['City'].str.title()
df['City'] = df['City'].str.replace(';' , ' ')
df['Last_Inspected'] = df['Last_Inspected'].str.replace(';' , ' ')
df['Violations'] = df['Violations'].str.replace(';' , ' ')
df['Inspection_Comments'] = df['Inspection_Comments'].str.replace(';' , ' ')

# TODO:
# get coordinate boundary for state and filter those outside it 

# drop rows with invalid coordinates
longs = df[df['Longitude'] > -72.000000]
df.drop(longs.index, axis=0, inplace=True)
longs = df[df['Longitude'] > -72.000000]
print(longs)
lats = df[df['Latitude'] > 46.000000]
df.drop(lats.index, axis=0, inplace=True)
lats = df[df['Latitude'] > 46.000000]
print(lats)

# add the index as a column for primary key
df.reset_index(inplace=True, drop=True)
df['State'] = 'NY'
df['DisplayName'] = df['Name'] + ', ' + df['City'] + ', ' + df['State']  + ', ' + df['Zip_Code'] 
df['Index'] = df.index
print(df.info())
df_reordered = df.iloc[:, [15,0,1,2,3,4,5,6,7,8,9,10,11,12,13,14]]
df = df_reordered
print(df.head())
print(len(df))
print(df.tail())
print(df['DisplayName'].head())
print(df.columns)

df.fillna(0, inplace = True)

# save to .csv to CleanFood resource folder
savepath = r'PATH TO RESOURCES FOLDER'
df.to_csv(savepath, sep=';', header=False, index=False)

# save .csv locally
savepath = r'inspections.csv'
df.to_csv(savepath, sep=';', header=True, index=False)

# generate the zipcodes file
zips = df['Zip_Code'].value_counts()
zips = sorted(zips.index) 
with open('zipcodes.txt', 'w') as z:
    for item in zips:
        z.write(item + '\n')         

# generate the counties file    
counties = df['County'].value_counts()
idx = sorted(counties.index)
with open('counties.txt', 'w') as c:
    for item in idx:
        c.write(item + '\n')


'''   
# save the dataframe as xml
df.to_xml('inspections.xml', index=False, root_name='Food', row_name='Facility')

# create the sqlite database
with sqlite3.connect('inspections.db3') as conn:
    result = df.to_sql('UpstateNY', conn, if_exists='replace', dtype={'Index': 'INTEGER PRIMARY KEY AUTOINCREMENT'}, index=False)
    print(result)

# print table names
con = sqlite3.connect('inspections.db3')
cursor = con.cursor()
cursor.execute("SELECT name FROM sqlite_master WHERE type='table';")
print(cursor.fetchall())

# test querying
with sqlite3.connect('inspections.db3') as conn:
    example = pd.read_sql_query("SELECT * FROM 'UpstateNY'", conn)
    print(example)    
    res = pd.read_sql_query("PRAGMA table_info('UpstateNY')", conn)
    print(res)
'''
