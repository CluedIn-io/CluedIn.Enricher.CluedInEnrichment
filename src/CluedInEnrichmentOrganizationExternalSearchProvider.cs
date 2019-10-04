// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CluedInEnrichmentOrganizationExternalSearchProvider.cs" company="Clued In">
//   Copyright (c) 2019 Clued In. All rights reserved.
// </copyright>
// <summary>
//   Implements the clued in enrichment organization external search provider class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// #define EXTERNALSEARCH_DEBUG

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;

using CluedIn.Core;
using CluedIn.Core.Configuration;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.ExternalSearch;
using CluedIn.Crawling.Helpers;
using CluedIn.ExternalSearch.Filters;
using CluedIn.ExternalSearch.Providers.CluedInEnrichment.Vocabularies;
using CluedIn.PublicApi.Models;

using RestSharp;

namespace CluedIn.ExternalSearch.Providers.CluedInEnrichment
{
    /// <summary>The CluedIn organization enrichment external search provider.</summary>
    /// <seealso cref="CluedIn.ExternalSearch.ExternalSearchProviderBase" />
    public class CluedInEnrichmentOrganizationExternalSearchProvider : ExternalSearchProviderBase
    {
        /**********************************************************************************************************
         * FIELDS
         **********************************************************************************************************/

        public static readonly Guid ProviderId = Guid.Parse("D57FE28F-068E-4187-895B-F1B28E88A435");
        
        /**********************************************************************************************************
         * CONSTRUCTORS
         **********************************************************************************************************/

        public CluedInEnrichmentOrganizationExternalSearchProvider()
            : base(ProviderId, EntityType.Organization)
        {
            var nameBasedTokenProvider = new NameBasedTokenProvider("CluedInEnrichment");

            if (nameBasedTokenProvider.ApiToken != null)
                this.TokenProvider = new RoundRobinTokenProvider(nameBasedTokenProvider.ApiToken.Split(',', ';'));

            this.TokenProviderIsRequired = true;
        }

        public CluedInEnrichmentOrganizationExternalSearchProvider(IExternalSearchTokenProvider tokenProvider)
            : base(ProviderId, EntityType.Organization)
        {
            this.TokenProvider           = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            this.TokenProviderIsRequired = true;
        }

        /**********************************************************************************************************
         * METHODS
         **********************************************************************************************************/

        public override IEnumerable<IExternalSearchQuery> BuildQueries(ExecutionContext context, IExternalSearchRequest request)
        {
            if (!this.Accepts(request.EntityMetaData.EntityType))
                yield break;

            var existingResults = request.GetQueryResults<OrganizationLookupResponse>(this).ToList();

            Func<string, bool> nameFilter = value => OrganizationFilters.NameFilter(context, value) || existingResults.Any(r => string.Equals(r.Data.Name, value, StringComparison.InvariantCultureIgnoreCase));

            var entityType = request.EntityMetaData.EntityType;

            var organizationName = request.QueryParameters.GetValue(Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, new HashSet<string>());
            var domainName       = request.QueryParameters.GetValue(Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmailDomainNames, new HashSet<string>());
            var website          = request.QueryParameters.GetValue(Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website, new HashSet<string>());

            if (organizationName != null && (domainName != null || website != null))
            {
                var organizationNameNormalized = organizationName.Select(NameNormalization.Normalize).ToHashSet();

                foreach (var name in organizationNameNormalized.Where(v => v != null && !nameFilter(v)))
                {
                    bool combinedQueryEmitted = false;

                    if (domainName.Count == 1 && website.Count == 1)
                    {
                        foreach (var domain in domainName)
                            foreach (var url in website)
                                yield return new ExternalSearchQuery(this, entityType, new Dictionary<string, string>()
                                                                                       {
                                                                                           { ExternalSearchQueryParameter.Uri.ToString(), domain },
                                                                                           { ExternalSearchQueryParameter.Domain.ToString(), url },
                                                                                           { ExternalSearchQueryParameter.Name.ToString(), name }
                                                                                       });
                        combinedQueryEmitted = true;
                    }

                    if (!combinedQueryEmitted || domainName.Count > 1)
                    {
                        foreach (var value in domainName)
                            yield return new ExternalSearchQuery(this, entityType, new Dictionary<string, string>()
                                                                                   {
                                                                                       { ExternalSearchQueryParameter.Domain.ToString(), value },
                                                                                       { ExternalSearchQueryParameter.Name.ToString(), name }
                                                                                   });
                    }

                    if (!combinedQueryEmitted || website.Count > 1)
                    {
                        foreach (var value in website)
                            yield return new ExternalSearchQuery(this, entityType, new Dictionary<string, string>()
                                                                                   {
                                                                                       { ExternalSearchQueryParameter.Uri.ToString(), value },
                                                                                       { ExternalSearchQueryParameter.Name.ToString(), name }
                                                                                   });
                    }
                }
            }
        }

