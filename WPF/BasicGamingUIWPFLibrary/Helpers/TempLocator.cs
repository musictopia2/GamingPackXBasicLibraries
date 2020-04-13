using BasicControlsAndWindowsCore.MVVMFramework.ViewLinkersPlusBinders;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using CommonBasicStandardLibraries.MVVMFramework.UIHelpers;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static CommonBasicStandardLibraries.BasicDataSettingsAndProcesses.BasicDataFunctions;

namespace BasicGamingUIWPFLibrary.Helpers
{
    //go ahead and keep here.  so if i ever have other cases where i need to figure out where there are duplicate views, i can do it.
    public class TempLocator : IViewLocator
    {
        private readonly CustomBasicList<ViewModelViewLinker> _linkList = new CustomBasicList<ViewModelViewLinker>();
        public static CustomBasicList<ViewModelViewLinker> ManuelVMList = new CustomBasicList<ViewModelViewLinker>();

        public static Assembly GetDefaultAssembly => Assembly.GetEntryAssembly()!;

        private CustomBasicList<Type> GetFilteredList(Assembly assembly, string searchTerm)
        {
            return assembly.GetTypes().Where(x => string.IsNullOrWhiteSpace(x.Namespace) == false && x.Namespace!.EndsWith(searchTerm) && x.IsNested == false).ToCustomBasicList();
        }

        private void LoadLinkList()
        {
            if (AssemblyLinker.ViewModelAssembly == null)
                throw new BasicBlankException("Needs to have view model assembly in order to link list");
            if (AssemblyLinker.ViewAssembly == null)
                throw new BasicBlankException("Needs to have the view assembly in order to link list");
            Assembly mains = GetDefaultAssembly;
            CustomBasicList<Type> viewLists = GetFilteredList(AssemblyLinker.ViewAssembly, "Views");
            CustomBasicList<Type> viewModelLists = GetFilteredList(AssemblyLinker.ViewModelAssembly, "ViewModels");
            CustomBasicList<Type> mores;
            if (mains != AssemblyLinker.ViewAssembly)
            {
                //more for view assembly
                mores = GetFilteredList(mains, "Views");
                viewLists.AddRange(mores);
            }
            if (mains != AssemblyLinker.ViewModelAssembly)
            {
                //more for view model assembly.
                mores = GetFilteredList(mains, "ViewModels");
                viewModelLists.AddRange(mores);
            }

            AssemblyLinker.ExtraViewModelLocations.ForEach(x =>
            {
                mores = GetFilteredList(x, "ViewModels");
                viewModelLists.AddRange(mores);
            });
            AssemblyLinker.ExtraViewLocations.ForEach(x =>
            {
                mores = GetFilteredList(x, "Views");
                viewLists.AddRange(mores);
            });
            viewModelLists.ForEach(x =>
            {
                string firstName;
                string toUse;
                toUse = "ViewModel";
                string genericName = "`1";
                bool useGenerics = false;
                if (x.Name.EndsWith(genericName))
                {
                    useGenerics = true;
                }
                if (useGenerics == true)
                {
                    toUse += genericName;
                }
                if (x.Name.EndsWith(toUse) == false)
                {
                    toUse = "VM"; //only vm for abbreviation could be used sometimes.
                    if (useGenerics)
                    {
                        toUse += genericName;
                    }
                }
                if (x.Name.EndsWith(toUse) == true)
                {
                    firstName = x.Name.Substring(0, x.Name.Count() - toUse.Count());

                    Type type = viewLists.Where(y => y.Name == $"{firstName}View").SingleOrDefault();
                    if (type != null)
                        _linkList.Add(new ViewModelViewLinker
                        {
                            ViewModelType = x,
                            ViewType = type
                        });
                }


            });


            viewLists.ForEach(x =>
            {
                cons!.RegisterInstanceType(x);
            });
            //i like the idea of also registering the manuellist as well
            ManuelVMList.ForEach(x =>
            {
                cons!.RegisterInstanceType(x.ViewType!);
            });
            _linkList.AddRange(ManuelVMList); //hopefully this simple.
        }

        async Task<IUIView>? IViewLocator.LocateViewAsync(object viewModel)
        {
            await Task.CompletedTask;
            if (_linkList.Count == 0)
                LoadLinkList();
            Type vmType = viewModel.GetType();
            var typeFound = _linkList.Where(x => x.ViewModelType!.Name == vmType.Name).SingleOrDefault();
            if (typeFound == null)
                return null!;
            var result = cons!.GetInstance(typeFound.ViewType!);
            if (result is IUIView output)
            {
                output.DataContext = viewModel; //for xamarin forms, just convert datacontext to the bindingcontext.
                return output;
            }
            else
                throw new BasicBlankException("Found a view.  However, did not implement the IUIView interface");
        }
    }
}
