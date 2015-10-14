using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Ekko.Modes
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu.Values;

    class Misc
    {
        internal void OnGapCloser(EloBuddy.AIHeroClient sender, EloBuddy.SDK.Events.Gapcloser.GapcloserEventArgs args ,EkkoCore core)
        {
            if (sender == null || sender.IsAlly ) return;
            var Gapcloser = core._menu.MiscMenu["MG"].Cast<CheckBox>().CurrentValue;
            if (Gapcloser != false)
            {
                if (sender.IsAttackingPlayer)
                {
                    core.spells.W.Cast(core.Player.Position);
                    this.CastEOut(sender, core);
                    var predQ = core.spells.Q.GetPrediction(sender);
                    if (predQ.HitChance >= HitChance.High)
                    {
                        core.spells.Q.Cast(predQ.CastPosition);
                    }
                }
            }
        }
        public void CastEOut(AIHeroClient target, EkkoCore core)
        {
            if (!core.spells.E.IsReady())
            {
                return;
            }
            var vec = core.Player.ServerPosition.Extend(target.ServerPosition, -core.spells.E.Range + 10);
            core.spells.E.Cast(vec.To3D());
        }
    }
}
