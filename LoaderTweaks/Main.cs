using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using R2API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HIFULoaderTweaks
{
    [BepInDependency(LanguageAPI.PluginGUID)]
    [BepInDependency(R2API.ContentManagement.R2APIContentManager.PluginGUID)]
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string PluginGUID = PluginAuthor + "." + PluginName;

        public const string PluginAuthor = "HIFU";
        public const string PluginName = "HIFULoaderTweaks";
        public const string PluginVersion = "1.1.2";

        public static ConfigFile HLTConfig;
        public static ManualLogSource HLTLogger;

        public void Awake()
        {
            HLTLogger = Logger;
            HLTConfig = Config;

            IEnumerable<Type> enumerable = from type in Assembly.GetExecutingAssembly().GetTypes()
                                           where !type.IsAbstract && type.IsSubclassOf(typeof(TweakBase))
                                           select type;

            HLTLogger.LogInfo("==+----------------==TWEAKS==----------------+==");

            foreach (Type type in enumerable)
            {
                TweakBase based = (TweakBase)Activator.CreateInstance(type);
                if (ValidateTweak(based))
                {
                    based.Init();
                }
            }

            IEnumerable<Type> enumerable2 = from type in Assembly.GetExecutingAssembly().GetTypes()
                                            where !type.IsAbstract && type.IsSubclassOf(typeof(MiscBase))
                                            select type;

            HLTLogger.LogInfo("==+----------------==MISC==----------------+==");

            foreach (Type type in enumerable2)
            {
                MiscBase based = (MiscBase)Activator.CreateInstance(type);
                if (ValidateMisc(based))
                {
                    based.Init();
                }
            }
        }

        public bool ValidateTweak(TweakBase tb)
        {
            if (tb.isEnabled)
            {
                bool enabledfr = Config.Bind(tb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ValidateMisc(MiscBase mb)
        {
            if (mb.isEnabled)
            {
                bool enabledfr = Config.Bind(mb.Name, "Enable?", true, "Vanilla is false").Value;
                if (enabledfr)
                {
                    return true;
                }
            }
            return false;
        }

        private void PeripheryMyBeloved()
        {
        }
    }
}