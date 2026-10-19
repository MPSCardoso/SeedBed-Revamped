# Regressões do SeedBed Revamped

O executável testa a DLL Release empacotada, sem iniciar Unity/Valheim e sem framework de testes externo. Os 24 checks originais de defaults, migração e mapeamento permanecem; novos checks verificam traduções, bytes do bundle, identidade de Germination e incorporação dos managers/YamlDotNet.

Na 1.3.1, a suíte também verifica as 27 conversões, o ciclo completo de Poteitr, migração por Defaults Version (0/1/2 e versão futura), overrides de ambos os itens, remoções anteriores, backup .pre-poteitr.bak, preservação do backup antigo, YAML inválido e nova tentativa. O mapeamento de Poteitr é coberto com Pickable só do vegetal, só das sementes e dos dois lados. Testes em jogo de Poteitr, incluindo fallback de resolução, visuais, XP, estatísticas e multiplayer, permanecem pendentes no roteiro de STATUS_E_PROXIMOS_PASSOS.md.

Validação da 1.3.1 em 19/10/2026: **56 checks C# aprovados**, teste de integridade das dependências aprovado e ZIP 1.3.1 validado pelo pipeline completo. Build Release sem erros, com os 8 avisos MSB3277 já documentados na base anterior. Log local: build_poteitr.log.

Validação funcional posterior: o mantenedor confirmou Poteitr no mundo de testes em 19/10/2026 e autorizou a publicação. Esse relato encerra a pendência do teste funcional local; não inclui confirmação separada de multiplayer ou conquistas nesta rodada.

```powershell
dotnet build src/SeedBed.csproj -c Release
dotnet run --project tests/SeedBed.RegressionChecks.csproj -c Release
```

Executar da raiz do repositório. Os caminhos locais vêm de BuildPaths.local.props ou de -p:ValheimPath=... e -p:BepInExCore=.... O build.ps1 executa esses passos e só atualiza a DLL de distribuição após aprovação dos checks.

Plantio, XP e avanço de Fazendeiro foram validados em solo pelo usuário na base 1.2.10. Multiplayer, construção, traduções e aparência continuam no roteiro de validação; consultar STATUS_E_PROXIMOS_PASSOS.md.

Validação da limpeza: **33 checks C# aprovados**, mais os cenários de cache válido e cache adulterado de DependencyRestore.Tests.ps1. Esse teste usa uma cópia exata do restaurador em fixture isolada, sem modificar as dependências reais ou acessar a rede.

O pipeline também valida nome/versão do manifest, identidade/versão/autoria da DLL, ícone PNG 256×256 e todos os arquivos do ZIP por SHA-256. O pacote contém uma única DLL e os créditos a blacks7ar e Tetus.
