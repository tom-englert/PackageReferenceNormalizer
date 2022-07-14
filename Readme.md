# Package Reference Normalizer

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

