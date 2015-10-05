using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Khazix.Modes
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Constants;
    using EloBuddy.SDK.Menu.Values;

    static class LastHit
    {
       public static void Do()
       {
           var UseQ = Program.LastHitMenu["QL"].Cast<CheckBox>().CurrentValue;
          // var UseW = Program.LastHitMenu["WL"].Cast<CheckBox>().CurrentValue;
           var OnlyIfDie = Program.LastHitMenu["UL"].Cast<CheckBox>().CurrentValue;

      /*     if (OnlyIfDie)
           {
               if (!Orbwalker.CanAutoAttack)
               {
                   var minion =
   EntityManager.GetLaneMinions(
       EntityManager.UnitTeam.Enemy,
       ObjectManager.Player.Position.To2D(),
       600,
       true);
                   if (minion != null)
                   {
                       var selectQ =
                           minion.Where(
                               t => Player.Instance.TotalAttackDamage >= t.Health && Program.myHero.Distance(t) < Program.getQ.Range)
                               .FirstOrDefault();
                       if (selectQ != null)
                       {
                           Program.getQ.Cast(selectQ);
                       }
                   }
               }
           }
           else
           {*/
                     var minion =
                EntityManager.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    ObjectManager.Player.Position.To2D(),
                    600,
                    true);
               if (minion != null)
               {
                   if (UseQ)
                   {
                       var selectQ =
                           minion.Where(
                               t =>
                               DamageCalc.GetQDamage(t) >= t.Health && Program.myHero.Distance(t) < Program.getQ.Range)
                               .FirstOrDefault();
                       if (selectQ != null)
                       {
                           Program.getQ.Cast(selectQ);
                       }
                   }
               }
// add w too lazy for dmg calcs XD

       //    }

       }
    }
}
