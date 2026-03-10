
using System;
using UnityEngine;

namespace DynamicUI
{

    // implementation detail. Derive your items from ListItemUIObject<T> instead.
    public abstract class BaseListItemUIObject : MonoBehaviour
    {
        public abstract void UpdateBaseData(IListItemData data);

        public abstract bool IsItemDataType(IListItemData data);
        
        public abstract bool IsItemDataObject(IListItemData data);

    }
}