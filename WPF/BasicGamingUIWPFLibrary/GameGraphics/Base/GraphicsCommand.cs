using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace BasicGamingUIWPFLibrary.GameGraphics.Base
{
    public abstract class GraphicsCommand : UserControl
    {
        //this can't have generics so i can use for basic commands.
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(GraphicsCommand), new FrameworkPropertyMetadata(new PropertyChangedCallback(CommandPropertyChanged)));
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }
        private static void CommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(GraphicsCommand), new FrameworkPropertyMetadata(new PropertyChangedCallback(CommandParameterPropertyChanged)));
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        private static void CommandParameterPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e) { }


        public GraphicsCommand()
        {
            
            MouseUp += GraphicsCommand_MouseUp;
            //Background = Brushes.Green;
        }

        //hint:  if this never fires, it could be because one control is set isenabled to false and never got reset.
        private void GraphicsCommand_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var tempCommand = Command;
            //SKPoint point = e.GetPosition(this); //hopefully this works but don't know though.
            var firsts = e.GetPosition(this);
            SKPoint point = new SKPoint((float) firsts.X, (float) firsts.Y);
            
            BeforeProcessCommand(point);
            if (tempCommand != null)
            {
                if (tempCommand.CanExecute(CommandParameter) == true)
                {
                    
                    BeforeProcessClick(tempCommand, CommandParameter, point);
                    tempCommand.Execute(CommandParameter);
                }
            }
        }

        protected virtual void BeforeProcessCommand(SKPoint point) { }


        protected virtual void BeforeProcessClick(ICommand thisCommand, object thisParameter, SKPoint clickPoint) { } //most of the time, do nothing.


    }
}
