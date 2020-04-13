using BasicGameFrameworkLibrary.BasicDrawables.Dictionary;
using BasicGameFrameworkLibrary.BasicDrawables.Interfaces;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.Extensions;
using BasicGameFrameworkLibrary.MiscProcesses;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq; //sometimes i do use linq.
using System.Reflection;
using System.Threading.Tasks;
namespace BasicGameFrameworkLibrary.DrawableListsObservable
{

    public class HandObservable<D> : SimpleControlObservable where D : IDeckObject, new()
    {
        public enum EnumHandList
        {
            Horizontal = 1,
            Vertical = 2
        }
        public enum EnumAutoType
        {
            None = 0,
            SelectOneOnly = 1,
            SelectAsMany = 2,
            ShowObjectOnly = 3 // this means it will raise the event for considering but nothing else.  the purpose of this is for games like fluxx where you need to see what the card is (extra details).  since there is no mouse move.
        }
        private bool _visible = true; //needed this part after all.  defaults to true though.

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

        public ControlCommand ObjectSingleClickCommand { get; set; } //one parameter.
        //public ControlCommand CardDoubleClickCommand { get; set; }
        //public ControlCommand CardMoveCommand { get; set; }
        public ControlCommand BoardSingleClickCommand { get; set; }
        //public ControlCommand BoardDoubleClickCommand { get; set; }
        //public ControlCommand BoardMoveCommand { get; set; }
        public HandObservable(CommandContainer command) : base(command)
        {
            MethodInfo method = this.GetPrivateMethod(nameof(ObjectClickProcessAsync));
            //method did not work still.
            ObjectSingleClickCommand = new ControlCommand(this, method, command);
            method = this.GetPrivateMethod(nameof(PrivateBoardSingleClickedAsync));
            BoardSingleClickCommand = new ControlCommand(this, method, command);
            HandList.CollectionChanged += HandList_CollectionChanged;
        }
        protected virtual async Task PrivateBoardSingleClickedAsync()
        {
            if (BoardClickedAsync == null)
                return;
            await BoardClickedAsync.Invoke(); //most likely an issue because using async.
        }
        protected virtual bool CanSelectSingleObject(D thisObject)
        {
            return true; // so something else can do something else
        }
        protected override void SetCommandsLimited()
        {
            ObjectSingleClickCommand.BusyCategory = EnumCommandBusyCategory.Limited;
            BoardSingleClickCommand.BusyCategory = EnumCommandBusyCategory.Limited; //i think.
        }
        public async Task ObjectClickProcessAsync(D thisObject)
        {
            if (AutoSelect == EnumAutoType.ShowObjectOnly)
                if (ConsiderSelectOneAsync != null)
                    await ConsiderSelectOneAsync.Invoke(thisObject);

            if (AutoSelect == EnumAutoType.SelectAsMany)
            {
                if (ConsiderSelectOneAsync != null)
                    await ConsiderSelectOneAsync.Invoke(thisObject);
                if (thisObject.IsSelected == true)
                {
                    if (_orderOfObjectsSelectedList.ObjectExist(thisObject.Deck) == true)
                        _orderOfObjectsSelectedList.RemoveSpecificItem(thisObject);
                    thisObject.IsSelected = false;
                    if (AutoSelectedOneCompletedAsync != null)
                        await AutoSelectedOneCompletedAsync.Invoke(); // well see if we still need this(?)
                    return;
                }
                if (BeforeAutoSelectObjectAsync != null)
                    await BeforeAutoSelectObjectAsync.Invoke();
                thisObject.IsSelected = true;
                if (_orderOfObjectsSelectedList.ObjectExist(thisObject.Deck) == false)
                    _orderOfObjectsSelectedList.Add(thisObject);
                if (AutoSelectedOneCompletedAsync != null)
                    await AutoSelectedOneCompletedAsync.Invoke();
                return;
            }
            if (AutoSelect == EnumAutoType.SelectOneOnly)
            {
                if (CanSelectSingleObject(thisObject) == false)
                    return;// because you can't even select the single card
                if (ConsiderSelectOneAsync != null)
                    await ConsiderSelectOneAsync.Invoke(thisObject);
                if (thisObject.IsSelected == true)
                {
                    thisObject.IsSelected = false;
                    if (AutoSelectedOneCompletedAsync != null)
                        await AutoSelectedOneCompletedAsync.Invoke();
                    return;
                }
                HandList.UnselectAllObjects();
                if (BeforeAutoSelectObjectAsync != null)
                    await BeforeAutoSelectObjectAsync.Invoke();
                thisObject.IsSelected = true;
                if (AutoSelectedOneCompletedAsync != null)
                    await AutoSelectedOneCompletedAsync.Invoke();
                return;
            }
            await ProcessObjectClickedAsync(thisObject, HandList.IndexOf(thisObject));
        }
        protected virtual async Task ProcessObjectClickedAsync(D thisObject, int index)
        {
            if (ObjectClickedAsync == null)
                return;
            await ObjectClickedAsync.Invoke(thisObject, HandList.IndexOf(thisObject));
        }
        private void NotifyCommands()
        {
            //CardDoubleClickCommand.ReportCanExecuteChange();
            //CardMoveCommand.ReportCanExecuteChange();
            //BoardDoubleClickCommand.ReportCanExecuteChange();
            //BoardMoveCommand.ReportCanExecuteChange();
            BoardSingleClickCommand.ReportCanExecuteChange();
        }
        private void HandList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (HandList.Count > Maximum && Maximum > 0 && IgnoreMaxRules == false)
                throw new BasicBlankException("The maximum objects allowed are " + Maximum);
            if (e.Action != NotifyCollectionChangedAction.Move)
                OnHandChange();// not for move.  otherwise, never ending loop
            if (e.Action == NotifyCollectionChangedAction.Add)
                AddObjects(e.NewItems);
        }
        protected virtual void OnHandChange() { }
        private void AddObjects(IList thisList) //is okay this time.
        {
            foreach (var thisObject in thisList)
                AddObjectToHand((D)thisObject);
        }
        protected virtual void AddObjectToHand(D thisObject) { }
        private EnumAutoType _autoSelect = EnumAutoType.SelectOneOnly;
        public EnumAutoType AutoSelect
        {
            get
            {
                return _autoSelect;
            }
            set
            {
                if (SetProperty(ref _autoSelect, value) == true) { }
            }
        }
        private bool _hasFrame = true;
        public bool HasFrame
        {
            get
            {
                return _hasFrame;
            }
            set
            {
                if (SetProperty(ref _hasFrame, value) == true) { }
            }
        }
        protected virtual bool CanEverEnable()
        {
            return true; // usually, it can.   however sometimes, you can't
        }
        private string _text = ""; // this is the display text.
        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                if (SetProperty(ref _text, value) == true)
                {
                }
            }
        }
        private int _maximum;
        public int Maximum
        {
            get { return _maximum; }
            set
            {
                if (SetProperty(ref _maximum, value))
                {
                    //can decide what to do when property changes
                }
            }
        }
        private int _sectionClicked;
        public int SectionClicked
        {
            get
            {
                return _sectionClicked;
            }
            set
            {
                if (SetProperty(ref _sectionClicked, value) == true) { }
            }
        }
        public virtual bool HasSections
        {
            get
            {
                return false;
            }
        }
        private bool _ignoreMaxRules;
        public bool IgnoreMaxRules
        {
            get
            {
                return _ignoreMaxRules;
            }
            set
            {
                if (SetProperty(ref _ignoreMaxRules, value) == true) { }
            }
        }
        public DeckObservableDict<D> HandList = new DeckObservableDict<D>();
        private readonly DeckRegularDict<D> _orderOfObjectsSelectedList = new DeckRegularDict<D>();
        public DeckRegularDict<D> GetObjectsInOrderSelected(bool alsoRemove)
        {
            if (alsoRemove == true)
            {
                _ = ListSelectedObjects(true);
                var newList = _orderOfObjectsSelectedList.ToRegularDeckDict();
                _orderOfObjectsSelectedList.Clear();
                return newList;
            }
            return _orderOfObjectsSelectedList.ToRegularDeckDict();
        }
        public void PopulateObjects(IDeckDict<D> thisList) // try just t (if regularcardinfo; then that will be the list.  knows it has to be at least baseimages.cardinfo
        {
            if (IgnoreMaxRules == false)
                if (thisList.Count > Maximum && Maximum > 0)
                    throw new BasicBlankException("The maximum objects allowed are " + Maximum);
            HandList.ReplaceRange(thisList); // i think its being replace.  if its different, can fix
            AfterPopulateObjects();
        }
        protected virtual void AfterPopulateObjects() { }
        public void ClearHand()
        {
            HandList.Clear();
            _orderOfObjectsSelectedList.Clear();
        }
        public virtual void EndTurn()
        {
            HandList.UnhighlightObjects();
        }
        public void SelectAllObjects()
        {
            HandList.SelectAllObjects();
            _orderOfObjectsSelectedList.Clear(); // because its doing automatically instead of manually
        }
        public virtual void UnselectAllObjects() //decided to make it virtual so overrided versions can do other things after unselecting all or before.
        {
            HandList.UnselectAllObjects();
            _orderOfObjectsSelectedList.Clear();
        }
        public bool HasSelectedObject() => HandList.Any(items => items.IsSelected);
        public int ObjectSelected()
        {
            try
            {
                var ID = (from Items in HandList
                          where Items.IsSelected == true
                          select Items.Deck).SingleOrDefault();
                return ID; // try this way.
            }
            catch (Exception)
            {
                throw new BasicBlankException("There was more than one object selected");
            }
        }
        public DeckRegularDict<D> ListSelectedObjects()
        {
            return ListSelectedObjects(false);
        }
        public DeckRegularDict<D> ListSelectedObjects(bool alsoRemove) // i think its okay to return a list of for this. (well see)
        {
            DeckRegularDict<D> newList = HandList.Where(Items => Items.IsSelected == true).ToRegularDeckDict();
            if (alsoRemove == true)
                HandList.RemoveSelectedItems(); //this works too.
            return newList;
        }
        public int HowManySelectedObjects => HandList.Count(Items => Items.IsSelected == true);
        public int HowManyUnselectedObjects => HandList.Count(Items => Items.IsSelected == false);
        public void SelectOneFromDeck(int deck)
        {
            D thisObject;
            thisObject = HandList.GetSpecificItem(deck);
            thisObject.IsSelected = true;
        }
        public void UnselectOneFromDeck(int deck)
        {
            D thisObject;
            thisObject = HandList.GetSpecificItem(deck);
            thisObject.IsSelected = false;
        }
        #region Events
        protected async Task OnBoardClickedAsync()
        {
            if (BoardClickedAsync == null)
                return;
            await BoardClickedAsync.Invoke();
        }
        protected override void EnableChange() { }
        protected override bool CanEnableFirst()
        {
            if (CanEverEnable() == false)
                return false;
            return true;
        }
        //protected override void VisibleChange()
        //{
        //    NotifyCommands();
        //}
        protected override void PrivateEnableAlways()
        {
            BoardSingleClickCommand.BusyCategory = EnumCommandBusyCategory.Limited;
            ObjectSingleClickCommand.BusyCategory = EnumCommandBusyCategory.Limited;
        }

        public event ObjectClickedEventHandler? ObjectClickedAsync;
        public delegate Task ObjectClickedEventHandler(D payLoad, int index);
        //public event CardMoveEventHandler CardMove;

        //public delegate void CardMoveEventHandler(D ThisCard, int Index);

        public event AutoSelectedOneCompletedEventHandler? AutoSelectedOneCompletedAsync;
        public delegate Task AutoSelectedOneCompletedEventHandler();
        public event BeforeAutoSelectCardEventHandler? BeforeAutoSelectObjectAsync;
        public delegate Task BeforeAutoSelectCardEventHandler();

        //public event CardDoubleClickedEventHandler CardDoubleClicked;

        //public delegate void CardDoubleClickedEventHandler(C ThisCard, int Index);

        public event BoardClickedEventHandler? BoardClickedAsync;
        public delegate Task BoardClickedEventHandler();

        //public event BoardDoubleClickedEventHandler BoardDoubleClicked;

        //public delegate void BoardDoubleClickedEventHandler();

        //public event BoardMoveEventHandler BoardMove;

        //public delegate void BoardMoveEventHandler();

        public event ConsiderSelectOneEventHandler? ConsiderSelectOneAsync; // this is needed for games like fluxx.  so when you click on one, it can show the details of the card you chose.  since there is no mouse over.
        public delegate Task ConsiderSelectOneEventHandler(D ThisObject);
        #endregion
    }
}