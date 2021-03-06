﻿using Moonbyte.UniversalServerAPI.Client;

namespace Moonbyte.UniversalServerAPI.Interface
{
    public interface IUniversalPlugin
    {
        string Name { get; }
        bool Initialize(string PluginDataDirectory);
        bool Invoke(ClientWorkObject clientObject, string[] commandArgs);
        void ConsoleInvoke(string[] commandArgs, Logger iLogger);
    }
}
