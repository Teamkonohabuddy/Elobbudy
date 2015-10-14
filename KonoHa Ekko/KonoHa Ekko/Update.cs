using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Ekko
{
    using EloBuddy.SDK;

    using KonoHa_Ekko.Modes;

    internal class Update
    {
        public Mode ComboMode, HarassMode, JungleclearMode, LaneclearMode,fleeMode;

        public Update()
        {
            this.ComboMode = new Combo();
            this.HarassMode = new Harrash();
            this.LaneclearMode = new Laneclear();
            this.JungleclearMode = new Jungleclear();
            this.fleeMode=new Flee();
        }

        public void update(EventArgs args, EkkoCore core)
        {

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo)) this.ComboMode.Update(core);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass)) this.HarassMode.Update(core);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee)) this.fleeMode.Update(core);
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                this.LaneclearMode.Update(core);
                this.JungleclearMode.Update(core);
            }
        }
    }
}
