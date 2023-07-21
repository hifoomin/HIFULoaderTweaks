using RoR2.Skills;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFULoaderTweaks.Skills
{
    public class SpikedFist : TweakBase
    {
        public static float massLimit;
        public static float cooldown;

        public override string Name => ": Secondary :: Spiked Fist";

        public override string SkillToken => "yankhook";

        public override string DescText => "<style=cIsDamage>Stunning</style>. Fire your gauntlet forward, dealing <style=cIsDamage>320% damage</style>. <style=cIsUtility>Pulls</style> you to heavy targets. Light targets are <style=cIsUtility>pulled to YOU</style> instead.";

        public override void Init()
        {
            massLimit = ConfigOption(500f, "Mass Limit", "Vanilla is 250");
            cooldown = ConfigOption(5f, "Cooldown", "Vanilla is 5");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var spikedFist = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderYankHook.prefab").WaitForCompletion();
            var projectileGrappleController = spikedFist.GetComponent<ProjectileGrappleController>();
            projectileGrappleController.yankMassLimit = massLimit;

            var spiked = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/FireYankHook.asset").WaitForCompletion();
            spiked.baseRechargeInterval = cooldown;
        }
    }
}