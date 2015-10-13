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

                     var minion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    ObjectManager.Player.Position,
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


       }
    }
}
