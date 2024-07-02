using System.Net;
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
      },
    };

    var signingStatus = await _signClient.Start(signingRequest);

    signingStatus.Status
      .Should()
      .BeOneOf(SigningStatus.InProgress, SigningStatus.NeedConfirm);

    signingStatus = await _signClient.GetStatus(signingStatus.OperationId);

    signingStatus.Status
      .Should()
      .Be(SigningStatus.NeedConfirm);

    var confirmationInfo = await _signClient.CreateConfirmationRequest(signingStatus.OperationId);

    signingStatus = await _signClient.GetStatus(signingStatus.OperationId);

    signingStatus.Status
      .Should()
      .Be(SigningStatus.NeedConfirm);

    await _signClient.Confirm(signingStatus.OperationId, null);

    signingStatus = await _signClient.GetStatus("SuccessStatus");

    signingStatus.Status
      .Should()
      .Be(SigningStatus.Success);

    var signs = await _signClient.GetSigns(signingStatus.OperationId);

    confirmationInfo.ConfirmationType
      .Should()
      .Be(ConfirmationType.MobileApp);

    confirmationInfo.ConfirmationData
      .Should()
      .Match<ConfirmationData[]>(res => res.Any(v => v.Type == ConfirmationDataType.QrCode));

    confirmationInfo.ConfirmationData
      .Should()
      .Match<ConfirmationData[]>(res => res.Any(v => v.Type == ConfirmationDataType.Link));

    signs[0]
      .Should()
      .Match<SigningResult>(res => res.DocumentName == signingRequest.Documents[0].Name);
  }

  [Test]
  public async Task Signing_Validation_Error()
  {
    var signingRequest = new SigningRequest
    {
      CertificateThumbprint = "AnyThumb",
      Login = null,
      DataType = DataType.Hash,
      Documents = new[]
      {
        new DocumentInfo
        {
          Data = "Hash1",
          Name = "DocumentName1",
        },
      },
    };

    await AssertApiException(() => _signClient.Start(signingRequest), HttpStatusCode.BadRequest, "ValidationError");
  }

  [Test]
  public async Task Signing_OperationIdNotFound_Error()
  {
    await AssertApiException(() => _signClient.GetStatus("OperationIdNotExitst"), HttpStatusCode.NotFound, "NotFoundError");
  }

  [Test]
  public async Task Signing_UnexpectedServerError_Error()
  {
    await AssertApiException(() => _signClient.GetSigns("UnexpectedServerError"), HttpStatusCode.InternalServerError, "InternalServerError");
  }

  [Test]
  public async Task Signing_UserNotSignDocumentInExternalApp_Error()
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

    signingStatus.Status
      .Should()
      .BeOneOf(SigningStatus.InProgress, SigningStatus.NeedConfirm);

    signingStatus = await _signClient.GetStatus(signingStatus.OperationId);

    signingStatus.Status
      .Should()
      .Be(SigningStatus.NeedConfirm);

    var confirmationInfo = await _signClient.CreateConfirmationRequest(signingStatus.OperationId);

    await AssertApiException(() => _signClient.Confirm("UserNotSignDocumentInExternalApp", null), HttpStatusCode.BadRequest, "UnconfirmedSigningStatusError");
  }
}
