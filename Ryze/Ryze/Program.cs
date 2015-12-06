using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
namespace Ryze
{
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;
    using System.Diagnostics;
    //Ryze
    internal class Program
    {
        private static Dictionary<AIHeroClient, Slider> _SkinVals = new Dictionary<AIHeroClient, Slider>();
        public static Menu menu, ComboMenu, DrawMenu, HarrashMenu, LaneClearMenu,JungleclearMenu,miscMenu, SkinHackMenu,ItemsMenu,PotionMenu,HumanizerMenu;
        public static Spell.Skillshot Q;
        public static Spell.Targeted W, E;
        public static Spell.Active R;
        public static AIHeroClient selected;
        public static AIHeroClient myHero
        {
            get
            {
                return ObjectManager.Player;
            }
        }
        readonly static Random Seeder = new Random();

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        public static void LoadMenu()
        {
            menu = MainMenu.AddMenu("Ryze", "Ryze");
            ComboMenu = menu.AddSubMenu("Combo", "combo");
            SkinHackMenu = menu.AddSubMenu("SkinHack", "SkinHack");
            HarrashMenu = menu.AddSubMenu("Harrash", "Harrash");
            HarrashMenu.Add("HMANA", new Slider("Min. mana for harrash :", 40, 0, 100));
            LaneClearMenu = menu.AddSubMenu("Laneclear", "Laneclear");
            JungleclearMenu = menu.AddSubMenu("Jungleclear", "Jungleclear");
            JungleclearMenu.Add("JQ", new CheckBox("Use Q"));
            JungleclearMenu.Add("JW", new CheckBox("Use W"));
            JungleclearMenu.Add("JE", new CheckBox("Use E"));
            JungleclearMenu.Add("JR", new CheckBox("Use R"));
            LaneClearMenu.Add("LQ", new CheckBox("Use Q"));
            LaneClearMenu.Add("LW", new CheckBox("Use W"));
            LaneClearMenu.Add("LE", new CheckBox("Use E"));
            LaneClearMenu.Add("LR", new CheckBox("Use R"));
            LaneClearMenu.Add("LMANA", new Slider("Min. mana for laneclear :", 0, 0, 100));
            DrawMenu = menu.AddSubMenu("Draw", "draw");
            HarrashMenu.Add("HQ", new CheckBox("Use Q"));
            HumanizerMenu = menu.AddSubMenu("Humanizer", "Humanizer");

            HumanizerMenu.Add("MinDelay",new Slider("Min Delay For Cast",150, 0, 400));
             HumanizerMenu.Add("MaxDelay",new Slider("Max Delay For Cast",250, 0, 500));
            HumanizerMenu.Add("Humanizer",new CheckBox("Humanizer Active",false));
            ComboMenu.Add("CQ", new CheckBox("Use Q"));
            ComboMenu.Add("CE", new CheckBox("Use E"));
            ComboMenu.Add("CW", new CheckBox("Use W"));
            ComboMenu.Add("CR", new CheckBox("Use R"));
            ComboMenu.Add("CRo", new CheckBox("Use R only on Root"));
            ComboMenu.Add("BlockAA", new CheckBox("Block AutoAttacks on combo"));
            ItemsMenu = menu.AddSubMenu("Items Menu", "Items");
      //      ItemsMenu.Add("TEAR", new CheckBox("Use Tear"));
      //      ItemsMenu.Add("TEARFO", new CheckBox("Use Tear only on fountain"));
    //        ItemsMenu.Add("tearSM", new Slider("Min % Mana to Stack Tear",40,0,100));
      //      ItemsMenu.AddSeparator(25);
            ItemsMenu.Add("SERAPH", new CheckBox("Use Serapth"));
            ItemsMenu.Add("seraphHP", new Slider("Hp% for Serapth", 40, 0, 100));
          /*  PotionMenu = ItemsMenu.AddSubMenu("PotionsMenu", "Potions Menu");
            PotionMenu.Add("autoPO", new CheckBox("Use AutoPot"));
            PotionMenu.Add("UsePotion", new CheckBox("Use Potion"));
            PotionMenu.Add("PotionHP", new Slider("Potion HP", 50, 0, 0));
            PotionMenu.Add("UseManaPotion", new CheckBox("Use Potion"));
            PotionMenu.Add("ManaPotionHP", new Slider("Mana % ", 50, 0, 0));
            PotionMenu.Add("UseBiscuit", new CheckBox("Use Potion"));
            PotionMenu.Add("BiscuitHP", new Slider("Biscuit HP", 50, 0, 0));
            PotionMenu.Add("flask", new CheckBox("Use flask"));
            PotionMenu.Add("flaskHP", new Slider("flask HP", 50, 0, 0));*/
            DrawMenu.Add("DQ", new CheckBox("Draw Q"));
            DrawMenu.Add("DW", new CheckBox("Draw W"));
            DrawMenu.Add("DE", new CheckBox("Draw E"));
            DrawMenu.Add("DD", new CheckBox("Draw Damage"));
            miscMenu = menu.AddSubMenu("Misc", "Misc");
            miscMenu.Add("WGapCloser", new CheckBox("Use W Gapcloser"));
            miscMenu.Add("WInterrupt", new CheckBox("Use W Interrupt"));
            miscMenu.Add("LockTar", new CheckBox("Active focus Selected Target"));
            var slid = SkinHackMenu.Add(ObjectManager.Player.BaseSkinName, new Slider("Select Ryze skin : ", 0, 0, 8));
            Player.SetSkinId(slid.CurrentValue);
            _SkinVals.Add(ObjectManager.Player, slid);
            _SkinVals[ObjectManager.Player].OnValueChange += Program_OnValueChange;

        }

