using CommonBasicStandardLibraries.MVVMHelpers;
using System;
namespace BasicGameFramework.MultiplayerClasses.BasicPlayerClasses
{
    public class SimplePlayer : ObservableObject, IPlayerItem, IEquatable<SimplePlayer>
    {
        public int Id { get; set; }
        private string _NickName = "";

        public string NickName
        {
            get { return _NickName; }
            set
            {
                if (SetProperty(ref _NickName, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private bool _InGame;
        public bool InGame
        {
            get
            {
                return _InGame;
            }

            set
            {
                if (SetProperty(ref _InGame, value) == true)
                {
                }
            }
        }
        private bool _IsReady;
        public bool IsReady
        {
            get
            {
                return _IsReady;
            }

            set
            {
                if (SetProperty(ref _IsReady, value) == true)
                {
                }
            }
        }
        private bool _MissNextTurn;

        public bool MissNextTurn
        {
            get { return _MissNextTurn; }
            set
            {
                if (SetProperty(ref _MissNextTurn, value))
                {
                    //can decide what to do when property changes
                }

            }
        }

        private EnumPlayerCategory _PlayerCategory;
        public EnumPlayerCategory PlayerCategory
        {
            get
            {
                return _PlayerCategory;
            }

            set
            {
                if (SetProperty(ref _PlayerCategory, value) == true)
                {
                }
            }
        }
        private bool _IsHost;

        public bool IsHost
        {
            get { return _IsHost; }
            set
            {
                if (SetProperty(ref _IsHost, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        public virtual bool CanStartInGame => true; //clue will have exceptions.

        public override bool Equals(object obj)
        {
            if (!(obj is SimplePlayer Temps))
                return false;
            return NickName.Equals(Temps.NickName);
        }
        public bool Equals(SimplePlayer other)
        {
            return NickName.Equals(other.NickName);
        }

        public override int GetHashCode()
        {
            return NickName.GetHashCode();
        }
    }
}