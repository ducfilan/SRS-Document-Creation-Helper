// -----------------------------------------------------------------------
// <copyright file="SqlItem.cs" company="">
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
    public abstract class SqlItem
    {
        public string Name { get; private set; }

        protected SqlItem(string itemName)
        {
            Name = itemName;
            SqlItemManager.RegisterItem(this);
        }

        public abstract void UpdateReferences();

        public abstract Status UpdateSchema();
    }
}
