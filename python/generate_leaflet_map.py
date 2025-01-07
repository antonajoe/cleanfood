# nys restaurant inspections

import pandas as pd
from shapely import Point
import time
import folium
from folium.plugins import MarkerCluster
from jinja2 import Template
from tqdm import tqdm


start = time.localtime()

df = pd.read_csv(r'inspections.csv', sep=';')   # takes the output of prepare_inspections_data.py

print(df.head())
print(df.columns)
print(df.info())

cols = ['Index', 'Name', 'Address', 'Last_Inspected', 'Violations',
       'Critical_Violations', 'Critical_Not_Corrected',
       'Non_Critical_Violations', 'County', 'City', 'Zip_Code',
       'Inspection_Comments', 'Latitude', 'Longitude', 'State', 'DisplayName']

# function that creates a template string to feed into jinja Template()
def create_template_string(info, initial = ''' '''):
    final_string = '\n'
    for item in info:
        if item[1] != '':
            final_string += str(item[0]) + ' : ' + str(item[1] + '<br>\n')
    #final_string = final_string + '<a href="could link to the gov't website/map'
    #print(final_string)
    return initial + final_string 

subs = df.values
print(subs[0])
print(len(subs))

# # centroid value for New York State
state_centroid = [42.7958, -75.4658]

# create the map
m = folium.Map(state_centroid, zoom_start=8, prefer_canvas=False, disable_3d=False)
mc = MarkerCluster(disableClusteringAtZoom=16).add_to(m)

# create and add folium Markers to the map
print()
print("Generating the map html file, it may take a minute...")
print()
for item in tqdm(subs):
    info = [[str(cols[i]), str(item[i])] if str(item[i]) != 'nan' else [str(cols[i]), ''] for i in range(2,len(item)-2)]
    name = str(item[1])
    initial = f"<h3>{name}</h3>"
    template_string = create_template_string(info, initial=initial)
    template = Template(template_string) 
    html = template.render()
    iframe = folium.IFrame(html=html, width=300, height=300)
    popup = folium.Popup(iframe, max_width=500, parse_html=True)
    tooltip = folium.Tooltip(text = {item[1]})
    folium.Marker(
        location=[item[12], item[13]],
        popup=popup,
        tooltip=tooltip,
        icon=folium.Icon(color="blue"),
    ).add_to(mc)

# save the map
print()
print("Saving file to disk...")
m.save('map.html')     # place output in solution Resources\Raw folder 
print()
end = time.localtime()
print("File creation finished!")
print()
print(start.tm_mday, " ", start.tm_hour, ':', start.tm_min, start.tm_sec)
print(end.tm_mday, " ", end.tm_hour, ':', end.tm_min, end.tm_sec)

### TODO:
# make cluster boundaries match zipcode or county boundaries
