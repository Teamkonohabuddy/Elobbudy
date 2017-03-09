using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrheshEB
{
    class Program
    {
        private static Spell.Skillshot Q;
        private static Spell.Skillshot W;
        private static Spell.Skillshot E;
        private static Spell.Active R;
        private static Menu ThreshMenu, ComboMenu,MiscMenu;
        private static AIHeroClient player => ObjectManager.Player;
        static void Main(string[] args)
        {

            Loading.OnLoadingComplete += Loading_OnLoading;
        }

        private static void Loading_OnLoading(EventArgs args)
        {
            if (Player.Instance.ChampionName == "Thresh")
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


                Game.OnTick += Game_OnUpdate;
                Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
                Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
                ThreshMenu = MainMenu.AddMenu("Thresh","thresh");
                ComboMenu = ThreshMenu.AddSubMenu("Combo");

                ComboMenu.Add("Q",new CheckBox("Use Q"));
                ComboMenu.Add("W", new CheckBox("Use W"));
                ComboMenu.Add("E", new CheckBox("Use E"));
                ComboMenu.Add("R", new CheckBox("Use R"));
                MiscMenu = ThreshMenu.AddSubMenu("Misc");
                MiscMenu.Add("QG", new CheckBox("Use Q gapcloser"));
                MiscMenu.Add("EG", new CheckBox("Use E gapcloser"));

                return;

            }
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
                Pull(sender);
                Q.Cast(sender);

        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            if (!sender.IsValidTarget()) return;
            if (MiscMenu["EG"].Cast<CheckBox>().CurrentValue)
            {
                if (Ally.Distance(e.End, true) < Ally.Distance(e.Start, true))
                {
                   Push(sender);
                }
                else
                {
                  Pull(sender);
                }
            }
            if (MiscMenu["EQ"].Cast<CheckBox>().CurrentValue)
            {
                if (Ally.Distance(e.End, true) > Ally.Distance(e.Start, true))
                {
                 Q.Cast(sender);
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var QPred = Q.GetPrediction(target);
            if(Pull(target))
            {
                Chat.Print("Pull");
            }
            else if(QPred.HitChance>=HitChance.High&&Q.IsReady()&&target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            else if(R.IsReady()&& target.IsValidTarget(R.Range))
            {
                R.Cast(target);
            }


        }
        public static AIHeroClient Ally
        {
            get
            {
                var ally =
                    EntityManager.Heroes.Allies.Where(h => h.IsValidTarget(E.Range) && !h.IsMe).FirstOrDefault();
                //                       .OrderByDescending(h => h. / h.HealthPercent)
                return ally ?? player;
            }
        }

        public static void Push(Obj_AI_Base target)
        {
            if (E.IsReady() && target.IsValidTarget(E.Range) && target.IsEnemy)
            {
                var pred = E.GetPrediction(target);
                if (pred.HitChance>= HitChance.High)
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }

        public static bool Pull(Obj_AI_Base target)
        {
            if (E.IsReady()&& target.IsValidTarget(E.Range) && target.IsEnemy)
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
        
        private static void Game_OnUpdate(EventArgs args)
        {
        if(Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
        }
        
    }
}
