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
        private static Menu ThreshMenu, ComboMenu, DrawingMenu,MiscMenu,DrawMenu,HarassMenu;
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
                ThreshMenu = MainMenu.AddMenu("Thresh","thresh");
                ComboMenu = ThreshMenu.AddSubMenu("Combo");

                ComboMenu.Add("Q1",new CheckBox("Use Q1"));
                ComboMenu.Add("Q2", new CheckBox("Use Q2"));
                ComboMenu.Add("W", new CheckBox("Use W to ally target"));
                ComboMenu.Add("E", new CheckBox("Use E"));
                ComboMenu.Add("R", new CheckBox("Use R"));


                HarassMenu.Add("HQ1", new CheckBox("Use Q1"));
                HarassMenu.Add("HQ2", new CheckBox("Use Q2"));
                HarassMenu.Add("HW", new CheckBox("Use W to ally target"));
                HarassMenu.Add("HE", new CheckBox("Use E"));
                HarassMenu.Add("HEM",new Slider("0-Pull 1-Push",1,0,1));
                MiscMenu = ThreshMenu.AddSubMenu("Misc");
                MiscMenu.Add("QG", new CheckBox("Use Q gapcloser"));
                MiscMenu.Add("EG", new CheckBox("Use E gapcloser"));
                DrawMenu = ThreshMenu.AddSubMenu("Drawings");
                DrawMenu.Add("DQ",new CheckBox("Draw Q"));
                DrawMenu.Add("DE", new CheckBox("Draw E"));
                DrawMenu.Add("DR", new CheckBox("Draw R"));
                Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
                Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
                Drawing.OnDraw += OnDraw; 
                return;

            }
        }

        private static void OnDraw(EventArgs args)
        {
            if(DrawMenu["DQ"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(player.Position, Q.Range, System.Drawing.Color.Red);
            }
            if (DrawMenu["DE"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(player.Position, E.Range, System.Drawing.Color.Gray);
            }
            if (DrawMenu["DR"].Cast<CheckBox>().CurrentValue)
            {
                Drawing.DrawCircle(player.Position, E.Range, System.Drawing.Color.GreenYellow);
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
            Chat.Print(sender.ChampionName);
            if (MiscMenu["EG"].Cast<CheckBox>().CurrentValue)
            {
              if(sender.Distance(player,false)<=E.Range)
                {
                    if (E.IsReady() && sender.IsEnemy)
                    {
                        E.Cast(sender.Position);
                    }
                }

                
            }
       /*     if (MiscMenu["QG"].Cast<CheckBox>().CurrentValue)
            {
                if (Ally.Distance(e.End, true) > Ally.Distance(e.Start, true))
                {
                 Q.Cast(sender);
                }
            }*/
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var QPred = Q.GetPrediction(target);
            if (ComboMenu["E"].Cast<CheckBox>().CurrentValue && Pull(target)) { }
            else if(ComboMenu["Q1"].Cast<CheckBox>().CurrentValue && QPred.HitChance>=HitChance.High&&Q.IsReady()&&target.IsValidTarget(Q.Range) && Q.Name=="ThreshQ")
            {
                Q.Cast(target);
            }
            else if( Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Name == "ThreshQLeap")
            {
                if(ComboMenu["W"].Cast<CheckBox>().CurrentValue)
                        {
                    var Atargets = EntityManager.Heroes.Allies.Where(x=>x!=player&&x.Distance(player)<=W.Range ).OrderBy(x=>x.Health).FirstOrDefault();
                    if (Atargets != null)
                    {
                        W.Cast(Atargets.Position);
                    }
                }
                if (ComboMenu["Q2"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(target);
                }
            }
            else if(ComboMenu["R"].Cast<CheckBox>().CurrentValue && R.IsReady()&& target.IsValidTarget(R.Range))
            {
                R.Cast(target);
            }


        }
        public bool select(AIHeroClient target)
        {
            if (ComboMenu["HEM"].Cast<Slider>().CurrentValue==0)
            {
                return Pull(target);
            }
            else
            {
                return Push(target);
            }
        }
        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            var QPred = Q.GetPrediction(target);
            if (ComboMenu["HE"].Cast<CheckBox>().CurrentValue && Pull(target)) { }
            else if (ComboMenu["HQ1"].Cast<CheckBox>().CurrentValue && QPred.HitChance >= HitChance.High && Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Name == "ThreshQ")
            {
                Q.Cast(target);
            }
            else if (Q.IsReady() && target.IsValidTarget(Q.Range) && Q.Name == "ThreshQLeap")
            {
                if (ComboMenu["HW"].Cast<CheckBox>().CurrentValue)
                {
                    var Atargets = EntityManager.Heroes.Allies.Where(x => x != player && x.Distance(player) <= W.Range).OrderBy(x => x.Health).FirstOrDefault();
                    if (Atargets != null)
                    {
                        W.Cast(Atargets.Position);
                    }
                }
                if (ComboMenu["HQ2"].Cast<CheckBox>().CurrentValue)
                {
                    Q.Cast(target);
                }
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

        public static bool Push(Obj_AI_Base target)
        {
            if (E.IsReady() && target.IsValidTarget(E.Range) && target.IsEnemy)
            {
                var pred = E.GetPrediction(target);
                if (pred.HitChance>= HitChance.High)
                {
                    E.Cast(pred.CastPosition);
                    return true;
                }
            }
            return false;
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
            if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }
                if (Orbwalker.ActiveModesFlags.Equals(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }
        }
        
    }
}
