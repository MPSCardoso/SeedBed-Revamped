# v1.3.0 — SeedBed Revamped

- Renomeia o mod para SeedBed Revamped, mantido por Tetus, com créditos a blacks7ar pelo SeedBed original.
- Inclui suporte aos vegetais do Deep North (Kale/KaleSeeds e Oat/OatSeeds), além de Lingonberries.
- Concede XP de Farming nativa ao plantar e colher na seed bed.
- Registra colheitas pelas APIs nativas para progresso de conquistas elegíveis; o desbloqueio global em partidas com mods requer uma ferramenta como Unshamed.
- Preserva GUID, nomes de configuração, assembly, prefab e componente Germination para compatibilidade com os canteiros existentes. Substitui o SeedBed original; não instalar os dois pacotes juntos.
- Prepara pacote Thunderstore com README, créditos, licenças e uma única DLL.
- Plantio, XP e avanço de Fazendeiro validados pelo usuário em solo na base 1.2.10; multiplayer permanece pendente.

# Limpeza de desenvolvimento — 1.2.10
- Remove fontes decompiladas de terceiros; compila managers oficiais fixados e usa YamlDotNet 16.0.0 do NuGet.
- Gera referências publicizadas na build e distribui uma DLL com ILRepack.
- Refatora casts, nomes e loops de visuais sem alterar os contratos do mod.
- Amplia regressões para localização e empacotamento; guarda backups fora da árvore versionada.
- PieceManager oficial 1.4.0: validação dentro do jogo ainda pendente.

# v1.2.10 — correção local
- Adiciona Kale/KaleSeeds, Oat/OatSeeds e Lingonberries com migração única do YAML e backup.
- Detecta pares semente/vegetal de forma genérica e resolve nomes novos por chaves de localização quando necessário.
- Concede Farming nativa ao plantar/colher e registra contadores/conquistas pelas APIs do Valheim.
- Confirma plantio/colheita no dono da SeedBed antes do crédito; devolve sementes em pedidos recusados.
- Exige versão 1.2.10 em clientes e servidor. Aparência e integração final ainda precisam de validação no jogo.

#
# v1.2.9
- updated for valheim 1.0 deepnorth
#
# v1.2.8
- updated for the latest bepinex build (5.4.2333).
- updated for the latest valheim build (221.4) call to arms update.
- updated unity project with the latest game rip.
#
# v1.2.7
- updated serversync.
#
# v1.2.6
- fixed visual bug for herbalist mod's plants and added the new plants on the example yml config.
#
# v1.2.5
- readded vulkan support
#
# v1.2.4
- updated piece manager
#
# v1.2.3
- updated for the latest valheim build (219.13) bog witch
#
# v1.2.2
- as requested disabled exp gain when planting for farming and herbalist to avoid exploit, you can only get exp when harvesting.
- minor code fixes
#
# v1.2.1
- added additional filter when adding data on the yml
- added more null checks to avoid null reference errors.
#
# v1.2.0
- reworked and rewritten to support other mods that adds their own pickable/plantable plants (currently tested supported mods: cookingadditions, herbalist and therzie's wizardry).
- you can now configure which plant is allowed in the seedbed through a yml file. (includes a yml file which contains 34 tested items vanilla and modded)
#
# v1.1.3
- fixed build menu tab scaling (updated piece manager).
#
# v1.1.2
- updated for the latest valheim build (218.15 ashlands)
#
# v1.1.1
- updated for the latest valheim build (217.46)
#
# v1.1.0
- added dandelion and thistle to be plantable at the seedbed.
- put seedbed into a custom build category called CollectorSeries (all of my collector mods will be put into this category) (configurable)
#
# v1.0.8
- added support to my cookingadditions mod, you can now plant chilipepper and tomato into the seedbed.
- added support to my herbalist mod, you can now plant herbal plants into the seedbed, you'll gain exp for herbalist skill if you plant and harvest herbal plants from the seedbed
#
# v1.0.3
- added support to Farming skill mod. You can now get an exp for farming skill when harvesting and planting at the seedbed (if farming skill is already learned).
#
# v1.0.2
- added snapping points to the seedbed.
- added mushrooms, yellowmushrooms, cloudberry, blueberry and rapsberry to be plantable.
- added config options for seedbeds required crafting materials.
#
# v1.0.0
- first release.
#