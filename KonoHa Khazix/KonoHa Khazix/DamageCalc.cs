using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Khazix
{
    using EloBuddy;
    using EloBuddy.SDK;

    static  class DamageCalc
    {
    public static double GetQDamage(Obj_AI_Base target)
      {
          if (Program.getQ.Range < 326)
          {
              return 0.984 * QDamage(target);
          }
          if (Program.getQ.Range > 325)
          {
              var isolated = IsoCheck(target);
              if (isolated)
              {
                  return 0.984 * QDamageIsolated(target);
              }
              return QDamage(target);
          }
          return 0;
      }
      public static float QDamage(Obj_AI_Base target)
      {
          //70 / 95 / 120 / 145 / 170 
          return Program.myHero.CalculateDamageOnUnit(
              target,
              DamageType.Magical,
              new[] { 70, 95, 120, 145, 170 }[Program.getQ.Level - 1] 
              +(120 /100 )*Program.myHero.TotalAttackDamage);
      }
      public static float QDamageIsolated(Obj_AI_Base target)
      {
          //70 / 95 / 120 / 145 / 170 
          if (Program.evolQ)
          {
              return Program.myHero.CalculateDamageOnUnit(
      target,
      DamageType.Magical,
      new[] { 71, 104, 136, 169, 201 }[Program.getQ.Level - 1]
      + ((156+104) / 100) * Program.myHero.TotalAttackDamage);
          }
          return Program.myHero.CalculateDamageOnUnit(
              target,
              DamageType.Magical,
              new[] { 71, 104, 136, 169, 201 }[Program.getQ.Level - 1]
              + (156 / 100) * Program.myHero.TotalAttackDamage);
      }
      public static bool IsoCheck(Obj_AI_Base target)
      {
          return !ObjectManager
              .Get<Obj_AI_Base>().Any(x => x.NetworkId != target.NetworkId && x.Team == target.Team && x.Distance(target) <= 500);
      }

    }

}
