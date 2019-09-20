using BaseGPXPagesAndControlsXF.BasicControls.SimpleControls;
using BaseGPXPagesAndControlsXF.GameGraphics.Base;
using BasicGameFramework.BasicDrawables.Interfaces;
using BasicGameFramework.BasicEventModels;
using BasicGameFramework.GameGraphicsCP.Animations;
using BasicGameFramework.GameGraphicsCP.BaseGraphics;
using BasicGameFramework.MultiplePilesViewModels;
using BasicXFControlsAndPages.Helpers;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.Messenging;
using SkiaSharp;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using Xamarin.Forms;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
namespace BaseGPXPagesAndControlsXF.BasicControls.MultipleFrameContainers
{
    public class BasicMultiplePilesXF<CA, GC, GW> : BaseFrameXF, IHandleAsync<AnimateCardInPileEventModel<CA>>
            where CA : class, IDeckObject, new()
            where GC : class, IDeckGraphicsCP, new()
            where GW : BaseDeckGraphicsXF<CA, GC>, new()
    {
        private CustomCanvasXF? _thisCanvas;
        private AnimateImageClass? _animates;
        private string TagUsed = "";
        public int Spacing { get; set; } = 3; // defaults to 3.
        private Grid? _thisGrid;
        private Grid? _parentGrid; // needs this so it can support the canvas for animations.
        private BasicMultiplePilesCP<CA>? _thisMod;
        private PrivateBasicIndividualPileXF<CA, GC, GW> FindControl(BasicPileInfo<CA> thisPile)
        {
            foreach (var thisFirst in _thisGrid!.Children)
            {
                if (Grid.GetColumn((ContentView)thisFirst!) == thisPile.Column - 1 && Grid.GetRow((ContentView)thisFirst!) == thisPile.Row - 1)
                    return (PrivateBasicIndividualPileXF<CA, GC, GW>)thisFirst!;
            }
            throw new BasicBlankException("No Control Found");
        }
        private SKPoint GetObjectLocation(PrivateBasicIndividualPileXF<CA, GC, GW> thisPile)
        {
            var firstLocation = thisPile.CardLocation;
            firstLocation.X += Spacing;
            firstLocation.Y += Spacing;
            float pileHeight;
            pileHeight = thisPile.CardSize.Height;
            float pileWidth;
            pileWidth = thisPile.CardSize.Width;
            if (Grid.GetRow(thisPile) == 0 && Grid.GetColumn(thisPile) == 0)
                return firstLocation;// its that simple because its the first column and first row
            float thisLeft = 0;
            float thisTop = 0;
            int x;
            if (Grid.GetRow(thisPile) == 0)
                thisTop = firstLocation.Y;
            else
            {
                var loopTo = _thisMod!.Rows - 1;
                for (x = 1; x <= loopTo; x++)
                {
                    thisTop += pileHeight + Spacing + Spacing; // not built in afterall.
                    if (Grid.GetRow(thisPile) == x)
                        break;
                }
            }
            if (Grid.GetColumn(thisPile) == 0)
                thisLeft = firstLocation.X;
            else
            {
                var loopTo1 = _thisMod!.Columns - 1;
                for (x = 1; x <= loopTo1; x++)
                {
                    thisLeft += pileWidth + Spacing + Spacing;
                    if (Grid.GetColumn(thisPile) == x)
                        break;
                }
            }
            return new SKPoint(thisLeft, thisTop);
        }
        private async Task PrepareToAnimateAsync(AnimateCardInPileEventModel<CA> data) // don't need index ever for this.
        {
            var thisPile = FindControl(data.ThisPile!);
            var topLocation = GetStartLocation(thisPile);
            var bottomLocaton = GetBottomLocation(thisPile);
            var objectLocation = GetObjectLocation(thisPile);
            switch (data.Direction)
            {
                case EnumAnimcationDirection.StartCardToDown:
                    {
                        _animates!.LocationFrom = objectLocation;
                        _animates.LocationTo = bottomLocaton;
                        break;
                    }

                case EnumAnimcationDirection.StartCardToUp:
                    {
                        _animates!.LocationFrom = objectLocation;
                        _animates.LocationTo = topLocation;
                        break;
                    }

                case EnumAnimcationDirection.StartDownToCard:
                    {
                        _animates!.LocationFrom = bottomLocaton;
                        _animates.LocationTo = objectLocation;
                        break;
                    }

                case EnumAnimcationDirection.StartUpToCard:
                    {
                        _animates!.LocationFrom = topLocation;
                        _animates.LocationTo = objectLocation;
                        break;
                    }
            }
            GW thisControl = new GW();
            thisControl.SendSize(TagUsed, data.ThisCard!);
            thisControl.Drew = false;
            thisControl.IsSelected = false; // not seleccted either regardless.
            await _animates!.DoAnimateAsync(thisControl);
        }
        public async Task HandleAsync(AnimateCardInPileEventModel<CA> message)
        {
            await PrepareToAnimateAsync(message);
        }
        private SKPoint GetStartLocation(PrivateBasicIndividualPileXF<CA, GC, GW> thisPile) // i do because i need to know the height.
        {
            var thisHeight = thisPile.CardSize.Height; // i think.   well see how that is done for xamarin forms (we seemed to do it successfully from xamarin forms
            var thisLeft = GetLeft(thisPile);
            return new SKPoint(thisLeft, thisHeight * -1.0f); // i think
        }
        private SKPoint GetBottomLocation(PrivateBasicIndividualPileXF<CA, GC, GW> thisPile)
        {
            var thisHeight = thisPile!.CardSize.Height; //well see
            var thisLeft = GetLeft(thisPile);
            return new SKPoint(thisLeft, thisHeight);
        }
        private float GetLeft(PrivateBasicIndividualPileXF<CA, GC, GW> thisPile)
        {
            var firstLocation = thisPile.CardLocation;
            firstLocation.X += Spacing;
            firstLocation.Y += Spacing;
            float thisLeft = 0;
            float pileWidth;
            pileWidth = thisPile.CardSize.Width;
            if (Grid.GetColumn(thisPile) == 0)
                thisLeft = firstLocation.X;
            else
            {
                var loopTo = _thisMod!.Columns - 1;
                for (var x = 1; x <= loopTo; x++)
                {
                    thisLeft += pileWidth + Spacing + Spacing;
                    if (Grid.GetColumn(thisPile) == x)
                        break;
                }
            }
            return thisLeft;
        }
        public void UpdateLists(BasicMultiplePilesCP<CA> mod)
        {
            _thisMod = mod;
            BindingContext = null;
            BindingContext = _thisMod;
            _thisGrid!.Children.Clear(); //i think this is best.
            GridHelper.AddAutoColumns(_thisGrid, _thisMod.Columns);
            GridHelper.AddAutoRows(_thisGrid, _thisMod.Rows);
            foreach (var pileMod in _thisMod.PileList!)
            {
                PrivateBasicIndividualPileXF<CA, GC, GW> PileG = new PrivateBasicIndividualPileXF<CA, GC, GW>();
                PileG.MainMod = _thisMod;
                PileG.ThisPile = pileMod;
                GridHelper.AddControlToGrid(_thisGrid, PileG, pileMod.Row - 1, pileMod.Column - 1);
                PileG.Margin = new Thickness(Spacing, Spacing, Spacing, Spacing);
                PileG.Init(TagUsed);
            }
        }
        public void Init(BasicMultiplePilesCP<CA> mod, string tagUsed)
        {
            _thisGrid = new Grid();
            _parentGrid = new Grid();
            _parentGrid.Children.Add(_thisGrid);
            _thisMod = mod;
            BindingContext = mod; // i think needs this as well.
            TagUsed = tagUsed;
            SetBinding(IsVisibleProperty, new Binding(nameof(BasicMultiplePilesCP<CA>.Visible)));
            GW tempCard = new GW();
            tempCard.SendSize(tagUsed, new CA()); //for testing.
            SKSize size = tempCard.ObjectSize;
            if (_thisMod.PileList!.Count == 0)
                throw new Exception("Must have at least one pile.  Otherwise, not worth even using this");
            if (_thisMod.HasFrame)
            {
                GridHelper.AddAutoColumns(_thisGrid, _thisMod.Columns);
                GridHelper.AddAutoRows(_thisGrid, _thisMod.Rows);
            }
            else
            {
                _thisMod.Columns.Times(x =>
                {
                    GridHelper.AddPixelColumn(_thisGrid, size.Width);
                });
                _thisMod.Rows.Times(x =>
                {
                    GridHelper.AddPixelRow(_thisGrid, size.Height);
                });
            }
            _thisGrid.RowSpacing = Spacing;
            _thisGrid.ColumnSpacing = Spacing;
            foreach (var pileMod in _thisMod.PileList)
            {
                PrivateBasicIndividualPileXF<CA, GC, GW> pileG = new PrivateBasicIndividualPileXF<CA, GC, GW>();
                pileG.MainMod = _thisMod;
                pileG.ThisPile = pileMod;
                GridHelper.AddControlToGrid(_thisGrid, pileG, pileMod.Row - 1, pileMod.Column - 1);
                pileG.Init(tagUsed);
            }
            Content = _parentGrid;
        }
        public void Unregister()
        {
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Unsubscribe(this, _animationTag);
        }
        private string _animationTag = "";
        public void StartAnimationListener(string animationTag) // has to be manual
        {
            _animationTag = animationTag;
            if (_thisCanvas != null)
                throw new BasicBlankException("You already started an animation listener");
            _thisCanvas = new CustomCanvasXF();
            _thisCanvas.HeightRequest = 20;
            _thisCanvas.WidthRequest = 20;
            _thisCanvas.InputTransparent = true;
            _parentGrid!.Children.Add(_thisCanvas);
            _animates = new AnimateImageClass();
            _animates.LongestTravelTime = 200;
            _animates.GameBoard = _thisCanvas;
            EventAggregator thisE = Resolve<EventAggregator>();
            thisE.Subscribe(this, animationTag);
        }
    }
}