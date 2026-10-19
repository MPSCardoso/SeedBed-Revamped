# Status e próximos passos — SeedBed Revamped / Valheim 1.0

## Correção 1.3.1 — Poteitr — 19/10/2026

- Adicionado o ciclo PoteitrSeeds → Poteitr e Poteitr → PoteitrSeeds, com consumo 5 e produção 15; total de 27 padrões. Resolução por prefab com fallback para $item_poteitr/$item_poteitrseeds, mapeamento e visuais genéricos existentes e logs Resource de Poteitr.
- Defaults Version = 2, local: versão 0 recebe os padrões do Deep North e Poteitr; versão 1 recebe somente Poteitr. Apenas From ausentes são acrescentados; destinos/quantidades personalizados e remoções anteriores de outras culturas são preservados.
- Backup .pre-poteitr.bak antes da escrita atômica, sem sobrescrever backups existentes nem .pre-valheim1.bak. YAML inválido permanece intacto; falhas permitem nova tentativa. A versão avança após sucesso, respeitando remoções posteriores.
- Versão 1.3.1 no plugin, assembly e manifest. Entrega: SeedBed.dll e artifacts/releases/SeedBed_Revamped-1.3.1.zip. Instalação de teste e publicação registradas abaixo.
- Pipeline completo aprovado: build Release sem erros, com os 8 avisos MSB3277 já documentados; **56 verificações de regressão aprovadas** contra a DLL mesclada; cache válido aceito e cache adulterado rejeitado; ZIP validado por lista de arquivos e SHA-256. A DLL da raiz foi atualizada após todas as verificações, com backup da distribuição anterior.
- SHA-256 da DLL: **f9ddd661d77a165c47aa98dbe3b6afed835318085874b27191920c477073fb82**. SHA-256 do ZIP publicado, reempacotado após registrar a validação: **3919306033a9abc26c1ea56f3484364d0fedf2ae659a32abf40be43d57c993d3**. Log do pipeline original: build_poteitr.log.
- **Validação funcional de Poteitr confirmada pelo mantenedor** no mundo de testes; versão 1.3.1 publicada no Hexium em 19/10/2026. A confirmação histórica de multiplayer da 1.3.0 continua separada deste relato.

### Instalação para teste no Gale — 19/10/2026

A pedido do usuário, a DLL 1.3.1 foi posteriormente instalada no perfil ValheimNerdola, em BepInEx/plugins/Tetus-SeedBed_Revamped/SeedBedRevamped/SeedBed.dll. Versão e SHA-256 conferidos contra a distribuição validada. DLL anterior preservada em artifacts/backups/Gale-ValheimNerdola-SeedBed-before-1.3.1-20261019-133042-065.dll. O jogo estava fechado; nenhuma configuração foi alterada. O perfil já usa Germination Duration = 60 e Defaults Version = 1, permitindo verificar a migração automática para 2 na próxima inicialização. Naquele momento, publicação e validação em jogo ainda estavam pendentes; ambas foram concluídas posteriormente, conforme os registros abaixo.

Teste rápido: iniciar o mundo de testes pelo perfil ValheimNerdola; no console F5, ativar devcommands e executar spawn PoteitrSeeds 10 e spawn Poteitr 10. Usar o item desejado pela barra rápida no canteiro vazio para escolher explicitamente o sentido da conversão. Cada lote deve consumir 5 itens e, após pouco mais de 60 segundos, produzir 15 do outro item; conferir visuais e Farming. Sair e reentrar para verificar persistência dos canteiros e ausência de duplicatas no YAML. Conferir também uma cultura anterior, como cenoura. Este roteiro não registra aprovação de conquistas com devcommands.

### Validação de Poteitr confirmada — 19/10/2026

Após instalar a 1.3.1 no perfil ValheimNerdola e receber o roteiro rápido, o mantenedor confirmou: "Perfeito, validado". Validação funcional de Poteitr no mundo de testes concluída por relato do usuário, autorizando a publicação. Não foram fornecidos novos logs, medições individuais nem confirmação separada de multiplayer ou conquistas nesta rodada. Os registros anteriores de pendência descrevem o estado antes dessa confirmação.

### Publicação 1.3.1 concluída — 19/10/2026

