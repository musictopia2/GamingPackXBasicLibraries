﻿using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
namespace BasicGameFramework.CommonInterfaces
{
    public interface IStartUp
    {
        /// <summary>
        /// this needs to not only figure out the proper nick names but also needs to figure out the user.  if there are any custom screens, will be here too.
        /// </summary>
        void StartVariables(BasicData data); //this way i don't have to depend on my custom processes.
        /// <summary>
        /// this is where i register custom classes including the registernetworks if necessary.
        /// i may even register the autoresume classes here as well.  might as well do at once.
        /// </summary>
        void RegisterCustomClasses(GamePackageDIContainer container, bool multiplayer, BasicData data); //decided to break the interface now since its new version anyways.
    }
}