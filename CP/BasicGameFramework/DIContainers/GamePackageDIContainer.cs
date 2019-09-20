using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.BasicExtensions;
using CommonBasicStandardLibraries.AdvancedGeneralFunctionsAndProcesses.RandomGenerator;
using CommonBasicStandardLibraries.CollectionClasses;
using CommonBasicStandardLibraries.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BasicGameFramework.DIContainers
{
    public class GamePackageDIContainer : IGamePackageResolver, IGamePackageRegister, IGamePackageDIContainer
    {
        private readonly HashSet<ContainerData> _thisSet = new HashSet<ContainerData>(); //still decided to use hashset.
        private readonly HashSet<string> _trueList = new HashSet<string>(); //this is all the tags that are set to true.
        private readonly object _lockObj = new object(); //try to prevent issues.
        private readonly RandomGenerator _rans; //this is most common.
        public GamePackageDIContainer()
        {
            _rans = new RandomGenerator();
        }
        public bool LookUpValue(string tag)
        {
            lock (_lockObj)
            {
                return _trueList.Contains(tag.ToLower());
            }
        }
        public bool ObjectExist<T>(string tag)
        {
            lock (_lockObj)
            {
                Type thisType = typeof(T);
                return _thisSet.Any(Items => thisType.IsAssignableFrom(Items.TypeOut) && Items.Tag == tag);
            }
        }
        public bool ObjectExist<T>()
        {
            return ObjectExist<T>(""); //has to be no tag period this time.
        }
        private void SetResults(ContainerData thisResults, string tag)
        {
            thisResults.Tag = tag;
            _thisSet.Add(thisResults);
        }
        private object PrivateInstance(Type thisType)
        {
            var constructor = thisType.GetConstructors().OrderByDescending(Items => Items.GetParameters().Length).FirstOrDefault(); //in the video, its first or default.
            var args = constructor.GetParameters().Select(Items => PrivateResolve(Items.ParameterType)).ToArray(); //for his test.  i could decide to use my custom collection instead.
            return Activator.CreateInstance(thisType, args);
        }
        public void RegisterType<TIn>(bool isSingleton = true) //i think if you want to register a type, you are not allowed to use a factory.
        {
            ContainerData thisResults = new ContainerData()
            {
                IsSingle = isSingleton,
                TypeOut = typeof(TIn),
                TypeIn = typeof(TIn),
                GetNewObject = new Func<object>(() => PrivateInstance(typeof(TIn)))
            };
            SetResults(thisResults, "");
        }
        void IGamePackageRegister.RegisterSingleton(Type thisType)
        {
            ContainerData thisResults = new ContainerData()
            {
                TypeOut = thisType,
                TypeIn = thisType,
                IsSingle = true,
                GetNewObject = new Func<object>(() => PrivateInstance(thisType))
            };
            SetResults(thisResults, "");
        }
        public void RegisterSingleton<TIn, TOut>() where TOut : TIn
        {
            ContainerData ThisResults = new ContainerData()
            {
                IsSingle = true,
                TypeOut = typeof(TOut), //i think
                TypeIn = typeof(TIn),
                GetNewObject = new Func<object>(() => PrivateInstance(typeof(TOut)))
            };
            SetResults(ThisResults, "");
        }
        public void RegisterSingleton<TIn, TOut>(string tag)
        {
            ContainerData thisResults = new ContainerData()
            {
                IsSingle = true,
                TypeOut = typeof(TOut),
                TypeIn = typeof(TIn),
                GetNewObject = new Func<object>(() => PrivateInstance(typeof(TOut)))
            };
            SetResults(thisResults, tag);
        }
        public void RegisterSingleton<TIn>(TIn ourObject, string tag = "")
        {
            if (ourObject == null)
                throw new BasicBlankException("You can't register an object that does not exist.  Most likely, you tried to register it too soon.");
            ContainerData thisResults = new ContainerData()
            {
                IsSingle = true,
                TypeIn = typeof(TIn),
                TypeOut = ourObject.GetType(),
                ThisObject = ourObject
            };
            SetResults(thisResults, tag);
        }
        public void RegisterSingleton(Type thisType, string tag)
        {
            ContainerData thisResults = new ContainerData()
            {
                IsSingle = true,
                TypeOut = thisType,
                TypeIn = thisType,
                GetNewObject = new Func<object>(() => PrivateInstance(thisType))
            };
            SetResults(thisResults, tag);
        }
        public void RegisterTrue(string tag)
        {
            if (_trueList.Contains(tag.ToLower()) == true)
                throw new BasicBlankException($"{tag} was already registered as true.  Maybe ignore.  Rethink");
            _trueList.Add(tag.ToLower());
        }
        public void ReplaceObject<T>(T newObject)
        {
            ReplaceObject<T>(newObject, "");
        }
        public void ReplaceObject<T>(T newObject, string tag)
        {
            lock (_lockObj)
            {
                Type thisType = typeof(T);
                try
                {
                    ContainerData thisData = _thisSet.Where(Items => thisType.IsAssignableFrom(Items.TypeOut) && Items.IsSingle == true && Items.Tag == tag).Single();
                    thisData.ThisObject = newObject;
                }
                catch (Exception ex)
                {
                    throw new BasicBlankException($"Unable to replace object.  The type you were trying to replace is {thisType.Name}.  Error was {ex.Message}");
                }
            }
        }
        public void ReplaceRegistration<TIn, TOut>() //i think if we are replacing, try with no tags.
        {
            lock (_lockObj)
            {
                Type thisType = typeof(TIn);
                CustomBasicList<ContainerData> tempList = _thisSet.Where(Items => thisType.IsAssignableFrom(Items.TypeOut)).ToCustomBasicList();
                if (tempList.Count == 0)
                    throw new BasicBlankException($"Nothing registered with requesting type of {thisType.Name} to replace registration");
                if (tempList.Count > 1)

                {
                    tempList = tempList = _thisSet.Where(Items => Items.TypeIn == thisType).ToCustomBasicList();
                    if (tempList.Count == 0)
                        throw new BasicBlankException($"Nothing registered with requesting type of {thisType.Name} when attempting to use in for replacing registrations");
                    else if (tempList.Count > 1)
                        throw new BasicBlankException($"Was a duplicate.  Registered {tempList.Count} for requesting type of {thisType.Name}  Happened even when using in argument.  Was trying to replace registration");
                }
                ContainerData thisData = tempList.Single();
                thisData.ThisObject = null; //because its now null.
                thisData.GetNewObject = new Func<object>(() => PrivateInstance(typeof(TOut)));
            }
        }
        public void DeleteRegistration<TIn>()
        {
            lock (_lockObj)
            {
                Type thisType = typeof(TIn);
                CustomBasicList<ContainerData> tempList = _thisSet.Where(Items => thisType.IsAssignableFrom(Items.TypeOut)).ToCustomBasicList();
                tempList.ForEach(Items =>
                {
                    _thisSet.Remove(Items);
                });
            }
        }
        private object PrivateResolve(Type thisType)
        {
            if (thisType.IsAssignableFrom(typeof(RandomGenerator)))
            {
                return _rans;
            }
            CustomBasicList<ContainerData> tempList = _thisSet.Where(Items => thisType.IsAssignableFrom(Items.TypeOut) && Items.Tag == "").ToCustomBasicList();
            if (tempList.Count == 0)
                throw new BasicBlankException($"Nothing registered for parameters with type of {thisType.Name}");
            if (tempList.Count > 1)
                throw new BasicBlankException($"Was a duplicate.  Registered {tempList.Count} for parameters of type of {thisType.Name}");
            ContainerData thisData = tempList.Single();
            if (thisData.IsSingle == true && thisData.ThisObject != null)
            {
                return thisData.ThisObject;
            }
            if (thisData.IsSingle == true)
            {
                if (thisData.GetNewObject == null)
                    throw new BasicBlankException("Must have a custom function");
                thisData.ThisObject = thisData.GetNewObject();
                return thisData.ThisObject;
            }
            if (thisData.GetNewObject == null)
                throw new BasicBlankException("Must have a custom function");
            return thisData.GetNewObject();
        }
        public T Resolve<T>(string tag)
        {
            if (typeof(T) == typeof(RandomGenerator)) //this is an exception.
            {
                object thisObj = _rans;
                return (T)thisObj;
            }
            lock (_lockObj)
            {
                Type thisType = typeof(T);
                CustomBasicList<ContainerData> tempList = _thisSet.Where(Items => thisType.IsAssignableFrom(Items.TypeOut) && Items.Tag == tag).ToCustomBasicList();
                if (tempList.Count == 0)
                    throw new BasicBlankException($"Nothing registered with tag '{tag}' and requesting type of {thisType.Name}");
                if (tempList.Count > 1)

                {
                    tempList = tempList = _thisSet.Where(Items => Items.TypeIn == thisType && Items.Tag == tag).ToCustomBasicList();
                    if (tempList.Count == 0)
                        throw new BasicBlankException($"Nothing registered with tag '{tag}' and requesting type of {thisType.Name} when attempting to use in");
                    else if (tempList.Count > 1)
                        throw new BasicBlankException($"Was a duplicate.  Registered {tempList.Count} for tag '{tag}' and requesting type of {thisType.Name}  Happened even when using in argument");
                }
                ContainerData thisData = tempList.Single();
                if (thisData.IsSingle == true && thisData.ThisObject != null)
                {
                    return (T)thisData.ThisObject;
                }
                //now needs to think about how to resolve.
                if (thisData.IsSingle == true)
                {
                    if (thisData.GetNewObject == null)
                        throw new BasicBlankException("Must have a custom function");
                    thisData.ThisObject = thisData.GetNewObject();
                    return (T)thisData.ThisObject;
                }
                if (thisData.GetNewObject == null)
                    throw new BasicBlankException("Must have a custom function");
                return (T)thisData.GetNewObject();
            }
        }
        public T Resolve<T>()
        {
            return Resolve<T>("");
        }
    }
}