# DotNet-Starter-Template
##### this is intended to be the best practice for defining your .NET solution directory
          
##
##
# Directory Structure
| Folder Name | Description |
| ------ | ------ |
| src | Main projects (the product code) |
| tests | Test projects |
| docs | Documentation stuff, markdown files, help files etc. |
| samples | (optional) - Sample projects |
| lib | Things that can NEVER exist in a nuget package |
| artifacts | Build outputs go here. Doing a build.cmd/build.sh generates artifacts here (nupkgs, dlls, pdbs, etc.) |
| packages | NuGet packages |
| build | Build customizations scripts|

CREDITS TO [https://gist.github.com/davidfowl/ed7564297c61fe9ab814] 
