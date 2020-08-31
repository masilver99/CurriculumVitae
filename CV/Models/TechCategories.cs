using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CV.Models
{
    public class TechCategories : List<TechCategory>
    {
        public TechCategories()
        {
        }

        public TechCategories(List<TechCategory> techCategories)
        {
            this.AddRange(techCategories);
        }

        public TechItem GetTechItemByName(string name)
        {
            foreach (var techCat in this)
            {
                foreach (var techitem in techCat.Items)
                {
                    if (techitem.Name == name)
                    {
                        return techitem;
                    }
                }
            }
            return null;
        }

        public TechItem GetTechItemByName(string name)
        {
            foreach (var techCat in this)
            {
                foreach (var techitem in techCat.Items)
                {
                    if (techitem.Name == name)
                    {
                        return techitem;
                    }
                }
            }
            return null;
        }

        public void SafeAddTechCat(TechCategory techCategory)
        {
            if (this.Where(t => t.Category == techCategory.Category).Count() == 0)
            {
                this.Add(techCategory);
            }
        }


    }
}
