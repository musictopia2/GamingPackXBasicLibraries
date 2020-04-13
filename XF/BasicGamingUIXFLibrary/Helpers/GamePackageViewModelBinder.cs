using BasicGameFrameworkLibrary.Attributes;
using BasicGameFrameworkLibrary.CommandClasses;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGamingUIXFLibrary.GameGraphics.Base;
using BasicXFControlsAndPages.MVVMFramework.Controls;
using BasicXFControlsAndPages.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.Attributes;
using CommonBasicStandardLibraries.MVVMFramework.Commands;
using CommonBasicStandardLibraries.MVVMFramework.Conductors;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace BasicGamingUIXFLibrary.Helpers
{

    internal class BindHelpClass
    {
        public bool IsValid { get; set; }
        public BindableProperty? Dependency { get; set; } //hopefully can still use this.
    }

    public static class GamePackageViewModelBinder
    {

        //public static bool StopRun { get; set; } = false;

        private static readonly Dictionary<Type, BindableProperty> _boundProperties = new Dictionary<Type, BindableProperty>()
        {
            {typeof(Label), Label.TextProperty },
            {typeof(Grid), VisualElement.IsVisibleProperty },
            {typeof(StackLayout), VisualElement.IsVisibleProperty}
        };

        //for hookparentcontainers, risk copy/paste from the xamarin forms version.

        private readonly static CustomBasicList<VisualElement> _controls = new CustomBasicList<VisualElement>();
        public static CustomBasicList<VisualElement> ManuelElements = new CustomBasicList<VisualElement>();

        //public static CustomBasicList<Button> ButtonList = new CustomBasicList<Button>();


        private static void GetLayoutChildren(Layout<View> layout)
        {
            foreach (var child in layout.Children)
                if (child is Layout<View> view)
                {
                    _controls.Add(child); //i think has to add this one too.  otherwise, won't show any grids.  we need that too.
                    GetChildren(view);
                }
                else
                    _controls.Add(child);
        }

        private static void GetChildren(VisualElement vv)
        {
            if (vv is ContentView cc)
                GetChildren(cc.Content);
            else if (vv is ContentPage pp)
                GetChildren(pp.Content);
            else if (vv is ParentSingleUIContainer)
            {
                //try to do nothing.  hopefully already added (?)
                //_controls.Add(aa); //hopefully this simple.
            }
            else if (vv is Layout<View> ll)
                GetLayoutChildren(ll);
            else
                _controls.Add(vv);
        }

        //will attempt without the manuel feature.  if we need the manuel feature back, will do.



        //attempt to copy/paste from wpf and make necessary changes.  taking lots of risks here.
        /// <summary>
        /// this is the process that needs to hook up the proper parent control
        /// not only to add to proper control
        /// but also to hook up to parent container so it can be removed later.
        /// </summary>
        /// <param name="parentViewModel"></param>
        /// <param name="parentViewScreen"></param>
        /// <param name="childViewModel"></param>
        public static void HookParentContainers(object parentViewModel, BindableObject parentViewScreen, IScreen childViewModel, IUIView childViewScreen)
        {
            //if (StopRun)
            //{
            //    return;
            //}
            //CustomBasicList<FrameworkElement> controls = FindVisualChildren(parentViewScreen).ToCustomBasicList();
            //CustomBasicList<VisualElement> controls = new CustomBasicList<VisualElement>();

            //if (parentvi)

            //GetChildren(parentViewScreen); //hopefully this simple.

            _controls.Clear(); //i think.
            if (parentViewScreen is VisualElement vv)
                GetChildren(vv);
            if (_controls.Count == 0)
                throw new BasicBlankException("No controls was found when trying to hook up parent to start with");
            _controls.KeepConditionalItems(x =>
            {
                if (string.IsNullOrWhiteSpace(x.GetName()) == true)
                    return false;
                Type type = x.GetType();
                if (typeof(IContentControl).IsAssignableFrom(type))
                    return true;
                return false;
            });


            if (_controls.Count == 0)
                throw new BasicBlankException("There was no container controls");



            Type type = parentViewModel.GetType();
            var properties = type.GetProperties().ToCustomBasicList();

            CustomBasicList<IContentControl> parents = new CustomBasicList<IContentControl>();
            bool isGuarantee = false;

            if (typeof(IConductorGuarantee).IsAssignableFrom(type))
                isGuarantee = true;


            //i think if there is no match, needs to raise exception for sure.
            foreach (var control in _controls)
            {
                var property = properties.Where(x => x.Name == control.GetName()).SingleOrDefault();
                if (property == null)
                    throw new BasicBlankException($"Parent container with the name of {control.GetName()} could not be found.  Rethink");
                IContentControl? fins = (IContentControl)control;
                if (fins == null)
                    throw new BasicBlankException("Was not parent.  Rethink");
                else
                    if (isGuarantee)
                    parents.Add(fins);
                else
                {


                    object thisObj = property.GetValue(parentViewModel, null)!;
                    if (thisObj != null)
                    {
                        if (!(thisObj is IMainScreen))
                        {
                            string name = control.GetName();
                            throw new BasicBlankException($"The control with the name of {name} was wrong because the control did not implement IMainScreen");
                        }
                        Type testType = thisObj.GetType();
                        Type childType = childViewModel.GetType();

                        if (testType.Name == childType.Name)
                        {
                            if (fins is ParentSingleUIContainer parent)
                            {
                                if (parent.Children.Count == 0)
                                {
                                    parents.Add(fins);
                                }
                            }
                            else
                            {
                                parents.Add(fins);
                            }
                        }
                    }

                    //object thisObj = property.GetValue(parentViewModel, null)!;
                    //if (thisObj != null)
                    //{
                    //    Type testType = thisObj.GetType();
                    //    Type childType = childViewModel.GetType();

                    //    if (testType.Name == childType.Name)
                    //        parents.Add(fins);

                    //}

                }
            }
            if (parents.Count == 0)
                throw new BasicBlankException("There was no parent controls found for hooking up the child");
            if (parents.Count == 1)
            {
                childViewModel.ParentContainer = parents.Single();
                childViewModel.ParentContainer.Add(childViewScreen);

            }

        }


        public static void Bind(object viewModel, BindableObject view, BindableObject? content = null)
        {
            //if (StopRun)
            //{
            //    return;
            //}
            Type type = viewModel.GetType();
            if (!(view is VisualElement element))
                return;
            var viewType = viewModel.GetType();
            var properties = viewType.GetProperties();
            var methods = viewType.GetMethods().ToCustomBasicList();

            _controls.Clear();
            if (view is VisualElement ee)
                GetChildren(ee);

            if (_controls.Count() == 0 && content is VisualElement vv)
                GetChildren(vv);
            _controls.AddRange(ManuelElements);

            if (_controls.Count == 0)
            {
                return;
            }


            if (_controls.Count == 1 && _controls.Single() == null)
            {
                return;
            }

            BindProperties(_controls, type, properties);
            var pList = methods.Where(x => x.Name.StartsWith("Can")).ToCustomBasicList();

            methods.KeepConditionalItems(x => x.HasCommandAttribute()); //because only
            if (methods.Any(x =>
            {
                if (x.ReturnType.Name != "Task" && x.ReturnType.Name != "Void")
                {
                    return true;
                }
                return false;
            }))
            {
                throw new BasicBlankException("Cannot put as command if the return type of any is not void or task.  Rethink");
            }



            BindStandardCommands(_controls, viewModel, methods, properties, pList);
            //do a separate one for combo boxes.  not enough in common with text boxes to do interfaces.
            //too many specialized stuff.
            //if i do right, combo may be easier to use than ever (well see).

        }
        private static bool HasCommandAttribute(this MethodInfo method)
        {
            var item = method.GetCustomAttribute<CommandAttribute>();
            return item != null;
        }
        private static bool HasOpenAttribute(this MethodInfo method)
        {
            var item = method.GetCustomAttribute<OpenChildAttribute>();
            return item != null;
        }

        private static void BindProperties(CustomBasicList<VisualElement> controls, Type viewmodelType, IEnumerable<PropertyInfo> properties)
        {

            foreach (var property in properties)
            {
                CustomBasicList<VisualElement> firstList = controls.Where(x => x.GetName().StartsWith(property.Name, StringComparison.InvariantCultureIgnoreCase)).ToCustomBasicList();
                
                
                
                //looks like i had to have the possibility of 2 of them.  did on desktop too.


                
                
                
                if (firstList.Count == 0)
                    continue;


                foreach (var foundControl in firstList)
                {
                    if (foundControl == null)
                        continue;
                    //if (foundControl is Combo custom)
                    //{
                    //    custom.SendPropertyBinding();
                    //    continue; //don't do anything else for the custom one.
                    //}
                    BindableProperty boundProperty;
                    if (!_boundProperties.TryGetValue(foundControl.GetType(), out boundProperty!))
                        continue;
                    if (foundControl.GetBinding(boundProperty) != null)
                        //because if it had to be done manually, don't want to override it.
                        //this means i can do manually if necessary.
                        continue;
                    var interpretedViewModelType = viewmodelType;
                    var interpretedProperty = property;
                    var stringPath = foundControl.GetName();
                    stringPath = stringPath.Replace("_", "."); //i can try to do here.
                    CustomBasicList<string> cList = stringPath.Split(".").ToCustomBasicList();
                    cList.RemoveFirstItem(); //i like this idea.
                    foreach (var item in cList)
                    {
                        interpretedViewModelType = interpretedProperty.PropertyType;
                        var nList = interpretedViewModelType.GetProperties(x => x.Name == item).ToCustomBasicList();
                        if (nList.Count == 0)
                            throw new BasicBlankException($"Unable to get the property for linked properties.  The full string was {foundControl.GetName()}.  The one unfound was {item}");
                        if (nList.Count > 1)
                            throw new BasicBlankException($"There was more than one match for linked properties.  The full string was {foundControl.GetName()}.  The one with duplicate was {item}");
                        interpretedProperty = nList.Single();
                    }



                    var binding = new Binding(stringPath);
                    ApplyBindingMode(binding, interpretedProperty);




                    ApplyStringFormat(binding, foundControl, interpretedProperty);
                    foundControl.SetBinding(boundProperty, binding);
                }

                


            }
        }



        private static string GetSearchName(MethodInfo method)
        {
            if (method.Name.ToLower().EndsWith("async"))
                return method.Name.Substring(0, method.Name.Count() - 5);
            return method.Name;
        }

        private static void BindStandardCommands(CustomBasicList<VisualElement> controls, object viewModel, IEnumerable<MethodInfo> methods, IEnumerable<PropertyInfo> properties, IEnumerable<MethodInfo> predicates)
        {
            Type type = viewModel.GetType();
            VisualElement view = controls.First();
            //i can use classid.  can't do name this time
            //the best i can do is classid.

            CustomBasicList<MethodInfo> specialList = methods.Where(x => x.HasOpenAttribute()).ToCustomBasicList();
            if (specialList.Count > 1)
                throw new BasicBlankException("You had more than one open child attribute.  Rethink");
            MethodInfo? openRealMethod = specialList.SingleOrDefault();
            PropertyInfo? openFunProperty = null;
            if (openRealMethod != null)
            {
                openFunProperty = properties.Where(x => x.Name == "CanOpenChild").SingleOrDefault();
                if (openFunProperty == null)
                    throw new BasicBlankException("Did not detect CanOpenChild function in the view model.  Rethink");

            }
            foreach (var method in methods)
            {
                if (method.ReturnType.Name != "Task" && method.ReturnType.Name != "Void")
                    continue; //do more in the loop but can't consider this one because only void and task are supported.
                bool isOpenChild = method.Equals(openRealMethod);
                string searchName;
                searchName = GetSearchName(method);
                var controlList = controls.Where(x => x.GetName().Equals(searchName, StringComparison.InvariantCultureIgnoreCase) || x.GetName().Equals(method.Name, StringComparison.InvariantCultureIgnoreCase)).ToCustomBasicList();
                foreach (var foundControl in controlList)
                {
                    if (foundControl == null)
                        continue;
                    var foundProperty = properties.FirstOrDefault(x => x.Name == "Can" + searchName);
                    MethodInfo? validateM = null;
                    ICommand command;
                    if (foundProperty == null && isOpenChild == false)
                        validateM = predicates.FirstOrDefault(x => x.Name == "Can" + searchName);
                    else if (isOpenChild == true && openFunProperty != null)
                        foundProperty = openFunProperty;
                    if (foundProperty != null && validateM != null)
                        throw new BasicBlankException("Cannot have the can for both property and method.  Rethink");

                    command = GetCommand(viewModel, method, validateM, foundProperty);
                    

                    //if (foundControl is Button button)
                    //{
                    //    button.Command = command;
                    //}
                    //else if (foundControl is GraphicsCommand gg)
                    //{
                    //    gg.Command = command;
                    //}
                    TrySetCommand(foundControl, command);
                }
            }
        }

        private static ICommand GetCommand(object viewModel, MethodInfo method, MethodInfo? validateM, PropertyInfo? foundProperty)
        {
            var item = method.GetCustomAttribute<CommandAttribute>();
            if (item == null)
            {
                throw new BasicBlankException("Was not even a custom command.  Rethink");
            }
            ICommand? output;
            if (!(viewModel is IBlankGameVM blank))
            {
                throw new BasicBlankException("This is not a blank game view model.  Rethink");
            }
            if (blank.CommandContainer == null)
            {
                throw new BasicBlankException("The command container for command not there.  Rethink");
            }
            switch (item.Category)
            {
                case EnumCommandCategory.Plain:
                    if (foundProperty == null && validateM != null)
                    {
                        output = new PlainCommand(viewModel, method, validateM, blank.CommandContainer);
                    }
                    else
                    {
                        output = new PlainCommand(viewModel, method, foundProperty!, blank.CommandContainer);
                    }
                    break;
                case EnumCommandCategory.Game:
                    if (!(viewModel is IBasicEnableProcess basics))
                    {
                        throw new BasicBlankException("You need to implement the IEnableAlways in order to use out of turn command.  Rethink");
                    }
                    if (foundProperty == null && validateM != null)
                    {
                        output = new BasicGameCommand(basics, method, validateM, blank.CommandContainer);
                    }
                    else
                    {
                        output = new BasicGameCommand(basics, method, foundProperty!, blank.CommandContainer);
                    }
                    break;
                case EnumCommandCategory.Limited:
                    if (!(viewModel is IBasicEnableProcess basics2))
                    {
                        throw new BasicBlankException("You need to implement the IEnableAlways in order to use out of turn command.  Rethink");
                    }
                    if (foundProperty == null && validateM != null)
                    {
                        output = new LimitedGameCommand(basics2, method, validateM, blank.CommandContainer);
                    }
                    else
                    {
                        output = new LimitedGameCommand(basics2, method, foundProperty!, blank.CommandContainer);
                    }
                    break;
                case EnumCommandCategory.OutOfTurn:

                    if (!(viewModel is IEnableAlways enables))
                    {
                        throw new BasicBlankException("You need to implement the IEnableAlways in order to use out of turn command.  Rethink");
                    }

                    output = new OutOfTurnCommand(enables, method, blank.CommandContainer);
                    break;
                case EnumCommandCategory.Open:
                    if (foundProperty == null && validateM != null)
                    {
                        output = new OpenCommand(viewModel, method, validateM, blank.CommandContainer);
                    }
                    else
                    {
                        output = new OpenCommand(viewModel, method, foundProperty!, blank.CommandContainer);
                    }
                    break;
                case EnumCommandCategory.Control:
                    if (!(viewModel is IControlObservable control))
                    {
                        throw new BasicBlankException("You need to implement the IControlVM in order to use control command.  Rethink");
                    }
                    output = new ControlCommand(control, method, blank.CommandContainer);
                    break;
                case EnumCommandCategory.Old:
                    if (foundProperty == null && validateM != null)
                    {
                        output = new ReflectiveCommand(viewModel, method, validateM);
                    }
                    else
                    {
                        output = new ReflectiveCommand(viewModel, method, foundProperty!);
                    }
                    break;
                default:
                    throw new BasicBlankException("Not supported");
            }
            if (output == null)
            {
                throw new BasicBlankException("No command.   Rethink");
            }
            return output;
        }




        /// <summary>
        /// Applies the appropriate binding mode to the binding.
        /// </summary>
        public static Action<Binding, PropertyInfo> ApplyBindingMode = (binding, property) =>
        {
            DefaultBindingMode(binding, property);
        };


        public static void DefaultBindingMode(Binding binding, PropertyInfo property)
        {
            var setMethod = property.GetSetMethod();
            binding.Mode = property.CanWrite && setMethod != null && setMethod.IsPublic ? BindingMode.TwoWay : BindingMode.OneWay;
        }

        /// <summary>
        /// Determines whether a custom string format is needed and applies it to the binding.
        /// </summary>
        public static Action<Binding, BindableObject, PropertyInfo> ApplyStringFormat = (binding, element, property) =>
        {
            DefaulyStringFormat(binding, property); //if i need to rethink based on dependencyobject, can do so.
        };

        public static void DefaulyStringFormat(Binding binding, PropertyInfo property)
        {
            if (typeof(DateTime).IsAssignableFrom(property.PropertyType))
                binding.StringFormat = "{0:MM/dd/yyyy}";
        }

        private static void TrySetCommand(object control, ICommand command)
        {
            if (!TrySetCommandBinding<Button>(control, Button.CommandProperty, command))
            {
                //may try other ones.  may even rethink as well.
                //for now, has to be done this way.
                TrySetCommandBinding<GraphicsCommand>(control, GraphicsCommand.CommandProperty, command);

                //TrySetCommandBinding<Hyperlink>(control, Hyperlink.CommandProperty, command);
            }
            //if i have other custom ones to set commands on, will do so.
            //because i could even do textboxes and f keys.
            //has to think about how i would map those.
            //that could require some rethinking.
            //i do have reflection one though.
            //i propose via bootstrap or something else to register.
            //i could even make something static for all classes or singleton pattern.
            //that way i can add new items to the list.

        }

        private static bool TrySetCommandBinding<T>(object control, BindableProperty property, ICommand command)
            where T : VisualElement
        {
            if (!(control is T commandSource))
                return false;

            if (commandSource.GetBinding(property) != null)
                return false; //because already there.  we don't want to override the ones done manually.
            commandSource.SetBinding(property, new Binding
            {
                Source = command
            });


            return true;
        }



    }
}
