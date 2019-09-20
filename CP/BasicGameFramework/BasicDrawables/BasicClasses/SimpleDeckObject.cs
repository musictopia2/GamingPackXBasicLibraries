using CommonBasicStandardLibraries.MVVMHelpers;
using SkiaSharp;
using System;
using static SkiaSharpGeneralLibrary.SKExtensions.RotateExtensions;
namespace BasicGameFramework.BasicDrawables.BasicClasses
{
    public abstract class SimpleDeckObject : ObservableObject, IEquatable<SimpleDeckObject>
    {

        private SKSize _DefaultSize;
        public SKSize DefaultSize
        {
            get { return _DefaultSize; }
            set
            {
                if (SetProperty(ref _DefaultSize, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        protected virtual void ChangeDeck() { }

        private int _Deck;
        public int Deck
        {
            get { return _Deck; }
            set
            {
                if (SetProperty(ref _Deck, value))
                {
                    //can decide what to do when property changes
                    ChangeDeck(); //so games like fluxx can change another property in response to this.
                }
            }
        }
        private bool _Drew;
        public bool Drew
        {
            get { return _Drew; }
            set
            {
                if (SetProperty(ref _Drew, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _IsUnknown;
        public bool IsUnknown
        {
            get { return _IsUnknown; }
            set
            {
                if (SetProperty(ref _IsUnknown, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private EnumRotateCategory _Angle;
        public EnumRotateCategory Angle
        {
            get { return _Angle; }
            set
            {
                if (SetProperty(ref _Angle, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _Visible = true;
        public bool Visible
        {
            get { return _Visible; }
            set
            {
                if (SetProperty(ref _Visible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (SetProperty(ref _IsSelected, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _IsEnabled = true;
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                if (SetProperty(ref _IsEnabled, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        public override bool Equals(object obj)
        {
            if (!(obj is SimpleDeckObject Temps))
                return false;
            return Deck.Equals(Temps.Deck);
        }
        public bool Equals(SimpleDeckObject other)
        {
            return other != null &&
                   Deck == other.Deck;
        }
        public override int GetHashCode()
        {
            return Deck.GetHashCode();
        }
    }
}