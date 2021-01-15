cd bin/debug
nuget.exe push  *.nupkg 123456 -Source http://192.168.100.134:8087/api/v2/package 
pause