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

    using SharpDX;

    using Color = System.Drawing.Color;

    static class DamageCalc
    {
        private static bool Enabled;
        private const int XOffset = 10;
        private const int YOffset = 0;
        private const int Width = 103;
        private const int Height = 8;
        private const int BarWidth = 104;
        public delegate float DamageToUnitDelegate(AIHeroClient hero);
        private static DamageToUnitDelegate DamageToUnit { get; set; }
        private static readonly Vector2 BarOffset = new Vector2(10, 25);
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
        public static void Initialize(DamageToUnitDelegate damageToUnit)
        {
            // Apply needed field delegate for damage calculation
            DamageToUnit = damageToUnit;
            DrawingColor = System.Drawing.Color.Green;
            Enabled = true;

            // Register event handlers
            Drawing.OnEndScene += DrawDamage;
        }
        public static void DrawDamage(EventArgs args)
        {

            foreach (var unit in EntityManager.Heroes.Enemies.Where(h => h.IsValid && h.IsHPBarRendered))
            {
                var damage = DamageToUnit(unit);

                // Continue on 0 damage
                //Chat.Print(""+damage);
                if (damage <= 0)
                    continue;

                // Get remaining HP after damage applied in percent and the current percent of health
                var damagePercentage = ((unit.Health - damage) > 0 ? (unit.Health - damage) : 0) / unit.MaxHealth;
                var currentHealthPercentage = unit.Health / unit.MaxHealth;

                // Calculate start and end point of the bar indicator
                var startPoint = new Vector2((int)(unit.HPBarPosition.X + BarOffset.X + damagePercentage * BarWidth), (int)(unit.HPBarPosition.Y + BarOffset.Y) - 5);
                var endPoint = new Vector2((int)(unit.HPBarPosition.X + BarOffset.X + currentHealthPercentage * BarWidth) + 1, (int)(unit.HPBarPosition.Y + BarOffset.Y) - 5);

                // Draw the line
                Drawing.DrawLine(startPoint, endPoint, 9,Color.Red);
            }
        }

        public static Color DrawingColor { get; set; }
    }
}
