// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CluedInEnrichmentTests.cs" company="Clued In">
//   Copyright Clued In
// </copyright>
// <summary>
//   Defines the CluedInEnrichmentTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using CluedIn.Core.Data;
using CluedIn.Core.Data.Parts;
using CluedIn.Core.Messages.Processing;
using CluedIn.ExternalSearch.Providers.CluedInEnrichment;
using CluedIn.Testing.Base.ExternalSearch;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace ExternalSearch.CluedInEnrichment.Integration.Tests
{
    public class CluedInEnrichmentOrganizationTests : BaseExternalSearchTest<CluedInEnrichmentOrganizationExternalSearchProvider>
    {
        private readonly ITestOutputHelper testOutputHelper;

        public CluedInEnrichmentOrganizationTests(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
        }

        [Theory]
        [InlineData("CluedIn", "cluedin.net", "https://cluedin.net/")]
        [InlineData("Nordea Bank AB", "nordea.com", "http://nordea.com/")]
        public void TestClueProduction(string name, string domainName, string website)
        {
            var dummyTokenProvider = new DummyTokenProvider("vB9-LbiuJU3i75I23nDCF73QGybpm_9COFcqZ7vIO4RflTBzSMVaHu69D5oHLjgUAl-1PuNohXnfQasgG60l67wIGPdtjy36_e6cfFVTRdyqK0vwcAbYwzKXMaDQ-kFz870eTkWCo-0dH-h2mhXG61vsX7WV90GIJBhYbmc47yI14HPwp5M_h0p4s15PdXzQSANu3bqj9GVjoURkyIhuCxBDVcdiwETt8101gGu-HGKFztPfY4NZ_YT7UrKtCBbcurnvgmcDOde9g-mG8HpK9lHf6k7rdjifd5KneKY-EhT-9_SB5CnPDjyCCp9ZQ3WHOciItutpRXgvcAMotzjsJEYYRoAPUDpc3I3rO1sjkSX6DEdP4OZaOcr6tzH-VXr3ilCb0CFnGYWpMllKdwwS_EIlttnvZ1aEc8i3vn3DDnEt57Wyd9Osc_6nFtcGFGvxATutGpWjWXhEy9_69pMwOJnhEBb-8EJCMAz_7eNpDgh3-h0LK9Sk3GM7FJwu2AvGfGUuwnsQXxIYLAnhd0mvFCP5-z929ErjE2vRz2-Ow2aZ0QbF");
            object[] parameters = { dummyTokenProvider };

            var properties = new EntityMetadataPart();
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, name);
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmailDomainNames, domainName);
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website, website);

            IEntityMetadata entityMetadata = new EntityMetadataPart() {
                Name = name,
                EntityType = EntityType.Organization,
                Properties = properties.Properties
            };

            this.Setup(parameters, entityMetadata);

            this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.AtLeastOnce);
            Assert.NotEmpty(this.clues);

            this.clues.ForEach(c => this.testOutputHelper.WriteLine(c.Decompress().Serialize()));
        }

        [Theory]
        [InlineData("Non-ExistingCompany", "localhost.dev", "http://localhost.dev")]
        [Trait("Category", "slow")]
        public void TestNoClueProduction(string name, string domainName, string website)
        {
            var dummyTokenProvider = new DummyTokenProvider("vB9-LbiuJU3i75I23nDCF73QGybpm_9COFcqZ7vIO4RflTBzSMVaHu69D5oHLjgUAl-1PuNohXnfQasgG60l67wIGPdtjy36_e6cfFVTRdyqK0vwcAbYwzKXMaDQ-kFz870eTkWCo-0dH-h2mhXG61vsX7WV90GIJBhYbmc47yI14HPwp5M_h0p4s15PdXzQSANu3bqj9GVjoURkyIhuCxBDVcdiwETt8101gGu-HGKFztPfY4NZ_YT7UrKtCBbcurnvgmcDOde9g-mG8HpK9lHf6k7rdjifd5KneKY-EhT-9_SB5CnPDjyCCp9ZQ3WHOciItutpRXgvcAMotzjsJEYYRoAPUDpc3I3rO1sjkSX6DEdP4OZaOcr6tzH-VXr3ilCb0CFnGYWpMllKdwwS_EIlttnvZ1aEc8i3vn3DDnEt57Wyd9Osc_6nFtcGFGvxATutGpWjWXhEy9_69pMwOJnhEBb-8EJCMAz_7eNpDgh3-h0LK9Sk3GM7FJwu2AvGfGUuwnsQXxIYLAnhd0mvFCP5-z929ErjE2vRz2-Ow2aZ0QbF");
            object[] parameters = { dummyTokenProvider };

            var properties = new EntityMetadataPart();
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, name);
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmailDomainNames, domainName);
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website, website);

            IEntityMetadata entityMetadata = new EntityMetadataPart() {
                Name = name,
                EntityType = EntityType.Organization,
                Properties = properties.Properties
            };

            this.Setup(parameters, entityMetadata);

            // Assert
            this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.Never);
            Assert.Empty(this.clues);
        }

        [Theory]
        [InlineData("", "", "")]
        public void TestQueryParameterEmpty(string name, string domainName, string website)
        {
            var dummyTokenProvider = new DummyTokenProvider("vB9-LbiuJU3i75I23nDCF73QGybpm_9COFcqZ7vIO4RflTBzSMVaHu69D5oHLjgUAl-1PuNohXnfQasgG60l67wIGPdtjy36_e6cfFVTRdyqK0vwcAbYwzKXMaDQ-kFz870eTkWCo-0dH-h2mhXG61vsX7WV90GIJBhYbmc47yI14HPwp5M_h0p4s15PdXzQSANu3bqj9GVjoURkyIhuCxBDVcdiwETt8101gGu-HGKFztPfY4NZ_YT7UrKtCBbcurnvgmcDOde9g-mG8HpK9lHf6k7rdjifd5KneKY-EhT-9_SB5CnPDjyCCp9ZQ3WHOciItutpRXgvcAMotzjsJEYYRoAPUDpc3I3rO1sjkSX6DEdP4OZaOcr6tzH-VXr3ilCb0CFnGYWpMllKdwwS_EIlttnvZ1aEc8i3vn3DDnEt57Wyd9Osc_6nFtcGFGvxATutGpWjWXhEy9_69pMwOJnhEBb-8EJCMAz_7eNpDgh3-h0LK9Sk3GM7FJwu2AvGfGUuwnsQXxIYLAnhd0mvFCP5-z929ErjE2vRz2-Ow2aZ0QbF");
            object[] parameters = { dummyTokenProvider };

            var properties = new EntityMetadataPart();
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, name);
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmailDomainNames, domainName);
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website, website);

            IEntityMetadata entityMetadata = new EntityMetadataPart() {
                Name = name,
                EntityType = EntityType.Organization,
                Properties = properties.Properties
            };

            this.Setup(parameters, entityMetadata);

            // Assert
            this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.Never);
            Assert.Empty(this.clues);
        }

        [Theory]
        [InlineData(null, null, null)]
        public void TestQueryParameterNull(string name, string domainName, string website)
        {
            var dummyTokenProvider = new DummyTokenProvider("vB9-LbiuJU3i75I23nDCF73QGybpm_9COFcqZ7vIO4RflTBzSMVaHu69D5oHLjgUAl-1PuNohXnfQasgG60l67wIGPdtjy36_e6cfFVTRdyqK0vwcAbYwzKXMaDQ-kFz870eTkWCo-0dH-h2mhXG61vsX7WV90GIJBhYbmc47yI14HPwp5M_h0p4s15PdXzQSANu3bqj9GVjoURkyIhuCxBDVcdiwETt8101gGu-HGKFztPfY4NZ_YT7UrKtCBbcurnvgmcDOde9g-mG8HpK9lHf6k7rdjifd5KneKY-EhT-9_SB5CnPDjyCCp9ZQ3WHOciItutpRXgvcAMotzjsJEYYRoAPUDpc3I3rO1sjkSX6DEdP4OZaOcr6tzH-VXr3ilCb0CFnGYWpMllKdwwS_EIlttnvZ1aEc8i3vn3DDnEt57Wyd9Osc_6nFtcGFGvxATutGpWjWXhEy9_69pMwOJnhEBb-8EJCMAz_7eNpDgh3-h0LK9Sk3GM7FJwu2AvGfGUuwnsQXxIYLAnhd0mvFCP5-z929ErjE2vRz2-Ow2aZ0QbF");
            object[] parameters = { dummyTokenProvider };

            var properties = new EntityMetadataPart();
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.OrganizationName, name);
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.EmailDomainNames, domainName);
            properties.Properties.Add(CluedIn.Core.Data.Vocabularies.Vocabularies.CluedInOrganization.Website, website);

            IEntityMetadata entityMetadata = new EntityMetadataPart() {
                Name = name,
                EntityType = EntityType.Organization,
                Properties = properties.Properties
            };

            this.Setup(parameters, entityMetadata);

            // Assert
            this.testContext.ProcessingHub.Verify(h => h.SendCommand(It.IsAny<ProcessClueCommand>()), Times.Never);
            Assert.Empty(this.clues);
        }
    }
}