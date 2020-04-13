using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using CommonBasicStandardLibraries.CollectionClasses;
using SkiaSharp;
namespace BasicGameFrameworkLibrary.GameGraphicsCP.GameboardPositionHelpers
{
    public class GameSpace
    {
        public SKRect Area
        {
            get
            {
                return _area;
            }
            set
            {
                _area = value;
                NewArea = new byte[(int)value.Width + 1, (int)value.Height + 1];
            }
        }
        private SKRect _area;
        internal byte[,]? NewArea { get; set; }
        public bool Enabled { get; set; } = true; // if false, then the space cannot even be clicked
        public int Index { get; set; }
        public int Row { get; set; }
        public int Column { get; set; } // don't worry about number (some games need that as well some don't)
        public CustomBasicList<SKRect> ObjectList { get; set; } = new CustomBasicList<SKRect>();
        public CustomBasicList<BaseGraphicsCP> PieceList { get; set; } = new CustomBasicList<BaseGraphicsCP>();
    }
}