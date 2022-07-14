# Package Reference Normalizer

[![Build Status](https://dev.azure.com/tom-englert/Open%20Source/_apis/build/status/tom-englert.PackageReferenceNormalizer?branchName=main)](https://dev.azure.com/tom-englert/Open%20Source/_build/latest?definitionId=50&branchName=main)
[![NuGet Status](http://img.shields.io/nuget/v/PackageReferenceNormalizer.svg?style=flat-square)](https://www.nuget.org/packages/PackageReferenceNormalizer)

A tool that scans your project files and converts

``` xml
<PackageReference Include="MyPackage">
  <Version>1.2.3.4</Version>
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
```

to 

``` xml
<PackageReference Include="MyPackage" Version="1.2.3.4" PrivateAssets="all" />
```

- All elements are converted to attributes
- `IncludeAssets` is removed if `PrivateAssets=All` 

### Instllation

To install the tool, run

`dotnet tool install -g PackageReferenceNormalizer`

### Usage
Run `PackageReferenceNormalizer c:\Dev\MyProject\*.csproj`

This will scan all `*.csproj` files in `c:\Dev\MyProject` and all sub-folders

Run `PackageReferenceNormalizer *.csproj`

This will scan all `*.csproj` files in the current directory and all sub-folders
