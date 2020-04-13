using System;
using System.Text;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using System.Linq;
using CommonBasicStandardLibraries.BasicDataSettingsAndProcesses;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;
using CommonBasicStandardLibraries.CollectionClasses;
using System.Threading.Tasks; //most of the time, i will be using asyncs.
using fs = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.FileHelpers;
using js = CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.JsonSerializers.NewtonJsonStrings; //just in case i need those 2.
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using CommonBasicStandardLibraries.MVVMFramework.ViewModels;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using CommonBasicStandardLibraries.MVVMFramework.Conductors;
using BasicGameFrameworkLibrary.CommonInterfaces;
using System.Reflection;
using BasicGameFrameworkLibrary.Attributes;
using CommonBasicStandardLibraries.MVVMFramework.Attributes;
using System.Windows.Data;
using CommonBasicStandardLibraries.MVVMFramework.Commands;
using System.Windows.Input;
using BasicGameFrameworkLibrary.ViewModelInterfaces;
using BasicGameFrameworkLibrary.CommandClasses;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using BasicControlsAndWindowsCore.Controls.NavigationControls;
using BasicGamingUIWPFLibrary.GameGraphics.Base;
using BasicGamingUIWPFLibrary.Bootstrappers;
//i think this is the most common things i like to do
namespace BasicGamingUIWPFLibrary.Helpers
{

    internal class BindHelpClass
    {
        public bool IsValid { get; set; }
        public DependencyProperty? Dependency { get; set; }
    }

    public static class GamePackageViewModelBinder
    {



        private static readonly BooleanToVisibilityConverter _booleanToVisibilityConverter = new BooleanToVisibilityConverter();
        //maybe the built in one will work (?)

        private static readonly Dictionary<Type, DependencyProperty> _boundProperties = new Dictionary<Type, DependencyProperty>()
        {
            //{typeof(TextBox), TextBox.TextProperty },
            //{typeof(CustomTextbox), CustomTextbox.TextProperty },
            //{typeof(Combo), Combo.ItemsSourceProperty }, //i think.
            {typeof(TextBlock), TextBlock.TextProperty },
            {typeof(Grid), UIElement.VisibilityProperty },
            {typeof(StackPanel), UIElement.VisibilityProperty},
            //{typeof(ComboBox), ItemsControl.ItemsSourceProperty }, //had to be combobox and not generic version unfortunately.
            //{typeof(MultiLineTextbox), TextBox.TextProperty },
            //{typeof(CustomCalender), Calendar.SelectedDateProperty },
            //{typeof(SimpleReader), SimpleReader.ItemsSourceProperty },
            //{typeof(ListBox), ItemsControl.ItemsSourceProperty }, //needs this for experimenting.
            //{typeof(ItemsControl), ItemsControl.ItemsSourceProperty }, //this is needed for the data entry forms.
            //{typeof(ListView), ItemsControl.ItemsSourceProperty } //this is needed for listview.
            //may need extensions to add other custom stuff.  not sure though.


        };

        //for now, i will not give the ability to handle the ones where they could not be matched.
        //has to be done manually.  maybe later can experiment further.


        public static bool HasBinding(FrameworkElement element, DependencyProperty property)
        {
            return element.GetBindingExpression(property) != null;
        }

