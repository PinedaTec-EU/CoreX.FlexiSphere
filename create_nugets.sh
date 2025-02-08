#!/bin/bash

# Leer el contenido del archivo version.nfo
version=$(cat version.nfo)

# Dividir la versión en partes usando el punto como separador
IFS='.' read -r -a parts <<< "$version"

# Incrementar el número intermedio
((parts[1]++))

# Reconstruir la versión
new_version="${parts[0]}.${parts[1]}.${parts[2]}"

# Guardar la nueva versión en el archivo version.nfo
echo "$new_version" > version.nfo

# Imprimir la nueva versión
echo "La nueva versión es: $new_version"

# Crear el release
dotnet build -c Release -p:Version=$new_version

# Crear el paquete NuGet
nuget pack src/ark.FlexiSphere/.nuspec -Version $new_version -outputdirectory ./nugets

# Publicar el paquete NuGet to local folder
cp ./nugets/ark.*.$new_version.nupkg ../nugets