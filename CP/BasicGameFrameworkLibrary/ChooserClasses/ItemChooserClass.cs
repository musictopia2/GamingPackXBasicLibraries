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
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using BasicGameFrameworkLibrary.CommonInterfaces;
using BasicGameFrameworkLibrary.DIContainers;
//i think this is the most common things i like to do
namespace BasicGameFrameworkLibrary.ChooserClasses
{
    public class ItemChooserClass<O>
        where O : ISimpleValueObject<int>
    {
        private readonly RandomGenerator _rs;
        public CustomBasicCollection<O>? ValueList; //most likely something else will call into it.
        public int ItemToChoose(bool requiredToChoose = true, bool useHalf = true)
        {
            if (requiredToChoose == false)
            {
                int ask1; //decided to not use weighted average after all.
                if (useHalf == true)
                {
                    ask1 = _rs.GetRandomNumber(2);
                    if (ask1 == 1)
                        return -1;
                }
                else
                {
                    ask1 = _rs.GetRandomNumber(ValueList!.Count + 1);
                    if (ask1 == ValueList.Count + 1)
                        return -1; //0 based.
                }
            }
            return ValueList!.GetRandomItem().ReadMainValue;
        }
        public ItemChooserClass(IGamePackageResolver resolver) //could be iffy.
        {
            _rs = resolver.Resolve<RandomGenerator>();

        }
    }
}
