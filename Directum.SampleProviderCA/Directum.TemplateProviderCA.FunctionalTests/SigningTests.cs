using Directum.Core.UniversalProvider.WebApiModels.Sign;
using FluentAssertions;

namespace Directum.Core.UniversalProvider.FunctionalTests;

public class SigningTests : FunctionalTestBase
{
  [Test]
  public async Task Signing_ExternalMobileAppWithQrCode_Success()
  {
    var signingRequest = new SigningRequest
    {
      CertificateThumbprint = "AnyThumb",
      Login = "UserLogin",
      DataType = DataType.Hash,
      Documents = new[]
      {
        new DocumentInfo
        {
          Data = "Hash1",
          Name = "DocumentName1",
        },
        new DocumentInfo
        {
          Data = "Hash2",
          Name = "DocumentName2",
        },
      },
    };

    var signingStatus = await _signClient.Start(signingRequest);

    signingStatus = await _signClient.GetStatus(signingStatus.OperationId);

    var confirmationInfo = await _signClient.CreateConfirmationRequest(signingStatus.OperationId);

    await _signClient.Confirm(signingStatus.OperationId, "12");

    var signs = await _signClient.GetSigns(signingStatus.OperationId);

    confirmationInfo.ConfirmationType
      .Should()
      .Be(ConfirmationType.MobileApp);

    confirmationInfo.ConfirmationData
      .Should()
      .ContainEquivalentOf(new ConfirmationData { Type = ConfirmationDataType.QrCode });

    confirmationInfo.ConfirmationData
      .Should()
      .ContainEquivalentOf(new ConfirmationData { Type = ConfirmationDataType.Link });

    signs[0]
      .Should()
      .BeEquivalentTo(new SigningResult { DocumentName = signingRequest.Documents[0].Name, });

    signs[1]
      .Should()
      .BeEquivalentTo(new SigningResult { DocumentName = signingRequest.Documents[1].Name, });
  }
}
