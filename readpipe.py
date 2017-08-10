import os
import time
import re

FIFO = '/opt/chromis.pipe'

while True:
    print("Opening FIFO...")
    filename = time.strftime("%Y%m%d-%H%M%S")
    outfile = open('/opt/json/' + filename  + '.txt','w')
    with open(FIFO) as fifo:
        print("FIFO opened")
        while True:
            data = fifo.read()
            if len(data) == 0:
                print("Writer closed")
                outfile.close()
                os.rename('/opt/json/' + filename  + '.txt', '/opt/json/' + filename  + '.json')                
                break
            if bool(re.search('\x01\x1b\x70',data)) == True:
                print("gaga-Zeugs")
                print(data.encode("hex"))
                break
            #print(data.encode("hex"))
            #print("\n")
            newdata = re.sub(r'[\x00\x1d\x21\x3d\x1b\x01\x0a\xb7\xc2]+','',data)
            newdata = newdata.decode('cp850').encode('utf8')
            lcb = newdata.rfind("}")
            newdata = newdata[:lcb+1]+"\n"
            print(newdata)
            outfile.write("%s" % newdata)
