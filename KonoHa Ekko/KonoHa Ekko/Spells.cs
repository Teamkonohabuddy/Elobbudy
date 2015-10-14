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

    internal class Spells
    {
        public Spell.Skillshot Q { get; private set; }

        public Spell.Skillshot W { get; private set; }

        public Spell.Skillshot E { get; private set; }

        public Spell.Active R { get; private set; }
        public Spell.Skillshot R1 { get; private set; }

        public Spells()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 250, 1650, 60);
            W = new Spell.Skillshot(SpellSlot.W, 1600, SkillShotType.Circular, 3750, null, 375);
            E = new Spell.Skillshot(SpellSlot.E, 350, SkillShotType.Linear, 250, null, 60);
            R = new Spell.Active(SpellSlot.R, 1600);
            R1 = new Spell.Skillshot(SpellSlot.R, 375, SkillShotType.Circular, (int).1f, int.MaxValue, 375);
        }

        public Obj_AI_Base Ghost
        {
            get
            {
                return
                    ObjectManager.Get<Obj_AI_Base>()
                        .FirstOrDefault(ghost => !ghost.IsEnemy && ghost.Name.Contains("Ekko"));
            }
        }

        public  double TotalDam(Obj_AI_Base target)
        {
            double dmg=0;
             dmg += TotalQDam(target);
             dmg += EDam(target);
             dmg += PassiveDam(target);
             dmg += RDam(target);
            return dmg;
        }
        public double TotalQDam(Obj_AI_Base target)
        {

            return QDam(target) + Q2Dam(target);
        }

        public double QDam(Obj_AI_Base target)
        {
            if (!Q.IsReady()) return 0;

            return Player.Instance.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                (float)(new double[] { 60, 75, 90, 105, 120 }[Q.Level - 1] + Player.Instance.TotalMagicalDamage * .2f));
        }

        public double PassiveDam(Obj_AI_Base target)
        {

            return Player.Instance.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                15 + (12 * Player.Instance.Level) + Player.Instance.TotalMagicalDamage * .7f);
        }

        public double Q2Dam(Obj_AI_Base target)
        {
            if (!Q.IsReady()) return 0;

            return Player.Instance.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                (float)(new double[] { 60, 85, 110, 135, 160 }[Q.Level - 1] + Player.Instance.TotalMagicalDamage * .6f));
        }

        public double EDam(Obj_AI_Base target)
        {
            if (!E.IsReady()) return 0f;

            return Player.Instance.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                (float)(new double[] { 50, 80, 110, 140, 170 }[E.Level - 1] + Player.Instance.TotalMagicalDamage * .2f));
        }


        public double RDam(Obj_AI_Base target)
        {
            if (!R.IsReady()) return 0f;

            return Player.Instance.CalculateDamageOnUnit(
                target,
                DamageType.Magical,
                (float)(new double[] { 200, 350, 500 }[R.Level - 1] + Player.Instance.TotalMagicalDamage * 1.3f));
        }
    }
}
