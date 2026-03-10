using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace DynamicUI
{
    public class DynamicList : MonoBehaviour
    {
        [Tooltip("If not set in the inspector, these will be set in Awake")]
        public BaseListItemUIObject[] listItemOriginals;

        private List<BaseListItemUIObject> listItems = new();

        public bool followOriginalSiblingIndex;

        protected virtual void Awake()
        {
            //if (listItemOriginal == null)
            //    listItemOriginal = GetComponentInChildren<BaseListItemUIObject>();
            //listItemOriginal.gameObject.SetActive(false);

            if (listItemOriginals == null || listItemOriginals.Length == 0)
                listItemOriginals = GetComponentsInChildren<BaseListItemUIObject>();

            foreach (var l in listItemOriginals)
            {
                l.gameObject.SetActive(false);
            }
        }

        public BaseListItemUIObject[] GetItems()
        {
            return listItems.ToArray();
        }

        public void SetItems<T>(IEnumerable<T> itemDataList) where T : IListItemData
        {
            if (itemDataList == null)
            {
                Debug.LogError("itemDataList is null", gameObject);
                return;
            }

            RemoveAllItems();
            foreach (T itemData in itemDataList)
            {
                AddItem(itemData);
            }
        }

        public void RemoveAllItems()
        {
            var listItemObjects = GetItems();
            if (listItemObjects != null)
            {
                foreach (var listItem in listItemObjects)
                {
                    RemoveItem(listItem);
                }
            }
            listItems.Clear();
        }

        public BaseListItemUIObject AddItem(IListItemData itemData)
        {
            var original = GetUIObjectOriginalFromData(itemData);
            if (original == null)
            {
                Debug.LogError("No suitable original found for data type: " + itemData.GetType().Name, gameObject);
                return null;
            }
            BaseListItemUIObject listItem = Instantiate(original, original.transform.parent);
            if (followOriginalSiblingIndex)
            {
                listItem.transform.SetSiblingIndex(original.transform.GetSiblingIndex());
            }
            listItem.gameObject.SetActive(true);
            listItem.UpdateBaseData(itemData);
            listItems.Add(listItem);
            return listItem;
        }

        private BaseListItemUIObject GetUIObjectOriginalFromData(IListItemData itemData)
        {
            foreach (var listItem in listItemOriginals)
            {
                if (listItem.IsItemDataType(itemData))
                {
                    return listItem;
                }
            }
            Debug.LogError("No original found for data type: " + itemData.GetType().Name, gameObject);
            return null;
        }

        // it is allowed to just Destroy the gameObject yourself, if that suits you.
        public void RemoveItem(BaseListItemUIObject itemObject)
        {
            if (itemObject != null)
            {
                if (listItems.Contains(itemObject))
                    listItems.Remove(itemObject);
                Destroy(itemObject.gameObject);
            }
        }

        // it is allowed to just Destroy the gameObject yourself, if that suits you.
        public void RemoveItem(int index)
        {
            var listItem = listItems[index];
            RemoveItem(listItem);
        }


    }
}