- GitHub: correção, DLL, metadados e documentação publicados em main no commit 93bb6f34abf0489ef0e52156cb7b1a329856b03a. Remoto origin atualizado sem divergência.
- Hexium: https://valheim.hexium.gg/mods/Tetus/SeedBed_Revamped — versão 1.3.1 pública e definida como latest, instalação Client & Server, README/descrição/changelog atualizados e download disponível. A página exibiu Scan successful.
- Bug report de TheXutz respondido em inglês, agradecendo o aviso e indicando a correção e a atualização para 1.3.1: https://valheim.hexium.gg/mods/Tetus/SeedBed_Revamped/threads/376#post-1057. Status confirmado como Done.
- Capturas locais: artifacts/publication/hexium-release-1.3.1.jpg e artifacts/publication/hexium-poteitr-report-done-1.3.1.jpg.
### Roteiro específico de Poteitr — referência para regressões

1. Com servidor e clientes na 1.3.1, iniciar usando a configuração 1.3.0. Conferir as entradas ausentes de Poteitr, o backup e Defaults Version = 2; reiniciar e verificar ausência de duplicatas. Repetir com overrides e culturas antigas removidas.
2. Plantar 5 PoteitrSeeds, conferir visual e tempo de germinação e colher 15 Poteitr. Plantar 5 Poteitr e colher 15 PoteitrSeeds. Conferir XP de Farming e estatísticas nas duas operações, conforme as regras nativas existentes.
3. Repetir em solo e multiplayer, usando canteiros existentes e de outro cliente. Disputar interações; conferir crédito apenas à ação aceita e devolução de itens recusados. Recarregar o mundo durante a germinação para conferir conteúdo e visuais.
4. Se houver falha, registrar BepInEx/LogOutput.log, especialmente Conversion PoteitrSeeds/Poteitr e Resource $item_poteitr/$item_poteitrseeds. Conferir também uma cultura anterior, como cenoura.

## Histórico da versão 1.3.0

Atualizado em 18/10/2026. **SeedBed Revamped 1.3.0**, mantido por **Tetus**, baseado no SeedBed 1.2.9 de **blacks7ar** e nas correções locais 1.2.10.

## Identidade e publicação

- Nome visível: **SeedBed Revamped**; pacote Thunderstore: **SeedBed_Revamped**; versão **1.3.0**. Créditos ao criador blacks7ar preservados no README, changelog, CREDITS.md, avisos de terceiros, metadados e log de inicialização.
- GUID blacks7ar.SeedBed, assembly/DLL SeedBed, componente SeedBed.Functions.Germination, prefab BSB_SeedBed e arquivos blacks7ar.SeedBed.cfg/.yml preservados para compatibilidade. Revamped substitui o original; manter apenas uma DLL ativa.
- Funcionalidades divulgadas: vegetais do Deep North e arando; Farming nativa ao plantar e colher; registro nativo da colheita para conquistas elegíveis. O bloqueio global do Valheim em sessões com mods exige uma ferramenta como Unshamed, não é removido por esta DLL.
- ZIP preparado em artifacts/releases/SeedBed_Revamped-1.3.0.zip, com uma única DLL, manifest, ícone 256×256, README de publicação, changelog, créditos e licenças. SHA-256 do ZIP: **02cf2a049368f0c2ada06f897ae584145aa8f10ecfb6905d4ae51154399b0ec4**.
- Ícone atualizado com image_gen, mantendo os canteiros originais e adicionando SeedBed Revamped. Descrição, README de publicação, changelog e créditos em inglês.
- **Publicado no Hexium em 18/10/2026:** https://valheim.hexium.gg/mods/Tetus/SeedBed_Revamped, equipe Tetus, versão 1.3.0, instalação Client & Server, visibilidade pública. Tags Farming, Pieces, Quality of Life e Valheim 1.0. Ícone e descrição em inglês confirmados na página; download disponível.
- O pacote publicado mantém website_url vazio; a página própria está registrada acima para uso em versões futuras. A instalação da 1.3.0 no Gale não foi realizada pela automação.

## Funcionalidades preservadas

- KaleSeeds → Kale, Kale → KaleSeeds, OatSeeds → Oat, Oat → OatSeeds e Lingonberries → Lingonberries; defaults de 5 para 15, junto das 20 conversões anteriores.
- Migração única do YAML por From ausente; preserva overrides e quantidades, salva backup .pre-valheim1.bak e mantém YAML inválido intacto. Defaults Version = 1 continua local, sem sincronização.
- Resolução dos itens novos por nome de prefab ou chave de localização confirmada; inversão genérica semente/vegetal, fallback e compatibilidade BCA. Arando usa a grade 5×5.
- Farming nativa ao plantar/colher; Exp Amount padrão 5 por operação, intervalo 0,01–25. Valores anteriores são preservados. Herbalist continua recebendo XP na colheita de ervas compatíveis.
- Estatísticas usam SeedAmount plantas: colheita por m_harvestStat e nome do prefab do item; plantio por Builds e peça Plant correspondente. PlaceStacks não é incrementado, conforme o fluxo vanilla verificado. Sem bônus aleatório adicional.
- Crédito apenas após confirmação do dono da SeedBed; pedidos de plantio recusados devolvem sementes ao inventário ou ao chão. GUID, namespaces públicos e RPCs foram mantidos. Nome visível atualizado para SeedBed Revamped; servidor e clientes precisam da versão 1.3.0.

