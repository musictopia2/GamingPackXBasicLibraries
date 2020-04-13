using BasicGameFrameworkLibrary.BasicEventModels;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.GameGraphicsCP.BaseGraphics;
using BasicGameFrameworkLibrary.MultiplayerClasses.MiscHelpers;
using BasicGameFrameworkLibrary.ViewModels;
using BasicGamingUIWPFLibrary.BasicControls.ChoicePickers;
using BasicGamingUIWPFLibrary.BasicControls.SimpleControls;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using CommonBasicStandardLibraries.Messenging;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using System.Windows.Controls;
namespace BasicGamingUIWPFLibrary.Views
{

    public static class BeginningColorDimensions
    {
        public static float GraphicsHeight { get; set; }
        public static float GraphicsWidth { get; set; }
    }
    public class BeginningChooseColorView<GC, GW, E> : UserControl, IUIView, IHandleAsync<LoadEventModel>
        where GC : BaseGraphicsCP, IEnumPiece<E>, new()
                where GW : BaseGraphicsWPF<GC>, new()
                where E : struct, Enum
    {
        private readonly IEventAggregator _aggregator;
        private readonly IBeginningColorModel<E, GC> _model;




        private readonly EnumPickerWPF<GC, GW, E> _color;
        public BeginningChooseColorView(IEventAggregator aggregator, IBeginningColorModel<E, GC> model)
        {
            _aggregator = aggregator;
            _model = model;
            _aggregator.Subscribe(this);

            StackPanel stack = new StackPanel();
            _color = new EnumPickerWPF<GC, GW, E>();
            if (BeginningColorDimensions.GraphicsHeight > 0)
            {
                _color.GraphicsHeight = BeginningColorDimensions.GraphicsHeight;
            }
            if (BeginningColorDimensions.GraphicsWidth > 0)
            {
                _color.GraphicsWidth = BeginningColorDimensions.GraphicsWidth;
            }
            stack.Children.Add(_color);
            SimpleLabelGrid simple = new SimpleLabelGrid();
            simple.AddRow("Turn", nameof(IBeginningColorViewModel.Turn));
            simple.AddRow("Instructions", nameof(IBeginningColorViewModel.Instructions));
            stack.Children.Add(simple.GetContent);

            Content = stack;
        }

        Task IHandleAsync<LoadEventModel>.HandleAsync(LoadEventModel message)
        {
            _color.LoadLists(_model.ColorChooser);

            return Task.CompletedTask;
        }

        Task IUIView.TryActivateAsync()
        {
            return Task.CompletedTask;
        }

        Task IUIView.TryCloseAsync()
        {
            _aggregator.Unsubscribe(this);
            return Task.CompletedTask;
        }
    }
}
