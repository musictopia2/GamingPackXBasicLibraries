using CommonBasicStandardLibraries.CollectionClasses;
namespace BasicGameFramework.CommandClasses
{
    public class CommandContainer
    {
        private readonly CustomBasicList<IGameCommand> _commandList = new CustomBasicList<IGameCommand>();
        private readonly CustomBasicList<IGameCommand> _openList = new CustomBasicList<IGameCommand>();
        public event ExecutingChangedEventHandler? ExecutingChanged; //don't need anything else because can call a method to see where it stands.
        public delegate void ExecutingChangedEventHandler(); //i think
        private readonly CustomBasicList<IControlVM> _controlList = new CustomBasicList<IControlVM>();
        public CommandContainer()
        {
            IsExecuting = true; //i think it needs to start out as true.
        }
        private bool _Executing;
        /// <summary>
        /// This is used when its not even your turn.
        /// Use Processing if you are able to do things out of turn as long
        /// as the other variable is false.
        /// </summary>
        /// <remarks>This is used when its not even your turn.
        /// Use Processing if you are able to do things out of turn as long
        /// as the other variable is false.</remarks>
        public bool IsExecuting
        {
            get
            {
                return _Executing;
            }
            set
            {

                if (value == _Executing)
                    return;
                _Executing = value;
                ReportAll();
            }
        }
        private bool _OpenBusy;
        public bool OpenBusy
        {
            get
            {
                return _OpenBusy;
            }
            set
            {
                if (value == _OpenBusy)
                    return;
                _OpenBusy = value;
                ReportOpen();
            }
        }
        public void ReportOpen()
        {
            _openList.ForEach(Items => Items.ReportCanExecuteChange()); //i think this simple.
        }
        private bool _Processing = true; //you need to start out that its processing.
        public bool Processing
        {
            get { return _Processing; }
            set
            {
                if (value == _Processing)
                    return;
                _Processing = value;
                ReportLimited();
            }
        }
        public void ReportLimited()
        {
            ReportItems(EnumCommandBusyCategory.Limited);
        }
        public bool ManuelFinish { get; set; } = false;
        public void ReportAll() //when changing, will report to all no matter what.  decided it can be good to notify all that something has changed.
        {
            ReportItems(EnumCommandBusyCategory.None);
            if (ExecutingChanged != null)
                ExecutingChanged.Invoke();
        }
        private void ReportItems(EnumCommandBusyCategory thisBusy)
        {
            _commandList.ForConditionalItems(Items => Items.BusyCategory == thisBusy
            , Items => Items.ReportCanExecuteChange());
            _controlList.ForConditionalItems(Items => Items.BusyCategory == thisBusy
            , Items => Items.ReportCanExecuteChange());
        }
        public void AddOpen(IGameCommand thisOpen)
        {
            _openList.Add(thisOpen);
        }
        public void AddCommand(IGameCommand thisCommand)
        {
            _commandList.Add(thisCommand);
        }
        public void AddControl(IControlVM thisControl)
        {
            _controlList.Add(thisControl);
        }
    }
}