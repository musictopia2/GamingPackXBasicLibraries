using System;
using System.Collections.Generic;
using System.Text;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.BasicPlayerClasses
{
    public interface IPlayerColors : IPlayerItem
    {
        bool DidChooseColor { get; }
        /// <summary>
        /// this is to show no color was chosen.
        /// </summary>
        void Clear();
    }
}