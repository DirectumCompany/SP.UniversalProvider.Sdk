
/// <summary>
/// Настройки доверенного издателя токенов.
/// </summary>
public class TrustedIssuerSettings
{
  /// <summary>
  /// Имя издателя.
  /// </summary>
  public string Issuer { get; set; }

  /// <summary>
  /// Ключ валидации токенов.
  /// </summary>
  public string EncryptionKey { get; set; }

  /// <summary>
  /// Отпечаток сертификата валидации токенов.
  /// </summary>
  public string SigningCertificateThumbprint { get; set; }

  /// <summary>
  /// Путь до файла сертификата валидации токенов.
  /// </summary>
  public string SigningCertificatePath { get; set; }
}
