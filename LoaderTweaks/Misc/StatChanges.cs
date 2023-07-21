using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFULoaderTweaks.Misc
{
    public class StatChanges : MiscBase
    {
        public static float baseMaxHealth;
        public static float armor;
        public override string Name => ":: Misc :: Base Stats";

        public override void Init()
        {
            baseMaxHealth = ConfigOption(110f, "Base Max Health", "Vanilla is 160");
            armor = ConfigOption(0f, "Base Armor", "Vanilla is 20");
            base.Init();
        }

        public override void Hooks()
        {
            Changes();
        }

        private void Changes()
        {
            var loader = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderBody.prefab").WaitForCompletion();
            var loaderBody = loader.GetComponent<CharacterBody>();
            loaderBody.baseMaxHealth = baseMaxHealth;
            loaderBody.levelMaxHealth = baseMaxHealth * 0.3f;
            loaderBody.baseArmor = armor;
        }
    }
}