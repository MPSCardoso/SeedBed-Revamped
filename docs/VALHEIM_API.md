# APIs vanilla verificadas em 18/10/2026

Atualização de dados em 19/10/2026: busca textual somente de leitura no resources.assets da instalação local confirmou item_poteitr (Poteitr) e item_poteitrseeds (Seed Poteitr). Essas chaves são usadas pelo fallback de resolução da 1.3.1; o par segue a regra genérica de sufixo seeds. A presença das strings não comprova o funcionamento em partida, os visuais nem as estatísticas específicas de Poteitr. Esta constatação veio da inspeção local por rg, não de nova evidência REA.

Inspeção estática da DLL instalada, sem executar o jogo.

- Caminho: C:/Program Files (x86)/Steam/steamapps/common/Valheim/valheim_Data/Managed/assembly_valheim.dll
- SHA-256: 94998885b103d6e5f5480123eaee0a093e82af2ddeeee37536c15bf555f24f3a
- MVID: 65d2b4f8-e94e-4805-8a80-1451498a82d7 (as referências publicizadas agora são geradas automaticamente em obj durante a build).
- Evidência de identidade REA: ev_a44966e5f6c2cd37c4f4d54dc53566c02dca62b643c028a70769daac3173418f.
- Evidência de membros REA: ev_b0eba26f4ab85de07e71256097799327a71aa694771f08cff095eb5c6612546d.

Assinaturas verificadas por metadados REA e decompilação local com ilspycmd:

```csharp
Game.IncrementPlayerStat(PlayerStatType stat, float amount = 1f, bool cheated = false)
PlayerProfile.IncrementStat(PlayerStatType stat, float amount = 1f, bool cheated = false)
PlayerProfile.IncrementStatPickable(string name, float amount = 1f, bool cheated = false)
PlayerProfile.IncrementStatBuildPiecePlaced(string name, float amount = 1f, bool cheated = false)
Skills.SkillType.Farming // 106
```

Pickable.Interact usa m_harvestStat e o nome de m_itemPrefab para a coleta, além de RaiseSkill(m_pickRaiseSkill) quando configurada. Os métodos de PlayerProfile atualizam os contadores e disparam verificações de conquistas, respeitando as condições nativas de elegibilidade.

Player.TryPlacePiece incrementa Builds e IncrementStatBuildPiecePlaced(piece.m_name). Player.PlacePiece e Plant não incrementam PlaceStacks no código inspecionado; portanto a correção não adiciona esse contador. A peça correspondente é inferida por Plant.m_grownPrefabs, sem depender de nomes Sapling_*.

Escolhas da correção: quantidade de plantas = SeedAmount; XP Farming por operação = configuração Exp Amount (padrão 5); sem bônus aleatório de produção. Os novos prefabs são resolvidos por nome ou pelas chaves de localização confirmadas no handoff: item_kale, item_kaleseeds, item_oat, item_oatseeds e item_lingonberries.

Esses dados comprovam as chamadas disponíveis e o fluxo estático vanilla. Não comprovam aparência, RPCs, contadores ou desbloqueios em uma partida; validar pelo roteiro do STATUS_E_PROXIMOS_PASSOS.md.

## Investigação de Fazendeiro — 18/10/2026

- Retorno final do usuário: plantio, XP e funcionamento do avanço da conquista Fazendeiro confirmados em solo. Sem uso de devcommands, com Unshamed para remover o bloqueio. O teste inicial havia sido feito com framboesas; a confirmação veio após a orientação de testar uma cultura agrícola.
- Localização em resources.assets: ach_grindharvest = Fazendeiro; ach_grindharvest_desc = Faça 500 colheitas. A descrição não substitui a inspeção dos requisitos serializados da conquista.
- PlayerProfile.IncrementStatPickable atualiza o contador geral e, se Achievements.CanGetAchievements(cheated) permitir, atualiza os contadores elegíveis, notifica a plataforma e chama CheckAchievementProgressUnlockPickable. IncrementStat notifica AchievementStatIncrementEvent(PlayerStatType, amount), que chama CheckAchievementProgressUnlock(stat).
- Evidência de membros REA do jogo: ev_371887a76730eccc9b021facc700865013ff62746799a55a4a9379c83efc4e8d. Decompilação local de PlayerProfile, Achievement, Achievements e AchievementsGui mantida apenas em artifacts, ignorado pelo Git.
- Log do perfil ValheimNerdola: Unshamed 1.0.8 carregado; Eligible for achievements: True naquela sessão. A elegibilidade reportada não comprova o incremento em uma colheita específica.
- Unshamed.dll SHA-256 c884f61bd8c2effb12c2962c4cce61f30c7f94e5a499fd937e54d58942370106; evidência REA ev_c2274cdd3dac6587f4839ccf91149f57d3c554ecd053f2910808dc56f9481c04. O mod oferece unshamed list, unshamed guide <id> e um painel com requisitos/progresso para orientar a comparação.
- Validação solo concluída por relato do usuário para o avanço de Fazendeiro. Não foram fornecidos os valores antes/depois nem confirmação de desbloqueio final. Crédito esperado pelo código: SeedAmount plantas (5 por padrão), não producedItems (15 por padrão). Multiplayer e demais conquistas permanecem fora do escopo dessa confirmação.
