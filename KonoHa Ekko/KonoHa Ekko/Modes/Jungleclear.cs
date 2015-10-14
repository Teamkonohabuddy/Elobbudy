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

    class Jungleclear : Mode
    {
        public override void Update(EkkoCore core)
        {
            base.Update(core);
            var useQ = core._menu.JungleclearMenu["QJ"].Cast<CheckBox>().CurrentValue;
            var useW = core._menu.JungleclearMenu["WJ"].Cast<CheckBox>().CurrentValue;
            var useE = core._menu.JungleclearMenu["EJ"].Cast<CheckBox>().CurrentValue;
             Obj_AI_Base minion =
        EntityManager.MinionsAndMonsters.GetJungleMonsters(

            ObjectManager.Player.Position,
            core.spells.Q.Range,
            true).FirstOrDefault();
            if (minion == null) return;
            if (useQ && core.spells.Q.IsInRange(minion))
            {
                var predQ = core.spells.Q.GetPrediction(minion);
                if (predQ.HitChance >= HitChance.High)
                {
                    core.spells.Q.Cast(predQ.CastPosition);
                }
            }
            if (useW && core.spells.W.IsInRange(minion))
            {
                var predQ = core.spells.W.GetPrediction(minion);
                if (predQ.HitChance >= HitChance.High)
                {
                    core.spells.Q.Cast(predQ.CastPosition);
                }
            }
            if (useE && core.spells.E.IsInRange(minion))
            {
              CastE(minion,core,useQ);
            }

        }
        public void CastE(Obj_AI_Base target, EkkoCore core, bool useQ)
        {
            if (!core.spells.E.IsReady())
            {
                return;
            }
            var vec = core.Player.ServerPosition.Extend(target.ServerPosition, core.spells.E.Range - 10);
            core.spells.E.Cast(vec.To3D());
            Core.DelayAction(() => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target), core.spells.E.CastDelay * 1000 + Game.Ping);
            if (useQ)
                Core.DelayAction(() => core.spells.Q.Cast(target.Position), (core.spells.E.CastDelay + (int)core.Player.AttackDelay) * 1000 + Game.Ping);
        }
    }
}
