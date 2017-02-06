import urllib
f = open('ardu.ino', 'w')
f.write(urllib.urlopen('https://github.com/ikscoder/WorldSkills/blob/master/ardu/ardu.ino').read())
f = open('Generator.py', 'w')
f.write(urllib.urlopen('https://raw.githubusercontent.com/ikscoder/WorldSkills/master/Generator/Generator/Generator.py').read())
f = open('AllEntities.twx', 'w')
f.write(urllib.urlopen('https://raw.githubusercontent.com/ikscoder/WorldSkills/master/AllEntities.twx').read())