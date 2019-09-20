using BasicGameFramework.BasicDrawables.Dictionary;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.MultiplayerClasses.BasicPlayerClasses;
using BasicGameFramework.ViewModelInterfaces;
using Newtonsoft.Json;
using System;
namespace BasicGameFramework.MultiplayerClasses.SavedGameClasses
{
    public abstract class BasicSavedTrickGamesClass<S, T, P> : BasicSavedCardClass<P, T>
        where S : struct, Enum //hopefully that works.
        where T : ITrickCard<S>, new()
        where P : class, IPlayerTrick<S, T>, new()
    {
        public DeckObservableDict<T> TrickList = new DeckObservableDict<T>();
        private S _TrumpSuit;
        public S TrumpSuit
        {
            get { return _TrumpSuit; }
            set
            {
                if (SetProperty(ref _TrumpSuit, value))
                {
                    if (TrickMod != null)
                        TrickMod.TrumpSuit = value;
                }
            }
        }
        [JsonIgnore]
        public ITrickGameVM<S>? TrickMod;
        public void LoadTrickVM(ITrickGameVM<S> TrickMod)
        {
            this.TrickMod = TrickMod;
            TrickMod.TrumpSuit = TrumpSuit;
        }
    }
}