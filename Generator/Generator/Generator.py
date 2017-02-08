import json
import requests
import time


app_key =''
ip_addr ='tvsn.cloud.thingworx.com'


thing_name ='DreamHouseThing'

def send(value,put_property_name):


    put_url = "http://"+ip_addr+"/Thingworx/Things/"+thing_name+"/Properties/"+put_property_name

    put_headers={'Content-type':'application/json','appKey':app_key}

    put_text= '{"'+put_property_name+'": '+str(value)+'}'

    put_r = requests.put(put_url,headers=put_headers,data=put_text)

def get(get_property_name):

    get_url = "http://" + ip_addr + "/Thingworx/Things/" + thing_name + "/Properties/" + get_property_name

    get_headers={'Accept':'application/json','appKey':app_key}

    get_r=requests.get(get_url,headers=get_headers)

    return get_r.json()['rows'][0][get_property_name]

'''
post_service_name='GetPropertyValues'
post_url = "http://" + ip_addr + "/Thingworx/Things/" + thing_name + "/Services/" + post_service_name

post_headers={'Accept':'application/json','Content-type':'application/json','appKey':app_key}

post_r=requests.post(post_url,headers=post_headers)

print post_r.text
'''

step=2
temperature= [float(x)/step for x in range(15*step,30*step)]
while(True):
    send(100,'Temperature')
    send(100,'Brightness')
	send(100,'Humidity')
    send(40,'Temperature')
    send(30,'Brightness')
	send(30,'Humidity')
    send(-100,'Temperature')
    send(0,'Brightness')
	send(0,'Humidity')