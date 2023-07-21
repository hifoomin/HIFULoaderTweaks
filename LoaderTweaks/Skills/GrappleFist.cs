using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HIFULoaderTweaks.Skills
{
    public class GrappleFist : TweakBase
    {
        public static float cooldown;

        public override string Name => ": Secondary : Grapple Fist";

        public override string SkillToken => "secondary";

        public override string DescText => "Fire your gauntlet forward, <style=cIsUtility>pulling</style> you to the target.";

        public override void Init()
        {
            cooldown = ConfigOption(5f, "Cooldown", "Vanilla is 5");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var grapple = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Loader/FireHook.asset").WaitForCompletion();
            grapple.baseRechargeInterval = cooldown;
        }
    }
}