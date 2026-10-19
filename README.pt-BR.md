# SeedBed Revamped

[English](README.md) · **Português (Brasil)**

Versão Revamped mantida por **Tetus**, baseada no [SeedBed original de blacks7ar](https://thunderstore.io/c/valheim/p/blacks7ar/SeedBed/). O código base, o canteiro, os assets e a ideia original são de blacks7ar. Permite cultivar sementes, vegetais, frutas e cogumelos em uma SeedBed, sem cultivar grandes áreas de terreno.

A versão **1.3.1** adiciona o ciclo completo de Poteitr: PoteitrSeeds → Poteitr e Poteitr → PoteitrSeeds. Mantém couve/aveia e suas sementes, arando, XP de Farming nativa ao plantar e colher e registro da colheita nas estatísticas nativas. As 27 conversões padrão consomem 5 itens e produzem 15.

O YAML continua personalizável. A atualização da 1.3.0 acrescenta somente os From ausentes de Poteitr, preservando destinos, quantidades e culturas antigas removidas pelo usuário. Instalações anteriores também recebem os padrões do Deep North. A migração cria .pre-poteitr.bak sem sobrescrever backups existentes; YAML inválido permanece intacto para nova tentativa na próxima inicialização. Defaults Version avança para 2 após sucesso, respeitando remoções posteriores.

## Conquistas

A SeedBed Revamped registra as colheitas pelas APIs nativas do Valheim e preserva as condições de elegibilidade do jogo. Ela não remove o bloqueio global de conquistas em partidas com mods. Para obter conquistas nesse cenário, é necessário habilitá-las com uma ferramenta como [Unshamed, de Azumatt](https://thunderstore.io/c/valheim/p/Azumatt/Unshamed/); o teste solo de Fazendeiro foi validado com Unshamed 1.0.8. Frutas e culturas agrícolas têm categorias de colheita distintas; o uso de sementes não é o critério. O crédito do lote usa SeedAmount plantas, não ProducedItems.

## Instalação

Com o jogo fechado, desativar o pacote SeedBed original e instalar a **SeedBed.dll** da raiz em BepInEx/plugins/SeedBedRevamped. Manter apenas uma DLL ativa da família SeedBed; mover backups para fora de BepInEx/plugins. Servidor e clientes precisam da versão 1.3.1. Os managers e YamlDotNet estão incorporados; não devem ser instalados separadamente.

O GUID interno blacks7ar.SeedBed, os nomes de configuração blacks7ar.SeedBed.cfg/.yml, o assembly SeedBed e o componente SeedBed.Functions.Germination são preservados para compatibilidade com instalações e canteiros existentes. Revamped substitui o original; não deve ser instalado junto dele.

Backups ficam em artifacts/backups, fora da pasta de plugins. A build não instala arquivos no Gale nem altera configurações do jogador.

## Pacote para publicação

O build.ps1 gera artifacts/releases/SeedBed_Revamped-1.3.1.zip após os checks. Inclui manifest, ícone 256×256, [README de publicação](docs/README_THUNDERSTORE.md), changelog, créditos/licenças e uma única DLL em BepInEx/plugins/SeedBedRevamped. A versão 1.3.1 foi publicada em 19/10/2026. O pacote mantém website_url vazio. A página do mod está no [Hexium](https://valheim.hexium.gg/mods/Tetus/SeedBed_Revamped) e o código no [GitHub](https://github.com/MPSCardoso/SeedBed-Revamped); a autoria da versão é Tetus.

O empacotamento também pode ser repetido sobre a DLL já validada:

```powershell
pwsh -NoProfile -File build/Create-Package.ps1
```

## Desenvolvimento

Requisitos: .NET 10 SDK, PowerShell 7, Valheim e BepInEx locais.

1. Copiar BuildPaths.example.props para BuildPaths.local.props.
2. Configurar ValheimPath para o diretório valheim_Data/Managed e BepInExCore para BepInEx/core do perfil.
3. Executar:

```powershell
pwsh -NoProfile -File ./build.ps1
```

O script restaura versões travadas, compila Release, executa as regressões contra a DLL empacotada e só então atualiza a DLL da raiz e gera o ZIP. A versão anterior é preservada em backup. Caminhos podem ser substituídos com os parâmetros -ValheimPath e -BepInExCore; -NuGetSource aceita uma fonte NuGet local/offline.

Para apenas compilar, sem atualizar a distribuição:

```powershell
dotnet build src/SeedBed.csproj -c Release
```

O resultado para distribuição fica em src/bin/Release/netstandard2.1/package/SeedBed.dll. A DLL fora da pasta package é intermediária e requer as dependências separadas.

## Organização

- src/SeedBed: código próprio e metadados do mod; Functions contém a integração Unity/Valheim.
- src/assets e src/translations: bundle e traduções originais, incorporados com os mesmos nomes lógicos.
- build: projetos de dependências, hashes e scripts de preparação/empacotamento.
- tests: regressões de conversões, migração, recursos, traduções e empacotamento.
- .cache: fontes oficiais restauradas e pacotes locais, ignorados pelo Git.
- artifacts/backups: snapshot anterior à limpeza e DLLs de backup, ignorados pelo Git.

As referências publicizadas do jogo são geradas nos diretórios obj. Nenhuma DLL do jogo é versionada ou distribuída com o mod.

## Dependências

YamlDotNet 16.0.0 vem do NuGet. PieceManager, ServerSync e LocalizationManager são compilados de fontes oficiais em revisões fixas, restauradas em cache e verificadas por SHA-256. O manifesto build/dependencies.lock.json registra origens e hashes; os arquivos packages.lock.json travam os pacotes NuGet. A build falha se uma fonte em cache divergir do hash esperado.

ILRepack.Lib.MSBuild.Task 2.0.46 incorpora as dependências em uma DLL; BepInEx.AssemblyPublicizer.MSBuild 0.4.3 prepara as referências locais. Não alterar fontes dentro do cache: mudanças deliberadas devem atualizar o manifesto e passar novamente pelas verificações.

[Créditos e licenças](docs/THIRD_PARTY_NOTICES.md) · [APIs vanilla verificadas](docs/VALHEIM_API.md) · [Status e roteiro de teste no jogo](STATUS_E_PROXIMOS_PASSOS.md)

A versão 1.3.0 foi testada pelo mantenedor em servidor multiplayer, com todas as funcionalidades validadas. O mantenedor confirmou a validação de Poteitr na 1.3.1 em seu mundo de testes. Consultar o [registro e roteiro](STATUS_E_PROXIMOS_PASSOS.md); não foi relatado um teste separado de Poteitr em multiplayer.

[Demonstração do mod original](https://www.youtube.com/watch?v=C85sJ1sT49w).
