using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Ekko.Modes
{
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Serialization;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu.Values;

    class Combo : Mode
    {
        public override void Update(EkkoCore core)
        {
            var useQ = core._menu.ComboMenu["QC"].Cast<CheckBox>().CurrentValue;
            var useW = core._menu.ComboMenu["WC"].Cast<CheckBox>().CurrentValue;
            var useE = core._menu.ComboMenu["EC"].Cast<CheckBox>().CurrentValue;
            var useR = core._menu.ComboMenu["RC"].Cast<CheckBox>().CurrentValue;
            var useRO = core._menu.ComboMenu["ROC"].Cast<CheckBox>().CurrentValue;
                   var useRlife = core._menu.ComboMenu["ROUC"].Cast<Slider>().CurrentValue;
                   var target = core.targetSelected.getTarget(core.spells.Q.Range);
            base.Update(core);
            if (target == null) return;
            switch (this.modeActive)
            {
                case mode.Normal:
                    this.NormalMode(core,target,useQ,useE,useW);
                    break;
                    case mode.Chase:
                    this.ChaseMode(core, target, useQ, useE, useW);
                    break;
                case mode.Flee:
                    this.FleeMode(core, target, useQ, useE, useW);
                    break;
                case mode.TeamFive:
                    this.Teamfive(core, target, useQ, useE, useR, useW);
                    break;

                default:
                 break;
            }
            if (useR && core.spells.R.IsReady() && TargetHitWithR(core,target))
            {
                if (core.spells.RDam(target) + core.spells.EDam(target) + (useQ ? core.spells.TotalQDam(target) : 0) + core.spells.PassiveDam(target) >= target.Health)
                {
                    core.spells.R.Cast();
                }
            }
            if (core.Player.Distance(target) <= core.spells.E.Range * 2)
            {
                if (core.spells.EDam(target) + (useQ ? core.spells.TotalQDam(target) : 0) + core.spells.PassiveDam(target) >= target.Health)
                {
                    this.CastE(target, core,useQ);
                }
                if (core.Player.Distance(target) <= 250 && core.spells.TotalQDam(target) + core.spells.PassiveDam(target) >= target.Health)
                {
                    var predQ = core.spells.Q.GetPrediction(target);
                    if (predQ.HitChance >= HitChance.High)
                    {
                        core.spells.Q.Cast(predQ.CastPosition);
                    }
                }
            }
            //killsteal checks

            if (useE && core.spells.E.IsReady() && core.Player.Distance(target) <= core.spells.E.Range * 2 && core.spells.EDam(target) >= target.Health)
            {
                CastE(target, core,useQ);
            }
            else if (useQ && core.spells.Q.IsReady() && core.spells.Q.IsInRange(target) && core.spells.TotalQDam(target) >= target.Health)
            {
                var predQ = core.spells.Q.GetPrediction(target);
                if (predQ.HitChance >= HitChance.High)
                {
                    core.spells.Q.Cast(predQ.CastPosition);
                }
            }

            else if (useRO&&core.spells.R.IsReady() && !(base.modeActive == mode.TeamFive && core._menu.ComboMenu["RPC"].Cast<CheckBox>().CurrentValue) &&  TargetHitWithR(core)>=1)
            {
                core.spells.R.Cast();
            }
            if (core.Player.HealthPercent <= useRlife)
            {
                core.spells.R.Cast();
            }
        }
        static bool TargetHitWithR(EkkoCore core,AIHeroClient hero)
        {
            if( core.spells.Ghost.Distance(Prediction.Position.PredictCircularMissile(hero, core.spells.R.Range, core.spells.R1.Radius, core.spells.R.CastDelay, core.spells.R1.Speed).UnitPosition) < 400)
            {
                    return true;
            }
            return false;
        }
        static int TargetHitWithR(EkkoCore core)
        {

            return HeroManager.Enemies.Where(x => x.IsValidTarget() && core.spells.RDam(x) >= x.Health).Count(x => core.spells.Ghost.Distance(Prediction.Position.PredictCircularMissile(x, core.spells.R.Range, core.spells.R1.Radius, core.spells.R.CastDelay, core.spells.R1.Speed).UnitPosition) < 400);
        }
        private void Teamfive(EkkoCore core, AIHeroClient target, bool useQ, bool useE, bool useR,bool useW)
        {
            var damageOverKill =core._menu.ComboMenu["RPC"].Cast<CheckBox>().CurrentValue;
            var rnu =core._menu.ComboMenu["RnC"].Cast<Slider>().CurrentValue;
            var rtc = core._menu.ComboMenu["RTC"].Cast<Slider>().CurrentValue;
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
            if (useR && damageOverKill)
            {
                if (this.getNumTargetsRange(375, rnu) && (rtc >= core.Player.Health || this.TheyaReDead(375,1,core)))
                {
                    core.spells.R.Cast();
                }
            }
        }

        private void FleeMode(EkkoCore core, AIHeroClient target, bool useQ, bool useE, bool useW)
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

        private void ChaseMode(EkkoCore core, AIHeroClient target, bool useQ, bool useE, bool useW)
        {
            if (core.spells.Q.IsInRange(target))
                if (useE && core.Player.Distance(target) > core.spells.E.Range * 2)
                {
                    this.CastE(target, core, useQ);
                }
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

        public void NormalMode(EkkoCore core, AIHeroClient target, bool useQ, bool useE, bool useW)
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
                this.CastE(target, core,useQ);
            }
               if (useW  && core.spells.W.IsInRange(target) && (target.HasBuffOfType(BuffType.Slow) ||target.HasBuffOfType(BuffType.Stun) ))
            {
                core.spells.W.Cast(target.Position);
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

        public void CastE(AIHeroClient target, EkkoCore core, bool useQ)
        {
            if (!core.spells.E.IsReady())
            {
                return;
            }
            var vec = core.Player.ServerPosition.Extend(target.ServerPosition, core.spells.E.Range - 10);
            core.spells.E.Cast(vec.To3D());
            Core.DelayAction(() => EloBuddy.Player.IssueOrder(GameObjectOrder.AttackUnit, target), core.spells.E.CastDelay * 1000 + Game.Ping);
          if(useQ)
            Core.DelayAction(() => core.spells.Q.Cast(target.Position), (core.spells.E.CastDelay+(int)core.Player.AttackDelay )* 1000 + Game.Ping);
        }

        public bool TheyaReDead(int range, int num, EkkoCore core)
        {
            var i = 0;
            foreach (var target in EntityManager.Heroes.Enemies)
            {
                if (target != null && !target.IsDead)
                    if (TargetHitWithR(core, target))
                    {
                        if (core.spells.RDam(target) <= target.Health)
                        {
                            i++;
                        }
                    }
            }
            if (i >= num) return true;
            return false;
        }

        public bool getNumTargetsRange(int range, int num,EkkoCore core)
        {
            var i = 0;
            foreach (var target in EntityManager.Heroes.Enemies)
            {
                if (target != null && !target.IsDead)
                    if (TargetHitWithR(core, target))
                    {
                        i++;
                    }
            }
            if (i >= num) return true;
            return false;
        }
    }
}
