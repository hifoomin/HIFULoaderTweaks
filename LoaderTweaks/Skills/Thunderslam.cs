using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace HIFULoaderTweaks.Skills
{
    public class Thunderslam : TweakBase
    {
        public static float damage;
        public static float cooldown;
        public static float aoe;
        public static float yVelocityCoeff;
        public static bool formula;
        public static BuffDef fallDamageReduction;

        public override string Name => ": Special :: Thunderslam";

        public override string SkillToken => "special_alt";

        public override string DescText => "<style=cIsUtility>Very Heavy</style>. <style=cIsDamage>Stunning</style>. Slam your fists down, dealing <style=cIsDamage>" + d(damage) + "</style> damage on impact.";

        public override void Init()
        {
            damage = ConfigOption(4f, "Damage", "Decimal. Vanilla is 20");
            aoe = ConfigOption(13f, "Area of Effect", "Vanilla is 11");
            cooldown = ConfigOption(9f, "Cooldown", "Vanilla is 8");
            yVelocityCoeff = ConfigOption(0.23f, "Y Axis Speed Coefficient", "Vanilla is 0");
            formula = ConfigOption(true, "Damage Formula", "if Damage + Y Velocity * -1 * YVelocityCoeff >=\nDamage + YVelocityCoeff * 25,\nset the Damage to that");

            fallDamageReduction = ScriptableObject.CreateInstance<BuffDef>();
            fallDamageReduction.isHidden = true;
            fallDamageReduction.isDebuff = false;
            fallDamageReduction.canStack = false;
            fallDamageReduction.isCooldown = false;

            ContentAddition.AddBuffDef(fallDamageReduction);

            base.Init();
        }

        public override void Hooks()
        {
            Changes();
            On.EntityStates.Loader.GroundSlam.OnEnter += GroundSlam_OnEnter;
            On.EntityStates.Loader.GroundSlam.FixedUpdate += GroundSlam_FixedUpdate;
            On.EntityStates.Loader.GroundSlam.OnExit += GroundSlam_OnExit;
            On.RoR2.GlobalEventManager.OnCharacterHitGroundServer += GlobalEventManager_OnCharacterHitGroundServer;
        }

        private void GlobalEventManager_OnCharacterHitGroundServer(On.RoR2.GlobalEventManager.orig_OnCharacterHitGroundServer orig, GlobalEventManager self, CharacterBody characterBody, Vector3 impactVelocity)
        {
            if (characterBody.HasBuff(fallDamageReduction))
            {
                impactVelocity *= 0.75f;
            }
            orig(self, characterBody, impactVelocity);
        }

        private void GroundSlam_OnExit(On.EntityStates.Loader.GroundSlam.orig_OnExit orig, EntityStates.Loader.GroundSlam self)
        {
            EntityStates.Loader.GroundSlam.blastDamageCoefficient = damage;
            orig(self);
            if (self.characterBody)
                self.characterBody.RemoveBuff(fallDamageReduction);
        }

        private void GroundSlam_FixedUpdate(On.EntityStates.Loader.GroundSlam.orig_FixedUpdate orig, EntityStates.Loader.GroundSlam self)
        {
            if (self.isAuthority && self.characterMotor)
            {
                float damageinc = damage + self.characterMotor.velocity.y * -1f * yVelocityCoeff;
                if (damageinc >= damage + yVelocityCoeff * 25f)
                {
                    EntityStates.Loader.GroundSlam.blastDamageCoefficient = damageinc;
                }
                orig(self);
            }
        }

        private void GroundSlam_OnEnter(On.EntityStates.Loader.GroundSlam.orig_OnEnter orig, EntityStates.Loader.GroundSlam self)
        {
            EntityStates.Loader.GroundSlam.blastRadius = aoe;
            if (self.characterBody)
                self.characterBody.AddBuff(fallDamageReduction);
            orig(self);
        }

        private void Changes()
        {
            var slam = Addressables.LoadAssetAsync<SteppedSkillDef>("RoR2/Base/Loader/GroundSlam.asset").WaitForCompletion();
            slam.baseRechargeInterval = cooldown;

            LanguageAPI.Add("KEYWORD_VERY_HEAVY", "<style=cKeywordName>Very Heavy</style><style=cSub>The skill deals much more damage the faster you are falling. <color=#FF7F7F>Watch out for backblast</color>.</style>");

            string[] keywords = new string[1];
            keywords[0] = "KEYWORD_VERY_HEAVY";
            var specialFamily = Addressables.LoadAssetAsync<SkillFamily>("RoR2/Base/Loader/LoaderBodySpecialFamily.asset").WaitForCompletion();
            specialFamily.variants[1].skillDef.keywordTokens = keywords;
        }
    }
}