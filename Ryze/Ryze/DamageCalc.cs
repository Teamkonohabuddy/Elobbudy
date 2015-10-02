using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryze
{
    using System.Drawing;
    using System.Runtime.CompilerServices;

    using EloBuddy;
    using EloBuddy.SDK;

    static class DamageCalc
    {
        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        public static float QDamage(Obj_AI_Base target)
        {
            return ObjectManager.Player.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                (float)
                (new double[] { 60, 85, 110, 135, 160 }[Program.Q.Level - 1] + 0.55 * Program.myHero.FlatMagicDamageMod
                 + new double[] { 2, 2.5, 3.0, 3.5, 4.0 }[Program.Q.Level - 1] / 100 * Program.myHero.MaxMana));
        }

        public static float WDamage(Obj_AI_Base target)
        {
            return Program.myHero.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                new[] { 80, 100, 120, 140, 160 }[Program.W.Level - 1] + 0.4f * Program.myHero.FlatMagicDamageMod
                + 0.02f * Program.myHero.MaxMana);
        }

        public static float EDamage(Obj_AI_Base target)
        {
            return Program.myHero.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                new[] { 36, 52, 68, 84, 100 }[Program.E.Level - 1] + 0.2f * Program.myHero.FlatMagicDamageMod + 0.025f * Program.myHero.MaxMana);
        }
        public static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage=0;
            if (Program.Q.IsReady() || Player.Instance.Mana <= Program.Q.Handle.SData.Mana )
             damage+= QDamage(enemy);

            if (Program.E.IsReady() || Player.Instance.Mana <= Program.E.Handle.SData.Mana)
                damage += EDamage(enemy);

            if (Program.W.IsReady() || Player.Instance.Mana <= Program.W.Handle.SData.Mana)
                damage += WDamage(enemy);
            return damage;
        }
        public static void DrawDamage()
        {
            foreach (var unit in HeroManager.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition+(unit.TotalHeal/10);
                var damage = GetComboDamage(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
           //     var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
             //   Drawing.DrawLine(barPos.X, barPos.Y + 10, barPos.X - (damage / 10), barPos.Y + 10, 1, Color.RoyalBlue);
              /*  var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var yPos = barPos.Y + YOffset;
                var xPosDamage = barPos.X + XOffset + Width * percentHealthAfterDamage;
                var xPosCurrentHp = barPos.X + XOffset + Width * unit.Health / unit.MaxHealth;
              //  Drawing.DrawText(Player.Instance.Position.WorldToScreen().X, Player.Instance.Position.WorldToScreen().Y, Color.Red, "Hola odos", 10);
                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1, Color.LightBlue);
                Drawing.DrawLine(xPosDamage, yPos, xPosDamage, yPos + Height, 1,Color.LightBlue);
                
                var differenceInHp = xPosCurrentHp - xPosDamage;
                var pos1 = barPos.X + 9 + (107 * percentHealthAfterDamage);
                for (var i = 0; i < differenceInHp; i++)
                {
                    Drawing.DrawLine(pos1 + i, yPos, pos1 + i, yPos + Height, 1, System.Drawing.Color.Goldenrod);
                }*/
            }
        }
    }
}
