using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFULoaderTweaks.Skills
{
    public class ChargedGauntlet : TweakBase
    {
        public static float minDamage;
        public static float maxDamage;
        public static float chargeRate;
        public static float speedCoeff;
        public static float cooldown;
        public static bool formula;

        public override string Name => ": Utility : Charged Gauntlet";

        public override string SkillToken => "utility";

        public override string DescText => "<style=cIsUtility>Heavy</style>. Charge up a <style=cIsUtility>piercing</style> punch for <style=cIsDamage>" + d(minDamage) + "-" + d(maxDamage) + " damage</style>.";

        public override void Init()
        {
            minDamage = ConfigOption(5f, "Minimum Damage", "Decimal. Vanilla is ???");
            maxDamage = ConfigOption(18f, "Maximum Damage", "Decimal. Vanilla is ???");
            chargeRate = ConfigOption(2f, "Charge Duration", "Vanilla is 2.5");
            speedCoeff = ConfigOption(0.45f, "Speed Coefficient", "Decimal. Vanilla is 0.3");
            cooldown = ConfigOption(5f, "Cooldown", "Vanilla is 5");
            formula = ConfigOption(true, "Damage Formula", "Damage based on total charge, MinDamage and MaxDamage + SpeedCoeff * (Current velocity - 20~90, depending on total charge)");
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
            if (self is EntityStates.Loader.SwingChargedFist)
            {
                EntityStates.Loader.BaseSwingChargedFist.velocityDamageCoefficient = speedCoeff;
                orig(self);
                self.bonusDamage -= EntityStates.Loader.BaseSwingChargedFist.velocityDamageCoefficient * self.damageStat * Mathf.Lerp(self.minLungeSpeed, self.maxLungeSpeed, self.charge); ;
                self.damageCoefficient = Mathf.Lerp(minDamage, maxDamage, self.charge);
            }
            else
            {
                orig(self);
            }
        }

        private void BaseChargeFist_OnEnter(On.EntityStates.Loader.BaseChargeFist.orig_OnEnter orig, EntityStates.Loader.BaseChargeFist self)
        {
            if (self is EntityStates.Loader.ChargeFist)
            {
                self.baseChargeDuration = chargeRate;
            }
            orig(self);
        }

        private void Changes()
        {
            var charged = Addressables.LoadAssetAsync<SteppedSkillDef>("RoR2/Base/Loader/ChargeFist.asset").WaitForCompletion();
            charged.baseRechargeInterval = cooldown;
        }
    }
}