using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Ekko
{
    using EloBuddy;
    using EloBuddy.SDK;

    using SharpDX;

    class DamageIndicator
    {
     //   private static DamageToUnitDelegate DamageToUnit { get; set; }
        private const int BarWidth = 104;
        private static readonly Vector2 BarOffset = new Vector2(-9, 11);
        private const int LineThickness = 9;
        private System.Drawing.Color DrawingColor;
        public bool HealthbarEnabled { get; set; }
    //    public delegate float DamageToUnitDelegate(AIHeroClient hero);
        private EkkoCore core;
        public  void Initialize(EkkoCore core)
        {
            // Apply needed field delegate for damage calculation
      //      DamageToUnit = damageToUnit;
            this.core = core;
            DrawingColor = System.Drawing.Color.Green;
            HealthbarEnabled = true;

            // Register event handlers
            Drawing.OnEndScene += OnEndScene;
        }

        private  void OnEndScene(EventArgs args)
        {
            if (HealthbarEnabled)
            {
                foreach (var unit in EntityManager.Heroes.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
                {
                    var damage =core.spells.TotalDam(unit);
                    if (damage <= 0)
                    {
                        continue;
                    }
           //         unit.AllShield
                        // Get remaining HP after damage applied in percent and the current percent of health
                        var damagePercentage = ((unit.TotalShieldHealth() - damage) > 0 ? (unit.TotalShieldHealth() - damage) : 0) /
                                               (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                        var currentHealthPercentage = unit.TotalShieldHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

                        // Calculate start and end point of the bar indicator
                        var startPoint = new Vector2((int)(unit.HPBarPosition.X + BarOffset.X + damagePercentage * BarWidth), (int)(unit.HPBarPosition.Y + BarOffset.Y) - 5);
                        var endPoint = new Vector2((int)(unit.HPBarPosition.X + BarOffset.X + currentHealthPercentage * BarWidth) + 1, (int)(unit.HPBarPosition.Y + BarOffset.Y) - 5);

                        // Draw the line
                        Drawing.DrawLine(startPoint, endPoint, LineThickness, DrawingColor);
                    
                }
            }
        }

    }
}
