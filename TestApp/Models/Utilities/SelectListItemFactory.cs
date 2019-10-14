using System.Web.Mvc;

namespace TestApp.Models
{
    public static class SelectListItemFactory
    {

        public static SelectListItem CrateSecretQuestion(string text)
        {
            SelectListItem item = new SelectListItem()
            {
                Value = text,
                Text = text
            };
            return item;
        }
    }

}