using System;
using System.Linq;
using System.Windows.Input;
using Ensage;
using Ensage.Common.Extensions;
using Ensage.Common;
using Ensage.Heroes;
using SharpDX;
using SharpDX.Direct3D9;

namespace MeepoSharp
{
    internal class Program
    {
        private static Hero me;
        private static Hero target;
        private static bool activated;
        private static Font txt;
        private static Font not;
        private static Key KeyCombo = Key.E;

        private static void Main(string[] args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Console.WriteLine("Meepo combo loaded!");

            txt = new Font(
                Drawing.Direct3DDevice9,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 16,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });

            not = new Font(
                Drawing.Direct3DDevice9,
                new FontDescription
                {
                    FaceName = "Tahoma",
                    Height = 24,
                    OutputPrecision = FontPrecision.Default,
                    Quality = FontQuality.Default
                });

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnEndScene += Drawing_OnEndScene;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
        }

        public static void Game_OnUpdate(EventArgs args)
        {
            if (!Game.IsInGame) return;
            me = ObjectMgr.LocalHero;
            if (me == null || me.ClassID != ClassID.CDOTA_Unit_Hero_Meepo)
            {
                return;
            }
            if (!activated) return;

            if (target == null || me.Distance2D(target) > 500)
                target = me.ClosestToMouseTarget();

            var net = me.Spellbook.SpellQ;
            //var poof = me.Spellbook.SpellW;
            var meepos = ObjectMgr.GetEntities<Meepo>().Where(meepo => meepo.Team == me.Team && !meepo.Equals(me)).ToList(); 
            var hex = me.FindItem("item_sheepstick");
            var blinkDagger = me.FindItem("item_blink");

            if (target != null && target.IsAlive && !target.IsInvul())
            {
                var targetDistance = me.Distance2D(target);

                if (targetDistance <= 1250 && targetDistance > 550 && CanCast(me, blinkDagger) &&
                    Utils.SleepCheck("BlinkDagger"))
                {

                    blinkDagger.UseAbility(target.Position);
                    Utils.Sleep(150 + Game.Ping, "BlinkDagger");
                }

                if (targetDistance < 550)
                {
                    if (!Utils.SleepCheck("Meepo_Combo")) return;
                    if (Utils.SleepCheck("Anything"))
                    {
                        foreach (var meepo in meepos)
                        {
                            var poof = meepo.Spellbook.SpellW;
                            if (CanCast(meepo, poof) && me.Distance2D(target) <= 400 && !target.IsMagicImmune())
                            {
                                poof.UseAbility(me);
                            }

                            var eb = meepo.Spellbook.SpellQ;
                            if (Utils.SleepCheck("Meepos_net"))
                                if (!target.Modifiers.ToList().Exists(x => x.Name == "modifier_meepo_earthbind") && !target.IsMagicImmune())
                                {
                                    if (eb.CastSkillShot(target) && CanCast(meepo, eb) && me.Distance2D(target) <= 500 &&
                                        !target.IsMagicImmune() && !meepo.IsChanneling())

                                    {
                                        eb.CastSkillShot(target);
                                        Utils.Sleep(750 + Game.Ping, "Meepos_net");
                                    }
                                }
                        }
                        Utils.Sleep(150, "Anything");
                    }

                    if (CanCast(me, hex) && !target.IsMagicImmune() && Utils.SleepCheck("hex"))
                    {
                        hex.UseAbility(target);
                        Utils.Sleep(150 + Game.Ping, "hex");
                    }


                    if (net.CastSkillShot(target) && CanCast(me, net) && me.Distance2D(target) <= 550 &&
                        !target.IsMagicImmune() &&
                        Utils.SleepCheck("Net"))
                    {
                        net.CastSkillShot(target);
                        Utils.Sleep(300 + Game.Ping, "Net");
                    }
                    me.Attack(target);
                    if (me.Distance2D(target) <= 400)
                    {
                        foreach (var meepo in meepos)
                            meepo.Attack(target);
                    }
                    Utils.Sleep(250, "Meepo_Combo");

                }
                else
                {
                    me.Move(Game.MousePosition);
                }

            }
            else
            {
                me.Move(Game.MousePosition);
            }
        }

        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (!Game.IsChatOpen)
            {
                if (Game.IsKeyDown(KeyCombo))
                {
                    activated = true;
                }
                else
                {
                    activated = false;
                }
            }
        }
         static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            txt.Dispose();
            not.Dispose();
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame)
                return;

            var player = ObjectMgr.LocalPlayer;
            if (player == null || player.Team == Team.Observer || me == null)
                return;

            if (me.ClassID != ClassID.CDOTA_Unit_Hero_Meepo)
                return;

            if (activated)
            {
                txt.DrawText(null, "MeepoSharp COMBOING now!\n", 4, 150, Color.Green);
            }

            if (!activated)
            {
                txt.DrawText(null, "MeepoSharp: Use  [" + KeyCombo + "] for start comboing!", 4, 150, Color.Red);
            }
        }

        static void Drawing_OnPostReset(EventArgs args)
        {
            txt.OnResetDevice();
            not.OnResetDevice();
        }

        static void Drawing_OnPreReset(EventArgs args)
        {
            txt.OnLostDevice();
            not.OnLostDevice();
        }

        static bool CanCast(Unit unit, Ability ability)
        {
            return ability != null && ability.CanBeCasted() && unit.CanCast() && ability.Cooldown.Equals(0);
        }
    }
}