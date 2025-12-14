using System.Collections.Generic;
using System.Linq;

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

        public TechItem GetTechItemById(string id)
        {
            foreach (var techCat in this)
            {
                foreach (var techitem in techCat.TechItems)
                {
                    if (techitem.Id == id)
                    {
                        return techitem;
                    }
                }
            }
            return null;
        }

        public void SafeAddTechCat(TechCategory techCategory)
        {
            if (this.Where(t => t.Name == techCategory.Name).Count() == 0)
            {
                this.Add(techCategory);
            }
        }


    }
}
