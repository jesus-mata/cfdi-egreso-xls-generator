#!/bin/bash

CJPROJ=/Users/jesus.mata/RiderProjects/CFDI-Egreso-Extractor/CFDI-Egreso-Extractor/CFDI-Egreso-Extractor.csproj

echo "Publish starting... Time: $(date)"

rm -rf /Users/jesus.mata/RiderProjects/CFDI-Egreso-Extractor/CFDI-Egreso-Extractor/bin/Release/net8.0/osx-arm64
dotnet publish -c Release --runtime osx-arm64 -p:PublishSingleFile=true -p:PublishReadyToRun=true -o /Users/jesus.mata/RiderProjects/CFDI-Egreso-Extractor/CFDI-Egreso-Extractor/bin/Release/net8.0/osx-arm64 $CJPROJ

echo "Mac arm64 finished! Time: $(date)" 

rm -rf /Users/jesus.mata/RiderProjects/CFDI-Egreso-Extractor/CFDI-Egreso-Extractor/bin/Release/net8.0/win-x64
dotnet publish -c Release --runtime win-x64 -p:PublishSingleFile=true -p:PublishReadyToRun=true -o /Users/jesus.mata/RiderProjects/CFDI-Egreso-Extractor/CFDI-Egreso-Extractor/bin/Release/net8.0/win-x64 $CJPROJ 

echo "Windows x64 finished! Time: $(date)"

cd /Users/jesus.mata/RiderProjects/CFDI-Egreso-Extractor/CFDI-Egreso-Extractor/bin/Release/net8.0/win-x64 || exit
rm /Users/jesus.mata/apps/cfdi-egreso-extractor.zip
zip /Users/jesus.mata/apps/cfdi-egreso-extractor.zip CFDI-Egreso-Extractor.exe

echo ""
echo "Publish finished! Time: $(date)"

echo "test release"
cp /Users/jesus.mata/RiderProjects/CFDI-Egreso-Extractor/CFDI-Egreso-Extractor/bin/Release/net8.0/osx-arm64/CFDI-Egreso-Extractor ~/apps/cfdi-egreso-extractor/ 
cd /Users/jesus.mata/apps/cfdi-egreso-extractor
./CFDI-Egreso-Extractor