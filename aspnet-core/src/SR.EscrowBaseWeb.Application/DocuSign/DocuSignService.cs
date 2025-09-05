using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DocuSign.eSign.Api;
using DocuSign.eSign.Client;
using DocuSign.eSign.Client.Auth;
using DocuSign.eSign.Model;
using Microsoft.Extensions.Configuration;

namespace SR.EscrowBaseWeb.DocuSign
{
    public class DocuSignService
    {
        private readonly IConfiguration _configuration;

        public DocuSignService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(string EnvelopeId, string SigningUrl)> SendEnvelopeForEmbeddedSigningAsync(
            string signerEmail,
            string signerName,
            byte[] pdfBytes)
        {
            // Step 1: Load configuration
            string integrationKey = _configuration["DocuSign:IntegrationKey"];
            string userId = _configuration["DocuSign:UserId"];
            string authServer = _configuration["DocuSign:AuthServer"];
            string basePath = _configuration["DocuSign:BasePath"];
            string redirectUrl = _configuration["DocuSign:RedirectUri"];
            string privateKeyPath = _configuration["DocuSign:PrivateKeyPath"]; // e.g. "private.key"

            // Step 2: Authenticate via JWT
            var apiClient = new ApiClient(basePath);

            // Read the private key from the file system
            byte[] privateKeyBytes = File.ReadAllBytes(privateKeyPath);

            // Request JWT user token
            OAuth.OAuthToken tokenInfo = apiClient.RequestJWTUserToken(
                integrationKey,
                userId,
                authServer,
                privateKeyBytes,
                1, // expiration in hours
                new List<string> { "signature", "impersonation" });

            string accessToken = tokenInfo.access_token;

            apiClient.Configuration.AccessToken = accessToken;

            // Get user info and account ID
            OAuth.UserInfo userInfo = apiClient.GetUserInfo(accessToken);
            string accountId = userInfo.Accounts.First().AccountId;

            // Update base path to the account's base URI
            string accountBasePath = userInfo.Accounts.First().BaseUri + "/restapi";
            apiClient = new ApiClient(accountBasePath);

            // Step 3: Create the Envelope
            var envelopeDefinition = new EnvelopeDefinition
            {
                EmailSubject = "Please sign this document",
                Documents = new List<Document>
                {
                    new Document
                    {
                        DocumentBase64 = Convert.ToBase64String(pdfBytes),
                        Name = "Sample Document",
                        FileExtension = "pdf",
                        DocumentId = "1"
                    }
                },
                Recipients = new Recipients
                {
                    Signers = new List<Signer>
                    {
                        new Signer
                        {
                            Email = signerEmail,
                            Name = signerName,
                            RecipientId = "1",
                            ClientUserId = "1000", // Required for embedded signing
                            Tabs = new Tabs
                            {
                                SignHereTabs = new List<SignHere>
                                {
                                    new SignHere
                                    {
                                        AnchorString = "/sn1/", // Anchor text to place the sign tab
                                        AnchorUnits = "pixels",
                                        AnchorXOffset = "20",
                                        AnchorYOffset = "10"
                                    }
                                }
                            }
                        }
                    }
                },
                Status = "sent" // Send the envelope immediately
            };

            var envelopesApi = new EnvelopesApi(apiClient);
            EnvelopeSummary envelopeSummary = envelopesApi.CreateEnvelope(accountId, envelopeDefinition);

            // Step 4: Create embedded signing view
            var recipientViewRequest = new RecipientViewRequest
            {
                ReturnUrl = redirectUrl,
                AuthenticationMethod = "none", // For embedded signing, authentication is handled by your application
                Email = signerEmail,
                UserName = signerName,
                ClientUserId = "1000", // Must match the ClientUserId used when creating the envelope
                PingUrl = redirectUrl // Ping back to your application if the user navigates away
            };

            ViewUrl viewUrl = envelopesApi.CreateRecipientView(accountId, envelopeSummary.EnvelopeId, recipientViewRequest);

            return (envelopeSummary.EnvelopeId, viewUrl.Url);
        }
    }
}