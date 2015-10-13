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

    static class Combo
    {
     public static bool Wnorm=true;
       public static void Do()
       {
           var isolatedlist = GetQTargets();
        //   isolatedlist.
         //  HitChance hitchance = HarassHitChance();
          var target = new AIHeroClient();
           if (isolatedlist != null && isolatedlist.Any())
           {
            var isolated = TargetSelector.GetTarget(Program.getE.Range, DamageType.Physical);
            target = isolated;
           }
           else
           {
              target = TargetSelector.GetTarget(Program.getE.Range, DamageType.Physical);
           }
           if (target == null && target.IsValidTarget(Program.getE.Range + 100) && !target.IsZombie)
           {
               target = TargetSelector.GetTarget(Program.getE.Range, DamageType.Physical);
           }

           if (target != null)
           {
               if (Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) <=Program.getQ.Range &&Program.ComboMenu["QC"].Cast<CheckBox>().CurrentValue&&
             Program.getQ.IsReady() && !Program.Jumping)
               {

                   Orbwalker.DisableAttacking = true;
                   Program.getQ.Cast(target);
                   Orbwalker.DisableAttacking = false;
               }

               if (Wnorm && Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) <= Program.getQ.Range && Program.ComboMenu["WC"].Cast<CheckBox>().CurrentValue &&
             Program.getW.IsReady() && Program.getW.GetPrediction(target).HitChance == HitChance.High )
               {
               
                   Program.getW.Cast(target);
               }

               if (Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) <= Program.getE.Range && Program.ComboMenu["EC"].Cast<CheckBox>().CurrentValue &&
                   Program.getE.IsReady() && Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) > Program.getQ.Range + (0.7 * Player.Instance.MoveSpeed))
               {
                   if (target.IsValid && !target.IsDead)
                   {
                       Program.getE.Cast(target);
                   }
               }


              if ((Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) <= Program.getE.Range + Program.getQ.Range + (0.7 * Player.Instance.MoveSpeed) && Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) > Program.getQ.Range && Program.getE.IsReady() &&
         Program.ComboMenu["ECG"].Cast<CheckBox>().CurrentValue) || (Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) <= Program.getE.Range + Program.getW.Range && Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) > Program.getQ.Range && Program.getE.IsReady() && Program.getW.IsReady() &&
           Program.ComboMenu["WCG"].Cast<CheckBox>().CurrentValue))
               {
                  // PredictionOutput pred = E.GetPrediction(target);

                  if (target.IsValid && !target.IsDead)
                   {
                       if (Program.getE.GetPrediction(target).HitChance == HitChance.High)
                       Program.getE.Cast(target);
                   }
                   // UseRCG
                  
                   if (Program.ComboMenu["RCG"].Cast<CheckBox>().CurrentValue && Program.getR.IsReady())
                   {
                       Program.getR.Cast();
                   }
               }

              if (Program.getR.IsReady() && !Program.getQ.IsReady() && !Program.getW.IsReady() && !Program.getE.IsReady() &&
                Program.ComboMenu["RC"].Cast<CheckBox>().CurrentValue)  
               {
                      Program.getR.Cast();
               }
               if (Program.evolW && Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) <= Program.getWE.Range && Program.ComboMenu["WC"].Cast<CheckBox>().CurrentValue &&
                    Program.getW.IsReady())
                {
                    var qpred = Program.getWE.GetPrediction(target);
                    if (Program.getWE.GetPrediction(target).HitChance == HitChance.High)
                    {
                        Program.getWE.Cast(target);
                    }

                    if (Program.getWE.GetPrediction(target).HitChance >= HitChance.Collision)
                    {
                        var PCollision = qpred.CollisionObjects;
                        var x = PCollision.Where(PredCollisionChar => PredCollisionChar.Distance(target) <= 30).FirstOrDefault();
                        if (x != null)
                        {
                            Program.getW.Cast(x.Position);
                        }
                    }
                }


               if (Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) <= Program.getE.Range + (0.7 * Player.Instance.MoveSpeed) && Vector3.Distance(Player.Instance.ServerPosition, target.ServerPosition) > Program.getQ.Range &&
                 Program.ComboMenu["EC"].Cast<CheckBox>().CurrentValue && Program.getE.IsReady())
               {
           
                   if (target.IsValid && !target.IsDead)
                   {
                       Program.getE.Cast(target);
                   }
               }

               if (Program.ComboMenu["IC"].Cast<CheckBox>().CurrentValue)
               {
                   UseItems(target);
               }
           }

       }
        
        public static void UseItems(AIHeroClient target)
        {
            
        }

       public static List<AIHeroClient> GetQTargets()
       {
           var validtargets = EntityManager.Heroes.AllHeroes.Where(h => h.IsValidTarget(Program.getE.Range));
           return (from x in validtargets let minions = ObjectManager.Get<Obj_AI_Base>().Where(xd => xd.IsEnemy && x.NetworkId != xd.NetworkId && x.ServerPosition.Distance(xd.ServerPosition) < 500 && (xd.Type == GameObjectType.AIHeroClient || xd.Type == GameObjectType.obj_AI_Minion || xd.Type == GameObjectType.obj_AI_Turret)) where !minions.Any() where !x.IsDead select x).ToList();
       }
   }
}
