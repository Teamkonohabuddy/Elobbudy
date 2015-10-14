using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Ekko
{
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;

    internal class Menu
    {
        public EloBuddy.SDK.Menu.Menu menu, ComboMenu, HarassMenu, FleeMenu,DrawMenu,LaneclearMenu,JungleclearMenu,MiscMenu;
        public Menu()
        {
            menu = MainMenu.AddMenu("Ekko", "Ekko");
            this.Combo();
            this.Harass();
            this.Flee();
            this.Draw();
            this.LaneClear();
            this.JungleClear();
            this.Misc();
        }

        public void Misc()
        {
            MiscMenu = menu.AddSubMenu("Misc", "Misc");
            MiscMenu.Add("MG", new CheckBox("Stop Gapcloser", true));

        }
        private void Combo()
        {
            ComboMenu = menu.AddSubMenu("Combo", "Combo");
            ComboMenu.Add("QC", new CheckBox("Use Q", true));
            ComboMenu.Add("WC", new CheckBox("Use W", true));
            ComboMenu.Add("EC", new CheckBox("Use E", true));
            ComboMenu.Add("RC", new CheckBox("Use R", true));
            ComboMenu.Add("ROC", new CheckBox("R if combo Killeable", false));
            ComboMenu.Add("RPC", new CheckBox("On teamfive priorize damage over kill", true));
            ComboMenu.Add("RNC", new Slider("num Enemys R on teamfive :", 3,1,5));
            ComboMenu.Add("RTC", new Slider("% Life for R all In min", 25, 0, 100));
            ComboMenu.Add("ROUC", new Slider("% Life for R all go out", 10, 0, 100));
            ComboMenu.Add("TeamfiveCheckC", new Slider("Range for check if you are in tf", 1650, 0, 2500));
        }
        private void Harass()
        {
            HarassMenu = menu.AddSubMenu("Harass", "Harass");
            HarassMenu.Add("QH", new CheckBox("Use Q", true));
            HarassMenu.Add("WH", new CheckBox("Use W", true));
            HarassMenu.Add("EH", new CheckBox("Use E", true));
        }

        private void LaneClear()
        {
            LaneclearMenu = menu.AddSubMenu("Laneclear", "Laneclear");
            LaneclearMenu.Add("QL", new CheckBox("Use Q", true));
            LaneclearMenu.Add("EL", new CheckBox("Use E", true));
        }
        private void JungleClear()
        {
            JungleclearMenu = menu.AddSubMenu("JungleClear", "JungleClear");
            JungleclearMenu.Add("QJ", new CheckBox("Use Q", true));
            JungleclearMenu.Add("WJ", new CheckBox("Use W", true));
            JungleclearMenu.Add("EJ", new CheckBox("Use E", true));
        }
        private void Flee()
        {
            FleeMenu = menu.AddSubMenu("Flee", "Flee");
            FleeMenu.Add("QF", new CheckBox("Use Q", true));
            FleeMenu.Add("WF", new CheckBox("Use W to run", true));
            FleeMenu.Add("EF", new CheckBox("Use E", true));
        }

        public void Draw()
        {
            DrawMenu = menu.AddSubMenu("Drawing", "Drawing");
            DrawMenu.Add("QD", new CheckBox("Q Draw", true));
            DrawMenu.Add("WD", new CheckBox("W Draw", true));
            DrawMenu.Add("ED", new CheckBox("E Draw", true));
            DrawMenu.Add("RD", new CheckBox("R Draw", true));
            DrawMenu.Add("DM", new CheckBox("Draw Mode", true));
            DrawMenu.Add("DTF", new CheckBox("Draw TF Range", false));
        }
    }
}
