#! /usr/bin/env bash
set -uvx
set -e
cwd=`pwd`
ts=`date "+%Y.%m%d.%H%M.%S"`
version="${ts}"

cd $cwd/WinLib
sed -i -e "s/<Version>.*<\/Version>/<Version>${version}<\/Version>/g" WinLib.csproj
rm -rf obj bin
rm -rf *.nupkg
java -jar ./antlr-4.13.1-complete.jar JSON5.g4 -Dlanguage=CSharp -package WinLib.Parser.Json5 -o Parser/Json5
dotnet pack -o . -p:Configuration=Release -p:Platform="Any CPU" WinLib.csproj

#exit 0

tag="WinLib-v$version"
cd $cwd
git add .
git commit -m"$tag"
git tag -a "$tag" -m"$tag"
git push origin "$tag"
git push
git remote -v
