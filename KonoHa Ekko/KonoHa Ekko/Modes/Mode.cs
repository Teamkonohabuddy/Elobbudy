using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Ekko.Modes
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu.Values;

    abstract  class Mode
    {
        public enum mode
        {
            Normal=0,Chase=1,Flee=2,TeamFive=3
        }

       public mode modeActive { get; private set; }
        public virtual void Update(EkkoCore core)
        {
            var check = core._menu.ComboMenu["TeamfiveCheckC"].Cast<Slider>().CurrentValue;
            var target = core.targetSelected.getTarget(check);
            if (target == null)
            {
                this.modeActive = mode.Normal;
                return;
            }
            if (core.Player.Distance(target)
                < core.Player.Distance(Prediction.Position.PredictUnitPosition(target, 1000)))
            {
                this.modeActive=mode.Chase;
            }
            else
            {
                this.modeActive = mode.Normal;
            }
            if (this.getNumTargetsRange((int)core.spells.R.Range, 3))
            {
                this.modeActive = mode.TeamFive;
            }

        }

        public bool getNumTargetsRange(int range, int num)
        {
            var i = 0;
            foreach (var target in EntityManager.Heroes.Enemies)
            {
               if(target!=null && !target.IsDead)
                if (target.Distance(Player.Instance) <= range)
                {
                    i++;
                }
            }
            if (i >= num) return true;
            return false;
        }
    }
}
