using RoR2.Skills;
using UnityEngine.AddressableAssets;

namespace HIFULoaderTweaks.Skills
{
    public class ThunderGauntlet : TweakBase
    {
        public static float damage;
        public static float chargeRate;
        public static float speedCoeff;
        public static float cooldown;
        public static bool formula;

        public override string Name => ": Utility :: Thunder Gauntlet";

        public override string SkillToken => "utility_alt1";

        public override string DescText => "<style=cIsUtility>Heavy</style>. Charge up a <style=cIsUtility>single-target</style> punch for <style=cIsDamage>" + d(damage) + " damage</style> that <style=cIsDamage>shocks</style> enemies in a cone for <style=cIsDamage>900% damage</style>.";

        public override void Init()
        {
            damage = ConfigOption(14f, "Damage", "Decimal. Vanilla is ???");
            chargeRate = ConfigOption(0.4f, "Charge Duration", "Vanilla is 0.4");
            speedCoeff = ConfigOption(0.13f, "Speed Coefficient", "Decimal. Vanilla is 0.3");
            cooldown = ConfigOption(5f, "Cooldown", "Vanilla is 5");
            formula = ConfigOption(true, "Damage Formula", "Damage + SpeedCoeff * (Current velocity - 70)");
            base.Init();
        }

        public override void Hooks()
        {
            On.EntityStates.Loader.BaseChargeFist.OnEnter += BaseChargeFist_OnEnter;
            On.EntityStates.Loader.BaseSwingChargedFist.OnEnter += BaseSwingChargedFist_OnEnter;
            Changes();
        }

        private void BaseSwingChargedFist_OnEnter(On.EntityStates.Loader.BaseSwingChargedFist.orig_OnEnter orig, EntityStates.Loader.BaseSwingChargedFist self)
        {
            if (self is EntityStates.Loader.SwingZapFist)
            {
                EntityStates.Loader.BaseSwingChargedFist.velocityDamageCoefficient = speedCoeff;
                self.damageCoefficient = damage;
                orig(self);
                // self.bonusDamage = EntityStates.Loader.BaseSwingChargedFist.velocityDamageCoefficient * self.damageStat * Mathf.Lerp(self.minLungeSpeed, self.maxLungeSpeed, self.charge);
            }
            else
            {
                orig(self);
            }
        }

        private void BaseChargeFist_OnEnter(On.EntityStates.Loader.BaseChargeFist.orig_OnEnter orig, EntityStates.Loader.BaseChargeFist self)
        {
            if (self is EntityStates.Loader.ChargeZapFist)
            {
                self.baseChargeDuration = chargeRate;
            }
            orig(self);
        }

        private void Changes()
        {
            var thunder = Addressables.LoadAssetAsync<SteppedSkillDef>("RoR2/Base/Loader/ChargeZapFist.asset").WaitForCompletion();
            thunder.baseRechargeInterval = cooldown;
        }
    }
}