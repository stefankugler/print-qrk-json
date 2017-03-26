#!/bin/bash

# script based on
# https://sourceforge.net/p/chromispos/discussion/help/thread/8971d976/#1438
# by Wildfox Coder

# Printer: Type: Epson, Mode: file, Port: /opt/chromis.pipe

# Pipe
# Create pipe: 
# mkfifo /opt/chromis.pipe
pipe="/opt/chromis.pipe"

# QRK server mode directory
qrkpath="/opt/json"

while :
do
    # cat $pipepath > "/opt/bon.txt"
    cat $pipe | sed 's/\x1D\x21//g;s/\x1B\x3D\x01//g;s/\x1B\x69\x0\x0//g;s/\x1Bd0//g;s/\x1B.//g;s/\x01\x0//g' > "$qrkpath/bon.txt"
    now="$(date +'%Y%m%d-%H%M%S')"
    mv "$qrkpath/bon.txt" "$qrkpath/$now.json"
done
