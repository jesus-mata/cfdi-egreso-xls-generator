rm -rf resources/*
# download latest archive from github as resources-sat-xml.zip
wget -O resources-sat-xml.zip https://github.com/phpcfdi/resources-sat-xml/archive/master.zip
# unzip the "resources" folder contents and place then into my-resources
unzip resources-sat-xml.zip 'resources-sat-xml-master/resources/*' -d resources

# move resources-sat-xml-master/resources/www.sat.gob.mx to resources
mv resources/resources-sat-xml-master/resources/www.sat.gob.mx/* resources

# remove resources-sat-xml.zip
rm resources-sat-xml.zip

# generate C# classes from XSD files
#./generar-code.sh

