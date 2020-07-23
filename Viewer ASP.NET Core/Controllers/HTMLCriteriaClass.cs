using static Viewer_ASP.NET_Core.Models.GeneralModel;

namespace Viewer_ASP.NET_Core.Controllers
{
    public class HTMLCriteriaClass
    {
        #region Properties
        public SearchCriteria PriceCriteria { get; set; }
        public SearchCriteria EmptyCriteria { get; set; }
        public SearchCriteria AdvertIDCriteria { get; set; }
        public SearchCriteria DescriptionCriteria { get; set; }
        public SearchCriteria ThumbnailCriteria { get; set; }
        public SearchCriteria LocationCriteria { get; set; }
        public SearchCriteria DateCriteria { get; set; }
        public DivisionCriteria ResultAttributeDivisionCriteria { get; set; }
        public DivisionCriteria AdvertTrimCriteria { get; set; }
        public DivisionCriteria AttributeTrimCriteria { get; set; }
        public DivisionCriteria AdvertSplitDivisionCriteria { get; set; }
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
            AdvertSplitDivisionCriteria = new DivisionCriteria()
            {
                DivisionStart = "<tr data-id=\"",
                DivisionEnd = "</tr>"
            };
            AdvertTrimCriteria = new DivisionCriteria()
            {
                DivisionStart = "<tbody class=\"searchResultsRowClass\">",
                DivisionEnd = "</tbody>"
            };
            AttributeTrimCriteria = new DivisionCriteria()
            {
                DivisionStart = "</td>\n            ",
                DivisionEnd = "<td class=\"searchResultsPriceValue\">"
            };
            PriceCriteria = new SearchCriteria()
            {
                SearchStart = "<td class=\"searchResultsPriceValue\">\n                        <div> ",
                SearchEnd = "</div>"
            };
            AdvertIDCriteria = new SearchCriteria()
            {
                SearchStart = "<tr data-id=\"",
                SearchEnd = "\"\n"
            };
            DescriptionCriteria = new SearchCriteria()
            {
                SearchStart = "/detay\">\n    ",
                SearchEnd = "</a>"
            };
            ThumbnailCriteria = new SearchCriteria()
            {
                SearchStart = " src=\"",
                SearchEnd = "\"\n"
            };
            LocationCriteria = new SearchCriteria()
            {
                SearchStart = "<td class=\"searchResultsLocationValue\">\n                        ",
                SearchEnd = "</td>"
            };
            DateCriteria = new SearchCriteria()
            {
                SearchStart = "<span>",
                SearchEnd = "</span>\n                    </td>"
            };
            DateCriteria = new SearchCriteria()
            {
                SearchStart = "<span>",
                SearchEnd = "</span>\n                    </td>"
            };
            ResultAttributeDivisionCriteria = new DivisionCriteria()
            {
                DivisionStart = "<td class=\"searchResultsAttributeValue\">\n",
                DivisionEnd = "</td>"
            };
        }
        #endregion
    }
}
