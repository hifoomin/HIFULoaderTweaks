using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFULoaderTweaks.Skills
{
    public class M551Pylon : TweakBase
    {
        public static float damage;
        public static float cooldown;
        public static float aoe;
        public static float fireRate;
        public static float procCoefficient;
        public static int bounces;
        public static float lifetime;

        public override string Name => ": Special : M551 Pylon";

        public override string SkillToken => "special";

        public override string DescText => "Throw a floating pylon that repeatedly <style=cIsDamage>zaps</style> up to <style=cIsDamage>3</style> nearby enemies for <style=cIsDamage>" + d(damage) + " damage</style>. Can be <style=cIsUtility>grappled</style>.";

        public override void Init()
        {
            damage = ConfigOption(0.5f, "Damage", "Decimal. Vanilla is 1");
            aoe = ConfigOption(35f, "Area of Effect", "Vanilla is 25");
            cooldown = ConfigOption(20f, "Cooldown", "Vanilla is 20");
            fireRate = ConfigOption(3f, "Fire Rate", "Vanilla is 1");
            procCoefficient = ConfigOption(0.4f, "Proc Coefficient", "Vanilla is 0.5");
            bounces = ConfigOption(1, "Bounce Count", "Vanilla is 0");
            lifetime = ConfigOption(6f, "Lifetime", "Vanilla is 15");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.Loader.ThrowPylon.OnEnter += ThrowPylon_OnEnter;
        }

        private void ThrowPylon_OnEnter(On.EntityStates.Loader.ThrowPylon.orig_OnEnter orig, EntityStates.Loader.ThrowPylon self)
        {
            EntityStates.Loader.ThrowPylon.damageCoefficient = damage;
            orig(self);
        }

        private void Changes()
        {
            var pylonDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/ThrowPylon.asset").WaitForCompletion();
            pylonDef.baseRechargeInterval = cooldown;

            var pylon = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderPylon.prefab").WaitForCompletion();
            var projectileProximityBeamController = pylon.GetComponent<ProjectileProximityBeamController>();
            projectileProximityBeamController.attackInterval = 1f / fireRate;
            projectileProximityBeamController.attackRange = aoe;
            projectileProximityBeamController.procCoefficient = procCoefficient;
            projectileProximityBeamController.bounces = bounces;

            var projectileSimple = pylon.GetComponent<ProjectileSimple>();
            projectileSimple.lifetime = lifetime;
        }
    }
}