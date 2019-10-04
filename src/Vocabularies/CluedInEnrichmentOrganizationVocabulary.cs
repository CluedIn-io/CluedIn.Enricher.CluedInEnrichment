// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CluedInEnrichmentOrganizationVocabulary.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the CluedInEnrichmentOrganizationVocabulary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using CluedIn.Core.Data;
using CluedIn.Core.Data.Vocabularies;
using CluedIn.Core.Data.Vocabularies.CluedIn;

namespace CluedIn.ExternalSearch.Providers.CluedInEnrichment.Vocabularies
{
    /// <summary>The CluedInEnrichmentPersonVocabulary.</summary>
    /// <seealso cref="CluedIn.Core.Data.Vocabularies.SimpleVocabulary" />
    public class CluedInEnrichmentOrganizationVocabulary : SimpleVocabulary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CluedInEnrichmentOrganizationVocabulary"/> class.
        /// </summary>
        public CluedInEnrichmentOrganizationVocabulary()
        {
            this.VocabularyName = "CluedInEnrichment Organization";
            this.KeyPrefix      = "cluedinEnrichment.organization";
            this.KeySeparator   = ".";
            this.Grouping       = EntityType.Organization;

            this.Name               = this.Add(new VocabularyKey("name", VocabularyKeyDataType.Text));
            this.Description        = this.Add(new VocabularyKey("description", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.ContactEmail       = this.Add(new VocabularyKey("contactEmail", VocabularyKeyDataType.Email, VocabularyKeyVisibility.Visible));
            this.ContactPhoneNumber = this.Add(new VocabularyKey("contactPhoneNumber", VocabularyKeyDataType.PhoneNumber, VocabularyKeyVisibility.Visible));
            this.Fax                = this.Add(new VocabularyKey("fax", VocabularyKeyDataType.PhoneNumber, VocabularyKeyVisibility.Visible));
            this.PhoneNumber        = this.Add(new VocabularyKey("phoneNumber", VocabularyKeyDataType.PhoneNumber, VocabularyKeyVisibility.Visible));
            this.EmployeeCount      = this.Add(new VocabularyKey("employeeCount", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.FoundingDate       = this.Add(new VocabularyKey("foundingDate", VocabularyKeyDataType.DateTime, VocabularyKeyVisibility.Visible));
            this.Industry           = this.Add(new VocabularyKey("industry", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.TaxId              = this.Add(new VocabularyKey("taxId", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.TickerSymbol       = this.Add(new VocabularyKey("tickerSymbol", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.VatNumber          = this.Add(new VocabularyKey("vatNumber", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.Logo               = this.Add(new VocabularyKey("logo", VocabularyKeyDataType.Uri, VocabularyKeyVisibility.Visible));
            this.Domains            = this.Add(new VocabularyKey("domains", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.Tags               = this.Add(new VocabularyKey("tags", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.Technologies       = this.Add(new VocabularyKey("technologies", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.Website            = this.Add(new VocabularyKey("website", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.Uris               = this.Add(new VocabularyKey("uris", VocabularyKeyDataType.Text, VocabularyKeyVisibility.Visible));
            this.SocialProfiles     = this.Add(new CluedInSocialLinksVocabulary().AsCompositeKey("socialProfiles"));

            this.AddMapping(this.Name, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName);
            this.AddMapping(this.ContactEmail, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.ContactEmail);
            this.AddMapping(this.ContactPhoneNumber, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.ContactPhoneNumber);
            this.AddMapping(this.Fax, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Fax);
            this.AddMapping(this.PhoneNumber, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.PhoneNumber);
            this.AddMapping(this.EmployeeCount, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmployeeCount);
            this.AddMapping(this.FoundingDate, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.FoundingDate);
            this.AddMapping(this.Industry, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Industry);
            this.AddMapping(this.TaxId, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.TaxId);
            this.AddMapping(this.TickerSymbol, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.TickerSymbol);
            this.AddMapping(this.VatNumber, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.VatNumber);
            //this.AddMapping(this.Logo, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Logo);
            this.AddMapping(this.Domains, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmailDomainNames);
            //this.AddMapping(this.Tags, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Tags);
            this.AddMapping(this.Technologies, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.UsedTechnologies);
            this.AddMapping(this.Website, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website);
            //this.AddMapping(this.Uris, CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Uris);

            // TODO: Add support for mapping composite keys
            this.AddMapping(this.SocialProfiles.LinkedIn        , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.LinkedIn);
            this.AddMapping(this.SocialProfiles.Twitter         , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Twitter);
            this.AddMapping(this.SocialProfiles.Facebook        , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Facebook);
            this.AddMapping(this.SocialProfiles.GooglePlus      , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.GooglePlus);
            this.AddMapping(this.SocialProfiles.Instagram       , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Instagram);
            this.AddMapping(this.SocialProfiles.Foursquare      , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Foursquare);
            this.AddMapping(this.SocialProfiles.YouTube         , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.YouTube);
            this.AddMapping(this.SocialProfiles.Blogger         , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Blogger);
            this.AddMapping(this.SocialProfiles.Flickr          , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Flickr);
            this.AddMapping(this.SocialProfiles.GoodReads       , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.GoodReads);
            this.AddMapping(this.SocialProfiles.TripIt          , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.TripIt);
            this.AddMapping(this.SocialProfiles.Tumblr          , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Tumblr);
            this.AddMapping(this.SocialProfiles.Vimeo           , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Vimeo);
            this.AddMapping(this.SocialProfiles.WordPress       , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.WordPress);
            this.AddMapping(this.SocialProfiles.Yahoo           , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Yahoo);
            this.AddMapping(this.SocialProfiles.Pinterest       , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Pinterest);
            this.AddMapping(this.SocialProfiles.Weibo           , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Weibo);
            this.AddMapping(this.SocialProfiles.Xing            , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Xing);
            this.AddMapping(this.SocialProfiles.GitHub          , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.GitHub);
            this.AddMapping(this.SocialProfiles.Stackoverflow   , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Stackoverflow);
            this.AddMapping(this.SocialProfiles.Klout           , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Klout);
            this.AddMapping(this.SocialProfiles.Gravatar        , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.Gravatar);
            this.AddMapping(this.SocialProfiles.AngelCo         , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.AngelCo);
            this.AddMapping(this.SocialProfiles.AboutMe         , CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Social.AboutMe);
        }

        public VocabularyKey Name { get; set; }
        public VocabularyKey Description { get; set; }
        public VocabularyKey ContactEmail { get; set; }
        public VocabularyKey ContactPhoneNumber { get; set; }
        public VocabularyKey Fax { get; set; }
        public VocabularyKey PhoneNumber { get; set; }
        public VocabularyKey EmployeeCount { get; set; }
        public VocabularyKey FoundingDate { get; set; }
        public VocabularyKey Industry { get; set; }
        public VocabularyKey TaxId { get; set; }
        public VocabularyKey TickerSymbol { get; set; }
        public VocabularyKey VatNumber { get; set; }
        public VocabularyKey Logo { get; set; }
        public VocabularyKey Domains { get; set; }
        public VocabularyKey Tags { get; set; }
        public VocabularyKey Technologies { get; set; }
        public VocabularyKey Website { get; set; }
        public VocabularyKey Uris { get; set; }
        public CluedInSocialLinksVocabulary SocialProfiles { get; set; }
    }
}
