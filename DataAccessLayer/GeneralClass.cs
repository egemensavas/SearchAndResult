using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class GeneralClass
    {
        public IEnumerable<TABLE_ADVERT> GetAdvertData(int SearchMasterID)
        {
            SAHIBINDENDBEntities entities = new SAHIBINDENDBEntities();
            if(SearchMasterID == 0)
                return entities.TABLE_ADVERT.ToList<TABLE_ADVERT>();
            return entities.TABLE_ADVERT.Where(x => x.SearchMasterID == SearchMasterID && x.IsDeleted == false).ToList<TABLE_ADVERT>();
        }

        public string NotificationMessage(int SearchMasterID)
        {
            SAHIBINDENDBEntities entities = new SAHIBINDENDBEntities();
            var temp = entities.VIEW_NOTIFICATION.Where(x => x.SearchMasterID == SearchMasterID);
            string result = string.Empty;
            if (temp != null && temp.Count() > 0)
                result = temp.First().NotificationMessage;
            return result;
        }

        public void UpdateSeen(int SearchMasterID)
        {
            using (var db = new SAHIBINDENDBEntities())
            {
                var result = db.TABLE_ADVERT.Where(b => b.IsSeen == false && b.SearchMasterID == SearchMasterID);
                if (result != null)
                    foreach (var item in result)
                        item.IsSeen = true;
                db.SaveChanges();
            }
        }
    }
}
