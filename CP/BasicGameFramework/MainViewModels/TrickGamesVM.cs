using System;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.MVVMHelpers;
using static CommonBasicStandardLibraries.MVVMHelpers.Command; //this is used so if you want to know if its still executing, can be done.
using System.Linq; //sometimes i do use linq.
using CommonBasicStandardLibraries.MVVMHelpers.Interfaces;
using System.Threading.Tasks;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.MultiplayerClasses.MainGameInterfaces;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.BasicGameDataClasses;
using BasicGameFramework.DIContainers;
using BasicGameFramework.ViewModelInterfaces;
using BasicGameFramework.MultiplayerClasses.InterfacesForHelpers;

namespace BasicGameFramework.MainViewModels
{
    public abstract class TrickGamesVM<SU, TR, P, G> : BasicCardGamesVM<TR, P, G>, ITrickGameVM<SU>
        where SU : struct, Enum
        where TR : ITrickCard<SU>, new()
        where P : class, IPlayerTrick<SU, TR>, new()
        where G : class, ICardGameMainProcesses<TR>, IBasicGameProcesses<P>, IEndTurn
    {
        private SU _TrumpSuit;
        public SU TrumpSuit
        {
            get { return _TrumpSuit; }
            set
            {
                if (SetProperty(ref _TrumpSuit, value)) { }
            }
        }
        public TrickGamesVM(ISimpleUI tempUI, IGamePackageResolver tempC, BasicData thisData) : base(tempUI, tempC, thisData) { }
    }
}