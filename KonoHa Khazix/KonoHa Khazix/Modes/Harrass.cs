using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Khazix.Modes
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Menu.Values;

    using SharpDX;

    static class Harrass
    {
        public static void Do()
        {
            AIHeroClient target = TargetSelector.GetTarget(Program.getW.Range, DamageType.Physical);
            if (target != null)
            {
                if (Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) <= Program.getQ.Range && Program.HarrassMenu["QH"].Cast<CheckBox>().CurrentValue && Program.getQ.IsReady() && !Program.Jumping)
                {
                    Orbwalker.DisableAttacking = true;
                    Program.getQ.Cast(target);
                    Orbwalker.DisableAttacking = false;
                }

                if (Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) <= Program.getW.Range && Program.HarrassMenu["WH"].Cast<CheckBox>().CurrentValue && Program.getW.IsReady() &&
                    Combo.Wnorm)
                {
                    if (Program.getW.GetPrediction(target).HitChance == HitChance.High)
                    Program.getW.Cast(target);
                }
            }
        }
    }
}
