// -----------------------------------------------------------------------
// <copyright file="SqlItemManager.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SpecHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class SqlItemManager
    {
        private static List<SqlItem> Items = new List<SqlItem>();

        public static IEnumerable<ProgramItem> ProgramItems
        {
            get
            {
                for (int index = Items.Count - 1; index >= 0; index--)
                {
                    var item = Items[index];
                    if (item is ProgramItem)
                    {
                        yield return item as ProgramItem;
                    }
                }
            }
        }

        public static SqlItem GetRegisteredItem(string itemName)
        {
            foreach (var registeredItem in Items)
            {
                if (registeredItem.Name.Equals(itemName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return registeredItem;
                }
            }

            return null;
        }

        public static void RegisterItem(SqlItem item)
        {
            foreach (var registeredItem in Items)
            {
                if (registeredItem.Name.Equals(item.Name, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new ArgumentException("Item with the same name has registered");
                }
            }

            Items.Add(item);
        }

        public static void UnregisterItem(string itemName) 
        {
            var removedItem = GetRegisteredItem(itemName);
            if (Items.Contains(removedItem))
            {
                Items.Remove(removedItem);
            }
        }
        
        public static void Init()
        {
            Items.Clear();
        }
    }
}
