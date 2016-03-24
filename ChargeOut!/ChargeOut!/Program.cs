using System;
using System.Linq;


using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common.Menu;


namespace ChargeOut_
{
    internal class Program
    {
        private static Ability charge;
        private static Hero me;
        private static Creep target;

        private static readonly Menu Menu = new Menu("ChargeOut!", "chargeout", true, "npc_dota_hero_spiritbreaker",
            true);

        private static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Menu.AddItem(new MenuItem("Charge", "charge").SetValue(new KeyBind('E', KeyBindType.Press)));
            Menu.AddToMainMenu();
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            if (!Game.IsInGame || Game.IsPaused || Game.IsWatchingGame)
                return;

            me = ObjectMgr.LocalHero;
            if (me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_SpiritBreaker) return;

            if (charge == null)
                charge = me.Spellbook.SpellQ;

            if (Menu.Item("charge").GetValue<KeyBind>().Active)

            {

                var target =
                ObjectMgr.GetEntities<Creep>()
                    .Where(
                        creep =>
                            (creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Lane ||
                             creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Siege ||
                             creep.ClassID == ClassID.CDOTA_BaseNPC_Creep_Neutral ||
                             creep.ClassID == ClassID.CDOTA_BaseNPC_Invoker_Forged_Spirit ||
                             creep.ClassID == ClassID.CDOTA_BaseNPC_Creep) &&
                             creep.IsAlive && creep.IsVisible && creep.IsSpawned &&
                             creep.Team != me.Team && creep.Position.Distance2D(me.Position) > 2000
                             ).ToList();
                if (!target.Any()) return;
                var furthesttarget = target.Last();

                if (charge != null && charge.CanBeCasted() && me.Mana > charge.ManaCost)
                {
                    charge.UseAbility(furthesttarget);
                }

            }
        }
    }
}
