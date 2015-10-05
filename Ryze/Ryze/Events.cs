using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ryze
{
    using EloBuddy;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu.Values;

    static class Events
    {
      public static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
            var Wuse = Program.miscMenu["WGapCloser"].Cast<CheckBox>().CurrentValue;
            if (!Wuse) return;
            Program.W.Cast(sender);
        }

       public static void InterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs e)
        {
            var wSpell = Program.miscMenu["Use W Interrupt"].Cast<CheckBox>().CurrentValue;
            if (!wSpell) return;
            Program.W.Cast(sender);
        }
    }
}