        private static void Program_OnValueChange(ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
        {
            var hero = ObjectManager.Get<AIHeroClient>().Where(x => x.BaseSkinName == sender.DisplayName.Replace("Skin ID ", "")).FirstOrDefault();
            if (hero == null)
                return;
            hero.SetSkinId(args.NewValue);
        }

        private static void OnLoad(EventArgs args)
        {
            if (Player.Instance.Hero != Champion.Ryze) return;
            Q = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 250, 1700, 100);
            //   SpellData.GetSpellData("Q").MissileSpeed;
            W = new Spell.Targeted(SpellSlot.W, 600);
            E = new Spell.Targeted(SpellSlot.E, 600);
            R = new Spell.Active(SpellSlot.R);
            LoadMenu();
       //     DamageCalc.Initialize(DamageCalc.GetComboDamage);
            Game.OnUpdate += Game_OnUpdate;
            Interrupter.OnInterruptableSpell += Events.InterruptableSpell;
            Gapcloser.OnGapcloser += Events.OnGapcloser;
            Game.OnWndProc += OnProc;

            Drawing.OnDraw += Drawing_OnDraw;
        }

     



        private static void OnProc(WndEventArgs args)
        {

            if (args.Msg != (uint)WindowMessages.LeftButtonDown)
            {
                return;
            }
            var trys = EntityManager.Heroes.Enemies
              .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
              .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
            if (trys != null)
            {
                selected = EntityManager.Heroes.Enemies
                    .FindAll(hero => hero.IsValidTarget() && hero.Distance(Game.CursorPos, true) < 40000) // 200 * 200
                    .OrderBy(h => h.Distance(Game.CursorPos, true)).FirstOrDefault();
            }

        }
        static float gametime;
       static float delay;
       static float startTime;
        private static void Game_OnUpdate(EventArgs args)
        {
           Orbwalker.DisableAttacking = false;
        //   gametime += Game.Time * 1000;
           gametime =( Game.Time - startTime)*1000;
           if ((gametime >= delay && HumanizerMenu["Humanizer"].Cast<CheckBox>().CurrentValue) || HumanizerMenu["Humanizer"].Cast<CheckBox>().CurrentValue==false)
            {
                gametime = 0;
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.None)
            {
                Items.Initzialize();
            }
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Combo)
            {
               Combo();
            }
            if (Orbwalker.ActiveModesFlags == Orbwalker.ActiveModes.Harass)
            {
                Harrash();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
            {
                Laneclear();
            }
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                JungleClear();
            }
             startTime = Game.Time;
        delay = Seeder.Next(HumanizerMenu["MinDelay"].Cast<Slider>().CurrentValue, HumanizerMenu["MaxDelay"].Cast<Slider>().CurrentValue);
        }
        }

        private static void JungleClear()
        {
            var jungleclearQ = JungleclearMenu["JQ"].Cast<CheckBox>().CurrentValue;
            var jungleclearW = JungleclearMenu["JW"].Cast<CheckBox>().CurrentValue;
            var jungleclearE = JungleclearMenu["JE"].Cast<CheckBox>().CurrentValue;
            var jungleclearR = JungleclearMenu["JR"].Cast<CheckBox>().CurrentValue;
            Obj_AI_Base minion =
                EntityManager.MinionsAndMonsters.GetJungleMonsters(
     
                    ObjectManager.Player.Position,
                    600,
                    true).FirstOrDefault();
            if (minion != null)
            {
                if (jungleclearQ && Q.IsReady())
                {
                    var Qpred = Q.GetPrediction(minion);
                    Q.Cast(Qpred.UnitPosition);
                }
                if (jungleclearE && E.IsReady())
                {
                    E.Cast(minion);
                }
                if (jungleclearW && W.IsReady())
                {
                    W.Cast(minion);
                }
                if (jungleclearR && R.IsReady() && (GetPassiveBuff >= 4 || myHero.HasBuff("ryzepassivecharged")))
                {
                    R.Cast();
                }
            }
        }

        private static void Laneclear()
        {
            var laneclearQ = LaneClearMenu["LQ"].Cast<CheckBox>().CurrentValue;
            var laneclearW = LaneClearMenu["LW"].Cast<CheckBox>().CurrentValue;
            var laneclearE = LaneClearMenu["LE"].Cast<CheckBox>().CurrentValue;
            var laneclearR = LaneClearMenu["LR"].Cast<CheckBox>().CurrentValue;
            var laneclearMinMana = LaneClearMenu["LMANA"].Cast<Slider>().CurrentValue;
            Obj_AI_Base minion =
                EntityManager.MinionsAndMonsters.GetLaneMinions(
                    EntityManager.UnitTeam.Enemy,
                    ObjectManager.Player.Position,
                    600,
                    true).FirstOrDefault();
            if (minion != null && Player.Instance.ManaPercent>laneclearMinMana)
            {
                if (laneclearQ && Q.IsReady())
                {
                    var Qpred = Q.GetPrediction(minion);
                    Q.Cast(Qpred.UnitPosition);
                }
                if (laneclearE && E.IsReady())
                {
                    E.Cast(minion);
                }
                if (laneclearW && W.IsReady())
                {
                    W.Cast(minion);
                }
                if (laneclearR && R.IsReady() && (GetPassiveBuff >= 4 || myHero.HasBuff("ryzepassivecharged")))
                {
                    R.Cast();
                }
            }
        }

        private static void Harrash()
        {
            var HarrashMinMana = HarrashMenu["hMANA"].Cast<Slider>().CurrentValue;
            var target = TargetSelector.GetTarget(900, DamageType.Magical);
            if (miscMenu["LockTar"].Cast<CheckBox>().CurrentValue && selected != null && selected.IsVisible && selected.Position.Distance(ObjectManager.Player) <= 570) target = selected;
            var qSpell = HarrashMenu["HQ"].Cast<CheckBox>().CurrentValue;
            if (target != null && Player.Instance.ManaPercent > HarrashMinMana)
            {
                var qpred = Q.GetPrediction(target);
                if (qSpell)
                {
                    if (Q.GetPrediction(target).HitChance == HitChance.High)
                    {
                        Q.Cast(target);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            var qSpell = DrawMenu["DQ"].Cast<CheckBox>().CurrentValue;
            var wSpell = DrawMenu["DW"].Cast<CheckBox>().CurrentValue;
            var eSpell = DrawMenu["DE"].Cast<CheckBox>().CurrentValue;
            if (qSpell) Circle.Draw(Color.AliceBlue, Q.Range, Player.Instance.Position);
            if (wSpell) Circle.Draw(Color.AliceBlue, W.Range, Player.Instance.Position);
            if (eSpell) Circle.Draw(Color.DarkGray, E.Range, Player.Instance.Position);
            if (miscMenu["LockTar"].Cast<CheckBox>().CurrentValue &&selected != null && selected.IsVisible)
            {
                Circle.Draw(Color.Red, 100, selected.Position);
            }
        }
        public static void AABlock()
        {
            if (ComboMenu["BlockAA"].Cast<CheckBox>().CurrentValue)
            {
                Orbwalker.DisableAttacking = true;
            }
        }

        public static int GetPassiveBuff
        {
            get
            {
                var data = ObjectManager.Player.Buffs.FirstOrDefault(b => b.DisplayName == "RyzePassiveStack");
                if (data != null)
                {
                    return data.Count == -1 ? 0 : data.Count == 0 ? 1 : data.Count;
                }
                return 0;
            }
        }

        private static void Combo()
        {
           AABlock();
            var target = TargetSelector.GetTarget(570, DamageType.Magical);
            if (miscMenu["LockTar"].Cast<CheckBox>().CurrentValue && selected != null && selected.IsVisible && selected.Position.Distance(ObjectManager.Player) <= 570) target = selected;
            if (target != null)
            {
                var qpred = Q.GetPrediction(target);
               var q= qpred.CollisionObjects;
                var qSpell = ComboMenu["CQ"].Cast<CheckBox>().CurrentValue;
                var eSpell = ComboMenu["CE"].Cast<CheckBox>().CurrentValue;
                var wSpell = ComboMenu["CW"].Cast<CheckBox>().CurrentValue;
                var rSpell = ComboMenu["CR"].Cast<CheckBox>().CurrentValue;
                var rwwSpell = ComboMenu["CRo"].Cast<CheckBox>().CurrentValue;
                if (target.IsValidTarget(Q.Range))
                {
                    if (GetPassiveBuff <= 2 || !ObjectManager.Player.HasBuff("RyzePassiveStack"))
                    {
                        if (target.IsValidTarget(Q.Range) && qSpell && Q.IsReady()) Q.Cast(qpred.UnitPosition);

                        if (target.IsValidTarget(W.Range) && wSpell && W.IsReady()) W.Cast(target);

                        if (target.IsValidTarget(E.Range) && eSpell && E.IsReady()) E.Cast(target);

                        if (R.IsReady() && rSpell)
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (DamageCalc.QDamage(target) + DamageCalc.EDamage(target)))
                            {
                                if (rwwSpell && target.HasBuff("RyzeW")) R.Cast();
                                if (!rwwSpell) R.Cast();
                            }
                        }
                    }


                    if (GetPassiveBuff == 3)
                    {
                        if (Q.IsReady() && target.IsValidTarget(Q.Range)) Q.Cast(qpred.UnitPosition);

                        if (E.IsReady() && target.IsValidTarget(E.Range)) E.Cast(target);

                        if (W.IsReady() && target.IsValidTarget(W.Range)) W.Cast(target);

                        if (R.IsReady() && rSpell)
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (DamageCalc.QDamage(target) + DamageCalc.EDamage(target)))
                            {
                                if (rwwSpell && target.HasBuff("RyzeW")) R.Cast();
                                if (!rwwSpell) R.Cast();
                            }
                        }
                    }

                    if (GetPassiveBuff == 4)
                    {
                        if (target.IsValidTarget(W.Range) && wSpell && W.IsReady()) W.Cast(target);

                        if (target.IsValidTarget(Q.Range) && Q.IsReady() && qSpell) Q.Cast(qpred.UnitPosition);

                        if (target.IsValidTarget(E.Range) && E.IsReady() && eSpell) E.Cast(target);

                        if (R.IsReady() && rSpell)
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (DamageCalc.QDamage(target) + DamageCalc.EDamage(target)))
                            {
                                if (rwwSpell && target.HasBuff("RyzeW")) R.Cast();
                                if (!rwwSpell) R.Cast();
                            }
                        }
                    }

                    if (myHero.HasBuff("ryzepassivecharged"))
                    {
                        if (wSpell && W.IsReady() && target.IsValidTarget(W.Range)) W.Cast(target);

                        if (qSpell && Q.IsReady() && target.IsValidTarget(Q.Range)) Q.Cast(qpred.UnitPosition);

                        if (eSpell && E.IsReady() && target.IsValidTarget(E.Range)) E.Cast(target);

                        if (R.IsReady() && rSpell)
                        {
                            if (target.IsValidTarget(W.Range) && target.Health > (DamageCalc.QDamage(target)) + DamageCalc.EDamage(target))
                            {
                                if (rwwSpell && target.HasBuff("RyzeW")) R.Cast();
                                if (!rwwSpell) R.Cast();
                                if (!E.IsReady() && !Q.IsReady() && !W.IsReady()) R.Cast();
                            }
                        }
                    }
                }
                else
                {
                    if (wSpell && W.IsReady() && target.IsValidTarget(W.Range)) W.Cast(target);

                    if (qSpell && Q.IsReady() && target.IsValidTarget(Q.Range)) Q.Cast(qpred.UnitPosition);

                    if (eSpell && E.IsReady() && target.IsValidTarget(E.Range)) E.Cast(target);
                }
                if (!R.IsReady() || GetPassiveBuff != 4 || !rSpell) return;

                if (Q.IsReady() || W.IsReady() || E.IsReady()) return;

                R.Cast();

            }
        }
    }
}
