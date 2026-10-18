# Status e próximos passos — SeedBed Revamped / Valheim 1.0

Atualizado em 18/10/2026. **SeedBed Revamped 1.3.0**, mantido por **Tetus**, baseado no SeedBed 1.2.9 de **blacks7ar** e nas correções locais 1.2.10.

## Identidade e publicação

- Nome visível: **SeedBed Revamped**; pacote Thunderstore: **SeedBed_Revamped**; versão **1.3.0**. Créditos ao criador blacks7ar preservados no README, changelog, CREDITS.md, avisos de terceiros, metadados e log de inicialização.
- GUID blacks7ar.SeedBed, assembly/DLL SeedBed, componente SeedBed.Functions.Germination, prefab BSB_SeedBed e arquivos blacks7ar.SeedBed.cfg/.yml preservados para compatibilidade. Revamped substitui o original; manter apenas uma DLL ativa.
- Funcionalidades divulgadas: vegetais do Deep North e arando; Farming nativa ao plantar e colher; registro nativo da colheita para conquistas elegíveis. O bloqueio global do Valheim em sessões com mods exige uma ferramenta como Unshamed, não é removido por esta DLL.
- ZIP preparado em artifacts/releases/SeedBed_Revamped-1.3.0.zip, com uma única DLL, manifest, ícone 256×256, README de publicação, changelog, créditos e licenças. SHA-256 do ZIP: **c34240a73663289fa29f0fc03606f5b4d97009c29776eaebae63530685cb8a57**.
- website_url permanece vazio até haver uma página própria. A publicação externa e a instalação da 1.3.0 no Gale não foram realizadas pela automação.

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
- Comparação estática com o snapshot confirmou os corpos dos oito fluxos de RPC, XP/estatísticas e inserção/uso de itens, descontando casts e formatação. A refatoração visual continua pendente de inspeção no jogo.
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
- Os demais itens do roteiro abaixo permanecem pendentes, salvo confirmação específica do usuário.

## Próximo passo: concluir a validação no jogo

Antes de publicar, fazer uma abertura de conferência da 1.3.0: nome SeedBed Revamped no log, configuração anterior carregada e canteiros existentes preservados.

1. Validar construção: bancada, categoria CollectorSeries, receita YmirRemains ×5 / BronzeNails ×8 / Wood ×18, configurações geradas, efeitos e shaders. O PieceManager foi substituído de 1.3.0 embutido para oficial 1.4.0.
2. Validar tradução inglesa e troca de idioma, incluindo arquivos externos de tradução compatíveis.
3. Iniciar com YAML antigo e uma conversão personalizada. Conferir backup, novos From, quantidades, ausência de duplicatas após reinício e respeito a remoções posteriores à migração.
4. Plantar KaleSeeds, Kale, OatSeeds, Oat e Lingonberries: consumo 5, visual, timer e produção 15 do item correto. Repetir CarrotSeeds/Carrot para o ciclo antigo.
5. **Validado em solo pelo usuário:** recebimento de XP e avanço da conquista Fazendeiro. Em testes adicionais, conferir outros contadores/conquistas elegíveis e o crédito por lote: SeedAmount plantas, não o número de itens produzidos.
6. Em multiplayer, interagir com uma SeedBed de outro cliente; repetir cliques e disputar plantio/colheita. Só a ação aceita deve receber crédito, sem perda de sementes no pedido recusado. Conferir inventário cheio na devolução e configurações sincronizadas.
7. Em caso de problema, consultar BepInEx/LogOutput.log: entradas Conversion e Resource. A grade 5×5 do arando pode exigir ajuste após inspeção visual.

[APIs vanilla verificadas](docs/VALHEIM_API.md) · [Créditos e licenças](docs/THIRD_PARTY_NOTICES.md) · [Regressões](tests/README.md)

O teste solo confirmou plantio, recebimento de experiência e avanço da conquista Fazendeiro, conforme relato do usuário. Os demais itens do roteiro, incluindo multiplayer, continuam pendentes de confirmação específica.
