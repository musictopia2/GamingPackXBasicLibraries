using BasicGameFramework.CommandClasses;
using BasicGameFramework.ViewModelInterfaces;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMHelpers;
using System;
namespace BasicGameFramework.MiscViewModels
{
    public abstract class SimpleControlViewModel : ObservableObject, IControlVM
    {
        private bool _IsEnabled;
        public bool IsEnabled
        {
            get
            {
                return _IsEnabled;
            }
            set
            {
                if (CanEnableFirst() == false)
                    value = false;// otherwise, value can always be false.
                if (SetProperty(ref _IsEnabled, value) == true)
                    EnableChange();
            }
        }
        public EnumCommandBusyCategory BusyCategory { get; set; } = EnumCommandBusyCategory.None; //most of the time, none.
        public bool AlwaysDisabled { set; private get; }
        protected CommandContainer CommandContainer;
        public SimpleControlViewModel(IBasicGameVM thisMod)
        {
            CommandContainer = thisMod.CommandContainer!;
            CommandContainer.AddControl(this); //i think it should be here instead.
        }
        protected virtual bool CanEnableFirst()
        {
            return !AlwaysDisabled;
        }
        public void ManualChange()
        {
            EnableChange();
        }
        protected abstract void EnableChange();
        private bool _Visible;
        public bool Visible
        {
            get
            {
                return _Visible;
            }
            set
            {
                if (SetProperty(ref _Visible, value) == true)
                    VisibleChange();
            }
        }
        protected abstract void VisibleChange();
        protected virtual bool SpecialEnable()
        {
            return true;
        }
        private IBasicEnableProcess? _networkProcess;
        private IEnableAlways? _alwaysProcess;
        private Func<bool>? _customFunction; // i think this will have no parameters for this one.
        private bool _useSpecial;
        public void SendEnableProcesses(IBasicEnableProcess nets, Func<bool> fun)
        {
            SendFunction(fun);
            _networkProcess = nets;
        }
        private void ShowSpecial()
        {
            if (_useSpecial == true)
                throw new BasicBlankException("Already Enabled");
            _useSpecial = true;
        }
        public void SendFunction(Func<bool> fun)
        {
            ShowSpecial();
            _customFunction = fun;
        }
        public void SendAlwaysEnable(IEnableAlways always)
        {
            ShowSpecial();
            _alwaysProcess = always;
            BusyCategory = EnumCommandBusyCategory.Limited;
            SetCommandsLimited();
        }
        protected virtual void SetCommandsLimited() { }
        protected abstract void PrivateEnableAlways(); //everything needs to decide what to do about it.
        public void ReportCanExecuteChange()
        {
            IsEnabled = true;
            if (CommandContainer.IsExecuting == true && BusyCategory == EnumCommandBusyCategory.None)
            {
                IsEnabled = false; //i think
                return;
            }
            if (CommandContainer.Processing == true && BusyCategory == EnumCommandBusyCategory.Limited)
            {
                IsEnabled = false;
                return;
            }
            if (_useSpecial == false)
            {
                if (AlwaysDisabled == true)
                    IsEnabled = false;
                else
                    IsEnabled = true; //for now until i figure out further.
                return;
            }
            if (_alwaysProcess != null)
            {
                IsEnabled = _alwaysProcess.CanEnableAlways();
                return;
            }
            if (_networkProcess != null)
            {
                if (_networkProcess.CanEnableBasics() == false)
                {
                    IsEnabled = false;
                    return;
                }
            }
            IsEnabled = _customFunction!();
        }
        bool IControlVM.CanExecute()
        {
            if (Visible == false)
                return false;
            if (IsEnabled == false)
                return false;
            return SpecialEnable();
        }
    }
}