## Limpeza implementada

- Removidas fontes decompiladas de YamlDotNet, managers e atributos auxiliares. Fontes próprias estão em src/SeedBed; metadados em src/Properties.
- YamlDotNet 16.0.0 do NuGet. Managers oficiais compilados em projetos separados a partir de revisões fixas verificadas por SHA-256; fontes restauradas apenas em .cache/dependencies, ignorado pelo Git.
- PieceManager oficial 1.4.0 (c3f70ff), ServerSync (c57c2aa) e LocalizationManager 1.3 (1bf7659). Manifesto: build/dependencies.lock.json; pacotes NuGet travados em packages.lock.json.
- BepInEx.AssemblyPublicizer.MSBuild 0.4.3 gera referências publicizadas em obj usando a instalação local, sem versionar DLLs do jogo.
- ILRepack.Lib.MSBuild.Task 2.0.46 incorpora os managers e uma única versão do YAML. A DLL intermediária fica separada da DLL empacotada, evitando mesclagem acumulada em builds incrementais.
- Removidos comentários de IL, casts redundantes, detector de Farming externo e atributos próprios sem uso. Loops de criação/limpeza dos visuais foram consolidados, mantendo transforms, escala, cores e efeitos.
- Bundle e tradução reorganizados em src/assets e src/translations, com os mesmos nomes lógicos incorporados.
- Caminhos locais centralizados em BuildPaths.local.props, ignorado pelo Git; exemplo versionado e substituição por propriedades de linha de comando.
- Snapshot anterior à limpeza: artifacts/backups/pre-cleanup-2026-10-18T17-44-50-007Z. DLL original: artifacts/backups/SeedBed.original-1.2.9.dll. Build anterior: artifacts/backups/SeedBed-1.2.10-before-cleanup.dll. A limpeza não instalou arquivos no Gale; a instalação para teste solo foi realizada posteriormente pelo usuário.

## Validação e distribuição

- Pipeline completo aprovado com restauração travada usando fonte NuGet local: **zero erros**, **8 avisos MSB3277** de referências System.Net.Http/System.IO.Compression entre SDK e jogo (dois por projeto). Nenhum aviso de C# e nenhuma supressão global.
- **33 verificações de regressão aprovadas**, incluindo as 24 originais, traduções, bytes do bundle, identidade de Germination e dependências incorporadas.
- Teste de integridade aprovado: cache válido aceito e arquivo adulterado rejeitado, usando fixture isolada.
- Empacotamento aprovado: DLL intermediária rejeitada; artefato mesclado aceito; os sete arquivos do ZIP foram conferidos por SHA-256, incluindo a DLL idêntica à distribuição da raiz.
- Comparação estática com o snapshot confirmou os corpos dos oito fluxos de RPC, XP/estatísticas e inserção/uso de itens, descontando casts e formatação. Posteriormente, o usuário confirmou todas as funcionalidades no jogo em servidor multiplayer.
- Árvore src reduzida de **549 para 11 arquivos C#**; fontes de terceiros e atributos gerados não estão mais na árvore versionável.
- DLL da raiz idêntica ao pacote testado: **1358848 bytes**. SHA-256: **87b60f7bde1fee69e7bc7c0617b58a9a2f3009a622852bd603f5a23edefb39fe**.
- Snapshot e DLLs anteriores preservados em artifacts/backups; cache, backups, caminhos locais e referências geradas foram confirmados como ignorados pelo Git.

O build.ps1 valida a DLL empacotada e o ZIP antes de atualizar SeedBed.dll na raiz, preservando a distribuição anterior em backup. O empacotamento confere nome/versão, metadados de autoria, ícone 256×256, lista exata de arquivos e hashes de cada entrada.

## Build

Requisitos: .NET 10 SDK, PowerShell 7 e Valheim/BepInEx locais. Copiar BuildPaths.example.props para BuildPaths.local.props e preencher os caminhos.

