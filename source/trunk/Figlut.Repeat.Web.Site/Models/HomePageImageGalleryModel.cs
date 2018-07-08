namespace Figlut.Repeat.Web.Site.Models
{
    #region Using Directives

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    #endregion //Using Directives

    public class HomePageImageGalleryModel
    {
        #region Constructors

        public HomePageImageGalleryModel()
        {
            _howitWorksImages = new List<string>();
        }

        #endregion //Constructors

        #region Fields

        private List<string> _howitWorksImages;
        private List<string> _whyitWorksImages;

        #endregion //Fields

        #region Properties

        public List<string> HowItWorksImages
        {
            get { return _howitWorksImages; }
            set { _howitWorksImages = value; }
        }

        public List<string> WhyItWorksImages
        {
            get { return _whyitWorksImages; }
            set { _whyitWorksImages = value; }
        }

        #endregion //Properties
    }
}