        public override IEnumerable<IExternalSearchQueryResult> ExecuteSearch(ExecutionContext context, IExternalSearchQuery query)
        {
            if (string.IsNullOrEmpty(this.TokenProvider?.ApiToken))
                throw new InvalidOperationException("ApiToken is not configured for CluedInEnrichment Provider");

            var serverUrl = ConfigurationManager.AppSettings.GetValue($"Feature.ExternalSearch.{this.ConfigurationName}.Url", "https://publicapi.cluedin.net/");

            var client = new RestClient(new Uri(new Uri(serverUrl), "api/v1/"));

            var serializer = JsonUtility.CreateDefaultSerializer(s =>
            {
                s.Error = (sender, args) =>
                {
                    if (object.Equals(args.ErrorContext.Member, "Metadata") && 
                        args.ErrorContext.OriginalObject.GetType() == typeof(OrganizationLookupResponse))
                    {
                        args.ErrorContext.Handled = true;
                    }
                };
            });

            client.AddHandler("application/json", new NewtonsoftJsonSerializer(serializer));

            var request = new RestRequest("enrichment/lookup/organization", Method.GET);

            var organizationName    = query.QueryParameters.GetValue<string, HashSet<string>>(ExternalSearchQueryParameter.Name.ToString(), new HashSet<string>()).FirstOrDefault();
            var website             = query.QueryParameters.GetValue<string, HashSet<string>>(ExternalSearchQueryParameter.Uri.ToString(), new HashSet<string>()).FirstOrDefault();
            var domain              = query.QueryParameters.GetValue<string, HashSet<string>>(ExternalSearchQueryParameter.Domain.ToString(), new HashSet<string>()).FirstOrDefault();

            if (string.IsNullOrEmpty(organizationName) && string.IsNullOrEmpty(website) && string.IsNullOrEmpty(domain))
                yield break;

            if (!string.IsNullOrEmpty(organizationName))
                request.AddQueryParameter("name", organizationName);

            if (!string.IsNullOrEmpty(website))
                request.AddQueryParameter("domainName", website);

            if (!string.IsNullOrEmpty(domain))
                request.AddQueryParameter("domainName", domain);

            request.AddHeader("Authorization", "Bearer " + this.TokenProvider.ApiToken);

#if EXTERNALSEARCH_DEBUG
            request.AddHeader("X-CluedIn-Debug", "1");
#endif

            var response = client.ExecuteTaskAsync<OrganizationLookupResponse>(request).Result;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (response.Data != null)
                    yield return new ExternalSearchQueryResult<OrganizationLookupResponse>(query, response.Data);
            }
            else if (response.StatusCode == HttpStatusCode.NoContent || response.StatusCode == HttpStatusCode.NotFound)
                yield break;
            else if (response.StatusCode == HttpStatusCode.Accepted)
            {
                var ex = new Exception("Enrichment queued");
                ex.Data[Constants.DoNotLogExceptionKey] = true;
                throw ex;
            }
            else if (response.ErrorException != null)
                throw new AggregateException(response.ErrorException.Message, response.ErrorException);
            else
                throw new ApplicationException("Could not execute external search query - StatusCode:" + response.StatusCode + "; Content: " + response.Content);
        }

        public override IEnumerable<Clue> BuildClues(ExecutionContext context, IExternalSearchQuery query, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<OrganizationLookupResponse>();

            var code = this.GetOriginEntityCode(resultItem);

            var clue = new Clue(code, context.Organization);

            this.PopulateMetadata(clue.Data.EntityData, resultItem);
            this.DownloadPreviewImage(context, resultItem.Data.Logo, clue, new Dictionary<string, string>() { { "Authorization", "Bearer " + this.TokenProvider.ApiToken } });

            return new[] { clue };
        }

