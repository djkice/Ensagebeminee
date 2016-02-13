using System;
using System.Collections.Generic;
using System.Linq;
using Ensage;
using Ensage.Common;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;

namespace Panic_
{
    internal class Program
    {
        private static Item bkb, ghost, ethereal, blink, force, tp, bot;
        private static Hero me;
        private static Unit fountain;
        private static bool panic;
        private static bool noitem;
        private static readonly Menu Menu = new Menu("Panic!", "panic", true, "", true);
        private static AbilityToggler menuValue;
        private static bool menuvalueSet;

        private static void Main()
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += No_Item;
            Game.OnWndProc += Game_OnWndProc;
            Console.WriteLine("Panic! LOADED!");
            var options = new Menu("Options", "opt");
            Menu.AddSubMenu(options);
            options.AddItem(new MenuItem("noitem", "Only TP Key").SetValue(new KeyBind(32, KeyBindType.Press)));
            options.AddItem(new MenuItem("panic", "Panic Key").SetValue(new KeyBind('E', KeyBindType.Press)));
            Menu.AddToMainMenu();
            var dict = new Dictionary<string, bool>
            {
                {"item_black_king_bar", true},
                {"item_ghost", false},
                {"item_ethereal_blade", false},
                {"item_blink", true},
                {"item_force_staff", true},
                {"item_tpscroll", true},
                {"item_travel_boots", true},

            };
            Menu.AddItem(new MenuItem("Items", "Items:").SetValue(new AbilityToggler(dict)));

        }

        public static void Game_OnUpdate(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                return;

            me = ObjectMgr.LocalHero;
            if (me == null) return;

            if (bkb == null)
                bkb = me.FindItem("item_black_king_bar");

            if (ghost == null)
                ghost = me.FindItem("item_ghost");

            if (ethereal == null)
                ethereal = me.FindItem("item_ethereal_blade");

            if (blink == null)
                ethereal = me.FindItem("item_blink");

            if (force == null)
                ethereal = me.FindItem("item_force_staff");

            if (tp == null)
                tp = me.FindItem("item_tpscroll");

            if (bot == null)
                bot = me.FindItem("item_travel_boots");

            if (!menuvalueSet)
            {
                menuValue = Menu.Item("Items").GetValue<AbilityToggler>();
                menuvalueSet = true;
            }

            if (panic)

            {
                if (fountain == null || !fountain.IsValid)
                {
                    fountain = ObjectMgr.GetEntities<Unit>()
                        .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain);
                }

                if (bkb != null && bkb.IsValid && bkb.CanBeCasted() && Utils.SleepCheck("bkb") &&
                    menuValue.IsEnabled(bkb.Name))

                {
                    bkb.UseAbility();
                    Utils.Sleep(150 + Game.Ping, "bkb");
                }

                if (ghost != null && ghost.IsValid && ghost.CanBeCasted() && Utils.SleepCheck("ghost") &&
                    menuValue.IsEnabled(ghost.Name))

                {
                    ghost.UseAbility();
                    Utils.Sleep(150 + Game.Ping, "ghost");
                }

                if (ethereal != null && ethereal.IsValid && ethereal.CanBeCasted() && Utils.SleepCheck("ethereal") &&
                    menuValue.IsEnabled(ethereal.Name))

                {
                    ethereal.UseAbility(me);
                    Utils.Sleep(150 + Game.Ping, "ethereal");
                }

                if (blink != null && blink.IsValid && blink.CanBeCasted() && Utils.SleepCheck("blink") &&
                    menuValue.IsEnabled(blink.Name))
                {
                    blink.UseAbility(me.NetworkPosition);
                    Utils.Sleep(150 + Game.Ping, "blink");
                }

                if (force != null && force.IsValid && force.CanBeCasted() && Utils.SleepCheck("force") &&
                    menuValue.IsEnabled(force.Name))
                {
                    force.UseAbility(me);
                    Utils.Sleep(150 + Game.Ping, "force");
                }

                if (bot != null && bot.IsValid && bot.CanBeCasted() && Utils.SleepCheck("bot") &&
                    menuValue.IsEnabled(bot.Name))

                {
                    bot.UseAbility(fountain);
                    Utils.Sleep(150 + Game.Ping, "bot");
                }

                else if (tp != null && tp.IsValid && bot.CanBeCasted() && Utils.SleepCheck("tp") &&
                         menuValue.IsEnabled(tp.Name))
                {
                    tp.UseAbility(fountain);
                    Utils.Sleep(150 + Game.Ping, "tp");
                }
            }
        }

            public static void No_Item (EventArgs args)
            {
                if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                {
                    return;
                }

                me = ObjectMgr.LocalHero;

                if (noitem)
                {

                    if (fountain == null || !fountain.IsValid)
                    {
                        fountain = ObjectMgr.GetEntities<Unit>()
                            .FirstOrDefault(x => x.Team == me.Team && x.ClassID == ClassID.CDOTA_Unit_Fountain);
                    }

                    if (bot != null && bot.IsValid && bot.CanBeCasted() && Utils.SleepCheck("bot") &&
                        menuValue.IsEnabled(bot.Name))

                    {
                        bot.UseAbility(fountain);
                        Utils.Sleep(150 + Game.Ping, "bot");
                    }

                    else if (tp != null && tp.IsValid && bot.CanBeCasted() && Utils.SleepCheck("tp") &&
                             menuValue.IsEnabled(tp.Name))
                    {
                        tp.UseAbility(fountain);
                        Utils.Sleep(150 + Game.Ping, "tp");
                    }

                }
            }

            private static void Game_OnWndProc(WndEventArgs args)
            {
                if (!Game.IsChatOpen)
                {

                    if (Menu.Item("panic").GetValue<KeyBind>().Active)
                    {
                        panic = true;
                    }
                    else
                    {
                        panic = false;
                    }

                    if (Menu.Item("noitem").GetValue<KeyBind>().Active)
                    {
                        noitem = true;
                    }
                    else
                    {
                        noitem = false;
                    }

                }
            }
        }
    }

        
    

