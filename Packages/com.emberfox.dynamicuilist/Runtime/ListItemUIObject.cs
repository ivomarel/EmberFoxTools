using UnityEngine;
using System.Collections;

namespace DynamicUI
{
    // make your UI items derive from this.
    public abstract class ListItemUIObject<T> : BaseListItemUIObject where T : IListItemData
    {
        protected T data;
        
        public override void UpdateBaseData(IListItemData data)
        {
            this.data = (T)data;
            UpdateData((T)data);
        }
        // override this to update your UI.
        public abstract void UpdateData(T newData);

        public T GetData()
        {
            return data;
        }

        public override bool IsItemDataType(IListItemData data)
        {
            return data is T;
        }

        public override bool IsItemDataObject(IListItemData data)
        {
            return ReferenceEquals(this.data, data);
        }
    }
}

