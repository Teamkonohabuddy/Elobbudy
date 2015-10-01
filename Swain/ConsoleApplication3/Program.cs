using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using EloBuddy.SDK.Rendering;
using SharpDX;

namespace KonohaSwain
{
    internal class Program
    {
        public static Spell.Targeted Q, E;
        public static Spell.Skillshot W;
        public static Spell.Active R;
        public static AIHeroClient selected;
        public static Menu menu,
            ComboMenu,
            HarrassMenu,
            LasthitMenu,
            LaneclearMenu,
            JungleclearMenu,
            MiscMenu,
            DrawingsMenu;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Swain") return;
            Q = new Spell.Targeted(SpellSlot.Q, 625);
            W = new Spell.Skillshot(SpellSlot.W, 820, SkillShotType.Circular, 500, 1250, 275);
            E = new Spell.Targeted(SpellSlot.E, 625);
            R = new Spell.Active(SpellSlot.R);
            Loadmenu();
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += OnProc;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var drawQ = DrawingsMenu["Draw Q"].Cast<CheckBox>().CurrentValue;
            var drawW = DrawingsMenu["Draw W"].Cast<CheckBox>().CurrentValue;
            var drawE = DrawingsMenu["Draw E"].Cast<CheckBox>().CurrentValue;
            var drawR = DrawingsMenu["Draw R"].Cast<CheckBox>().CurrentValue;
            if(drawQ)
                Circle.Draw(Color.SlateBlue, Q.Range, Player.Instance.Position);
            if(drawW)
                Circle.Draw(Color.SlateBlue, W.Range, Player.Instance.Position);
            if(drawE)
                Circle.Draw(Color.SlateBlue, E.Range, Player.Instance.Position);
            if(drawR)
                Circle.Draw(Color.SlateBlue,700, Player.Instance.Position);
            if (selected != null && selected.IsVisible)
            {
                Circle.Draw(Color.Red, 100, selected.Position);
            }
        }

        public static bool Rac;
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
            {
                Combo();
            }
            else
            {
                if (Rac == true && R.Handle.ToggleState == 2)
                {
                    R.Cast();
                    Rac = false;
                }


            }
        }

        private static void Combo()
        {
            var comboQ = ComboMenu["SQ"].Cast<CheckBox>().CurrentValue;
            var comboW = ComboMenu["SW"].Cast<CheckBox>().CurrentValue;
            var comboE = ComboMenu["SE"].Cast<CheckBox>().CurrentValue;
            var comboR = ComboMenu["SR"].Cast<CheckBox>().CurrentValue;
            var target = TargetSelector.GetTarget(700, DamageType.Magical);
            if ( selected != null && selected.IsVisible && selected.Position.Distance(ObjectManager.Player) <= 570) target = selected;
            if (target != null)
            {

                var Wpred = W.GetPrediction(target);
                E.Cast(target);
                if (ObjectManager.Player.Distance(target) <= 550)
                    Q.Cast(target);
                if(target.HasBuffOfType(BuffType.Slow))
                W.Cast(Wpred.UnitPosition);
        
                if (R.Handle.ToggleState == 1)
                {
                    R.Cast();
                    Rac = true;
                }
            }
            if (target == null && R.Handle.ToggleState == 2)
            {
                R.Cast();
                Rac = false;
            }

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
                selected = HeroManager.Enemies
                    .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
                    .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
            }

        }
        private static void Loadmenu()
        {
            menu = MainMenu.AddMenu("Swain", "Swain");
            ComboMenu = menu.AddSubMenu("Combo", "combomenu");
            HarrassMenu = menu.AddSubMenu("Harrass", "Harrassmenu");
            LasthitMenu = menu.AddSubMenu("Lasthit", "Lasthitmenu");
            JungleclearMenu = menu.AddSubMenu("Laneclear", "Laneclearmenu");
            MiscMenu = menu.AddSubMenu("Misc", "Miscmenu");
            DrawingsMenu = menu.AddSubMenu("Drawings", "Drawingsmenu");
            ComboMenu.Add("SQ", new CheckBox("Use Q"));
            ComboMenu.Add("SW", new CheckBox("Use W"));
            ComboMenu.Add("SE", new CheckBox("Use E"));
            ComboMenu.Add("SR", new CheckBox("Use R"));
            ComboMenu.Add("StopRMana%", new Slider("Stop R when ur MP %", 1, 0, 100));
            ComboMenu.Add("ManualR", new CheckBox("Manual R"));
            HarrassMenu.Add("SQ", new CheckBox("Use Q"));
            HarrassMenu.Add("SE", new CheckBox("Use E"));
            HarrassMenu.Add("SR", new CheckBox("Use R"));
            LasthitMenu.Add("SQ", new CheckBox("Use Q"));
            LasthitMenu.Add("SE", new CheckBox("Use E"));
            LasthitMenu.Add("SR", new CheckBox("Use R"));
            JungleclearMenu.Add("SQ", new CheckBox("Use Q"));
            JungleclearMenu.Add("SW", new CheckBox("Use W"));
            JungleclearMenu.Add("SE", new CheckBox("Use E"));
            JungleclearMenu.Add("SR", new CheckBox("Use R"));
            MiscMenu.Add("Antigapclosers", new CheckBox("Use W Antigapclosers"));
            MiscMenu.Add("RecoverHp", new Slider("Use R when ur HP % ", 1, 0, 100));
            DrawingsMenu.Add("Draw Q", new CheckBox("Draw Q"));
            DrawingsMenu.Add("Draw W", new CheckBox("Draw W"));
            DrawingsMenu.Add("Draw E", new CheckBox("Draw E"));
            DrawingsMenu.Add("Draw R", new CheckBox("Draw R"));
        }
    }
}
