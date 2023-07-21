using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFULoaderTweaks.Misc
{
    public class FallDamageReduction : MiscBase
    {
        public static float fallDamageReduction;
        public override string Name => ":: Misc : Scrap Barrier";

        public override void Init()
        {
            fallDamageReduction = ConfigOption(0.7f, "Fall Damage Reduction", "Decimal. Vanilla is 1");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        public static void Changes()
        {
            On.RoR2.HealthComponent.TakeDamage += (On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo) =>
            {
                if (damageInfo != null && damageInfo.damageType.HasFlag(DamageType.FallDamage))
                {
                    if (self.body && self.body.name == "LoaderBody(Clone)")
                    {
                        damageInfo.damage *= 1f - fallDamageReduction;
                    }
                }
                orig(self, damageInfo);
            };
            LanguageAPI.Add("LOADER_PASSIVE_DESCRIPTION", "The Loader has her fall damage <style=cIsUtility>reduced by " + (fallDamageReduction * 100f) + "%</style>. Striking enemies with the Loader's gauntlets grants a <style=cIsHealing>temporary barrier</style>.");
            var lodr = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderBody.prefab").WaitForCompletion();
            lodr.GetComponent<CharacterBody>().bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
        }
    }
}