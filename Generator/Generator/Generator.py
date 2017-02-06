import json
import requests
import time


app_key ='983fee7d-5713-48b6-b6a1-8704a1c1fc9d'#'e72278dc-8ad1-4df6-bde5-e4785cf2f236'
ip_addr ='34.249.39.144'#'34.248.238.197'


thing_name ='TestT'#'HARThing'

def send(value,put_property_name):
    post_service_name='GetPropertyValues'

    put_url = "http://"+ip_addr+"/Thingworx/Things/"+thing_name+"/Properties/"+put_property_name

    put_headers={'Content-type':'application/json','appKey':app_key}

    put_text= '{"temperature": '+str(value)+'}'

    put_r = requests.put(put_url,headers=put_headers,data=put_text)

def get(get_property_name):

    get_url = "http://" + ip_addr + "/Thingworx/Things/" + thing_name + "/Properties/" + get_property_name

    get_headers={'Accept':'application/json','appKey':app_key}

    get_r=requests.get(get_url,headers=get_headers)

    return get_r.json()['rows'][0]['windowstate']

'''
post_url = "http://" + ip_addr + "/Thingworx/Things/" + thing_name + "/Services/" + post_service_name

post_headers={'Accept':'application/json','Content-type':'application/json','appKey':app_key}

post_r=requests.post(post_url,headers=post_headers)

print post_r.text

step=2
temperature= [float(x)/step for x in range(15*step,30*step)]
while(True):
    for t in temperature:
        #time.sleep(0.005)
        send(t,'temperature')
    temperature.reverse()
'''
''''''

