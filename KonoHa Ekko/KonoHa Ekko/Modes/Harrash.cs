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

    class Harrash : Mode
    {
        public override void Update(EkkoCore core)
        {
            var useQ=core._menu.HarassMenu["QH"].Cast<CheckBox>().CurrentValue;
            var useE = core._menu.HarassMenu["EH"].Cast<CheckBox>().CurrentValue;
            var useW = core._menu.HarassMenu["WH"].Cast<CheckBox>().CurrentValue;
            var target = core.targetSelected.getTarget(core.spells.Q.Range);
            if (target == null) return;
            base.Update(core);
            switch (modeActive)
            {
                case mode.TeamFive:
                case mode.Normal:
                    NormalMode(core, target, useQ, useE, useW);
                    break;
                case mode.Chase:
                    ChaseMode(core, target, useQ, useE, useW);
                    break;
                case mode.Flee:
                    FleeMode(core, target, useQ, useE, useW);
                    break;


                default:
                    break;
            }
        }

        private void FleeMode(EkkoCore core, EloBuddy.AIHeroClient target, bool useQ, bool useE, bool useW)
        {
            CastEOut(target, core);
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
        public void CastE(AIHeroClient target, EkkoCore core, bool useQ)
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
        public void CastEOut(AIHeroClient target, EkkoCore core)
        {
            if (!core.spells.E.IsReady())
            {
                return;
            }
            var vec = core.Player.ServerPosition.Extend(target.ServerPosition, -core.spells.E.Range + 10);
            core.spells.E.Cast(vec.To3D());
        }
        private void ChaseMode(EkkoCore core, EloBuddy.AIHeroClient target, bool useQ, bool useE, bool useW)
        {
            if (core.spells.Q.IsInRange(target))
                this.CastE(target, core, useQ);
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

        private void NormalMode(EkkoCore core, EloBuddy.AIHeroClient target, bool useQ, bool useE, bool useW)
        {
            if (useQ && (core.Player.Distance(target) > core.spells.E.Range || core.Player.Distance(target) <= 100 && (core.spells.E.IsReady()) && useE))
            {
                var predQ = core.spells.Q.GetPrediction(target);
                if (predQ.HitChance >= HitChance.High)
                {
                    core.spells.Q.Cast(predQ.CastPosition);
                }
            }
            if (useE && core.Player.Distance(target) > core.spells.E.Range * 2)
            {
                this.CastE(target, core, useQ);
            }
            if (useW && core.spells.W.IsInRange(target) && (target.HasBuffOfType(BuffType.Slow) || target.HasBuffOfType(BuffType.Stun)))
            {
                core.spells.W.Cast(target.Position);
            }
        }

    }
}
