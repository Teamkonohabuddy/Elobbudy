using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Ekko
{

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using KonoHa_Ekko.Modes;

    class EkkoCore
    {
        private Drawings _draw;
        public Menu _menu;
        public Update _update;
        public Spells spells;
        public MyTs targetSelected;

        public DamageIndicator DmgIndicator;
        public Modes.Misc misc;
        public AIHeroClient Player
        {
            get
            {
                return EloBuddy.Player.Instance;
            }
        }
        internal void OnLoad(EventArgs args)
        {
            this._menu= new Menu();
            this.spells=new Spells();
            this._update=new Update();
            this._draw=new Drawings();
            this.DmgIndicator=new DamageIndicator();
            DmgIndicator.Initialize(this);
            this.misc=new Misc();
            this.targetSelected = new MyTs();
            Gapcloser.OnGapcloser += this.OnGapCloser;
            EloBuddy.Game.OnUpdate += this.Onupdate;
            EloBuddy.Drawing.OnDraw += this.Ondraw;

        }
         
        private void OnGapCloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs e)
        {
misc.OnGapCloser(sender,e,this);
        }

        private void Onupdate(EventArgs args)
        {
           this._update.update(args,this);
        }

        private void Ondraw(EventArgs args)
        {
         this._draw.Update(args,this);
        }


    }
}
