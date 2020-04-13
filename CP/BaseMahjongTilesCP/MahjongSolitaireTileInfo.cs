using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using SkiaSharp;
namespace BaseMahjongTilesCP
{
    public class MahjongSolitaireTileInfo : BasicMahjongTile, IDeckObject, IMahjongTileInfo
    {
        private void RecalculateDefaultSize()
        {
            DefaultSize = GetDefaultSize();
        }
        public static SKSize GetDefaultSize()
        {
            return new SKSize(68, 88);
        }
        public void Populate(int chosen)
        {

            RecalculateDefaultSize();
            MahjongBasicTileHelper.PopulateTile(this, chosen);
        }
        public void Reset() { }
    }
}