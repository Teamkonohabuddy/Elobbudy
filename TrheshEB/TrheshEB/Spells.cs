using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrheshEB
{
    class Spells
    {
        private  Spell.Skillshot Q;
        private  Spell.Skillshot W;
        private  Spell.Skillshot E;
        private  Spell.Active R;
        private static AIHeroClient player => ObjectManager.Player;
        public  AIHeroClient Ally
        {
            get
            {
                var ally =
                    EntityManager.Heroes.Allies.Where(h => h.IsValidTarget(E.Range) && !h.IsMe).FirstOrDefault();
                //                       .OrderByDescending(h => h. / h.HealthPercent)
                return ally ?? player;
            }
        }
        public Spells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1040, SkillShotType.Linear, 500, 1900, 60) //RealRange = 1075, RealWidth = 70.
            {
                AllowedCollisionCount = 0
            };
            W = new Spell.Skillshot(SpellSlot.W, 950, SkillShotType.Circular, 250, 1800, 300)
            {
                AllowedCollisionCount = int.MaxValue
            };
            E = new Spell.Skillshot(SpellSlot.E, 480, SkillShotType.Linear, 0, 2000, 110)
            {
                AllowedCollisionCount = int.MaxValue
            };
            R = new Spell.Active(SpellSlot.R, 450);
        }
        public void castQ()
        {

        }
        public  void Push(Obj_AI_Base target)
        {
            
            if (E.IsReady() && target.IsValidTarget(E.Range) && target.IsEnemy)
            {
                var pred = E.GetPrediction(target);
                if (pred.HitChance >= HitChance.High)
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }
        public  bool Pull(Obj_AI_Base target)
        {
            if (E.IsReady() && target.IsValidTarget(E.Range) && target.IsEnemy)
            {
                var pred = E.GetPrediction(target);
                if (pred.HitChance >= HitChance.High)
                {
                    Vector3? bestPosition = null;
                    if (!bestPosition.HasValue)
                    {
                        var turret = EntityManager.Turrets.Allies.FirstOrDefault(t => !t.IsDead && player.IsInRange(t, 1000));
                        if (turret != null)
                        {
                            bestPosition = turret.Position;
                        }
                    }
                    if (!bestPosition.HasValue)
                    {
                        if (Ally != null)
                        {
                            bestPosition = Ally.Position;
                        }
                    }
                    if (bestPosition.HasValue)
                    {
                        //     if (EntityManager.Heroes.Allies.HealthPercent(TargetSelector.Range) >= EntityManager.Heroes.Enemies.HealthPercent(TargetSelector.Range))
                        //   {
                        var info = player.Position.To2D().ProjectOn(pred.CastPosition.To2D(), bestPosition.Value.To2D());
                        var distance = info.SegmentPoint.Distance(player.Position.To2D());
                        if (distance <= E.Width)
                        {
                            player.Spellbook.CastSpell(SpellSlot.E, player.Position + (bestPosition.Value - pred.CastPosition).Normalized() * E.Range);
                            return true;
                        }
                        // }
                    }
                    var pos = player.Position + (player.Position - pred.CastPosition).Normalized() * E.Range;
                    player.Spellbook.CastSpell(SpellSlot.E, pos);
                    return true;
                }

            }
            return false;
        }
        
    }
}
