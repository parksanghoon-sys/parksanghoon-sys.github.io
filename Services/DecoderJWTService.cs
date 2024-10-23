using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace parksanghoonsys.github.io.Services;

public class DecoderJWTService
{
    public string JwtToken { get; private set; }
    public JsonDocument JwtHeader { get; private set; }
    public JsonDocument JwtPayload { get; private set; }
    public string JwtSignature { get; private set; }
    public string ErrorMessage { get; private set; }
    public bool IsSignatureValid { get; private set; }

    private string DecodeBase64Url(string base64Url)
    {
        var base64 = base64Url.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }
        var bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }
    // JWT 디코드 및 검증 함수
    public void DecodeAndVerify(string jwtToken, string secretKey)
    {
        JwtToken = jwtToken;
        try
        {
            ErrorMessage = null;

            // JWT 구조: Header.Payload.Signature
            var parts = jwtToken.Split('.');
            if (parts.Length != 3)
            {
                ErrorMessage = "Invalid JWT format.";
                return;
            }

            // Base64 URL 디코딩
            JwtHeader = JsonDocument.Parse(DecodeBase64Url(parts[0]));
            JwtPayload = JsonDocument.Parse(DecodeBase64Url(parts[1]));
            JwtSignature = parts[2];

            // 알고리즘 확인 (HS256만 처리)
            var alg = JwtHeader.RootElement.GetProperty("alg").GetString();
            if (alg != "HS256")
            {
                ErrorMessage = "Unsupported algorithm: " + alg;
                return;
            }

            // 서명 검증 (HS256)
            IsSignatureValid = VerifySignatureHS256(parts[0], parts[1], JwtSignature, secretKey);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error decoding JWT: {ex.Message}";
        }
    }

    // HS256 서명 검증 함수
    private bool VerifySignatureHS256(string header, string payload, string signature, string secret)
    {
        try
        {
            // Header와 Payload 결합
            var unsignedToken = $"{header}.{payload}";

            // HMAC-SHA256으로 서명 생성
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            var unsignedBytes = Encoding.UTF8.GetBytes(unsignedToken);
            using (var hmac = new HMACSHA256(secretBytes))
            {
                var hashBytes = hmac.ComputeHash(unsignedBytes);
                var computedSignature = Convert.ToBase64String(hashBytes)
                    .Replace('+', '-')
                    .Replace('/', '_')
                    .TrimEnd('=');

                // 생성된 서명과 JWT 서명 비교
                return computedSignature == signature;
            }
        }
        catch
        {
            return false;
        }
    }
}
