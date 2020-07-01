using static SahibindenWinForm.Models.GeneralModel;

namespace SahibindenWinForm.Classes
{
    public class HTMLCriteriaClass
    {
        #region Properties
        public SearchCriteria PriceCriteria { get; set; }
        public SearchCriteria EmptyCriteria { get; set; }
        public SearchCriteria AdvertLinkCriteria { get; set; }
        public SearchCriteria DescriptionCriteria { get; set; }
        public SearchCriteria ThumbnailCriteria { get; set; }
        public SearchCriteria LocationCriteria { get; set; }
        public SearchCriteria DateCriteria { get; set; }
        #endregion

        #region Constructor
        public HTMLCriteriaClass()
        {
            XMLSearchCriteriaDefinition();
        }
        #endregion

        #region Helpers
        void XMLSearchCriteriaDefinition()
        {
            PriceCriteria = new SearchCriteria()
            {
                DivisionStart = "<td class=\"searchResultsPriceValue\">",
                DivisionEnd = "<td class=\"searchResultsDateValue\">",
                SearchStart = "<div> ",
                SearchEnd = "</div>",
                MaxIndex = 20
            };
            AdvertLinkCriteria = new SearchCriteria()
            {
                DivisionStart = "<a class=\" classifiedTitle\"",
                DivisionEnd = "</a>",
                SearchStart = "href=\"",
                SearchEnd = "/detay\">",
                MaxIndex = 20
            };
            DescriptionCriteria = new SearchCriteria()
            {
                DivisionStart = "<a class=\" classifiedTitle\"",
                DivisionEnd = "</td>",
                SearchStart = "/detay\">\n    ",
                SearchEnd = "</a>",
                MaxIndex = 20
            };
            ThumbnailCriteria = new SearchCriteria()
            {
                DivisionStart = "<td class=\"searchResultsLargeThumbnail\">",
                DivisionEnd = "</td>",
                SearchStart = "<img class=\"\"\n        \n        src=\"",
                SearchEnd = "\"\n",
                MaxIndex = 20
            };
            LocationCriteria = new SearchCriteria()
            {
                DivisionStart = "<td class=\"searchResultsLocationValue\">",
                DivisionEnd = "<td class=\"ignore-me\">",
                SearchStart = "<td class=\"searchResultsLocationValue\">\n                        ",
                SearchEnd = "</td>",
                MaxIndex = 20
            };
            DateCriteria = new SearchCriteria()
            {
                DivisionStart = "<td class=\"searchResultsDateValue\">",
                DivisionEnd = "<td class=\"searchResultsLocationValue\">",
                SearchStart = "<span>",
                SearchEnd = "</span>\n                    </td>",
                MaxIndex = 20
            };
            EmptyCriteria = new SearchCriteria()
            {
                DivisionStart = "<tbody class=\"searchResultsRowClass\">",
                DivisionEnd = "</table>",
                SearchStart = "<body ",
                SearchEnd = "</body>",
                MaxIndex = 1
            };
        }
        #endregion
    }
}
