import urllib
f = open('ardu.ino', 'w')
f.write(urllib.urlopen('https://github.com/ikscoder/WorldSkills/blob/master/ardu/ardu.ino').read())
f = open('Generator.py', 'w')
f.write(urllib.urlopen('https://raw.githubusercontent.com/ikscoder/WorldSkills/master/Generator/Generator/Generator.py').read())


f = open('MainWindow.xaml.cs', 'w')
f.write(urllib.urlopen('https://raw.githubusercontent.com/ikscoder/WorldSkills/master/LocalServer/LocalServer/MainWindow.xaml.cs').read())
f = open('MainWindow.xaml', 'w')
f.write(urllib.urlopen('https://raw.githubusercontent.com/ikscoder/WorldSkills/master/LocalServer/LocalServer/MainWindow.xaml').read())
f = open('Diagram.cs', 'w')
f.write(urllib.urlopen('https://raw.githubusercontent.com/ikscoder/WorldSkills/master/LocalServer/LocalServer/Diagram.cs').read())
f = open('DataModel.cs', 'w')
f.write(urllib.urlopen('https://raw.githubusercontent.com/ikscoder/WorldSkills/master/LocalServer/LocalServer/DataModel.cs').read())
