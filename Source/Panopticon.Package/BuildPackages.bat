copy ..\Panopticon.Net40\bin\Release\KodeKandy.Core.dll lib\net40
copy ..\Panopticon.Net40\bin\Release\KodeKandy.Panopticon.dll lib\net40
copy ..\Panopticon.Portable\bin\Release\KodeKandy.Core.dll lib\portable-net45+sl50+win+wp80
copy ..\Panopticon.Portable\bin\Release\KodeKandy.Panopticon.dll lib\portable-net45+sl50+win+wp80
mkdir BuiltPackages
nuget pack KodeKandy.Panopticon.0.5.0.0.nuspec -OutputDirectory BuiltPackages