using System;
using System.Linq;
using Ensage;
using SharpDX;
using Ensage.Common.Extensions;
using Ensage.Common;
using System.Windows.Input;
using SharpDX.Direct3D9;


namespace TemplarCombo
{
    class Program
    {
        private static bool _activated;
        private static Font _txt;
        private static Font _not;
        private const Key KeyCombo = Key.E;
        private const Key BkbToggleKey = Key.F;
        private static bool _bkbToggle;
        private static Hero _me;
        private static Hero _target;
        
        static void Main()
        {
            Game.OnUpdate += Game_OnUpdate;
            Game.OnWndProc += Game_OnWndProc;
            Console.WriteLine("> TemplarCombo have been injected and ready to use ! ");
            _txt = new Font(Drawing.Direct3DDevice9, new FontDescription
            {
                FaceName = "Tahoma",
                Height = 16,
                OutputPrecision = FontPrecision.Default,
                Quality = FontQuality.Default
            });
            _not = new Font(Drawing.Direct3DDevice9, new FontDescription
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
            _me = ObjectMgr.LocalHero;
            if (_me == null || _me.ClassID != ClassID.CDOTA_Unit_Hero_TemplarAssassin)
            {
                return;
            }
            if (!_activated) return;

            if(_target == null || _me.Distance2D(_target) > 500)
                _target = _me.ClosestToMouseTarget();

            var refraction = _me.Spellbook.SpellQ;
            var meld = _me.Spellbook.SpellW;
            var psionicTrap = _me.Spellbook.SpellR;

            var stick = _me.FindItem("item_magic_stick");
            var wand = _me.FindItem("item_magic_wand");
            var cheese = _me.FindItem("item_cheese");

            //var phase = _me.FindItem("item_phase_boots");
            //var manta = _me.FindItem("item_manta");
            var orchid = _me.FindItem("item_orchid");
            var satanic = _me.FindItem("item_satanic");
            var mjollnir = _me.FindItem("item_mjollnir");
            var blinkDagger = _me.FindItem("item_blink");
            var bkb = _me.FindItem("item_black_king_bar");
            var abyssal = _me.FindItem("item_abyssal_blade");
            var halberd = _me.FindItem("item_heavens_halberd");
            var diffusal = _me.FindItem("item_diffusal_blade");
            var solarcrest = _me.FindItem("item_solar_crest");
            var medal = _me.FindItem("item_medallion_of_courage");
            var maskofmadness = _me.FindItem("item_mask_of_madness");

            if (_target != null && _target.IsAlive && !_target.IsInvul())
            {
                if(!Utils.SleepCheck("TA_Combo_Wait")) return;
                var targetDistance = _me.Distance2D(_target);
                var attackRange = 190 + (60 * _me.Spellbook.Spell3.Level);
                if (targetDistance <= 1200 && targetDistance > attackRange && CanCast(_me, blinkDagger) && Utils.SleepCheck("BlinkDagger"))
                {
                    blinkDagger.UseAbility(_target.Position);
                    Utils.Sleep(150 + Game.Ping, "BlinkDagger");
                }
                if (!(targetDistance <= attackRange)) return;
                if (CanCast(_me, bkb) && _bkbToggle && Utils.SleepCheck("BKB")) {
                    bkb.UseAbility();
                    Utils.Sleep(150 + Game.Ping, "BKB");
                }
                if (CanCast(_me, mjollnir) && Utils.SleepCheck("mjollnir")) {
                    mjollnir.UseAbility(_me);
                    Utils.Sleep(150 + Game.Ping, "mjollnir");
                }
                if (CanCast(_me, abyssal) && Utils.SleepCheck("abyssal")) {
                    abyssal.UseAbility(_target);
                    Utils.Sleep(150 + Game.Ping, "abyssal");
                }
                if (CanCast(_me, solarcrest) && Utils.SleepCheck("solarcrest")) {
                    solarcrest.UseAbility(_target);
                    Utils.Sleep(150 + Game.Ping, "solarcrest");
                }
                if (CanCast(_me, medal) && Utils.SleepCheck("medal"))
                {
                    medal.UseAbility(_target);
                    Utils.Sleep(150 + Game.Ping, "medal");
                }
                if (CanCast(_me, diffusal) && !_target.IsMagicImmune() && _target.Modifiers.FirstOrDefault(modifier => modifier.Name == "modifier_item_diffusal_blade_slow") == null && Utils.SleepCheck("diffusal")) {
                    diffusal.UseAbility(_target);
                    Utils.Sleep(150 + Game.Ping, "diffusal");
                }
                if (CanCast(_me, orchid) && !_target.IsMagicImmune() && Utils.SleepCheck("orchid")) {
                    orchid.UseAbility(_target);
                    Utils.Sleep(150 + Game.Ping, "orchid");
                }
                if (CanCast(_me, halberd) && !_target.IsMagicImmune() && Utils.SleepCheck("halberd")) {
                    halberd.UseAbility(_target);
                    Utils.Sleep(150 + Game.Ping, "halberd");
                }
                if (CanCast(_me, maskofmadness) && Utils.SleepCheck("maskofmadness")) {
                    maskofmadness.UseAbility();
                    Utils.Sleep(150 + Game.Ping, "maskofmadness");
                }
                if (((decimal)_me.Health / _me.MaximumHealth <= (decimal)0.3) && Utils.SleepCheck("Stick/Wand/Cheese"))
                    if ((CanCast(_me, stick) || CanCast(_me, wand)) || CanCast(_me, cheese))
                    {
                        if (wand != null && wand.CurrentCharges > 0)
                            wand.UseAbility();
                        else if (stick != null && stick.CurrentCharges > 0)
                            stick.UseAbility();
                        if (cheese != null)
                            cheese.UseAbility();
                        Utils.Sleep(150 + Game.Ping, "Stick/Wand/Cheese");
                    }

                if (CanCast(_me, satanic) && ((double)_me.Health / _me.MaximumHealth <= 0.3))
                    satanic.UseAbility();

                if (CanCast(_me, refraction) && Utils.SleepCheck("Refraction")) {
                    refraction.UseAbility();
                    Utils.Sleep(150 + Game.Ping, "Refraction");
                }
                if (CanCast(_me, psionicTrap) && Utils.SleepCheck("PsionicTrap")) {
                    psionicTrap.UseAbility(_target.Position);
                    Utils.Sleep(150 + Game.Ping, "PsionicTrap");
                }
                var traps = ObjectMgr.GetEntities<Unit>().Where(unit => unit.Name == "npc_dota_templar_assassin_psionic_trap" && _target.Distance2D(unit) <= 370).ToList();
                if (!traps.Any()) return;
                var closestTrap = traps.First();
                if (closestTrap.CanCast() && Utils.SleepCheck("trap")) {
                    closestTrap.Spellbook.Spell1.UseAbility();
                    Utils.Sleep(150 + Game.Ping, "trap");
                }
                if (CanCast(_me, meld) && Utils.SleepCheck("Meld"))
                {
                    meld.UseAbility();
                    Utils.Sleep(150 + Game.Ping, "Meld");
                }
                _me.Attack(_target);
                Utils.Sleep(250, "TA_Combo_Wait");
            } else {
                _me.Move(Game.MousePosition);
            }
        }
        private static void Game_OnWndProc(WndEventArgs args)
        {
            if (Game.IsChatOpen) return;
            _activated = Game.IsKeyDown(KeyCombo);
            if (!Game.IsKeyDown(BkbToggleKey) || !Utils.SleepCheck("toggleBKB")) return;
            _bkbToggle = !_bkbToggle;
            Utils.Sleep(250, "toggleBKB");
        }
        static void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            _txt.Dispose();
            _not.Dispose();
        }
        static void Drawing_OnEndScene(EventArgs args)
        {
            if (Drawing.Direct3DDevice9 == null || Drawing.Direct3DDevice9.IsDisposed || !Game.IsInGame) return;
            if (_me == null || _me.Player.Team == Team.Observer || _me.ClassID != ClassID.CDOTA_Unit_Hero_TemplarAssassin) return;
            if (_activated)
            {
                _txt.DrawText(null, "TemplarCombo is COMBOING now!\n", 4, 150, Color.Green);
            }
            if (!_activated && !_bkbToggle)
            {
                _txt.DrawText(null, "TemplarCombo: Use  [" + KeyCombo + "] for start comboing. BKB Disabled. Use " + BkbToggleKey + " to turn it on!", 4, 150, Color.Red);
            }
            if (!_activated && _bkbToggle)
            {
                _txt.DrawText(null, "TemplarCombo: Use  [" + KeyCombo + "] for start comboing. BKB Enabled. Use " + BkbToggleKey + " to turn it off!", 4, 150, Color.Red);
            }
        }
        static void Drawing_OnPostReset(EventArgs args)
        {
            _txt.OnResetDevice();
            _not.OnResetDevice();
        }
        static void Drawing_OnPreReset(EventArgs args)
        {
            _txt.OnLostDevice();
            _not.OnLostDevice();
        }

        static bool CanCast(Unit hero, Ability ability)
        {
            return ability != null && ability.CanBeCasted() && hero.CanCast() && ability.Cooldown.Equals(0);
        }
    }
}