        /// <summary>
        /// this is the process that needs to hook up the proper parent control
        /// not only to add to proper control
        /// but also to hook up to parent container so it can be removed later.
        /// </summary>
        /// <param name="parentViewModel"></param>
        /// <param name="parentViewScreen"></param>
        /// <param name="childViewModel"></param>
        public static void HookParentContainers(object parentViewModel, UIElement parentViewScreen, IScreen childViewModel, IUIView childViewScreen)
        {
            CustomBasicList<FrameworkElement> controls = FindVisualChildren(parentViewScreen).ToCustomBasicList();


            if (controls.Count == 0)
            {
                var nextScreen = (ContentControl)parentViewScreen;
                controls = FindVisualChildren((DependencyObject)nextScreen.Content).ToCustomBasicList();
                //if (controls.Count == 0)

                //    if (content is FrameworkElement cc)


                //        controls = new CustomBasicList<FrameworkElement>() { cc };

            }

            controls.KeepConditionalItems(x =>
            {
                if (string.IsNullOrWhiteSpace(x.Name) == true)
                    return false;
                Type type = x.GetType();
                if (typeof(IContentControl).IsAssignableFrom(type))
                    return true;
                return false;
            });


            if (controls.Count == 0)
            {
                //var nextList = FindVisualChildren(parentViewScreen).ToCustomBasicList();
                //CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.VBCompat.Stop();
                throw new BasicBlankException("There was no container controls");
            }



            Type type = parentViewModel.GetType();
            var properties = type.GetProperties().ToCustomBasicList();

            CustomBasicList<IContentControl> parents = new CustomBasicList<IContentControl>();
            bool isGuarantee = false;

            if (typeof(IConductorGuarantee).IsAssignableFrom(type))
                isGuarantee = true;


            //i think if there is no match, needs to raise exception for sure.
            foreach (var control in controls)
            {
                var property = properties.Where(x => x.Name == control.Name).SingleOrDefault();
                if (property == null)
                    throw new BasicBlankException($"Parent container with the name of {control.Name} could not be found.  Rethink");
                IContentControl? fins = (IContentControl)control;
                if (fins == null)
                {
                    throw new BasicBlankException("Was not parent.  Rethink");
                }
                else if (isGuarantee)
                {
                    parents.Add(fins);
                }
                else
                {
                    object thisObj = property.GetValue(parentViewModel, null)!;
                    if (thisObj != null)
                    {
                        if (!(thisObj is IMainScreen))
                        {
                            throw new BasicBlankException($"The control with the name of {control.Name} was wrong because the control did not implement IMainScreen");
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
                }
            }
            if (parents.Count == 0)
            {
                throw new BasicBlankException("There was no parent controls found for hooking up the child");
            }
            if (parents.Count == 1)
            {
                childViewModel.ParentContainer = parents.Single();
                childViewModel.ParentContainer.Add(childViewScreen);

            }
            else
            {

            }
        }

        //private CustomBasicList<FrameworkElement> GetBasic

        public static void Bind(object viewModel, DependencyObject view, DependencyObject? content = null)
        {
            Type type = viewModel.GetType();
            if (!(view is FrameworkElement element))
                return;
            var viewType = viewModel.GetType();
            var properties = viewType.GetProperties();
            var methods = viewType.GetMethods().ToCustomBasicList();

            CustomBasicList<FrameworkElement> controls;


            controls = FindVisualChildren(view).ToCustomBasicList();
            if (controls.Count == 0 && content != null)
            {
                controls = FindVisualChildren(content).ToCustomBasicList();
                if (controls.Count == 0)

                    if (content is FrameworkElement cc)


                        controls = new CustomBasicList<FrameworkElement>() { cc };
            }
            controls.AddRange(ManuelElements);
            if (controls.Count == 0)
                throw new BasicBlankException("There was no controls.  Rethink");
            //if (!_boundProperties.TryGetValue(foundControl.GetType(), out boundProperty!))
            //    continue;
            BindProperties(controls,
                type,
                properties,
                (list, p) =>
                {
                    return list.Where(x => x.Name.StartsWith(p.Name, StringComparison.InvariantCultureIgnoreCase)).ToCustomBasicList();
                }, item =>
                {
                    bool rets = _boundProperties.TryGetValue(item, out DependencyProperty? d);
                    BindHelpClass bind = new BindHelpClass();
                    bind.IsValid = rets;
                    bind.Dependency = d;
                    return bind;
                }, item =>
                {
                    return item.Name;
                }
                );

            

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
            BindStandardCommands(controls, viewModel, methods, properties, pList);

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




        private static void BindProperties(CustomBasicList<FrameworkElement> controls,
            Type viewmodelType,
            IEnumerable<PropertyInfo> properties,
            Func<CustomBasicList<FrameworkElement>, PropertyInfo, CustomBasicList<FrameworkElement>> listFunct,
            Func<Type, BindHelpClass> bindInfo,
            Func<FrameworkElement, string> stringName
            )
        {
            foreach (var property in properties)
            {

                var firstList = listFunct(controls, property);

                //var firstList = controls.Where(x => x.Name.StartsWith(property.Name, StringComparison.InvariantCultureIgnoreCase)).ToCustomBasicList();
                //if (firstList.Count > 1)
                //    throw new BasicBlankException($"{property.Name} showed 2 controls with the name.  Only one is allowed for now for properties.  If the problem was an _ then really requires rethinking.  Most likely had wrong names.  If I am wrong, rethink");
                if (firstList.Count == 0)
                    continue;

                //looks like we may have more than one now.   that could be true for cases like dates.
                //where it needs to be displayed in more than one place.
                foreach (var foundControl in firstList)
                {
                    if (foundControl == null)
                        continue;
                    DependencyProperty boundProperty;

                    BindHelpClass helps = bindInfo.Invoke(foundControl.GetType());
                    if (helps.IsValid == false)
                    {
                        continue; //because we already have binding.
                    }
                    boundProperty = helps.Dependency!;
                    //if (!_boundProperties.TryGetValue(foundControl.GetType(), out boundProperty!))
                    //    continue;
                    if (foundControl.GetBindingExpression(boundProperty) != null)
                        //because if it had to be done manually, don't want to override it.
                        //this means i can do manually if necessary.
                        continue;
                    var interpretedViewModelType = viewmodelType;
                    var interpretedProperty = property;
                    //not always name.
                    var stringPath = stringName(foundControl);
                    //var stringPath = foundControl.Name;
                    stringPath = stringPath.Replace("_", "."); //i can try to do here.
                    CustomBasicList<string> cList = stringPath.Split(".").ToCustomBasicList();
                    cList.RemoveFirstItem(); //i like this idea.
                    foreach (var item in cList)
                    {
                        interpretedViewModelType = interpretedProperty.PropertyType;
                        var nList = interpretedViewModelType.GetProperties(x => x.Name == item).ToCustomBasicList();
                        if (nList.Count == 0)
                            throw new BasicBlankException($"Unable to get the property for linked properties.  The full string was {foundControl.Name}.  The one unfound was {item}");
                        if (nList.Count > 1)
                            throw new BasicBlankException($"There was more than one match for linked properties.  The full string was {foundControl.Name}.  The one with duplicate was {item}");
                        interpretedProperty = nList.Single();
                    }



                    var binding = new Binding(stringPath);


                    ApplyBindingMode(binding, interpretedProperty);
                    ApplyValidation(binding, interpretedViewModelType, interpretedProperty, foundControl);



                    ApplyValueConverter(binding, boundProperty!, interpretedProperty);

                    ApplyUpdateSourceTrigger(boundProperty!, foundControl, binding, interpretedProperty);
                    ApplyStringFormat(binding, foundControl, interpretedProperty);
                    BindingOperations.SetBinding(foundControl, boundProperty, binding);


                }

                //var foundControl = firstList.Single();

            }
        }




        private static void BindStandardCommands(CustomBasicList<FrameworkElement> controls, object viewModel, IEnumerable<MethodInfo> methods, IEnumerable<PropertyInfo> properties, IEnumerable<MethodInfo> predicates)
        {
            //throw new BasicBlankException("Bind standard help");
            Type type = viewModel.GetType();

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
                searchName = CommandHelpers.GetSearchName(method);
                var controlList = controls.Where(x => x.Name.Equals(searchName, StringComparison.InvariantCultureIgnoreCase) || x.Name.Equals(method.Name, StringComparison.InvariantCultureIgnoreCase)).ToCustomBasicList();
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

        //private static void TryCustomCommand(object control, )

        private static void TrySetCommand(object control, ICommand command)
        {
            if (!TrySetCommandBinding<ButtonBase>(control, ButtonBase.CommandProperty, command))
            {
                //may try other ones.  may even rethink as well.
                //for now, has to be done this way.
                //started out with buttonbase.
                TrySetCommandBinding<GraphicsCommand>(control, GraphicsCommand.CommandProperty, command);
                //TrySetCommandBinding<BaseGraphicsWPF<>>


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

        private static bool TrySetCommandBinding<T>(object control, DependencyProperty property, ICommand command)
            where T : FrameworkElement
        {
            if (!(control is T commandSource))
                return false;

            if (commandSource.GetBindingExpression(property) != null)
                return false; //because already there.  we don't want to override the ones done manually.

            BindingOperations.SetBinding(commandSource, property, new Binding
            {
                Source = command
            });


            return true;
        }


        public static CustomBasicList<FrameworkElement> ManuelElements = new CustomBasicList<FrameworkElement>();

        //internal static bool StepThrough { get; set; }

        public static IEnumerable<FrameworkElement> FindVisualChildren(DependencyObject depObj)
        {
            if (depObj != null)
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)

                {

                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);


                    //when i try that idea. it gets worse.


                    //if (child != null && child is Grid grid)
                    //{
                    //    foreach (FrameworkElement? childOfGrid in grid.Children)
                    //    {
                    //        //if (StepThrough)
                    //        //{
                    //        //    CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.VBCompat.Stop();
                    //        //}
                    //        yield return childOfGrid!;
                    //    }
                    //}

                    if (child != null && child is FrameworkElement element && string.IsNullOrWhiteSpace(element.Name) == false)

                        yield return element;

                    if (child is ParentSingleUIContainer)
                    {
                        continue; //hopefully this does not cause another problem.  will probably have to fix this in the game package as well.
                    }



                    foreach (FrameworkElement childOfChild in FindVisualChildren(child!))
                        yield return childOfChild;
                }
        }
        /// <summary>
        /// this is the default formula for the validations.
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="viewModelType"></param>
        public static void DefaultValidation()
        {
            //no validation because this is for the game package.
            //will have others but need to start out with a proof of concept.

            //if (typeof(IDataErrorInfo).IsAssignableFrom(viewModelType))
            //{
            //    if (control is MultiLineTextbox text)
            //    {
            //        binding.ValidatesOnDataErrors = text.ValidateWhileTyping;
            //    }
            //    else
            //    {
            //        binding.ValidatesOnDataErrors = true;
            //    }
            //    binding.ValidatesOnExceptions = true;
            //}
        }


        /// <summary>
        /// Determines whether or not and what type of validation to enable on the binding.
        /// </summary>
        public static Action<Binding, Type, PropertyInfo, DependencyObject> ApplyValidation = (binding, viewModelType, property, control) =>
        {
            DefaultValidation();


        };

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
        /// Determines whether a value converter is is needed and applies one to the binding.
        /// </summary>
        public static Action<Binding, DependencyProperty, PropertyInfo> ApplyValueConverter = (binding, bindableProperty, property) =>
        {
            DefaultValueConverter(binding, bindableProperty, property);
        };

        public static void DefaultValueConverter(Binding binding, DependencyProperty bindableProperty, PropertyInfo property)
        {
            if (bindableProperty == UIElement.VisibilityProperty && typeof(bool).IsAssignableFrom(property.PropertyType))
            {
                binding.Converter = _booleanToVisibilityConverter;
            }
            //if (bindableProperty == CustomTextbox.TextProperty && (typeof(DateTime).IsAssignableFrom(property.PropertyType) || typeof(DateTime?).IsAssignableFrom(property.PropertyType)))
            //{
            //    binding.Converter = new DateConverter(); //i think will always use my custom date converter if using custom textbox and its assignable from date and time.
            //}
        }

        /// <summary>
        /// Determines whether a custom update source trigger should be applied to the binding.
        /// </summary>
        public static Action<DependencyProperty, DependencyObject, Binding, PropertyInfo> ApplyUpdateSourceTrigger = (bindableProperty, element, binding, info) =>
        {
            DefaultUpdateSourceTrigger(binding);
        };

        public static void DefaultUpdateSourceTrigger(Binding binding)
        {
            //could require rethinking with dates.
            binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //only custom textboxes need the special part for dates.

        }

        /// <summary>
        /// Determines whether a custom string format is needed and applies it to the binding.
        /// </summary>
        public static Action<Binding, DependencyObject, PropertyInfo> ApplyStringFormat = (binding, element, property) =>
        {
            DefaulyStringFormat(binding, property); //if i need to rethink based on dependencyobject, can do so.
        };

        public static void DefaulyStringFormat(Binding binding, PropertyInfo property)
        {
            if (typeof(DateTime).IsAssignableFrom(property.PropertyType) && property.HasAttribute<FullDateAttribute>() == false)
            {
                binding.StringFormat = "{0:MM/dd/yyyy}";
            }
        }
    }
}
