del published -Recurse
dotnet publish -c Release -o published -r linux-x64 --self-contained false -p:PublishReadyToRun=true
C:\"Program Files"\7-Zip\7z.exe a -r -mx8 published/cv.zip .\published\*

Transfer to home/masilver/docker/temp/cv
unzip into home/masilver/docker/temp/cv
Docker build new container
Docker compose
