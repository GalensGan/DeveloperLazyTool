InputBox, OutputVar , "insert MAC", "please enter a validate MAC address："
SendRaw,telnet 192.168.100.3
Send,{Enter}
SendRaw,admin
Send,{Enter}
SendRaw,admin12345