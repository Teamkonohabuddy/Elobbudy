using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Ekko
{
    using EloBuddy;
    using EloBuddy.SDK;

    class MyTs
    {
        public MyTs()
        {
            Game.OnWndProc += this.OnProc;
        }
        public AIHeroClient Selected { private set; get; }

        public AIHeroClient getTarget(float Range)
        {
            if (Selected != null && Selected.Distance(Player.Instance.Position) <= Range)
            {
                return Selected;
                
            }
        return TargetSelector.GetTarget(Range, DamageType.Magical);
        }
        private  void OnProc(WndEventArgs args)
        {

            if (args.Msg != (uint)WindowMessages.LeftButtonDown)
            {
                return;
            }
            var trys = HeroManager.Enemies
              .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
              .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
            if (trys != null)
            {
                Selected = HeroManager.Enemies
                    .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
                    .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
            }

        }
    }
}
