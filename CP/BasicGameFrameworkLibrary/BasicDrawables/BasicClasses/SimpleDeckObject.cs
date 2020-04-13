using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using SkiaSharp;
using System;
using static SkiaSharpGeneralLibrary.SKExtensions.RotateExtensions;
namespace BasicGameFrameworkLibrary.BasicDrawables.BasicClasses
{
    public abstract class SimpleDeckObject : ObservableObject, IEquatable<SimpleDeckObject>
    {

        private SKSize _defaultSize;
        public SKSize DefaultSize
        {
            get { return _defaultSize; }
            set
            {
                if (SetProperty(ref _defaultSize, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        protected virtual void ChangeDeck() { }

        private int _deck;
        public int Deck
        {
            get { return _deck; }
            set
            {
                if (SetProperty(ref _deck, value))
                {
                    //can decide what to do when property changes
                    ChangeDeck(); //so games like fluxx can change another property in response to this.
                }
            }
        }
        private bool _drew;
        public bool Drew
        {
            get { return _drew; }
            set
            {
                if (SetProperty(ref _drew, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _isUnknown;
        public bool IsUnknown
        {
            get { return _isUnknown; }
            set
            {
                if (SetProperty(ref _isUnknown, value))
                {
                    //can decide what to do when property changes
                }

            }
        }
        private EnumRotateCategory _angle;
        public EnumRotateCategory Angle
        {
            get { return _angle; }
            set
            {
                if (SetProperty(ref _angle, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                if (SetProperty(ref _visible, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (SetProperty(ref _isSelected, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (SetProperty(ref _isEnabled, value))
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