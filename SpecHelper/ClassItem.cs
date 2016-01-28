using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpecHelper
{
    public class ClassItem
    {
        public string Name { get; set; }

        // Add equivalent to ParentItem
        private readonly List<ProgramItem> _programItems = new List<ProgramItem>();
        public IEnumerable<ProgramItem> ProgramItems
        {
            get
            {
                foreach(var item in _programItems)
                {
                    yield return item;
                }
            }
        }

        public void AddProgramItem(string itemName)
        {
            if(SqlItemManager.GetRegisteredItem(itemName) != null)
            {
                _programItems.Add(SqlItemManager.GetRegisteredItem(itemName) as ProgramItem);
            }
            else
            {
                _programItems.Add(new ProgramItem(itemName));
            }
        }

        public void UpdateReferences()
        {
            var connection = ConnectionManager.GetConnection();
            if (connection != null)
            {
                foreach (var item in _programItems)
                {
                    item.UpdateReferences();
                }
            }
        }
    }
}
