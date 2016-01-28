// -----------------------------------------------------------------------
// <copyright file="ColumnItem.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SpecHelper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public enum DataTypes
    {
        Undefined,
        NVarchar
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ColumnItem
    {
        public bool IsPK
        { get; set; }

        public bool IsLocalized
        { get; set; }

        public DataTypes DataType
        { get; set; }

        public string Name
        {
            get; private set;
        }

        public ColumnItem(string columnName)
        {
            Name = columnName;
        }
    }
}