        public override IEntityMetadata GetPrimaryEntityMetadata(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            var resultItem = result.As<OrganizationLookupResponse>();
            return this.CreateMetadata(resultItem);
        }

        public override IPreviewImage GetPrimaryEntityPreviewImage(ExecutionContext context, IExternalSearchQueryResult result, IExternalSearchRequest request)
        {
            return this.DownloadPreviewImageBlob<OrganizationLookupResponse>(context, result, r => r.Data.Logo);
        }

        private IEntityMetadata CreateMetadata(IExternalSearchQueryResult<OrganizationLookupResponse> resultItem)
        {
            var metadata = new EntityMetadataPart();

            this.PopulateMetadata(metadata, resultItem);

            return metadata;
        }

        private EntityCode GetOriginEntityCode(IExternalSearchQueryResult<OrganizationLookupResponse> resultItem)
        {
            var value = resultItem.Data.TaxId ?? resultItem.Data.TickerSymbol ?? resultItem.Data.Website?.Url;

            if (value != null)
                return new EntityCode(EntityType.Organization, this.GetCodeOrigin(), value);

            return new EntityCode(EntityType.Organization, CodeOrigin.CluedIn.CreateSpecific("name"), resultItem.Data.Name);
        }

        private CodeOrigin GetCodeOrigin()
        {
            return CodeOrigin.CluedIn.CreateSpecific("enrichment");
        }

        private void PopulateMetadata(IEntityMetadata metadata, IExternalSearchQueryResult<OrganizationLookupResponse> resultItem)
        {
            var code = this.GetOriginEntityCode(resultItem);
            var vocab = new CluedInEnrichmentOrganizationVocabulary();

            metadata.EntityType         = EntityType.Organization;
            metadata.Name               = resultItem.Data.Name;
            metadata.Description        = resultItem.Data.Description;
            metadata.OriginEntityCode   = code;

            metadata.Codes.Add(code);

            metadata.Properties[vocab.Name]                 = resultItem.Data.Name;
            metadata.Properties[vocab.Description]          = resultItem.Data.Description;

            metadata.Properties[vocab.ContactEmail]         = resultItem.Data.ContactInfo?.ContactEmail;
            metadata.Properties[vocab.ContactPhoneNumber]   = resultItem.Data.ContactInfo?.ContactPhoneNumber;
            metadata.Properties[vocab.Fax]                  = resultItem.Data.ContactInfo?.Fax;
            metadata.Properties[vocab.PhoneNumber]          = resultItem.Data.ContactInfo?.PhoneNumber;

            metadata.Properties[vocab.EmployeeCount]        = resultItem.Data.EmployeeCount;
            metadata.Properties[vocab.FoundingDate]         = resultItem.Data.FoundingDate;
            metadata.Properties[vocab.Industry]             = resultItem.Data.Industry;
            metadata.Properties[vocab.TaxId]                = resultItem.Data.TaxId;
            metadata.Properties[vocab.TickerSymbol]         = resultItem.Data.TickerSymbol;
            metadata.Properties[vocab.VatNumber]            = resultItem.Data.VatNumber;
            metadata.Properties[vocab.Logo]                 = resultItem.Data.Logo.PrintIfAvailable();

            metadata.Properties[vocab.Domains]              = resultItem.Data.Domains.PrintIfAvailable(v => string.Join(";", v));
            metadata.Properties[vocab.Tags]                 = resultItem.Data.Tags.PrintIfAvailable(v => string.Join(";", v));
            metadata.Properties[vocab.Technologies]         = resultItem.Data.Technologies.PrintIfAvailable(v => string.Join(", ", v));
            metadata.Properties[vocab.Website]              = resultItem.Data.Website?.Url;
            metadata.Properties[vocab.Uris]                 = resultItem.Data.Uris.PrintIfAvailable(v => string.Join(";", v));

            metadata.Tags.AddRange((resultItem.Data.Tags ?? new string[0]).Select(t => new Tag(t)));
            metadata.ExternalReferences.AddRange(resultItem.Data.Uris ?? new List<Uri>(0));

            foreach (var socialProfile in resultItem.Data.SocialProfiles)
            {
                var key = UriUtility.GetSocialVocabularyKey(socialProfile.Type, vocab.SocialProfiles);

                if (key != null)
                    metadata.Properties[key] = socialProfile.Value;
            }
        }
    }
}