﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BasicGameFrameworkLibrary.MultiplayerClasses.InterfacesForHelpers
{
    public interface IAdditionalRollProcess
    {
        Task<bool> CanRollAsync();
        Task BeforeRollingAsync();
    }
}
