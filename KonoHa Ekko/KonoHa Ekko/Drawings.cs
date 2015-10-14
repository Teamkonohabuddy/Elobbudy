using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Ekko
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using SharpDX;

    internal class Drawings
    {
        public Drawings()
        {

        }

        public void Update(EventArgs args, EkkoCore core)
        {

            var drawQ = core._menu.DrawMenu["QD"].Cast<CheckBox>().CurrentValue;
            var drawW = core._menu.DrawMenu["WD"].Cast<CheckBox>().CurrentValue;
            var drawE = core._menu.DrawMenu["ED"].Cast<CheckBox>().CurrentValue;
            var drawR = core._menu.DrawMenu["RD"].Cast<CheckBox>().CurrentValue;
            var drawmode = core._menu.DrawMenu["DM"].Cast<CheckBox>().CurrentValue;
            var drawTf = core._menu.DrawMenu["DTF"].Cast<CheckBox>().CurrentValue;
            if (drawTf)
            {
                try
                {
                    var check = core._menu.ComboMenu["TeamfiveCheckC"].Cast<Slider>().CurrentValue;
                    Circle.Draw(Color.Red, check, Player.Instance.Position);
                }
                catch
                {
                }

            }
            if (drawmode)
            {
                if (core._update.ComboMode.modeActive != null)
                {
                    string mode = core._update.ComboMode.modeActive.ToString();
                    Vector2 pos = core.Player.Position.WorldToScreen();
                    Drawing.DrawText(pos.X, pos.Y, System.Drawing.Color.Red, mode, 25);
                }
            }
            if (drawQ && core.spells.Q.IsReady())
            {
                Circle.Draw(Color.Yellow, core.spells.Q.Range, Player.Instance.Position);
            }
            if (drawW && core.spells.W.IsReady())
            {
                Circle.Draw(Color.Yellow, core.spells.W.Range, Player.Instance.Position);
            }
            if (drawE && core.spells.E.IsReady())
            {
                Circle.Draw(Color.Yellow, core.spells.E.Range, Player.Instance.Position);
            }
            if (drawR && core.spells.R.IsReady())
            {
                Circle.Draw(Color.WhiteSmoke, core.spells.R.Range, Player.Instance.Position);
            }
            if (drawR && core.spells.R.IsReady())
            {
                var Ghost = core.spells.Ghost;
                Circle.Draw(Color.Red, 50, Ghost.Position);
                EloBuddy.Drawing.DrawLine(
                    core.Player.Position.WorldToScreen().X,
                    core.Player.Position.WorldToScreen().Y,
                    Ghost.Position.WorldToScreen().X,
                    Ghost.Position.WorldToScreen().Y,
                    5f,
                    System.Drawing.Color.YellowGreen);
                Circle.Draw(Color.YellowGreen, 375, Ghost.Position);
            }
            if (core.targetSelected.Selected != null)
            {
                Circle.Draw(Color.WhiteSmoke, 100, Player.Instance.Position);
            }
        }

    }
}
