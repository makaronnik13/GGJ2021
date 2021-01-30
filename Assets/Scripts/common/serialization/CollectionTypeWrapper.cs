using System;
using System.Collections;
using System.Collections.Generic;
using com.armatur.common.save;
using com.armatur.common.util;

namespace com.armatur.common.serialization
{
    public class CollectionTypeWrapper : ComplexTypeWrapper
    {
        public readonly TypeWrapper ElementTypeWrapper;

        public CollectionTypeWrapper(Type type) : base(type)
        {
            Type elementType;
            type.IsIEnumerable(out elementType);
            ElementTypeWrapper = TypeWrapperPool.Instance.GetWrapper(elementType);
            var testSet = new HashSet<string>();
            ElementTypeWrapper.AvailableTypes.ForEach(s =>
            {
                testSet.Add(s);
            });
            Members.ForEach(memberWrapper =>
            {
                if (!(memberWrapper.AllowInheritance && memberWrapper.OmitOuterName))
                    return;
                memberWrapper.GetAvailableTypes().ForEach(s =>
                {
                    if (!testSet.Add(s))
                        throw new Exception(SerializeUtils.GetTypeFriendlyName(Type) + ": Several members with duplicate known types " + s);
                });
            });
        }

        protected override bool IsOneValueInternal()
        {
            return Members.Count == 0;
        }

        public override void Save(SaveProcessor processor, object o)
        {
            base.Save(processor, o);
            foreach (var element in (IEnumerable) o)
            {
                var typeWrapper = TypeWrapperPool.Instance.GetWrapper(element.GetType());
                if (processor.AddLevel(typeWrapper.Name(), false, true))
                {
                    typeWrapper.Save(processor, element);
                    processor.RemoveOneLevel(false);
                }
            }
        }

        public override bool Load(SaveProcessor processor, ref object o)
        {
            if (!ReflectionUtils.IsList(o)) return false;
            var list = o as IList;
            processor.ForEachLevel(s =>
            {
                var typeWrapper = ElementTypeWrapper.GetPolymorphicTypeWrapper(s);
                if (typeWrapper == null) return;
                var elem = typeWrapper.CreateDefault();
                typeWrapper.Load(processor, ref elem);

                /*
                // replace element if same there is element with the same Id in the collection
                var indexedElem = elem as IIndexedItem;
                if (indexedElem != null)
                {
                    foreach (var item in list)
                    {
                        var indexedItem = item as IIndexedItem;
                        if (indexedItem != null)
                        {
                            if (indexedElem.Id == indexedItem.Id)
                            {
                                MiscUtil.MapObject(item, elem, true);
                                list?.Remove(item);
                                break;
                            }
                        }
                    }
                }
                */

                list?.Add(elem);
            });
            return base.Load(processor, ref o);
        }
    }
}