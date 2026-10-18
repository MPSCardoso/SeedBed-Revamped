# Regressões do SeedBed Revamped

O executável testa a DLL Release empacotada, sem iniciar Unity/Valheim e sem framework de testes externo. Os 24 checks originais de defaults, migração e mapeamento permanecem; novos checks verificam traduções, bytes do bundle, identidade de Germination e incorporação dos managers/YamlDotNet.

```powershell
dotnet build src/SeedBed.csproj -c Release
dotnet run --project tests/SeedBed.RegressionChecks.csproj -c Release
```

Executar da raiz do repositório. Os caminhos locais vêm de BuildPaths.local.props ou de -p:ValheimPath=... e -p:BepInExCore=.... O build.ps1 executa esses passos e só atualiza a DLL de distribuição após aprovação dos checks.

Plantio, XP e avanço de Fazendeiro foram validados em solo pelo usuário na base 1.2.10. Multiplayer, construção, traduções e aparência continuam no roteiro de validação; consultar STATUS_E_PROXIMOS_PASSOS.md.

Validação da limpeza: **33 checks C# aprovados**, mais os cenários de cache válido e cache adulterado de DependencyRestore.Tests.ps1. Esse teste usa uma cópia exata do restaurador em fixture isolada, sem modificar as dependências reais ou acessar a rede.

O pipeline também valida nome/versão do manifest, identidade/versão/autoria da DLL, ícone PNG 256×256 e todos os arquivos do ZIP por SHA-256. O pacote contém uma única DLL e os créditos a blacks7ar e Tetus.
