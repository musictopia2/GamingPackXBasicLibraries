using BasicGameFramework.ViewModelInterfaces;
using System;
namespace BasicGameFramework.MainViewModels
{
    public interface IBasicTwoPlayerChoiceVM<E> : IBasicGameVM, ISimpleMultiPlayerVM where E : Enum
    {
        bool DoChoose { get; set; }
        E PieceChosen { get; set; }
        void FinalStart();
        void EndFirstLoad();
    }
}