using BepInEx;
using MonoMod.Cil;
using RoR2;
using System;

namespace NoVoidAllies
{
    [BepInPlugin("com.Moffein.NoVoidAllies", "No Void Allies", "1.0.0")]
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
        }
    }
}
