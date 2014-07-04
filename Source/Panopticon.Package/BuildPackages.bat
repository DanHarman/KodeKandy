%windir%\Microsoft.NET\Framework\v4.0.30319\MsBuild ..\KodeKandy.sln /p:Configuration=Release
rmdir lib -r
mkdir lib
mkdir lib\net40\
mkdir lib\portable-net45+sl50+win+wp80\
copy ..\Panopticon.Net40\bin\Release\KodeKandy.*.dll lib\net40\
copy ..\Panopticon.Net40\bin\Release\KodeKandy.*.pdb lib\net40\
copy ..\Panopticon.Portable\bin\Release\KodeKandy.*.dll "lib\portable-net45+sl50+win+wp80\"
copy ..\Panopticon.Portable\bin\Release\KodeKandy.*.pdb "lib\portable-net45+sl50+win+wp80\"
mkdir BuiltPackages
nuget pack KodeKandy.Panopticon.nuspec -OutputDirectory BuiltPackages

PAUSE