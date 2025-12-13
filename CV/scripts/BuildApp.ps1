del published -Recurse
dotnet publish -c Release -o published -r linux-x64 --self-contained false 
C:\"Program Files"\7-Zip\7z.exe a -r -mx8 published/cv.zip .\published\*
cmd.exe /c echo y | pscp -pw Everex12!! published/cv.zip masilver@michaelsilver.us:/home/masilver/docker/temp/cv.zip
plink -ssh -t -pw Everex12!! masilver@michaelsilver.us -m scripts/hostscript.txt
