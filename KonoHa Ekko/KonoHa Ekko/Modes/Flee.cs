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

    class Flee : Mode
    {
        public override void Update(EkkoCore core)
        {
            var useQ = core._menu.FleeMenu["QF"].Cast<CheckBox>().CurrentValue;
            var useW = core._menu.FleeMenu["WF"].Cast<CheckBox>().CurrentValue;
            var useE = core._menu.FleeMenu["EF"].Cast<CheckBox>().CurrentValue;
            base.Update(core);
            var target = core.targetSelected.getTarget(core.spells.Q.Range);
            if(target!=null)
            this.FleeMode(core,target,useQ,useE,useW);
        }
        private void FleeMode(EkkoCore core, EloBuddy.AIHeroClient target, bool useQ, bool useE, bool useW)
        {
            this.CastEOut(target, core);
            if (useQ && core.spells.Q.IsInRange(target))
            {
                var predQ = core.spells.Q.GetPrediction(target);
                if (predQ.HitChance >= HitChance.High)
                {
                    core.spells.Q.Cast(predQ.CastPosition);
                }
            }
            if (useW && core.spells.W.IsInRange(Prediction.Position.PredictUnitPosition(target, core.spells.W.CastDelay * 1000 + Game.Ping).To3D()))
            {
                var pos = Prediction.Position.PredictUnitPosition(target, core.spells.W.CastDelay * 1000 + Game.Ping);
                core.spells.Q.Cast(pos.To3D());
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
