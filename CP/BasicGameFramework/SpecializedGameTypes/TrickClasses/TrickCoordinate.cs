using CommonBasicStandardLibraries.MVVMHelpers;
namespace BasicGameFramework.SpecializedGameTypes.TrickClasses
{
    public class TrickCoordinate : ObservableObject
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public int Player { get; set; } // this is the id.
        public bool IsSelf { get; set; } // well see if this is needed or not.
        private bool _Visible = true;
        public bool Visible
        {
            get
            {
                return _Visible;
            }

            set
            {
                if (SetProperty(ref _Visible, value) == true)
                {
                }
            }
        }// this is needed for the bindings.
        public bool PossibleDummy { get; set; }
        public string Text { get; set; } = ""; // i think it needs text.  would be best to go ahead and put in the proper text
    }
}