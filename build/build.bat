cd ../src/ImageRep

dotnet restore
dotnet build --no-restore
dotnet publish -o ../../deploy