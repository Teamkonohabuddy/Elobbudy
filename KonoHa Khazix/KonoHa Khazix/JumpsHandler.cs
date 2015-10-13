using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Khazix
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    using SharpDX;

    static class JumpsHandler
    {
        public static Vector3 bases;
        private static Vector3 Jumppoint1, Jumppoint2;
        public static Vector3 GetJumpPoint(AIHeroClient Qtarget, bool firstjump = true)
        {
            return Player.Instance.ServerPosition.Extend(bases, Program.getE.Range).To3D();
        }


        internal static void DoubleJumpLogic(EventArgs args)
        {
            if (!Program.getE.IsReady() || !Program.evolE || Program.DoubleJumpMenu["DE"].Cast<CheckBox>().CurrentValue || Player.Instance.IsDead || Player.Instance.IsRecalling())
                return;
            var Targets = EntityManager.Heroes.Enemies.Where(x=>x.IsValidTarget() && !x.IsInvulnerable && !x.IsZombie);
            if (Program.getQ.IsReady())
            {
                var CheckQKillable = Targets.FirstOrDefault(x => Vector3.Distance(Player.Instance.ServerPosition, x.ServerPosition) < Program.getQ.Range - 50 && DamageCalc.GetQDamage(x) > x.Health);
                if (CheckQKillable != null)
                {
                    Program.Jumping = true;
                    Jumppoint1 = GetJumpPoint(CheckQKillable);
                    Program.getQ.Cast(CheckQKillable);
                    var oldpos = Player.Instance.ServerPosition;
                        if (Program.getE.IsReady())
                        {
                            Jumppoint2 = GetJumpPoint(CheckQKillable, false);
                            Program.getE.Cast(Jumppoint2);
                        }
                        Program.Jumping = false;
                }

            }

        }
    }
}
