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

    internal class Program
    {
        private static Menu menu, ComboMenu,DrawMenu,HarrashMenu,LaneClearMenu;

        private static Spell.Skillshot Q;

        private static Spell.Targeted W, E;

        private static Spell.Active R;
        public static AIHeroClient myHero { get { return ObjectManager.Player; } }
        private static double QDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(target, DamageType.Magical,
                (float)(new double[] { 60, 85, 110, 135, 160 }[Q.Level - 1] + 0.55 * myHero.FlatMagicDamageMod +
                        new double[] { 2, 2.5, 3.0, 3.5, 4.0 }[Q.Level - 1] / 100 * myHero.MaxMana));
        }
        public static float WDamage(Obj_AI_Base target)
        {
            return myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 80, 100, 120, 140, 160 }[W.Level - 1] +
                0.4f * myHero.FlatMagicDamageMod +
                0.02f * myHero.MaxMana);
        }
        public static float EDamage(Obj_AI_Base target)
        {
            return myHero.CalculateDamageOnUnit(target, DamageType.Magical,
                new[] { 36, 52, 68, 84, 100 }[E.Level - 1] +
                0.2f * myHero.FlatMagicDamageMod +
                0.025f * myHero.MaxMana);
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
     //       LaneClearMenu = menu.AddSubMenu("Laneclear", "Laneclear");
     //       LaneClearMenu.Add("LQ", new CheckBox("Use Q"));
     //       LaneClearMenu.Add("LW", new CheckBox("Use W"));
      //      LaneClearMenu.Add("LE", new CheckBox("Use E"));
      //      LaneClearMenu.Add("LR", new CheckBox("Use R"));
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
            Drawing.OnDraw += Drawing_OnDraw;
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
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.LaneClear)
            {
            }
        }

        private static void Laneclear()
        {
         
        }
        private static void Harrash()
        {

            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            var qSpell = HarrashMenu["HQ"].Cast<CheckBox>().CurrentValue;
            var qpred = Q.GetPrediction(target);
            if (qSpell)
            {
                if (Q.GetPrediction(target).HitChance == HitChance.High)
                {
                    Q.Cast(target);
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
            var target = TargetSelector.GetTarget(600, DamageType.Magical);
            var qpred = Q.GetPrediction(target);
           var qSpell =ComboMenu["CQ"].Cast<CheckBox>().CurrentValue;
           var eSpell = ComboMenu["CE"].Cast<CheckBox>().CurrentValue;
           var wSpell = ComboMenu["CW"].Cast<CheckBox>().CurrentValue;
          var rSpell = ComboMenu["CR"].Cast<CheckBox>().CurrentValue;
         var rwwSpell = ComboMenu["CRo"].Cast<CheckBox>().CurrentValue;
            if (target.IsValidTarget(Q.Range))
            {
                if (GetPassiveBuff <= 2 || !ObjectManager.Player.HasBuff("RyzePassiveStack"))
                {
                    if (target.IsValidTarget(Q.Range) && qSpell && Q.IsReady())Q.Cast(qpred.UnitPosition);

                    if (target.IsValidTarget(W.Range) && wSpell && W.IsReady()) W.Cast(target);

                    if (target.IsValidTarget(E.Range) && eSpell && E.IsReady()) E.Cast(target);

                    if (R.IsReady() && rSpell)
                    {
                        if (target.IsValidTarget(W.Range)
                            && target.Health > (QDamage(target) + EDamage(target)))
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
                        if (target.IsValidTarget(W.Range)
                            && target.Health > (QDamage(target) + EDamage(target)))
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
                        if (target.IsValidTarget(W.Range)
                            && target.Health > (QDamage(target) + EDamage(target)))
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
                        if (target.IsValidTarget(W.Range)
                            && target.Health > (QDamage(target)) +EDamage(target))
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
