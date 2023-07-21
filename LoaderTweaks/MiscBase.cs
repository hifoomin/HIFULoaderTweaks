﻿namespace HIFULoaderTweaks
{
    public abstract class MiscBase
    {
        public abstract string Name { get; }
        public virtual bool isEnabled { get; } = true;

        public T ConfigOption<T>(T value, string name, string description)
        {
            return Main.HLTConfig.Bind<T>(Name, name, value, description).Value;
        }

        public abstract void Hooks();

        public string d(float f)
        {
            return (f * 100f).ToString() + "%";
        }

        public virtual void Init()
        {
            Hooks();
            Main.HLTLogger.LogInfo("Added " + Name);
        }
    }
}