using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonoHa_Ekko
{
    using EloBuddy;
    using EloBuddy.SDK.Events;

    class Program
    {
        private static EkkoCore core;
        static void Main(string[] args)
        {
         //   Chat.Print(Player.Instance.Name);
           if (Player.Instance.ChampionName != "Ekko") return;
            core = new EkkoCore();
            Loading.OnLoadingComplete += core.OnLoad;
        }

    }
}