```powershell
pwsh -NoProfile -File ./build.ps1
```

Para compilar sem atualizar a distribuição:

```powershell
dotnet build src/SeedBed.csproj -c Release
dotnet run --project tests/SeedBed.RegressionChecks.csproj -c Release
```

Distribuição empacotada: src/bin/Release/netstandard2.1/package/SeedBed.dll. Não instalar a DLL intermediária fora da pasta package.

## Teste solo — retorno do usuário em 18/10/2026

Os testes no jogo foram feitos na base 1.2.10. A versão 1.3.0 altera nome/metadados/log de inicialização e empacotamento, preservando o código das funções validadas.

- **Confirmado pelo usuário:** plantio dos itens, recebimento de experiência pelo personagem e funcionamento do avanço da conquista **Fazendeiro** no teste solo.
- Conquista acompanhada: **Fazendeiro** (chave localizada ach_grindharvest; descrição do jogo instalado: Faça 500 colheitas). O usuário informa não ter usado devcommands e utilizar um mod que remove o bloqueio de conquistas.
- Log da sessão: Unshamed 1.0.8 carregado e Eligible for achievements: True. Isso confirma a elegibilidade reportada naquela sessão, mas não o avanço do contador.
- Inspeção estática: IncrementStatPickable atualiza m_pickableStats e, quando elegível, chama CheckAchievementProgressUnlockPickable; IncrementStat dispara a verificação por PlayerStatType. SeedBed chama ambas conforme m_harvestStat, com crédito de SeedAmount.
- Validação de Fazendeiro concluída por relato do usuário após a orientação de repetir o teste com uma cultura agrícola. O teste inicial havia sido feito com framboesas. Não foram informados valores do contador nem o desbloqueio final das 500 colheitas; a confirmação registrada é do funcionamento do avanço.
- Posteriormente, o usuário confirmou o funcionamento em servidor multiplayer e a validação de todas as funcionalidades do mod.

## Validação multiplayer concluída — retorno do usuário em 18/10/2026

O usuário testou a versão publicada em um servidor multiplayer e confirmou que todas as funcionalidades do mod foram validadas. A validação funcional no jogo está concluída conforme esse relato; não foram fornecidos novos logs ou medições individuais para cada cenário de teste.

README principal em inglês, com português preservado em README.pt-BR.md. Projeto: https://github.com/MPSCardoso/SeedBed-Revamped. Descrição de publicação atualizada com a confirmação multiplayer e o link do código.

## Roteiro de referência para futuras regressões

Em futuras versões, conferir nome SeedBed Revamped no log, configuração anterior carregada e canteiros existentes preservados.

1. Validar construção: bancada, categoria CollectorSeries, receita YmirRemains ×5 / BronzeNails ×8 / Wood ×18, configurações geradas, efeitos e shaders. O PieceManager foi substituído de 1.3.0 embutido para oficial 1.4.0.
2. Validar tradução inglesa e troca de idioma, incluindo arquivos externos de tradução compatíveis.
3. Iniciar com YAML antigo e uma conversão personalizada. Conferir backup, novos From, quantidades, ausência de duplicatas após reinício e respeito a remoções posteriores à migração.
4. Plantar KaleSeeds, Kale, OatSeeds, Oat e Lingonberries: consumo 5, visual, timer e produção 15 do item correto. Repetir CarrotSeeds/Carrot para o ciclo antigo.
5. **Validado em solo pelo usuário:** recebimento de XP e avanço da conquista Fazendeiro. Em testes adicionais, conferir outros contadores/conquistas elegíveis e o crédito por lote: SeedAmount plantas, não o número de itens produzidos.
6. Em multiplayer, interagir com uma SeedBed de outro cliente; repetir cliques e disputar plantio/colheita. Só a ação aceita deve receber crédito, sem perda de sementes no pedido recusado. Conferir inventário cheio na devolução e configurações sincronizadas.
7. Em caso de problema, consultar BepInEx/LogOutput.log: entradas Conversion e Resource. A grade 5×5 do arando pode exigir ajuste após inspeção visual.

[APIs vanilla verificadas](docs/VALHEIM_API.md) · [Créditos e licenças](docs/THIRD_PARTY_NOTICES.md) · [Regressões](tests/README.md)

O teste solo confirmou plantio, recebimento de experiência e avanço da conquista Fazendeiro. O teste posterior em servidor multiplayer confirmou todas as funcionalidades, conforme relato do usuário.
