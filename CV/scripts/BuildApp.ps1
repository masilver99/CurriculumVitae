param(
    [Parameter(Mandatory=$true)]
    [string]$Password
)

del published -Recurse
dotnet publish -c Release -o published -r linux-x64 --self-contained false 
C:\"Program Files"\7-Zip\7z.exe a -r -mx8 published/cv.zip .\published\*
cmd.exe /c echo y | pscp -pw $Password published/cv.zip ubuntu@michaelsilver.us:/home/ubuntu/docker/sites/cv.michaelsilver.us/temp/cv.zip
cmd.exe /c echo y | pscp -pw $Password Dockerfile-release ubuntu@michaelsilver.us:/home/ubuntu/docker/sites/cv.michaelsilver.us/temp/Dockerfile-release 
plink -ssh -t -pw $Password ubuntu@michaelsilver.us -m scripts/hostscript.txt
