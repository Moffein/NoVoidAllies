using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;

namespace NoVoidAllies
{
    [BepInPlugin("com.Moffein.NoVoidAllies", "No Void Allies", "1.0.1")]
    public class NoVoidAllies : BaseUnityPlugin
    {
        public void Awake()
        {
            IL.EntityStates.VoidInfestor.Infest.OnEnter += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(
                     x => x.MatchCallvirt<BullseyeSearch>("RefreshCandidates")
                    );
                c.EmitDelegate<Func<BullseyeSearch, BullseyeSearch>>(search =>
                {
                    search.teamMaskFilter.RemoveTeam(TeamIndex.Player);
                    return search;
                });
            };

            IL.EntityStates.VoidInfestor.Infest.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                     x => x.MatchCallvirt<CharacterBody>("get_isPlayerControlled")
                    );
                c.Emit(OpCodes.Ldloc_3);
                c.EmitDelegate<Func<bool, CharacterBody, bool>>((playerControlled, body) =>
                {
                    return playerControlled || (body.teamComponent && body.teamComponent.teamIndex == TeamIndex.Player);
                });
            };
        }
    }
}
