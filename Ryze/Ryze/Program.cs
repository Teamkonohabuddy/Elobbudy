using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
namespace Ryze
{
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;
    //Ryze
    internal class Program
    {
        private static Menu menu, ComboMenu, DrawMenu, HarrashMenu, LaneClearMenu,JungleclearMenu,miscMenu;

        private static Spell.Skillshot Q;

        private static Spell.Targeted W, E;

        private static Spell.Active R;

        public static AIHeroClient selected;
        public static AIHeroClient myHero
        {
            get
            {
                return ObjectManager.Player;
            }
        }

        private static double QDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                (float)
                (new double[] { 60, 85, 110, 135, 160 }[Q.Level - 1] + 0.55 * myHero.FlatMagicDamageMod
                 + new double[] { 2, 2.5, 3.0, 3.5, 4.0 }[Q.Level - 1] / 100 * myHero.MaxMana));
        }

        public static float WDamage(Obj_AI_Base target)
        {
            return myHero.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                new[] { 80, 100, 120, 140, 160 }[W.Level - 1] + 0.4f * myHero.FlatMagicDamageMod
                + 0.02f * myHero.MaxMana);
        }

        public static float EDamage(Obj_AI_Base target)
        {
            return myHero.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                new[] { 36, 52, 68, 84, 100 }[E.Level - 1] + 0.2f * myHero.FlatMagicDamageMod + 0.025f * myHero.MaxMana);
        }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        public static void LoadMenu()
        {
            menu = MainMenu.AddMenu("Ryze", "Ryze");
            ComboMenu = menu.AddSubMenu("Combo", "combo");
            HarrashMenu = menu.AddSubMenu("Harrash", "Harrash");
            LaneClearMenu = menu.AddSubMenu("Laneclear", "Laneclear");
            JungleclearMenu = menu.AddSubMenu("Jungleclear", "Jungleclear");
            JungleclearMenu.Add("JQ", new CheckBox("Use Q"));
            JungleclearMenu.Add("JW", new CheckBox("Use W"));
            JungleclearMenu.Add("JE", new CheckBox("Use E"));
            JungleclearMenu.Add("JR", new CheckBox("Use R"));
            LaneClearMenu.Add("LQ", new CheckBox("Use Q"));
            LaneClearMenu.Add("LW", new CheckBox("Use W"));
            LaneClearMenu.Add("LE", new CheckBox("Use E"));
            LaneClearMenu.Add("LR", new CheckBox("Use R"));
            DrawMenu = menu.AddSubMenu("Draw", "draw");
            HarrashMenu.Add("HQ", new CheckBox("Use Q"));
            
            ComboMenu.Add("CQ", new CheckBox("Use Q"));
            ComboMenu.Add("CE", new CheckBox("Use E"));
            ComboMenu.Add("CW", new CheckBox("Use W"));
            ComboMenu.Add("CR", new CheckBox("Use R"));
            ComboMenu.Add("CRo", new CheckBox("Use R only on Root"));
            DrawMenu.Add("DQ", new CheckBox("Draw Q"));
            DrawMenu.Add("DW", new CheckBox("Draw W"));
            DrawMenu.Add("DE", new CheckBox("Draw E"));
            miscMenu = menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("LockTar", new CheckBox("Active focus Selected Target"));

        }

        private static void OnLoad(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Ryze) return;
            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1700, 100);
            //   SpellData.GetSpellData("Q").MissileSpeed;
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Targeted(SpellSlot.E, 600);
            R = new Spell.Active(SpellSlot.R);
            LoadMenu();
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += OnProc;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void OnProc(WndEventArgs args)
        {

            if (args.Msg != (uint)WindowMessages.LeftButtonDown)
            {
                return;
            }
            var trys = HeroManager.Enemies
              .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
              .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
            if (trys != null)
            {
                selected= HeroManager.Enemies
                    .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
                    .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
            }

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
            {
               Combo();
            }
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Harass)
            {
                Harrash();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Laneclear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
        }

        private static void JungleClear()
        {
            var jungleclearQ = JungleclearMenu["JQ"].Cast<CheckBox>().CurrentValue;
            var jungleclearW = JungleclearMenu["JW"].Cast<CheckBox>().CurrentValue;
            var jungleclearE = JungleclearMenu["JE"].Cast<CheckBox>().CurrentValue;
            var jungleclearR = JungleclearMenu["JR"].Cast<CheckBox>().CurrentValue;
            Obj_AI_Base minion =
                EntityManager.GetJungleMonsters(
     
                    ObjectManager.Player.Position.To2D(),
                    600,
                    true).FirstOrDefault();
            if (jungleclearQ && Q.IsReady())
            {
                var Qpred = Q.GetPrediction(minion);
                Q.Cast(Qpred.UnitPosition);
            }
            if (jungleclearE && E.IsReady())
            {
                E.Cast(minion);
            }
            if (jungleclearW && W.IsReady())
            {
                W.Cast(minion);
            }
            if (jungleclearR && R.IsReady() && GetPassiveBuff >= 4)
            {
                R.Cast();
            }
        }

        private static void Laneclear()
        {
            var laneclearQ = LaneClearMenu["LQ"].Cast<CheckBox>().CurrentValue;
            var laneclearW = LaneClearMenu["LW"].Cast<CheckBox>().CurrentValue;
            var laneclearE = LaneClearMenu["LE"].Cast<CheckBox>().CurrentValue;
            var laneclearR = LaneClearMenu["LR"].Cast<CheckBox>().CurrentValue;
            Obj_AI_Base minion =
                EntityManager.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    ObjectManager.Player.Position.To2D(),
                    600,
                    true).FirstOrDefault();
            if (minion != null)
            {
                if (laneclearQ && Q.IsReady())
                {
                    var Qpred = Q.GetPrediction(minion);
                    Q.Cast(Qpred.UnitPosition);
                }
                if (laneclearE && E.IsReady())
                {
                    E.Cast(minion);
                }
                if (laneclearW && W.IsReady())
                {
                    W.Cast(minion);
                }
                if (laneclearR && R.IsReady() && GetPassiveBuff >= 4)
                {
                    R.Cast();
                }
            }
        }

        private static void Harrash()
        {

            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (miscMenu["LockTar"].Cast<CheckBox>().CurrentValue && selected != null && selected.IsVisible && selected.Position.Distance(ObjectManager.Player) <= 570) target = selected;
            var qSpell = HarrashMenu["HQ"].Cast<CheckBox>().CurrentValue;
            if (target != null)
            {
                var qpred = Q.GetPrediction(target);
                if (qSpell)
                {
                    if (Q.GetPrediction(target).HitChance == HitChance.High)
                    {
                        Q.Cast(target);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var qSpell = DrawMenu["DQ"].Cast<CheckBox>().CurrentValue;
            var wSpell = DrawMenu["DW"].Cast<CheckBox>().CurrentValue;
            var eSpell = DrawMenu["DE"].Cast<CheckBox>().CurrentValue;
            if (qSpell) Circle.Draw(Color.AliceBlue, Q.Range, Player.Instance.Position);
            if (wSpell) Circle.Draw(Color.AliceBlue, W.Range, Player.Instance.Position);
            if (eSpell) Circle.Draw(Color.DarkGray, E.Range, Player.Instance.Position);
            if (miscMenu["LockTar"].Cast<CheckBox>().CurrentValue &&selected != null && selected.IsVisible)
            {
                Circle.Draw(Color.Red, 100, selected.Position);
            }
        }

        public static int GetPassiveBuff
        {
            get
            {

                var data = ObjectManager.Player.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack");
                return data != null ? data.Count : 0;
            }
        }

        private static void Combo()
        {

            var target = TargetSelector.GetTarget(570, DamageType.Magical);
            if (miscMenu["LockTar"].Cast<CheckBox>().CurrentValue && selected != null && selected.IsVisible && selected.Position.Distance(ObjectManager.Player) <= 570) target = selected;
            if (target != null)
            {
                var qpred = Q.GetPrediction(target);
                var qSpell = ComboMenu["CQ"].Cast<CheckBox>().CurrentValue;
                var eSpell = ComboMenu["CE"].Cast<CheckBox>().CurrentValue;
                var wSpell = ComboMenu["CW"].Cast<CheckBox>().CurrentValue;
                var rSpell = ComboMenu["CR"].Cast<CheckBox>().CurrentValue;
                var rwwSpell = ComboMenu["CRo"].Cast<CheckBox>().CurrentValue;
                if (target.IsValidTarget(Q.Range))
                {
                    if (GetPassiveBuff <= 2 || !ObjectManager.Player.HasBuff("RyzePassiveStack"))
                    {
                        if (target.IsValidTarget(Q.Range) && qSpell && Q.IsReady()) Q.Cast(qpred.UnitPosition);

                        if (target.IsValidTarget(W.Range) && wSpell && W.IsReady()) W.Cast(target);

                        if (target.IsValidTarget(E.Range) && eSpell && E.IsReady()) E.Cast(target);

                        if (R.IsReady() && rSpell)
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (QDamage(target) + EDamage(target)))
                            {
                                if (rwwSpell && target.HasBuff("RyzeW")) R.Cast();
                                if (!rwwSpell) R.Cast();
                            }
                        }
                    }


                    if (GetPassiveBuff == 3)
                    {
                        if (Q.IsReady() && target.IsValidTarget(Q.Range)) Q.Cast(qpred.UnitPosition);

                        if (E.IsReady() && target.IsValidTarget(E.Range)) E.Cast(target);

                        if (W.IsReady() && target.IsValidTarget(W.Range)) W.Cast(target);

                        if (R.IsReady() && rSpell)
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (QDamage(target) + EDamage(target)))
                            {
                                if (rwwSpell && target.HasBuff("RyzeW")) R.Cast();
                                if (!rwwSpell) R.Cast();
                            }
                        }
                    }

                    if (GetPassiveBuff == 4)
                    {
                        if (target.IsValidTarget(W.Range) && wSpell && W.IsReady()) W.Cast(target);

                        if (target.IsValidTarget(Q.Range) && Q.IsReady() && qSpell) Q.Cast(qpred.UnitPosition);

                        if (target.IsValidTarget(E.Range) && E.IsReady() && eSpell) E.Cast(target);

                        if (R.IsReady() && rSpell)
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (QDamage(target) + EDamage(target)))
                            {
                                if (rwwSpell && target.HasBuff("RyzeW")) R.Cast();
                                if (!rwwSpell) R.Cast();
                            }
                        }
                    }

                    if (myHero.HasBuff("ryzepassivecharged"))
                    {
                        if (wSpell && W.IsReady() && target.IsValidTarget(W.Range)) W.Cast(target);

                        if (qSpell && Q.IsReady() && target.IsValidTarget(Q.Range)) Q.Cast(qpred.UnitPosition);

                        if (eSpell && E.IsReady() && target.IsValidTarget(E.Range)) E.Cast(target);

                        if (R.IsReady() && rSpell)
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (QDamage(target)) + EDamage(target))
                            {
                                if (rwwSpell && target.HasBuff("RyzeW")) R.Cast();
                                if (!rwwSpell) R.Cast();
                                if (!E.IsReady() && !Q.IsReady() && !W.IsReady()) R.Cast();
                            }
                        }
                    }
                }
                else
                {
                    if (wSpell && W.IsReady() && target.IsValidTarget(W.Range)) W.Cast(target);

                    if (qSpell && Q.IsReady() && target.IsValidTarget(Q.Range)) Q.Cast(qpred.UnitPosition);

                    if (eSpell && E.IsReady() && target.IsValidTarget(E.Range)) E.Cast(target);
                }
                if (!R.IsReady() || GetPassiveBuff != 4 || !rSpell) return;

                if (Q.IsReady() || W.IsReady() || E.IsReady()) return;

                R.Cast();

            }
        }
    }
}
