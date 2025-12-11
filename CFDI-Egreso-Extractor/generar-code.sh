#!/bin/bash
set -e 
#Tool 'dotnet-xscgen' was successfully updated from version '2.1.1057' to version '2.1.1160'.
baseDotNetNamespace="JAMS.SAT.CFDI.Xml"
resPath=resources/sitio_internet

# create function to generate code
# $1 = xsd path to generate code
# $2 = namespace to map
function generateCode {
    echo "Generando clases para $1"
    
    # if namespace is not set, use the path as namespace
    if [ -z "$2" ]
    then
        xscgen -p $baseDotNetNamespace --nullable --nc --cn --sf --nh -v -o XML --enumAsString --namespaceFile=namespaces_mapping.txt $resPath/$1 &
    else
        xscgen -p $baseDotNetNamespace --nullable --nc --cn --sf --nh -v -o XML --enumAsString --namespaceFile=namespaces_mapping.txt -n $2 $resPath/$1 & 
    fi
}
trap "trap - SIGTERM && kill -- -$$" SIGINT SIGTERM EXIT # kill all child processes when this script is killed

echo "Starting class generation time: $(date)"

# clean previous generated code
rm -rf XML/NextSolution/*

#ver3
#echo "Generando código de CFDI ver 4.0"
#xscgen --nc --cn --sf --nh --namespaceFile=namespaces_mapping.txt -v -o cfd resources/resources-sat-xml-master/resources/www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd
#echo "Generando código de CFDI ver 3.3"
#xscgen --nc --cn --sf --nh --namespaceFile=namespaces_mapping.txt -n http://www.sat.gob.mx/cfd/3=cfd.v33 -v -o cfd resources/resources-sat-xml-master/resources/www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd
#echo "Generando código de CFDI ver 3.2"
#xscgen --nc --cn --sf --nh --namespaceFile=namespaces_mapping.txt -n http://www.sat.gob.mx/cfd/3=cfd.v32 -v -o cfd resources/resources-sat-xml-master/resources/www.sat.gob.mx/sitio_internet/cfd/3/cfdv32.xsd
#echo "Generando código para divisas"
#xscgen --nc --cn --sf --nh --namespaceFile=namespaces_mapping.txt -v -o cfd resources/resources-sat-xml-master/resources/www.sat.gob.mx/sitio_internet/cfd/divisas/divisas.xsd

generateCode "cfd/4/cfdv40.xsd" "http://www.sat.gob.mx/cfd/4=v40" 
# generateCode "cfd/3/cfdv33.xsd" "http://www.sat.gob.mx/cfd/3=v33" 
# generateCode "cfd/3/cfdv32.xsd" "http://www.sat.gob.mx/cfd/3=v32" 

# Divisas
generateCode "cfd/divisas/divisas.xsd" "http://www.sat.gob.mx/divisas=Divisas" 

# Complemento de Estado de Cuenta Bancaria
generateCode "cfd/ecb/ecb.xsd" "http://www.sat.gob.mx/ecb=ECB"

# Complemento ecc
generateCode "cfd/ecc/ecc.xsd" "http://www.sat.gob.mx/ecc=ECC"

# Complemento Estado de Cuenta de Combustibles
generateCode "cfd/EstadoDeCuentaCombustible/ecc12.xsd" "http://www.sat.gob.mx/EstadoDeCuentaCombustible=EstadoCuentaCombustible.v12"

# Commplemento INE
generateCode "cfd/ine/ine11.xsd" "http://www.sat.gob.mx/ine=INE.v11"

# Complemento Nomina
generateCode "cfd/nomina/nomina11.xsd" "http://www.sat.gob.mx/nomina=Nomina.v11" 
generateCode "cfd/nomina/nomina12.xsd" "http://www.sat.gob.mx/nomina=Nomina.v12" 

# Complemento Carta Porte
generateCode "cfd/CartaPorte/*.xsd" 

# Complemento Timbre Fiscal Digital
generateCode "cfd/TimbreFiscalDigital/TimbreFiscalDigital.xsd" "http://www.sat.gob.mx/TimbreFiscalDigital=TFD.v1" 
generateCode "cfd/TimbreFiscalDigital/TimbreFiscalDigitalv11.xsd" "http://www.sat.gob.mx/TimbreFiscalDigital=TFD.v11"

# Complemento Comercio Exterior 2.0
generateCode "cfd/ComercioExterior20/ComercioExterior20.xsd"

# Complemento Pagos
generateCode "cfd/Pagos/Pagos20.xsd" "http://www.sat.gob.mx/Pagos20=Pagos.v20"

# Complemento Leyendas Fiscales
generateCode "cfd/leyendasFiscales/leyendasFisc.xsd" "http://www.sat.gob.mx/leyendasFiscales=LeyendasFiscales"

echo "Esperando a que terminen los procesos en segundo plano"
wait # for all background processes to finish

echo "Class generation finished! Time: $(date)"