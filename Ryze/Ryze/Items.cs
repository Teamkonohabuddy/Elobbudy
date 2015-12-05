using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryze
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    static class Items
    {
        private static Item _tearoftheGoddess = new Item(3070, 0);
        private static Item _tearoftheGoddesss = new Item(3072, 0);
        private static Item _tearoftheGoddessCrystalScar = new Item(3073, 0);
        private static Item _archangelsStaff = new Item(3048, 0);
        private static Item _archangelsStaffCrystalScar = new Item(3007, 0);
        private static Item _manamune = new Item(3004, 0);
        private static Item _manamuneCrystalScar = new Item(3008, 0);

        private static Item _healthPotion = new Item(2003, 0);
        private static Item _crystallineFlask = new Item(2041, 0);
        private static Item _manaPotion = new Item(2004);
        private static Item _biscuitofRejuvenation = new Item(2010, 0);
        public static void Initzialize()
        {
        //    UsePotions();
        //    UseTear();
            UseSerapth();
            UseManaMune();
        }

        private static void UsePotions()
        {
            var autoPotion = Program.PotionMenu["autoPO"].Cast<CheckBox>().CurrentValue;
            var hPotion = Program.PotionMenu["UsePotion"].Cast<CheckBox>().CurrentValue;
            var mPotion = Program.PotionMenu["UseManaPotion"].Cast<CheckBox>().CurrentValue;
            var bPotion = Program.PotionMenu["UseBiscuit"].Cast<CheckBox>().CurrentValue;
            var fPotion = Program.PotionMenu["flask"].Cast<CheckBox>().CurrentValue;
            var pSlider = Program.PotionMenu["PotionHP"].Cast<Slider>().CurrentValue;
            var mSlider = Program.PotionMenu["ManaPotionHP"].Cast<Slider>().CurrentValue;
            var bSlider = Program.PotionMenu["BiscuitHP"].Cast<Slider>().CurrentValue;
            var fSlider = Program.PotionMenu["flaskHP"].Cast<Slider>().CurrentValue;
            //for now shop range
            if (Player.Instance.IsRecalling() || Player.Instance.IsInShopRange()) return;
            if (!autoPotion) return;

            if (hPotion
                && Player.Instance.HealthPercent <= pSlider
                && Player.Instance.CountEnemiesInRange(1000) >= 0
                && _healthPotion.IsReady()
                && !Player.Instance.HasBuff("FlaskOfCrystalWater")
                && !Player.Instance.HasBuff("ItemCrystalFlask")
                && !Player.Instance.HasBuff("RegenerationPotion"))
                _healthPotion.Cast();

            if (mPotion
                && Player.Instance.ManaPercent <= mSlider
                && Player.Instance.CountEnemiesInRange(1000) >= 0
                && _manaPotion.IsReady()
                && !Player.Instance.HasBuff("RegenerationPotion")
                && !Player.Instance.HasBuff("FlaskOfCrystalWater"))
                _manaPotion.Cast();

            if (bPotion
                && Player.Instance.HealthPercent <= bSlider
                && Player.Instance.CountEnemiesInRange(1000) >= 0
                && _biscuitofRejuvenation.IsReady()
                && !Player.Instance.HasBuff("ItemMiniRegenPotion"))
                _biscuitofRejuvenation.Cast();

            if (fPotion
                &&  Player.Instance.HealthPercent <= fSlider
                &&  Player.Instance.CountEnemiesInRange(1000) >= 0
                && _crystallineFlask.IsReady()
                && !Player.Instance.HasBuff("ItemMiniRegenPotion")
                && !Player.Instance.HasBuff("ItemCrystalFlask")
                && !Player.Instance.HasBuff("RegenerationPotion")
                && !Player.Instance.HasBuff("FlaskOfCrystalWater"))
                _crystallineFlask.Cast();
        }

        public static void UseTear()
        {
            var useTears = Program.ItemsMenu["TEAR"].Cast<CheckBox>().CurrentValue;
            var useTearsfo = Program.ItemsMenu["TEARFO"].Cast<CheckBox>().CurrentValue;
            var mtears = Program.ItemsMenu["tearSM"].Cast<Slider>().CurrentValue;
            if (!useTears) return;
            if (Program.GetPassiveBuff == 4) return;
            //     if (useTearsfo &&Player.Instance.in )
            //      return; añadir infountain
          //  if (Player.Instance.IsRecalling()) return;
            if (Program.Q.IsReady() ||
              (!_tearoftheGoddess.IsOwned(Player.Instance) && !_tearoftheGoddessCrystalScar.IsOwned(Player.Instance) &&
               !_archangelsStaff.IsOwned(Player.Instance) && !_archangelsStaffCrystalScar.IsOwned(Player.Instance) &&
               !_manamune.IsOwned(Player.Instance) && !_manamuneCrystalScar.IsOwned(Player.Instance)) || !(Player.Instance.ManaPercent >= mtears))
                return;
            if (!Game.CursorPos.IsZero)
                Program.Q.Cast(Game.CursorPos);
            else
               Program.Q.Cast();
        }

        public static void UseSerapth()
        {
            var useSerapth = Program.ItemsMenu["SERAPH"].Cast<CheckBox>().CurrentValue;
            var serapthHP = Program.ItemsMenu["seraphHP"].Cast<Slider>().CurrentValue;

            if (!useSerapth || !Item.HasItem(_archangelsStaff.Id,Player.Instance) || !(Player.Instance.HealthPercent <= serapthHP)) return;
            Item.UseItem(_archangelsStaff.Id);
        }

        public static void UseManaMune()
        {
            
        }



    }
}
