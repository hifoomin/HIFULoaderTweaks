using RoR2;
using UnityEngine;

namespace HIFULoaderTweaks.Skills
{
    public class Knuckleboom : TweakBase
    {
        public static float damage;
        public static float fireRate;
        public static float burstDamage;
        public static float burstAoE;
        public static float barrierGain;

        public override string Name => ": Primary : Knuckleboom";

        public override string SkillToken => "primary";

        public override string DescText => "Swing at nearby enemies for <style=cIsDamage>" + d(damage) + " damage</style>. Every third hit knocks up for an additional <style=cIsDamage>" + d(burstDamage) + " damage</style> in a large radius.";

        public override void Init()
        {
            damage = ConfigOption(1.7f, "Damage", "Decimal. Vanilla is 3.2");
            burstDamage = ConfigOption(2.3f, "Third Hit Damage", "Decimal. Vanilla is 0");
            burstAoE = ConfigOption(12f, "Third Hit AoE", "Vanilla is 0");
            fireRate = ConfigOption(2.5f, "Fire Rate", "Vanilla is 1.66666667");
            barrierGain = ConfigOption(0.02f, "Barrier Gain per Hit", "Decimal. Vanilla is 0.05");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.BasicMeleeAttack.OnEnter += BasicMeleeAttack_OnEnter;
            On.EntityStates.BasicMeleeAttack.OnMeleeHitAuthority += BasicMeleeAttack_OnMeleeHitAuthority;
            On.EntityStates.Loader.LoaderMeleeAttack.OnMeleeHitAuthority += LoaderMeleeAttack_OnMeleeHitAuthority;
        }

        private void LoaderMeleeAttack_OnMeleeHitAuthority(On.EntityStates.Loader.LoaderMeleeAttack.orig_OnMeleeHitAuthority orig, EntityStates.Loader.LoaderMeleeAttack self)
        {
            EntityStates.Loader.LoaderMeleeAttack.barrierPercentagePerHit = barrierGain;
            orig(self);
        }

        private void BasicMeleeAttack_OnMeleeHitAuthority(On.EntityStates.BasicMeleeAttack.orig_OnMeleeHitAuthority orig, EntityStates.BasicMeleeAttack self)
        {
            if (self is EntityStates.Loader.LoaderMeleeAttack)
            {
                self.characterBody.gameObject.GetComponent<KnuckleboomButBased>().HitCount++;
            }
            orig(self);
        }

        private void BasicMeleeAttack_OnEnter(On.EntityStates.BasicMeleeAttack.orig_OnEnter orig, EntityStates.BasicMeleeAttack self)
        {
            if (self is EntityStates.Loader.LoaderMeleeAttack)
            {
                self.baseDuration = 1f / fireRate;
                self.damageCoefficient = damage;
                self.pushAwayForce = 50f;
                if (self.characterBody.gameObject.GetComponent<KnuckleboomButBased>() == null)
                {
                    self.characterBody.gameObject.AddComponent<KnuckleboomButBased>();
                }
            }
            orig(self);
        }
    }

    public class KnuckleboomButBased : MonoBehaviour
    {
        public int HitCount;
        public CharacterBody body;

        public void Start()
        {
            body = GetComponent<CharacterBody>();
        }

        public void FixedUpdate()
        {
            if (HitCount >= 3)
            {
                EffectManager.SpawnEffect(EntityStates.Loader.GroundSlam.blastEffectPrefab, new EffectData
                {
                    origin = body.footPosition,
                    scale = Knuckleboom.burstAoE
                }, true);
                if (Util.HasEffectiveAuthority(gameObject))
                {
                    new BlastAttack
                    {
                        attacker = body.gameObject,
                        baseDamage = body.damage * Knuckleboom.burstDamage,
                        baseForce = 50f,
                        bonusForce = new Vector3(0f, 1500f, 0f),
                        crit = body.RollCrit(),
                        damageType = DamageType.Generic,
                        procCoefficient = 1f,
                        radius = Knuckleboom.burstAoE,
                        position = body.footPosition,
                        attackerFiltering = AttackerFiltering.NeverHitSelf,
                        teamIndex = body.teamComponent.teamIndex,
                        falloffModel = BlastAttack.FalloffModel.None,
                        impactEffect = EffectCatalog.FindEffectIndexFromPrefab(EntityStates.Loader.GroundSlam.blastImpactEffectPrefab)
                    }.Fire();
                }

                HitCount = 0;
            }
        }
    }
}